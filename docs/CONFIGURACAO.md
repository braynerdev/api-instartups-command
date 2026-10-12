# Configuração

[← Índice](README.md)

A configuração segue o padrão do ASP.NET Core: `appsettings.json` → `appsettings.{Ambiente}.json` → user-secrets (em Development) → variáveis de ambiente → argumentos de linha de comando. A fonte mais à direita vence.

Em variáveis de ambiente, use `__` como separador de seção, por exemplo `ConnectionStrings__PostgresConnection` e `JWT__PublicKeyPath`.

## Chaves de configuração

| Chave | Obrigatória | Descrição |
| --- | --- | --- |
| `ConnectionStrings:PostgresConnection` | Sim | Connection string do PostgreSQL (Npgsql). Sem ela a aplicação não sobe (`Connection string 'PostgresConnection' não configurada.`). |
| `ConnectionStrings:InstartupsIntegrationDb` | Só para testes de integração | Banco dedicado aos testes; o nome **precisa** terminar em `_tests`. Veja [Testes](TESTES.md). |
| `JWT:PublicKeyPath` | Sim | Caminho do arquivo PEM com a **chave pública RSA** da `api-instartups-auth`. O arquivo é lido na inicialização. |
| `JWT:Issuer` | Sim | Emissor (`iss`) esperado no token. |
| `JWT:Audience` | Sim | Público (`aud`) esperado no token. |
| `CorsPadrao` | Sim | Array de origens permitidas pela política CORS `FRONTEND`. |
| `Serilog` | Sim | Níveis mínimos, sinks e propriedades dos logs (ver abaixo). |
| `AllowedHosts` | Não | Padrão do ASP.NET Core (`*`). |

`appsettings.json` traz apenas valores de exemplo (`EXEMPLO`, `NOME APP`, `https://IP:PORT`). Os valores reais devem vir de user-secrets, variáveis de ambiente ou de um cofre de segredos no ambiente de deploy.

Exemplo de `appsettings.Development.json` (sem segredos):

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Port=5432;Database=instartups_db;Username=postgres;Password=<senha>",
    "InstartupsIntegrationDb": "Host=localhost;Port=5432;Database=instartups_db_integration_tests;Username=postgres;Password=<senha>"
  },
  "JWT": {
    "PublicKeyPath": "/caminho/para/backend/keys/jwt-public.pem",
    "Issuer": "<mesmo issuer da api-instartups-auth>",
    "Audience": "<mesma audience da api-instartups-auth>"
  },
  "CorsPadrao": ["http://localhost:8080"]
}
```

> **Atenção:** o `appsettings.Development.json` versionado hoje contém a senha do banco local e um caminho absoluto de máquina para a chave. Prefira mover esses valores para `dotnet user-secrets` (comandos no [README](README.md#passo-a-passo)).

## Autenticação (JWT)

Configurada em `Api/Configurations/AuthenticationConfig.cs`.

- **Esquema:** JWT Bearer, assinatura **RS256**. A API só tem a chave **pública**, e os tokens são emitidos pela `api-instartups-auth`.
- **Validações:** issuer, audience, tempo de vida e assinatura. `ClockSkew = 0`: o token expira exatamente no `exp`.
- **Claims de papel:** `RoleClaimType = "roles"`.
- **Identidade do usuário:** claim `sub` (exposta como `ClaimTypes.NameIdentifier`), que precisa ser um GUID não vazio. Veja [Arquitetura › Usuário autenticado](ARQUITETURA.md#usuário-autenticado).
- `RequireHttpsMetadata = false` e `SaveToken = false`.
- **Respostas customizadas** (sempre `ResponseDTO<string>` com `isSuccess: false`):

  | Situação | Status | `message` |
  | --- | --- | --- |
  | Sem token, token inválido, assinatura/issuer/audience errados | 401 | `É necessário estar autenticado para acessar este recurso.` |
  | Token expirado | 401 | `O token de acesso expirou.` |
  | Autenticado mas sem permissão (política) | 403 | `Você não possui permissão para acessar este recurso.` |

## Autorização

`AuthorizationConfig.cs` registra a política **`Admin`** (`PermissoesConst.Admin`), que exige usuário autenticado com a claim `roles` = `Admin`. Hoje **nenhum endpoint usa essa política**: todos usam apenas `[Authorize]` (qualquer usuário autenticado).

## CORS

`CorsConfig.cs` cria a política `FRONTEND` (`OrigensCorsConst.Frontend`) com as origens de `CorsPadrao`, qualquer header e qualquer método. A política é aplicada globalmente com `app.UseCors(...)`.

## Rate limiting

`RateLimiterConfig.cs` define um **limitador global** do tipo *token bucket*, particionado por usuário:

| Parâmetro | Valor |
| --- | --- |
| Partição | `ClaimTypes.NameIdentifier` do token; requisições sem usuário compartilham a partição `anonymous`. |
| `TokenLimit` | 5 |
| `TokensPerPeriod` / `ReplenishmentPeriod` | 5 tokens a cada 20 s |
| `QueueLimit` | 2 requisições aguardando (ordem `OldestFirst`) |
| Rejeição | `429 Too Many Requests` com `message`: `Limite de requisições atingido. Tente novamente em instantes.` |

Na prática, cada usuário pode fazer uma rajada de 5 requisições, mais 2 enfileiradas, a cada 20 segundos.

## Logs (Serilog)

- Lido da seção `Serilog` (`ReadFrom.Configuration`), enriquecido com `LogContext` e com a propriedade `Application`.
- Sinks configurados: `Console` (tema ANSI) e `Debug`.
- Níveis padrão: `Information`. Em Development, `Microsoft` e `Wolverine` ficam em `Fatal` para reduzir ruído; em produção, `Warning`.
- O Wolverine não loga execução nem sucesso das mensagens (`MessageExecutionLogLevel` e `MessageSuccessLogLevel` = `None`).
- **Log por requisição** (`UseSerilogRequestLogging`) com as propriedades `HostRequisicao`, `CaminhoRequisicao` e `Metodo`. Nível:
  - `Error` se houve exceção ou status ≥ 500;
  - `Warning` se demorou mais de 1000 ms;
  - `Information` nos demais casos.
- O `ExceptionsMiddleware` loga `Error` (com stack trace) para erros 500 e `Warning` (`Erro de negócio: <mensagem>`) para os demais.

## Wolverine

`WolverineConfig.cs`:

| Opção | Valor | Efeito |
| --- | --- | --- |
| `Durability.Mode` | `MediatorOnly` | Wolverine é usado só como mediator em memória (sem outbox, filas ou transporte). |
| `Discovery.IncludeAssembly` | assembly da Application | Onde handlers e validators são descobertos. |
| `UseFluentValidation()` | — | Executa os validators antes dos handlers. |
| `CodeGeneration.AlwaysUseServiceLocationFor<AppDbContext>()` | — | Resolve o `DbContext` pelo container a cada execução. |
| `CritterStackDefaults` | Development: `TypeLoadMode.Dynamic`<br/>Produção: `TypeLoadMode.Static` + `AssertAllPreGeneratedTypesExist = true` | Fora de Development o Wolverine **exige código pré-gerado** para os handlers. |

> **Atenção (deploy):** não há código pré-gerado versionado no repositório, então fora de `Development` a aplicação falha na inicialização. Os testes de integração forçam `Development` justamente por isso. Para gerar o código é preciso habilitar a CLI do JasperFx: o `Program.cs` hoje termina com `app.Run()` e precisaria terminar com `return await app.RunJasperFxCommands(args);`. Depois disso, `dotnet run --project Instartups.Command.Api -- codegen write` grava os arquivos gerados, que devem entrar no build.

## Controllers e serialização

`ControllersConfig.cs` aplica a todos os controllers:

- `[Produces("application/json")]` e `[Consumes("application/json")]`.
- `ReturnHttpNotAcceptable = true`: um `Accept` que não aceite JSON recebe `406`.
- `AllowEmptyInputInBodyModelBinding = false`: body vazio em endpoints com `[FromBody]` gera `400`.
- `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true`: a obrigatoriedade é decidida pelos validators, não pelo model binding.
- Resposta customizada para `ModelState` inválido (JSON malformado, tipo errado): `400` com `FORMATO_INVALIDO`. Veja [Validações e erros](VALIDACOES-E-ERROS.md#erros-de-formato-json-malformado).
- `LowerCasesConfig.cs`: URLs e query strings geradas em minúsculas.
- JSON com as opções padrão do ASP.NET Core (`camelCase`, case-insensitive na leitura).

## OpenAPI e Scalar

Apenas em **Development** (`OpenApiConfig.cs`):

- Documento `v1` em `/openapi/v1.json`, título `Instartups Command`, com o esquema de segurança `Bearer` (HTTP bearer, formato JWT) aplicado globalmente.
- Interface **Scalar** em `/docs`, com favicon `wwwroot/img/favicon-logo.svg` e configuração em `wwwroot/js/scalar.js` (tema `moon`, layout `classic`, cliente padrão Node.js Fetch, autenticação não persistida, telemetria desligada).

## Ambientes

| | Development | Demais ambientes |
| --- | --- | --- |
| OpenAPI/Scalar | Habilitados | Desabilitados |
| HSTS + redirecionamento HTTPS | Não | Sim |
| Código Wolverine | Gerado em runtime | Pré-gerado obrigatório |
| Logs `Microsoft`/`Wolverine` | `Fatal` | `Warning` |

## Portas

- `Properties/launchSettings.json`, perfil `http`: **http://localhost:8081**, abrindo `/docs`.
- `Instartups.Command.Api.http` usa `http://localhost:8080`, que é a origem do frontend em `CorsPadrao`. Ajuste a variável `@Instartups.Api_HostAddress` para `8081` se for usar esse arquivo.
