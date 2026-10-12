# Referência da API

[← Índice](README.md)

Em ambiente de desenvolvimento, a referência interativa (Scalar) fica em `http://localhost:8081/docs`. Este documento descreve as convenções e cada endpoint, incluindo as regras de negócio que não aparecem no OpenAPI.

## Convenções

| Item | Valor |
| --- | --- |
| Rota base | `/api/instartups/command` (`RotasBaseConst`) |
| Recursos | `/perfil` e `/postagem` |
| Formato | `Content-Type: application/json` e `Accept: application/json`. Outro `Accept` → `406`. |
| JSON | Propriedades em `camelCase` (a leitura não diferencia maiúsculas). Datas `DateOnly` em `yyyy-MM-dd`. |
| URLs | Minúsculas. Ids de rota são GUIDs (restrição `:guid`; um id que não é GUID resulta em `404`). |
| Autenticação | **Todos** os endpoints exigem `Authorization: Bearer <jwt>`. |
| Rate limit | 5 requisições + 2 em fila a cada 20 s por usuário → `429`. |
| Leitura | Não há endpoints `GET`; consultas são feitas na API de Query. |

### Identidade: usuário × perfil

O usuário é sempre identificado pelo token (claim `sub`). Cada usuário tem **no máximo um perfil** (startup **ou** investidor), e todas as operações agem sobre o perfil do usuário autenticado. Não é preciso (nem possível) informar o id do próprio perfil.

Única exceção: em `/perfil/{seguidoId}/seguir`, `seguidoId` é o **id do perfil** a ser seguido (não o id do usuário dono dele).

### Envelope de resposta

Toda resposta com corpo usa `ResponseDTO<T>`:

```json
{
  "isSuccess": true,
  "message": "Perfil cadastrado com sucesso.",
  "dateTimeResponse": "2026-10-12T14:03:12.512Z",
  "data": { "id": "0199d6a4-6b1e-7c3a-9f0e-2a1b3c4d5e6f" }
}
```

| Campo | Tipo | Descrição |
| --- | --- | --- |
| `isSuccess` | boolean | `true` em sucesso, `false` em erro. |
| `message` | string | Mensagem legível para o usuário. |
| `dateTimeResponse` | string (ISO 8601, UTC) | Momento da resposta. |
| `data` | objeto \| array \| null | Payload: `{ "id": ... }` nos cadastros, lista de erros em `400`, `null` nos demais erros. |

Operações que não criam recurso retornam **`204 No Content`** sem corpo.

### Respostas de erro comuns

| Status | Quando | `data` |
| --- | --- | --- |
| `400` | Validação do command ou JSON malformado | `[{ "campo", "codigo", "mensagem" }]` |
| `401` | Token ausente, inválido ou expirado; `sub` ausente ou inválido | `null` |
| `403` | Usuário não é o autor da postagem | `null` |
| `404` | Perfil, postagem, mídia, curtida ou relação de seguir não encontrados | `null` |
| `409` | Estado já é o desejado (já ativo/desativado, já curtiu, já segue, já tem perfil) | `null` |
| `422` | Regra de negócio violada (perfil/postagem desativados, tipo de perfil errado, limite de mídias…) | `null` |
| `429` | Rate limit excedido | `null` |
| `500` | Erro inesperado (`message`: `Erro inesperado`) | `null` |

Exemplo de `400`:

```json
{
  "isSuccess": false,
  "message": "Erro de validação.",
  "dateTimeResponse": "2026-10-12T14:03:12.512Z",
  "data": [
    { "campo": "Perfil.Nome", "codigo": "CAMPO_OBRIGATORIO", "mensagem": "O campo é obrigatório." },
    { "campo": "Pitch", "codigo": "TAMANHO_MAXIMO", "mensagem": "O tamanho do campo deve ser no máximo 1000." }
  ]
}
```

Exemplo de erro de negócio:

```json
{
  "isSuccess": false,
  "message": "O usuário já possui um perfil cadastrado.",
  "dateTimeResponse": "2026-10-12T14:03:12.512Z",
  "data": null
}
```

O catálogo completo de mensagens e códigos está em [Validações e erros](VALIDACOES-E-ERROS.md).

## Resumo dos endpoints

| Método | Rota | Ação | Sucesso |
| --- | --- | --- | --- |
| `POST` | `/perfil/investidor` | [Cadastrar perfil de investidor](#cadastrar-perfil-de-investidor) | `201` |
| `PUT` | `/perfil/investidor` | [Editar perfil de investidor](#editar-perfil-de-investidor) | `204` |
| `POST` | `/perfil/startup` | [Cadastrar perfil de startup](#cadastrar-perfil-de-startup) | `201` |
| `PUT` | `/perfil/startup` | [Editar perfil de startup](#editar-perfil-de-startup) | `204` |
| `PATCH` | `/perfil/coordenada` | [Editar coordenada do perfil](#editar-coordenada-do-perfil) | `204` |
| `PATCH` | `/perfil/desativar` | [Desativar perfil](#desativar-perfil) | `204` |
| `PATCH` | `/perfil/reativar` | [Reativar perfil](#reativar-perfil) | `204` |
| `POST` | `/perfil/{seguidoId}/seguir` | [Seguir perfil](#seguir-perfil) | `204` |
| `DELETE` | `/perfil/{seguidoId}/seguir` | [Deixar de seguir perfil](#deixar-de-seguir-perfil) | `204` |
| `POST` | `/postagem` | [Cadastrar postagem](#cadastrar-postagem) | `201` |
| `PUT` | `/postagem/{postagemId}` | [Editar postagem](#editar-postagem) | `204` |
| `PATCH` | `/postagem/{postagemId}/desativar` | [Desativar postagem](#desativar-postagem) | `204` |
| `PATCH` | `/postagem/{postagemId}/reativar` | [Reativar postagem](#reativar-postagem) | `204` |
| `POST` | `/postagem/{postagemId}/curtir` | [Curtir postagem](#curtir-postagem) | `204` |
| `DELETE` | `/postagem/{postagemId}/curtir` | [Descurtir postagem](#descurtir-postagem) | `204` |

Todas as rotas acima são relativas a `/api/instartups/command`.

---

## Perfil

### Objetos compartilhados

**`perfil`**, usado no cadastro e na edição de startup e investidor:

| Campo | Tipo | Obrigatório | Regras |
| --- | --- | --- | --- |
| `nome` | string | Sim | Máx. 100 caracteres. Espaços nas pontas são removidos. |
| `imagemPerfilUrl` | string \| null | Não | URL absoluta `http`/`https`, máx. 400. |
| `imagemFundoUrl` | string \| null | Não | URL absoluta `http`/`https`, máx. 400. |
| `coordenada` | objeto | Sim | Ver abaixo. |

**`coordenada`**:

| Campo | Tipo | Obrigatório | Regras |
| --- | --- | --- | --- |
| `latitude` | number | Sim | Entre −90 e 90 e diferente de `0`. |
| `longitude` | number | Sim | Entre −180 e 180 e diferente de `0`. |

> Latitude/longitude `0` são rejeitadas com `CAMPO_OBRIGATORIO` (a regra `NotEmpty` trata `0` como vazio). Ver [Pontos de atenção](VALIDACOES-E-ERROS.md#pontos-de-atenção).

Na edição (`PUT`), **todos** os campos são substituídos: omitir `imagemPerfilUrl` ou `imagemFundoUrl` grava `null`.

### Cadastrar perfil de investidor

`POST /api/instartups/command/perfil/investidor`

Cria o perfil do usuário autenticado com tipo `INVESTIDOR`.

```json
{
  "perfil": {
    "nome": "Maria Anjo",
    "imagemPerfilUrl": "https://cdn.exemplo.com/perfil.png",
    "imagemFundoUrl": null,
    "coordenada": { "latitude": -8.0631, "longitude": -34.8711 }
  },
  "teseInvestimento": "Startups early-stage de healthtech e edtech no Nordeste.",
  "ticketMinimo": 50000.00,
  "ticketMaximo": 250000.00
}
```

| Campo | Regras |
| --- | --- |
| `teseInvestimento` | Obrigatório, máx. 1000. |
| `ticketMinimo` | Obrigatório, > 0, até 12 dígitos com 2 casas decimais. |
| `ticketMaximo` | Obrigatório, **maior que `ticketMinimo`**, até 12 dígitos com 2 casas decimais. |

**`201 Created`**

```json
{
  "isSuccess": true,
  "message": "Perfil cadastrado com sucesso.",
  "dateTimeResponse": "2026-10-12T14:03:12.512Z",
  "data": { "id": "<id do perfil>" }
}
```

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `409` | `O usuário já possui um perfil cadastrado.` |

### Editar perfil de investidor

`PUT /api/instartups/command/perfil/investidor`

Mesmo corpo e mesmas regras do cadastro. Substitui os dados do perfil e da tese/tickets.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `404` | `Perfil não encontrado.` (usuário sem perfil) |
| `422` | `O perfil não é do tipo INVESTIDOR.` · `Não é possível realizar esta ação porque o perfil está desativado.` |

### Cadastrar perfil de startup

`POST /api/instartups/command/perfil/startup`

Cria o perfil do usuário autenticado com tipo `STARTUP`.

```json
{
  "perfil": {
    "nome": "Saúde Já",
    "imagemPerfilUrl": null,
    "imagemFundoUrl": null,
    "coordenada": { "latitude": -8.0631, "longitude": -34.8711 }
  },
  "pitch": "Telemedicina para o interior de Pernambuco.",
  "dataFundacao": "2024-03-15",
  "tamanhoEquipe": 6,
  "valorBuscado": 500000.00
}
```

| Campo | Regras |
| --- | --- |
| `pitch` | Obrigatório, máx. 1000. |
| `dataFundacao` | Obrigatória (`yyyy-MM-dd`), não pode ser futura (comparada à data UTC atual). |
| `tamanhoEquipe` | Entre 1 e 32767. |
| `valorBuscado` | Obrigatório, > 0, até 12 dígitos com 2 casas decimais. |

**`201 Created`** com `data.id` do perfil, mensagem `Perfil cadastrado com sucesso.`

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `409` | `O usuário já possui um perfil cadastrado.` |

### Editar perfil de startup

`PUT /api/instartups/command/perfil/startup`

Mesmo corpo e mesmas regras do cadastro.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `404` | `Perfil não encontrado.` |
| `422` | `O perfil não é do tipo STARTUP.` · perfil desativado. |

### Editar coordenada do perfil

`PATCH /api/instartups/command/perfil/coordenada`

Atualiza só a localização do perfil (startup ou investidor).

```json
{ "latitude": -8.0476, "longitude": -34.8770 }
```

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `400` | Validação (ver regras de `coordenada`). |
| `404` | `Perfil não encontrado.` |
| `422` | Perfil desativado. |

### Desativar perfil

`PATCH /api/instartups/command/perfil/desativar`, sem corpo.

Exclusão lógica do perfil (`ativo = false`). Um perfil desativado não pode ser editado, criar postagens, curtir, seguir ou ser seguido. As postagens do perfil **não** são desativadas automaticamente.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `404` | `Perfil não encontrado.` |
| `409` | `O perfil já está desativado.` |

### Reativar perfil

`PATCH /api/instartups/command/perfil/reativar`, sem corpo.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `404` | `Perfil não encontrado.` |
| `409` | `O perfil já está ativo.` |

### Seguir perfil

`POST /api/instartups/command/perfil/{seguidoId}/seguir`, sem corpo.

| Parâmetro | Descrição |
| --- | --- |
| `seguidoId` | Id do **perfil** a seguir (GUID). |

Cria a relação e incrementa `totalSeguindo` do seguidor e `totalSeguidores` do seguido.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `404` | `Perfil não encontrado.` (usuário sem perfil ou `seguidoId` inexistente) |
| `409` | `Você já segue este perfil.` |
| `422` | `Um perfil não pode seguir a si mesmo.` · algum dos perfis está desativado. |

### Deixar de seguir perfil

`DELETE /api/instartups/command/perfil/{seguidoId}/seguir`, sem corpo.

Remove a relação e decrementa os contadores.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `404` | `Perfil não encontrado.` · `Você não segue este perfil.` |
| `422` | Algum dos perfis está desativado. |

---

## Postagem

### Objeto `midia`

| Campo | Tipo | Obrigatório | Regras |
| --- | --- | --- | --- |
| `url` | string | Sim | URL absoluta `http`/`https`, máx. 400. |
| `tipo` | string | Sim | `FOTO` ou `VIDEO` (não diferencia maiúsculas; espaços nas pontas são ignorados). Outro valor → `VALOR_INVALIDO_ENUM`. |

Uma postagem tem **no máximo 5 mídias**.

### Cadastrar postagem

`POST /api/instartups/command/postagem`

Cria uma postagem cujo autor é o perfil do usuário autenticado.

```json
{
  "descricao": "Fechamos nossa rodada pré-seed! 🚀",
  "midias": [
    { "url": "https://cdn.exemplo.com/foto1.jpg", "tipo": "FOTO" },
    { "url": "https://cdn.exemplo.com/video.mp4", "tipo": "VIDEO" }
  ]
}
```

| Campo | Regras |
| --- | --- |
| `descricao` | Obrigatória, máx. 500. |
| `midias` | Opcional (`null` ou ausente = nenhuma). Cada item segue o objeto `midia`. Máx. 5 itens. |

**`201 Created`**

```json
{
  "isSuccess": true,
  "message": "Postagem cadastrada com sucesso.",
  "dateTimeResponse": "2026-10-12T14:03:12.512Z",
  "data": { "id": "<id da postagem>" }
}
```

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `404` | `Perfil não encontrado.` (usuário sem perfil) |
| `422` | Perfil desativado · `Não é possível adicionar mais de 5 mídias a uma postagem.` |

### Editar postagem

`PUT /api/instartups/command/postagem/{postagemId}`

Substitui a descrição e aplica um *diff* de mídias: primeiro remove as indicadas, depois adiciona as novas.

```json
{
  "descricao": "Fechamos nossa rodada pré-seed! Obrigado a todos.",
  "midiasRemovidas": ["0199d6a4-7a10-7c3a-9f0e-000000000001"],
  "midiasAdicionadas": [
    { "url": "https://cdn.exemplo.com/foto2.jpg", "tipo": "FOTO" }
  ]
}
```

| Campo | Regras |
| --- | --- |
| `descricao` | Obrigatória, máx. 500. |
| `midiasRemovidas` | Opcional. Ids (GUID não vazio) de mídias **desta** postagem. |
| `midiasAdicionadas` | Opcional. Itens no formato `midia`. O total após a edição não pode passar de 5. |

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `403` | `Apenas o autor pode realizar esta ação na postagem.` |
| `404` | `Perfil não encontrado.` · `Postagem não encontrada.` · `Mídia da postagem não encontrada.` |
| `422` | Perfil desativado · `Não é possível realizar esta ação porque a postagem está desativada.` · limite de 5 mídias. |

### Desativar postagem

`PATCH /api/instartups/command/postagem/{postagemId}/desativar`, sem corpo.

Exclusão lógica. As curtidas da postagem são **subtraídas** do `totalCurtidas` do autor (e devolvidas na reativação).

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `400` | Validação (`postagemId` vazio). |
| `403` | Usuário não é o autor. |
| `404` | Perfil ou postagem não encontrados. |
| `409` | `A postagem já está desativada.` |
| `422` | Perfil desativado. |

### Reativar postagem

`PATCH /api/instartups/command/postagem/{postagemId}/reativar`, sem corpo.

Reativa a postagem e soma de volta as curtidas dela ao `totalCurtidas` do autor.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `403` | Usuário não é o autor. |
| `404` | Perfil ou postagem não encontrados. |
| `409` | `A postagem já está ativa.` |
| `422` | Perfil desativado. |

### Curtir postagem

`POST /api/instartups/command/postagem/{postagemId}/curtir`, sem corpo.

Registra a curtida do perfil autenticado e incrementa `totalCurtidas` da postagem **e** do perfil autor.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `404` | Perfil ou postagem não encontrados. |
| `409` | `Você já curtiu esta postagem.` |
| `422` | `Um perfil não pode curtir a própria postagem.` · postagem desativada · perfil do autor ou de quem curte desativado. |

### Descurtir postagem

`DELETE /api/instartups/command/postagem/{postagemId}/curtir`, sem corpo.

Remove a curtida e decrementa os contadores da postagem e do autor.

**`204 No Content`**

| Erro | Motivo |
| --- | --- |
| `400` | Validação. |
| `404` | `Perfil não encontrado.` · `Você não curtiu esta postagem.` |
| `422` | Postagem desativada · perfil do autor ou de quem curte desativado. |

---

## Exemplo com cURL

```bash
TOKEN="<jwt emitido pela api-instartups-auth>"

curl -i -X POST http://localhost:8081/api/instartups/command/postagem \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{ "descricao": "Olá, Porto Digital!", "midias": [] }'
```
