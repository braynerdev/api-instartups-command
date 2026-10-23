# Validações e erros

[← Índice](README.md)

## Visão geral

Uma requisição pode falhar em quatro pontos, nesta ordem:

1. **Pipeline HTTP:** autenticação (`401`), rate limit (`429`), `Accept` não suportado (`406`).
2. **Model binding:** JSON malformado ou de tipo incompatível (`400` `FORMATO_INVALIDO`).
3. **Validação do command** (FluentValidation, executado pelo Wolverine antes do handler): `400` com a lista de erros.
4. **Handler / domínio:** exceções tipadas convertidas pelo `ExceptionsMiddleware` (`401`, `403`, `404`, `409`, `422`, `500`).

Todas as respostas de erro usam o envelope `ResponseDTO` com `isSuccess: false` (ver [Referência da API](API.md#envelope-de-resposta)).

## Erros de validação (400)

```json
{
  "isSuccess": false,
  "message": "Erro de validação.",
  "dateTimeResponse": "2026-10-12T14:03:12.512Z",
  "data": [
    { "campo": "Perfil.Coordenada.Latitude", "codigo": "VALOR_ENTRE", "mensagem": "O valor do campo deve estar entre -90 e 90." },
    { "campo": "Midias[1].Tipo", "codigo": "VALOR_INVALIDO_ENUM", "mensagem": "O valor informado não é válido para este campo." }
  ]
}
```

- `campo` é o caminho da propriedade **no command** (PascalCase, com `.` para objetos aninhados e `[i]` para listas). Na maioria dos casos coincide com o JSON de entrada; nos endpoints de edição de postagem e seguir/curtir, ids de rota aparecem como `PostagemId`/`SeguidoId`.
- Todos os erros de todos os campos são retornados de uma vez.

### Códigos de erro (`CodigosErroConst`)

| Código | Mensagem (`MensagensErroConst`) |
| --- | --- |
| `CAMPO_OBRIGATORIO` | O campo é obrigatório. |
| `TAMANHO_MAXIMO` | O tamanho do campo deve ser no máximo `{MaxLength}`. |
| `VALOR_ENTRE` | O valor do campo deve estar entre `{From}` e `{To}`. |
| `VALOR_MINIMO` | O valor do campo deve ser maior que `{ComparisonValue}`. |
| `URL_INVALIDA` | O campo deve ser uma URL válida. |
| `PRECISAO_INCORRETA` | O valor do campo excede a precisão ou escala permitida. |
| `DATA_FUTURA` | O campo não pode ser uma data futura. |
| `VALOR_INVALIDO_ENUM` | O valor informado não é válido para este campo. |
| `FORMATO_INVALIDO` | Formato da requisição inválido. (model binding) |

### Regras por campo

As regras ficam em `Application/Common/Validator/Extensions/` (uma extensão por regra) e os limites de tamanho vêm de `Domain/Constants/TamanhosColunasConst.cs`, os mesmos usados nas colunas do banco.

| Campo | Onde | Regras (código) |
| --- | --- | --- |
| `Nome` | Perfil | obrigatório (`CAMPO_OBRIGATORIO`) · máx. 100 (`TAMANHO_MAXIMO`) |
| `ImagemPerfilUrl` | Perfil | opcional; se informado: máx. 400 (`TAMANHO_MAXIMO`) · URL absoluta http/https (`URL_INVALIDA`) |
| `ImagemFundoUrl` | Perfil | idem `ImagemPerfilUrl` |
| `Coordenada` | Perfil, editar coordenada | obrigatório (`CAMPO_OBRIGATORIO`) |
| `Latitude` | Coordenada | obrigatório e ≠ 0 (`CAMPO_OBRIGATORIO`) · −90 a 90 (`VALOR_ENTRE`) |
| `Longitude` | Coordenada | obrigatório e ≠ 0 (`CAMPO_OBRIGATORIO`) · −180 a 180 (`VALOR_ENTRE`) |
| `Perfil` | Startup, investidor | obrigatório (`CAMPO_OBRIGATORIO`) |
| `TeseInvestimento` | Investidor | obrigatório · máx. 1000 |
| `TicketMinimo` | Investidor | obrigatório e ≠ 0 · > 0 (`VALOR_MINIMO`) · precisão 12, escala 2 (`PRECISAO_INCORRETA`) |
| `TicketMaximo` | Investidor | obrigatório e ≠ 0 · > `TicketMinimo` (`VALOR_MINIMO`) · precisão 12, escala 2 |
| `Pitch` | Startup | obrigatório · máx. 1000 |
| `DataFundacao` | Startup | obrigatório (≠ `0001-01-01`) · ≤ hoje em UTC (`DATA_FUTURA`) |
| `TamanhoEquipe` | Startup | 1 a 32767 (`VALOR_ENTRE`) |
| `ValorBuscado` | Startup | obrigatório e ≠ 0 · > 0 (`VALOR_MINIMO`) · precisão 12, escala 2 |
| `Descricao` | Postagem | obrigatório · máx. 500 |
| `Midias[i]` / `MidiasAdicionadas[i]` | Postagem | item não nulo (`CAMPO_OBRIGATORIO`) |
| `Midias[i].Url` | Postagem | obrigatório · máx. 400 · URL absoluta http/https |
| `Midias[i].Tipo` | Postagem | `FOTO` ou `VIDEO`, sem diferenciar maiúsculas (`VALOR_INVALIDO_ENUM`) |
| `MidiasRemovidas[i]` | Editar postagem | GUID não vazio (`CAMPO_OBRIGATORIO`) |
| `PostagemId`, `SeguidoId`, `AutorId`, `UserId` | Commands | GUID não vazio (`CAMPO_OBRIGATORIO`) |

> "Precisão 12, escala 2" significa até 12 dígitos no total, com no máximo 2 casas decimais (até `9.999.999.999,99`). Zeros à direita são ignorados (`ignoreTrailingZeros: true`).

### Erros de formato (JSON malformado)

Quando o corpo não pode ser desserializado (JSON inválido, tipo errado como texto em `ticketMinimo`, data fora do formato `yyyy-MM-dd`, GUID inválido em `midiasRemovidas`) ou o corpo está vazio, o ASP.NET rejeita antes de chegar ao Wolverine:

```json
{
  "isSuccess": false,
  "message": "Formato inválido.",
  "dateTimeResponse": "2026-10-12T14:03:12.512Z",
  "data": [
    { "campo": "$.ticketMinimo", "codigo": "FORMATO_INVALIDO", "mensagem": "Formato da requisição inválido." }
  ]
}
```

O `campo` aqui é o caminho do model binding (por exemplo `$.ticketMinimo` ou `dto`), não o do command.

## Mapeamento de exceções para HTTP

`Api/Middleware/ExceptionsMiddleware.cs` decide o status pela **classe base** da exceção:

| Exceção | Status | `message` | `data` |
| --- | --- | --- | --- |
| `FluentValidation.ValidationException` | 400 | `Erro de validação.` | lista de `ValidationErrorDTO` |
| `Domain…Base.ValidationException` | 400 | mensagem da exceção | `null` |
| `UnauthorizedException` | 401 | mensagem da exceção | `null` |
| `ForbiddenException` | 403 | mensagem da exceção | `null` |
| `NotFoundException` | 404 | mensagem da exceção | `null` |
| `ConflictException` | 409 | mensagem da exceção | `null` |
| `DomainException` | 422 | mensagem da exceção | `null` |
| `OperationCanceledException` | 499 | `Requisição cancelada pelo cliente` | `null` |
| Qualquer outra | 500 | `Erro inesperado` | `null` |

Todas as exceções de negócio herdam de `BaseException` (em `Domain/Exceptions/Base/`). Erros 500 são logados como `Error` com stack trace; os demais como `Warning`.

## Catálogo de exceções de domínio

22 exceções em `Instartups.Command.Domain/Exceptions/`:

| Exceção | Base | Status | Mensagem |
| --- | --- | --- | --- |
| `UsuarioNaoIdentificadoException` | `UnauthorizedException` | 401 | Não foi possível identificar o usuário autenticado. |
| `PerfilNaoEhAutorPostagemException` | `ForbiddenException` | 403 | Apenas o autor pode realizar esta ação na postagem. |
| `PerfilNaoEncontradoException` | `NotFoundException` | 404 | Perfil não encontrado. |
| `PostagemNaoEncontradaException` | `NotFoundException` | 404 | Postagem não encontrada. |
| `MidiaPostagemNaoEncontradaException` | `NotFoundException` | 404 | Mídia da postagem não encontrada. |
| `PerfilNaoSeguidoException` | `NotFoundException` | 404 | Você não segue este perfil. |
| `PostagemNaoCurtidaException` | `NotFoundException` | 404 | Você não curtiu esta postagem. |
| `UsuarioJaPossuiPerfilException` | `ConflictException` | 409 | O usuário já possui um perfil cadastrado. |
| `PerfilJaAtivoException` | `ConflictException` | 409 | O perfil já está ativo. |
| `PerfilJaDesativadoException` | `ConflictException` | 409 | O perfil já está desativado. |
| `PerfilJaSeguidoException` | `ConflictException` | 409 | Você já segue este perfil. |
| `PostagemJaAtivaException` | `ConflictException` | 409 | A postagem já está ativa. |
| `PostagemJaDesativadaException` | `ConflictException` | 409 | A postagem já está desativada. |
| `PostagemJaCurtidaException` | `ConflictException` | 409 | Você já curtiu esta postagem. |
| `PerfilDesativadoException` | `DomainException` | 422 | Não é possível realizar esta ação porque o perfil está desativado. |
| `PerfilNaoEhInvestidorException` | `DomainException` | 422 | O perfil não é do tipo INVESTIDOR. |
| `PerfilNaoEhStartupException` | `DomainException` | 422 | O perfil não é do tipo STARTUP. |
| `PerfilNaoPodeSeguirASiMesmoException` | `DomainException` | 422 | Um perfil não pode seguir a si mesmo. |
| `PerfilNaoPodeCurtirPropriaPostagemException` | `DomainException` | 422 | Um perfil não pode curtir a própria postagem. |
| `PostagemDesativadaException` | `DomainException` | 422 | Não é possível realizar esta ação porque a postagem está desativada. |
| `LimiteMidiasPostagemExcedidoException` | `DomainException` | 422 | Não é possível adicionar mais de 10 mídias a uma postagem. |
| `TotalNegativoException` | `DomainException` | 422 | O total não pode ser negativo. |

## Erros do pipeline

| Status | Origem | `message` |
| --- | --- | --- |
| 401 | `JwtBearerEvents.OnChallenge`: sem token ou token inválido | É necessário estar autenticado para acessar este recurso. |
| 401 | `OnChallenge` com `SecurityTokenExpiredException` | O token de acesso expirou. |
| 403 | `JwtBearerEvents.OnForbidden` | Você não possui permissão para acessar este recurso. |
| 406 | `ReturnHttpNotAcceptable` | (sem corpo) |
| 415 | `Content-Type` diferente de `application/json` | (resposta padrão do ASP.NET) |
| 429 | Rate limiter | Limite de requisições atingido. Tente novamente em instantes. |

## Pontos de atenção

Comportamentos do código atual que vale conhecer (não são necessariamente bugs, mas surpreendem):

- **Latitude/longitude `0` são rejeitadas.** `ValidarLatitude`/`ValidarLongitude` usam `NotEmpty()`, que para `double` trata `0` como vazio. Coordenadas exatamente sobre o equador ou o meridiano de Greenwich retornam `CAMPO_OBRIGATORIO`. Não afeta o Nordeste, mas a regra não está explícita.
- **Valores monetários `0` geram dois erros** (`CAMPO_OBRIGATORIO` e `VALOR_MINIMO`), porque as regras não interrompem a cadeia na primeira falha.
- **Descurtir exige postagem ativa:** não é possível remover a curtida de uma postagem desativada (`422`).
- **Desativar um perfil não desativa suas postagens.** As postagens continuam ativas, mas ninguém consegue curti-las (o autor está desativado → `422`).
- **Comparação de autor em `CurtirPostagemService`** lança `ArgumentException` (→ `500`) se o autor passado não for o da postagem. Isso só aconteceria por erro de programação, porque o handler sempre passa `postagem.Autor`.
