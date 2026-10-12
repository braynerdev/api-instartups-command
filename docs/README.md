# Instartups Command API

API do **lado de escrita (Command)** da Instartups, plataforma que conecta startups do Porto Digital a mentores e investidores anjo do Nordeste.

O sistema segue CQRS em serviços separados:

| Serviço | Responsabilidade |
| --- | --- |
| `api-instartups-auth` | Cadastro, login e emissão do JWT (RS256). |
| **`api-instartups-command`** (este) | Todas as operações que **alteram estado**: perfis, postagens, curtidas e seguidores. |
| API de Query | Consultas e leituras. Esta API não expõe nenhum `GET`. |

## Índice

| Documento | Conteúdo |
| --- | --- |
| [Arquitetura](ARQUITETURA.md) | Camadas, fluxo de uma requisição, pipeline HTTP e padrões usados. |
| [Configuração](CONFIGURACAO.md) | Chaves de configuração, JWT, CORS, rate limit, logs, Wolverine e ambientes. |
| [Referência da API](API.md) | Convenções e todos os endpoints, com exemplos de requisição e resposta. |
| [Validações e erros](VALIDACOES-E-ERROS.md) | Regras de cada campo, códigos de erro, exceções e status HTTP. |
| [Domínio](DOMINIO.md) | Entidades, value objects, enums e regras de negócio. |
| [Banco de dados](BANCO-DE-DADOS.md) | Tabelas, colunas, índices, relacionamentos e migrations. |
| [Migrations](MIGRATE.md) | Comandos do `dotnet ef` para criar e aplicar migrations. |
| [Testes](TESTES.md) | Projetos de teste, como rodar e a infraestrutura dos testes de integração. |
| [Contribuindo](CONTRIBUINDO.md) | Passo a passo para criar um novo caso de uso e as convenções do projeto. |

## Stack

| Tecnologia | Uso |
| --- | --- |
| .NET 10 / ASP.NET Core (SDK `10.0.100`, ver `global.json`) | Runtime e Web API com controllers. |
| [Wolverine](https://wolverinefx.net/) 6 (`DurabilityMode.MediatorOnly`) | Mediator: envia os commands para os handlers. |
| FluentValidation 12 (via `WolverineFx.FluentValidation`) | Validação dos commands antes do handler. |
| EF Core 10 + Npgsql + NetTopologySuite | Persistência em PostgreSQL com PostGIS. |
| JWT Bearer (RS256) | Autenticação com a chave pública emitida pela `api-instartups-auth`. |
| Serilog | Logs estruturados no console. |
| OpenAPI + [Scalar](https://scalar.com/) | Documentação interativa em `/docs` (só em Development). |
| Mapster | Registrado na DI (`AddMapster`), ainda sem mapeamentos próprios. |
| xUnit v3, NSubstitute, Shouldly, Respawn | Testes unitários e de integração. |
| Husky.Net | Git hooks: build no pre-commit e testes no pre-push. |

## Estrutura da solution

```
api-instartups-command/
├── Instartups.Command.slnx
├── global.json                         # SDK .NET e runner de testes (Microsoft.Testing.Platform)
├── Instartups.Command.Api/             # Controllers, DTOs, middleware, configuração do host
├── Instartups.Command.Application/     # Commands, handlers, validators, interfaces (UoW, repositórios)
├── Instartups.Command.Domain/          # Entidades, value objects, enums, domain services, exceções, constantes
├── Instartups.Command.Infrastructure/  # EF Core (DbContext, configurations, migrations), repositórios, UoW
├── tests/                              # 5 projetos de teste + Tests.Common (builders)
├── docs/                               # Esta documentação
└── .husky/                             # Git hooks
```

Detalhes em [Arquitetura](ARQUITETURA.md).

## Rodando localmente

### Pré-requisitos

- .NET SDK 10.
- PostgreSQL com a extensão **PostGIS** instalada.
- A **chave pública** RSA (PEM) usada pela `api-instartups-auth` para assinar os tokens (no monorepo fica em `backend/keys/jwt-public.pem`).
- `dotnet-ef` para aplicar as migrations: `dotnet tool install --global dotnet-ef`.

### Passo a passo

Todos os comandos a partir da raiz da solution (`backend/api-instartups-command`).

1. **Configure os segredos.** Prefira `user-secrets` a editar o `appsettings.Development.json`:

   ```bash
   dotnet user-secrets init --project Instartups.Command.Api   # só na primeira vez

   dotnet user-secrets set "ConnectionStrings:PostgresConnection" \
     "Host=localhost;Port=5432;Database=instartups_db;Username=postgres;Password=<senha>" \
     --project Instartups.Command.Api

   dotnet user-secrets set "JWT:PublicKeyPath" "/caminho/absoluto/para/jwt-public.pem" \
     --project Instartups.Command.Api
   ```

   `JWT:Issuer` e `JWT:Audience` precisam ser **iguais** aos usados pela `api-instartups-auth`. Veja [Configuração](CONFIGURACAO.md).

2. **Aplique as migrations:**

   ```bash
   dotnet ef database update --project Instartups.Command.Infrastructure --startup-project Instartups.Command.Api
   ```

3. **Suba a API:**

   ```bash
   dotnet run --project Instartups.Command.Api
   ```

   O perfil `http` do `launchSettings.json` sobe em **http://localhost:8081** com `ASPNETCORE_ENVIRONMENT=Development` e abre a documentação Scalar em **http://localhost:8081/docs** (o JSON do OpenAPI fica em `/openapi/v1.json`).

4. **Obtenha um token** na `api-instartups-auth` e informe-o no Scalar (apenas o token, sem o prefixo `Bearer`).

> O primeiro `dotnet restore`/`build` também executa `dotnet tool restore` e `dotnet husky install` (target `Husky` no `.csproj` da Api). Para pular, defina `HUSKY=0` ou `CI=true`.

### Rodando os testes

```bash
dotnet test
```

Os testes de integração exigem um banco PostgreSQL dedicado cujo nome termine em `_tests`. Veja [Testes](TESTES.md).
