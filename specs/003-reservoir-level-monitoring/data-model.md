# Data Model: Monitoramento de nível do reservatório

## Property

Representa a propriedade à qual o reservatório pertence.

| Field | Meaning | Constraints |
|---|---|---|
| `id` | Identificador da propriedade | UUID, único |
| `name` | Nome exibido | Obrigatório |
| `createdAt` | Criação do registro | Instante UTC |

Uma propriedade contém o reservatório da demonstração. Não há cadastro de usuário/autorização
neste escopo local de usuário único.

## Reservoir

Representa o reservatório monitorado e o referencial usado para interpretar o nível.

| Field | Meaning | Constraints |
|---|---|---|
| `id` | Identificador do reservatório | UUID, único |
| `propertyId` | Propriedade associada | Obrigatório, referencia Property |
| `name` | Nome exibido no painel | Obrigatório |
| `levelReferenceLabel` | Descrição do referencial da leitura | Obrigatório para interpretação; não representa geometria ou volume |
| `createdAt` | Criação do registro | Instante UTC |

O seed local fornece um reservatório de demonstração. Não são armazenados limites físicos ou uma
curva nível-volume nesta feature.

## Measurement Receipt

Representa um dado recebido do simulador e o resultado da validação. O payload recebido continua
rastreável mesmo se não puder ser normalizado.

| Field | Meaning | Constraints |
|---|---|---|
| `id` | Identificador local do recebimento | UUID, único |
| `sourceEventId` | Chave idempotente enviada pelo simulador | UUID, única por fonte |
| `reservoirId` | Reservatório de destino | Obrigatório; deve existir |
| `source` | Origem do dado | `simulated` nesta feature |
| `rawPayload` | Conteúdo JSON recebido | Preservado para rastrear dados inválidos do simulador |
| `measurementKind` | Tipo reconhecido | `water_level` ou `precipitation`; tipo não reconhecido resulta em inválido |
| `numericValue` | Valor normalizado | Decimal nullable; ausente para valor inválido/não numérico |
| `rawValue` | Representação de entrada do valor | Mantida para explicar rejeição sem perder o dado recebido |
| `unit` | Unidade informada | `m` para nível; `mm` para precipitação; ausente/incompatível invalida |
| `measuredAt` | Instante declarado da medição | UTC nullable; não interpretável/ausente invalida e não entra em séries temporais |
| `receivedAt` | Instante de recepção pela API | UTC, sempre obrigatório, atribuído pela API |
| `accumulationStart` | Início da acumulação de precipitação | UTC nullable; aplicável à precipitação |
| `accumulationEnd` | Fim da acumulação de precipitação | UTC nullable; aplicável à precipitação; posterior ao início |
| `validationStatus` | Resultado de validação | `valid` ou `invalid`; toda linha é também um recebimento |
| `validationMessages` | Motivos da invalidez | Lista vazia se válido; obrigatória se inválido |

### Validação e estados

- O recebimento é persistido antes/de forma atômica com a classificação de validade.
- Valor numérico finito, unidade esperada, instante UTC interpretável e tipo conhecido são
  pré-requisitos para normalização válida.
- Limites de valor só podem ser aplicados quando fornecidos pela configuração do cenário de
  simulação. Não equivalem a limites científicos do reservatório.
- JSON sintaticamente inválido e ausência da chave idempotente são recusados na entrada HTTP, pois
  não constituem recebimento identificável. Conteúdo JSON sintaticamente válido, mas com campos
  semânticos inválidos, é preservado com estado `invalid`.
- Uma chave `sourceEventId` repetida com payload idêntico retorna o recebimento original. A mesma
  chave com conteúdo diferente é conflito e não cria uma segunda linha.
- O histórico de nível considera apenas recebimentos válidos de `water_level` com `measuredAt`
  dentro do intervalo consultado. Ordenação usa `measuredAt`, não `receivedAt`.

## Level Variation (derived)

Não é persistida. Para o intervalo inclusivo `[from, to]`, selecionar recebimentos válidos de nível
com instantes distintos. Com ao menos dois, usar a primeira e última medição cronologicamente:

`Δh = h_final − h_inicial`

O resultado contém `deltaMeters`, `startMeasuredAt`, `endMeasuredAt` e direção (`rising`,
`falling`, `no_observed_change`). Com menos de duas medições aptas, o resultado é `unavailable`.
Não há taxa temporal, interpolação ou categoria de estabilidade física.

## Precipitation Context

Usa `Measurement Receipt` do tipo `precipitation`, em milímetros, com período de acumulação
explícito. É uma série separada. Zero é um valor válido; ausência de registro ou intervalo
desconhecido é indisponibilidade. Não há cálculo estatístico ou regra causal com nível.

## Relationships

```text
Property 1 ─── 1 Reservoir (MVP seed)
Reservoir 1 ─── N Measurement Receipt
Measurement Receipt ─── 0..1 derived Level Variation (per query interval)
```
