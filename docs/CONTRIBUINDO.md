# Contribuindo

[← Índice](README.md)

## Criando um novo caso de uso

Exemplo: "fixar postagem no perfil" (`PATCH /postagem/{postagemId}/fixar`). Siga as camadas de dentro para fora.

### 1. Domínio (`Instartups.Command.Domain`)

- Coloque a regra na **entidade** quando ela envolve só a própria entidade (ex.: `PostagemEntity.Fixar()`), ou num **domain service** estático em `Services/` quando envolve mais de uma (ex.: precisa do perfil autor).
- Mantenha os setters privados e chame `Atualizar()` em toda mutação.
- Se o fluxo altera contadores de outra entidade, exponha o método como `internal` e chame-o pelo domain service.
- Crie as exceções necessárias em `Exceptions/`, herdando da base que corresponde ao status HTTP desejado:

  | Base | Status | Use quando |
  | --- | --- | --- |
  | `NotFoundException` | 404 | Recurso não existe. |
  | `ConflictException` | 409 | O estado já é o pedido (já fixada, já curtiu…). |
  | `ForbiddenException` | 403 | O usuário não tem direito sobre o recurso. |
  | `DomainException` | 422 | Regra de negócio violada. |
  | `UnauthorizedException` | 401 | Identidade inválida. |

  ```csharp
  public class PostagemJaFixadaException : ConflictException
  {
      public PostagemJaFixadaException()
          : base("A postagem já está fixada.")
      {
      }
  }
  ```

- Novos tamanhos de coluna vão em `Constants/TamanhosColunasConst`, e novas mensagens/códigos de validação em `MensagensErroConst`/`CodigosErroConst`.

### 2. Application (`Instartups.Command.Application/UseCases/FixarPostagem/`)

```csharp
// FixarPostagemCommand.cs
public sealed record FixarPostagemCommand(
    Guid PostagemId,
    Guid UserId
) : ICommand;

// FixarPostagemCommandValidator.cs
public class FixarPostagemCommandValidator : AbstractValidator<FixarPostagemCommand>
{
    public FixarPostagemCommandValidator()
    {
        RuleFor(x => x.PostagemId)
            .ValidarId();
    }
}

// FixarPostagemCommandHandler.cs
public class FixarPostagemCommandHandler(
        IUnitOfWork unitOfWork
    ) : IVoidCommandHandler<FixarPostagemCommand>
{
    public async Task Handle(FixarPostagemCommand command, CancellationToken ct)
    {
        var perfil = await ObterPerfil(command.UserId, ct);
        var postagem = await ObterPostagem(command.PostagemId, ct);
        // regra de domínio...
        await unitOfWork.CommitAsync(ct);
    }

    private async Task<PerfilEntity> ObterPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfil = await unitOfWork.Perfil.ObterPorUsuarioIdAsync(usuarioId, ct);
        return perfil ?? throw new PerfilNaoEncontradoException();
    }

    // ...
}
```

Regras:

- O handler precisa ter sufixo `Handler` e um método `Handle(command, ct)` para que o Wolverine o encontre. O validator é descoberto automaticamente.
- Use `ICommandHandler<TCommand, IdResponse>` se o caso de uso cria um recurso, e `IVoidCommandHandler<TCommand>` nos demais.
- Reaproveite as extensões de `Common/Validator/Extensions` (`ValidarId`, `ValidarNome`, `ValidarDescricaoPostagem`…). Para uma regra de campo nova, crie uma extensão no mesmo formato, sempre com `.WithErrorCode(CodigosErroConst.X).WithMessage(MensagensErroConst.X)`.
- O handler **orquestra**: carrega entidades, chama o domínio, adiciona/remove pelo repositório e faz **um** `CommitAsync`. Regras de negócio ficam no domínio.
- Se precisar de uma consulta nova (por exemplo com `Include`), declare-a na interface em `Interfaces/Repositories/` e implemente no repositório da Infrastructure.

### 3. Infrastructure

- Novas entidades: crie `XConfig : BaseConf<XEntity>` em `Persistence/Configurations/`, chame `base.Configure(builder)`, use `snake_case` e os tamanhos de `TamanhosColunasConst`. Adicione o `DbSet` no `AppDbContext`.
- Novos repositórios: implemente sobre `RepositoriesGeneric<T>` e exponha no `IUnitOfWork`/`UnitOfWork`.
- Mudou o modelo? Crie uma migration (ver [MIGRATE.md](MIGRATE.md)) e revise o arquivo gerado.

### 4. Api

- Se o endpoint tem corpo, crie um DTO `sealed record` em `DTOs/<Recurso>/` com um método `ToCommand(...)` que recebe o `userId` e os ids de rota. Não coloque validação no DTO.
- Adicione a action no controller do recurso:

  ```csharp
  [HttpPatch("{postagemId:guid}/fixar")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
  [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status403Forbidden)]
  [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
  [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
  public async Task<IActionResult> Fixar([FromRoute] Guid postagemId, CancellationToken ct)
  {
      var userId = PegarContextoRequisicao.UserId;
      var command = new FixarPostagemCommand(postagemId, userId);

      await MessageBus.InvokeAsync(command, ct);

      return NoContent();
  }
  ```

- Declare **todos** os status possíveis com `ProducesResponseType` (401 e 429 já são globais) para que o OpenAPI fique correto.
- Cadastros retornam `201` com `ResponseDTO<IdResponse>.Success(MensagensSucessoConst.X, response)`; as demais operações retornam `204`.
- Nunca pegue o id do usuário do corpo: use sempre `IPegarContextoRequisicao.UserId`.

### 5. Testes

- **Domain.UnitTests:** regras da entidade ou do domain service (caminho feliz e cada exceção).
- **Application.UnitTests:** validator (cada regra/código) e handler (com `UnitOfWorkMock`: não encontrado, regra violada, `CommitAsync` chamado).
- **Api.UnitTests:** a action monta o command certo e retorna o status certo; o DTO converte corretamente.
- **Api.IntegrationTests:** o fluxo completo contra o banco, incluindo os status de erro. Adicione a rota em `Fixtures/Rotas.cs`.

Veja [Testes](TESTES.md).

### 6. Documentação

Atualize [API.md](API.md) (endpoint e erros), [VALIDACOES-E-ERROS.md](VALIDACOES-E-ERROS.md) (novas regras e exceções), [DOMINIO.md](DOMINIO.md) (regras de negócio) e [BANCO-DE-DADOS.md](BANCO-DE-DADOS.md) (se mudou o schema).

## Convenções de código

| Elemento | Convenção | Exemplo |
| --- | --- | --- |
| Idioma | Domínio e nomes em **português** | `CadastrarPostagem`, `TotalSeguidores` |
| Entidades | Sufixo `Entity`, `sealed` | `PostagemEntity` |
| Value objects | Sufixo `VO`, `sealed record` | `TotalVO` |
| Enums | Sufixo `Enum`, valores em PascalCase começando em 1 | `TiposMidiaEnum.Foto` |
| Constantes | Classe estática com sufixo `Const` | `RotasBaseConst` |
| Exceções | Sufixo `Exception`, mensagem fixa no construtor | `PostagemJaCurtidaException` |
| Casos de uso | Verbo + substantivo; `Command`, `CommandValidator`, `CommandHandler` | `DeixarDeSeguirPerfilCommandHandler` |
| DTOs | Sufixo `DTO`, `sealed record` | `CadastrarPostagemDTO` |
| Configuração do host | Classe estática `XConfig` com métodos de extensão `AddXConfig`/`UseXConfig` | `RateLimiterConfig` |
| Tabelas/colunas | `snake_case`, tabelas no plural | `curtidas_postagens.perfil_autor_curtida_id` |

Estilo de código: seguir o `.editorconfig` da raiz. `Nullable` e `ImplicitUsings` estão habilitados em todos os projetos.

## Commits

O histórico usa **Conventional Commits** com descrição em português:

```
feat: implementando pipeline de curtir e descurtir postagem
fix: corrigindo factory dos testes
refactor: padronizando retorno
chore: atualizando versão dos pacotes
```

Antes de cada commit o Husky roda `dotnet build`, e antes de cada push roda `dotnet test`. Mantenha o banco de testes de integração disponível (ver [Testes](TESTES.md#testes-de-integração)).
