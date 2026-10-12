# Testes

[← Índice](README.md)

## Projetos

| Projeto | Tipo | Testes* | O que cobre |
| --- | --- | --- | --- |
| `Instartups.Command.Domain.UnitTests` | Unitário | 129 | Entidades (`Entities/`), value objects, enums/mapeamentos e domain services. |
| `Instartups.Command.Application.UnitTests` | Unitário | 185 | Validators e handlers de cada caso de uso (`UseCases/<Caso>/`), validators comuns e `ValidarUrl`. |
| `Instartups.Command.Infrastructure.UnitTests` | Unitário | 10 | `PegarContextoRequisicao` (leitura das claims). |
| `Instartups.Command.Api.UnitTests` | Unitário | 58 | Controllers (montagem do command, status retornado), rotas, DTOs (`ToCommand`), `ResponseDTO` e `ExceptionsMiddleware`. |
| `Instartups.Command.Api.IntegrationTests` | Integração | 134 | API completa sobre PostgreSQL real: todos os endpoints de perfil e postagem, além de testes transversais (autenticação, formato da requisição, contrato JSON, rate limiter). |
| `Instartups.Command.Tests.Common` | Biblioteca | — | Builders de entidades e commands compartilhados. |

\* Quantidade de métodos `[Fact]`/`[Theory]` (cada `[Theory]` roda uma vez por caso).

## Stack

Definida uma vez em `tests/Directory.Build.props` para todo projeto cujo nome termina em `Tests`:

- **xUnit v3** rodando sobre **Microsoft.Testing.Platform** (`global.json` → `"test": { "runner": "Microsoft.Testing.Platform" }`; os projetos de teste são executáveis, `OutputType=Exe`).
- **NSubstitute** (+ analyzers) para mocks.
- **Shouldly** para asserções.
- **Microsoft.Testing.Extensions.CodeCoverage** para cobertura.
- `using` globais de `Xunit`, `Shouldly` e `NSubstitute`.

Integração usa ainda `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`) e **Respawn**.

## Rodando

Na raiz da solution:

```bash
# todos os testes
dotnet test

# um projeto
dotnet test --project tests/Instartups.Command.Domain.UnitTests

# só os unitários (sem precisar de banco)
for p in Domain Application Infrastructure Api; do
  dotnet test --project tests/Instartups.Command.$p.UnitTests
done

# com cobertura
dotnet test --coverage --coverage-output-format cobertura
```

## Testes de integração

### Pré-requisitos

1. PostgreSQL com PostGIS acessível.
2. A connection string **`ConnectionStrings:InstartupsIntegrationDb`**, definida no `appsettings.Development.json` da Api (copiado para o output do projeto de testes) ou na variável de ambiente `ConnectionStrings__InstartupsIntegrationDb`:

   ```bash
   export ConnectionStrings__InstartupsIntegrationDb="Host=localhost;Port=5432;Database=instartups_db_integration_tests;Username=postgres;Password=<senha>"
   ```

3. **O nome do banco precisa terminar em `_tests`.** A fábrica se recusa a rodar em qualquer outro banco, porque os testes **apagam todos os dados** antes de cada caso.

Não é preciso ter a chave JWT nem rodar migrations manualmente.

### Como funciona

`Fixtures/InstartupsApiFactory` (um `WebApplicationFactory<Program>`):

- Força o ambiente `Development` (fora dele o Wolverine exige código pré-gerado).
- Sobrescreve `ConnectionStrings:PostgresConnection` com o banco de testes.
- Gera um par de chaves **RSA 2048 em memória**, grava a chave pública num arquivo temporário e aponta `JWT:PublicKeyPath`, `JWT:Issuer` e `JWT:Audience` para valores de teste.
- Reduz o nível de log para `Warning`.
- **Desliga o rate limiter** (substitui por `GetNoLimiter`), já que 5 req/20 s derrubaria a suíte.
- Na inicialização aplica as migrations (`Database.MigrateAsync()`) e cria um `Respawner` que limpa o schema `public`, ignorando `__EFMigrationsHistory` e `spatial_ref_sys`.

`Fixtures/IntegrationCollection` compartilha **um único host e um único banco** por toda a suíte e serializa a execução. `Fixtures/IntegrationTestBase` reseta o banco antes de cada teste e oferece helpers:

| Helper | Uso |
| --- | --- |
| `CriarCliente(usuarioId?)` | `HttpClient`, autenticado com um token válido para o usuário informado (ou anônimo). |
| `CriarClienteComToken(token)` | Cliente com um token arbitrário (expirado, outra chave…). |
| `PostAsync`, `PutAsync`, `PatchAsync`… | Atalhos de envio em JSON com o `CancellationToken` do teste. |

Outros fixtures:

- `JwtTokenFactory`: emite tokens RS256 no mesmo formato da `api-instartups-auth` (`sub`, `unique_name`, `email`, `PhoneNumber`, `roles`) e variações inválidas (`GerarExpirado`, `GerarComIssuer`, `GerarComAudience`, `GerarComOutraChave`, `GerarSemSub`, `GerarComSub`).
- `Rotas`: rotas dos endpoints centralizadas.
- `RespostaApi<T>`, `IdResposta`, `ErroValidacao`: espelhos do `ResponseDTO` para desserializar respostas (o original tem `private init`).
- `InstartupsApiComRateLimiterFactory`: variante com o **rate limiter real**, usada por `Transversal/RateLimiterTests`.

### Organização

```
tests/Instartups.Command.Api.IntegrationTests/
├── Fixtures/        # fábrica, base, tokens, rotas, modelos de resposta
├── Perfil/          # um arquivo por endpoint/fluxo de perfil
├── Postagem/        # um arquivo por endpoint/fluxo de postagem
└── Transversal/     # autenticação, formato da requisição, contrato JSON, rate limiter
```

## Testes unitários: convenções

- **Builders** (`Tests.Common/Builders`): `PerfilBuilder`, `PostagemBuilder`, `CurtidaPostagemBuilder`, `PerfilSeguidorBuilder`, `PerfilCommandBuilder`, `PostagemCommandBuilder`. Começam com dados válidos e permitem alterar só o que importa no teste. Estados que o domínio não expõe (perfil desativado, contadores) são definidos por reflexão (`Tests.Common/Extensions/ReflectionExtensions`).
- **`UnitOfWorkMock`** (Application): monta o `IUnitOfWork` com repositórios substitutos e métodos fluentes como `ComPerfilDoUsuario(perfil)` e `SemPerfilDoUsuario(id)`.
- **Bases de validators** (`Common/Validators/*TestsBase`): regras compartilhadas (perfil, startup, investidor, postagem) testadas uma vez e reaproveitadas pelos testes dos validators concretos.
- Api: `MessageBusExtensions` para configurar o `IMessageBus` substituto e `LoggerSpy` para verificar os logs do middleware.
- Nomes de teste em português no padrão `Metodo_Condicao_DeveResultado`, por exemplo `Handle_QuandoMidiaRemovidaNaoExiste_DeveLancarMidiaPostagemNaoEncontradaException`.

## Git hooks (Husky.Net)

Configurados em `.husky/task-runner.json` e instalados automaticamente no primeiro restore/build:

| Hook | Executa |
| --- | --- |
| `pre-commit` | `dotnet build --no-restore` |
| `pre-push` | `dotnet test` (inclui integração, então o banco de testes precisa estar disponível) |

Para pular a instalação dos hooks (por exemplo em CI), defina `HUSKY=0` ou `CI=true`.
