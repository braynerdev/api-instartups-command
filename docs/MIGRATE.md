# Migrations (EF Core)

O `AppDbContext` e as migrations ficam no projeto **Instartups.Command.Infrastructure** (`Persistence/` e `Persistence/Migrations/`). A configuração (connection string e DI) é lida do projeto **Instartups.Command.Api**.

## Pré-requisitos

- .NET SDK 10 (ver `global.json`)
- Ferramenta `dotnet-ef` instalada:

  ```bash
  dotnet tool install --global dotnet-ef
  # ou, se já estiver instalada:
  dotnet tool update --global dotnet-ef
  ```

- PostgreSQL com a extensão **PostGIS** disponível (o `AppDbContext` habilita `postgis`).
- Connection string `ConnectionStrings:PostgresConnection` configurada. Prefira `user-secrets` em vez de colocar a senha no `appsettings.json`:

  ```bash
  # só na primeira vez, se o projeto ainda não tiver UserSecretsId
  dotnet user-secrets init --project Instartups.Command.Api

  dotnet user-secrets set "ConnectionStrings:PostgresConnection" \
    "Host=localhost;Port=5432;Database=instartups;Username=postgres;Password=postgres" \
    --project Instartups.Command.Api
  ```

- `JWT:PublicKeyPath` apontando para a chave pública (ex.: `keys/jwt-public.pem`). O `dotnet ef` sobe o host da Api para montar o `DbContext`, então a configuração de autenticação também precisa estar válida:

  ```bash
  dotnet user-secrets set "JWT:PublicKeyPath" "$(pwd)/keys/jwt-public.pem" \
    --project Instartups.Command.Api
  ```

## Comandos

Todos os comandos devem ser executados na **raiz da solution**.

### Criar uma migration

Use um nome em PascalCase que descreva a mudança (ex.: `AdicionaTabelaPostagem`).

```bash
dotnet ef migrations add <NomeDaMigration> \
  --project Instartups.Command.Infrastructure \
  --startup-project Instartups.Command.Api \
  --output-dir Persistence/Migrations
```

### Aplicar as migrations no banco

```bash
dotnet ef database update \
  --project Instartups.Command.Infrastructure \
  --startup-project Instartups.Command.Api
```

### Listar as migrations

```bash
dotnet ef migrations list \
  --project Instartups.Command.Infrastructure \
  --startup-project Instartups.Command.Api
```

### Remover a última migration (ainda não aplicada)

```bash
dotnet ef migrations remove \
  --project Instartups.Command.Infrastructure \
  --startup-project Instartups.Command.Api
```

### Reverter o banco para uma migration específica

Informe o nome da migration de destino. Use `0` para desfazer todas.

```bash
dotnet ef database update <NomeDaMigration> \
  --project Instartups.Command.Infrastructure \
  --startup-project Instartups.Command.Api
```

### Gerar script SQL (ex.: para produção)

`--idempotent` gera um script que pode ser executado com segurança em qualquer estado do banco.

```bash
dotnet ef migrations script --idempotent -o migrations.sql \
  --project Instartups.Command.Infrastructure \
  --startup-project Instartups.Command.Api
```

## Parâmetros

| Parâmetro           | Descrição                                                          |
| ------------------- | ------------------------------------------------------------------ |
| `--project`         | Projeto onde ficam o `DbContext` e as migrations (Infrastructure). |
| `--startup-project` | Projeto que carrega a configuração e a injeção de dependência (Api). |
| `--output-dir`      | Pasta das migrations, relativa ao `--project`.                     |

## Dicas e problemas comuns

- **`Connection string 'PostgresConnection' não configurada.`** — a connection string não foi encontrada; configure via `user-secrets` ou `appsettings.Development.json`.
- **`The value cannot be an empty string. (Parameter 'path')`** seguido de **`Unable to create a 'DbContext' of type 'AppDbContext'`** — o `JWT:PublicKeyPath` está vazio; configure conforme os pré-requisitos.
- **Não edite uma migration já aplicada ou commitada.** Crie uma nova migration com a correção.
- **Revise o arquivo gerado** antes de commitar, principalmente operações de `DropColumn`/`DropTable`.
- `--output-dir` só é obrigatório na primeira migration, mas mantê-lo não causa problema.
