# Feature Specification: Monitoramento de nível do reservatório com dados simulados

**Feature Branch**: `003-reservoir-level-monitoring`

**Created**: 2026-11-09

**Status**: Draft

**Input**: User description: "Desenvolver a primeira funcionalidade do AquaGuard: permitir que um usuário acompanhe o nível de água de um reservatório utilizando dados simulados, visualize o histórico de medições e identifique variações. Definir requisitos e comportamento antes da implementação, considerando Constitution, visão do produto e avaliações de águas continentais, meteorologia e cliente rural."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Acompanhar o nível e a variação recente (Priority: P1)

Como proprietário ou administrador rural, quero ver o último nível simulado do reservatório e sua
variação em relação a outra medição válida, para entender o que foi registrado e se o nível subiu
ou baixou.

**Why this priority**: O nível atual e sua mudança são o valor central desta funcionalidade e
permitem validar o fluxo de medição sem hardware real.

**Independent Test**: Disponibilizar duas ou mais medições simuladas válidas, com valores, unidades
e horários conhecidos. O usuário identifica a medição mais recente, sua origem simulada e a
variação entre as medições escolhidas, incluindo o intervalo comparado.

**Acceptance Scenarios**:

1. **Given** existe ao menos uma medição válida, **When** o usuário abre o monitoramento do
   reservatório, **Then** vê o nível mais recente em metros, o horário da medição e a indicação de
   que o dado é simulado.
2. **Given** existem duas medições válidas com horários distintos no período selecionado,
   **When** o usuário consulta a variação, **Then** vê a diferença entre o nível final e o inicial,
   em metros, e os horários usados na comparação.
3. **Given** a diferença calculada é positiva, negativa ou zero, **When** ela é apresentada,
   **Then** o sistema a descreve respectivamente como elevação, redução ou ausência de variação
   observada entre aquelas medições, sem atribuir estabilidade física ao reservatório.
4. **Given** não há medição ou não há duas medições válidas com horários distintos no período,
   **When** o usuário consulta nível ou variação, **Then** o sistema informa que o dado ou cálculo
   está indisponível e não inventa um valor.
5. **Given** a simulação deixa de produzir novas medições, **When** o usuário abre o monitoramento,
   **Then** a última medição continua associada ao seu horário e não é apresentada como uma leitura
   recém-observada.

---

### User Story 2 - Consultar o histórico de nível (Priority: P2)

Como proprietário ou administrador rural, quero consultar as medições do reservatório em um
período escolhido, para reconhecer como o nível variou ao longo do tempo.

**Why this priority**: O histórico dá contexto à leitura mais recente e permite comparar pontos
temporais sem transformar uma amostra isolada em tendência duradoura.

**Independent Test**: Disponibilizar medições em diferentes horários e consultar um período que
inclua parte delas. O usuário confere a ordem temporal, os valores, unidades e origem simulada.

**Acceptance Scenarios**:

1. **Given** há medições no período solicitado, **When** o usuário abre o histórico, **Then** as
   medições são apresentadas em ordem de horário de medição, com valor, unidade e origem.
2. **Given** há medições válidas nos limites do período, **When** o usuário consulta a variação do
   período, **Then** o cálculo usa a primeira e a última medição válida dentro do período e informa
   ambos os horários.
3. **Given** não há medições no período, **When** o usuário abre o histórico, **Then** o sistema
   informa que não há dados disponíveis para aquele período.
4. **Given** existem lacunas entre medições ou os intervalos entre elas são irregulares, **When** o
   usuário consulta o histórico, **Then** os horários reais permanecem visíveis e o sistema não
   interpola valores nem apresenta uma taxa por unidade de tempo como se os intervalos fossem
   uniformes.

---

### User Story 3 - Ver precipitação como contexto (Priority: P3)

Como proprietário ou administrador rural, quero poder visualizar a precipitação simulada junto ao
histórico de nível, para observar a coincidência temporal entre as duas séries sem receber uma
conclusão causal não comprovada.

**Why this priority**: A chuva pode ajudar a contextualizar observações, mas a relação não é
necessária para compreender o nível e não prova a causa de sua variação.

**Independent Test**: Disponibilizar medições simuladas de nível e precipitação com horários e
períodos de acumulação conhecidos. O usuário vê suas origens, unidades e intervalos, e nenhuma
causalidade é declarada.

**Acceptance Scenarios**:

1. **Given** há precipitação simulada disponível para o período, **When** o usuário habilita ou
   consulta o contexto meteorológico do histórico, **Then** vê a precipitação em milímetros, seu
   intervalo de acumulação e a indicação de origem simulada.
2. **Given** o intervalo de acumulação da precipitação não coincide com o período de variação do
   nível, **When** ambas as séries são exibidas, **Then** cada intervalo é identificado e a
   comparação não é descrita como equivalente.
3. **Given** precipitação e nível aumentam em períodos coincidentes, **When** o usuário visualiza
   as séries, **Then** o sistema pode mostrar a coincidência temporal, mas não afirma que a chuva
   causou a elevação.
4. **Given** a precipitação está ausente em um intervalo, **When** o usuário consulta o contexto,
   **Then** o sistema diferencia dado ausente de precipitação igual a zero.

### Edge Cases

- Ainda não há medições; nível, variação e histórico aparecem como indisponíveis, sem valor
  substituto.
- Há apenas uma medição válida no período; ela pode ser mostrada como última medição, mas a
  variação entre medições permanece indisponível.
- Uma medição tem estrutura inválida, valor não numérico, unidade ausente ou unidade incompatível;
  ela é identificada como inválida e não substitui a última medição válida nem participa da
  variação.
- Uma medição chega atrasada; ela aparece no histórico pelo horário da medição e não se torna a
  leitura mais recente apenas por ter sido recebida depois.
- Duas medições têm o mesmo horário e não há ordenação temporal suficiente para determinar a
  primeira e a última; a variação entre elas é informada como indisponível.
- A última medição é antiga; o produto mostra seu horário e a identifica como última medição
  registrada, sem afirmar que descreve o estado físico atual.
- Os valores simulados estão dentro dos limites declarados pelo cenário de simulação, mas esses
  limites não foram definidos como limites físicos universais do reservatório.
- A precipitação não está disponível, possui intervalo desconhecido ou cobre período diferente;
  o sistema não a trata como zero nem calcula uma relação quantitativa com o nível.
- A geometria ou curva nível-volume do reservatório não está disponível; nenhum volume é inferido
  a partir do nível.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: A funcionalidade MUST apresentar medições de nível associadas a um reservatório e
  expressar cada valor em metros.
- **FR-002**: Cada medição apresentada MUST identificar seu horário de medição e indicar que sua
  origem é simulada.
- **FR-003**: A funcionalidade MUST distinguir medições recebidas, medições válidas e variações
  calculadas.
- **FR-004**: Uma medição MUST ser considerada válida para esta funcionalidade somente se possuir
  valor numérico finito, unidade conhecida e horário de medição identificável. Limites físicos
  adicionais só podem ser aplicados se documentados para o reservatório ou cenário de simulação.
- **FR-005**: Medições inválidas MUST permanecer identificáveis como recebidas, mas MUST NOT
  substituir a última medição válida nem participar de variações calculadas.
- **FR-006**: A funcionalidade MUST selecionar a última medição válida pelo horário de medição, e
  MUST NOT usar apenas o horário de recebimento para determinar qual medição é mais recente.
- **FR-007**: O usuário MUST poder consultar o histórico de nível dentro de um período escolhido.
- **FR-008**: O histórico MUST apresentar as medições por horário de medição, mantendo visíveis
  valor, unidade e origem simulada.
- **FR-009**: Para um período com pelo menos duas medições válidas em horários distintos, a
  variação MUST ser calculada como nível da última medição válida menos nível da primeira medição
  válida no período: Δh = h_final − h_inicial.
- **FR-010**: Toda variação MUST ser expressa em metros e identificar os horários inicial e final
  usados no cálculo.
- **FR-011**: A descrição da variação MUST indicar elevação quando Δh for positivo, redução quando
  Δh for negativo e ausência de variação observada entre as medições quando Δh for zero. Ela MUST
  NOT chamar essa comparação de estabilidade física do reservatório.
- **FR-012**: A funcionalidade MUST informar variação indisponível quando houver menos de duas
  medições válidas com horários distintos no período solicitado.
- **FR-013**: A funcionalidade MUST preservar o horário de medição de registros atrasados e MUST
  NOT interpolar medições ausentes.
- **FR-014**: A funcionalidade MUST apresentar a última medição registrada com seu horário, sem
  representar uma medição antiga como recém-observada.
- **FR-015**: Quando dados simulados de precipitação estiverem disponíveis, a funcionalidade MUST
  permitir exibi-los como contexto separado do nível, com unidade, origem e intervalo de
  acumulação identificáveis.
- **FR-016**: A funcionalidade MUST distinguir precipitação zero de precipitação ausente ou
  desconhecida.
- **FR-017**: A funcionalidade MUST NOT afirmar que precipitação causou uma variação de nível; uma
  coincidência temporal pode ser apresentada apenas como contexto ou associação observada.
- **FR-018**: A funcionalidade MUST NOT inferir volume do reservatório a partir do nível sem uma
  relação nível-volume documentada para aquele reservatório.
- **FR-019**: A funcionalidade MUST NOT gerar diagnóstico de risco, alerta de variação anormal ou
  recomendação de operação com base em limites não definidos nesta especificação.

### Key Entities *(include if feature involves data)*

- **Reservatório**: Estrutura hídrica monitorada, com identidade e referencial de nível associado
  às medições; não implica que haja geometria ou curva nível-volume cadastrada.
- **Medição de nível**: Valor em metros, horário da medição, origem simulada e estado de validação.
- **Variação de nível**: Diferença entre duas medições válidas no período consultado, incluindo os
  horários e valores de referência.
- **Medição de precipitação**: Valor simulado em milímetros, horário ou intervalo de acumulação e
  estado de disponibilidade.
- **Período consultado**: Intervalo temporal escolhido pelo usuário para visualizar histórico e
  calcular variação.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em avaliação de usabilidade, pelo menos 90% dos participantes conseguem identificar
  o nível mais recente, sua unidade, horário e origem simulada em até dois minutos sem ajuda.
- **SC-002**: Para todos os pares de medições válidas da tabela de aceitação, a variação exibida
  corresponde a h_final − h_inicial em metros, respeitando a precisão apresentada ao usuário.
- **SC-003**: Em pelo menos 90% das tarefas de avaliação, usuários conseguem identificar se o
  nível subiu, caiu ou não apresentou variação entre os pontos comparados, sem interpretar isso
  como diagnóstico causal ou de estabilidade física.
- **SC-004**: Em todos os casos de teste sem dados, com apenas uma medição válida, com horário
  ambíguo ou com registros inválidos, a variação é apresentada como indisponível quando não há
  base válida para calculá-la.
- **SC-005**: Em todos os históricos avaliados, medições são ordenadas pelo horário de medição e
  permanecem distinguíveis como simuladas.
- **SC-006**: Em todos os casos de teste meteorológicos, precipitação ausente é distinguida de
  zero, e nenhum caso apresenta causalidade entre chuva e variação de nível como fato comprovado.

## Assumptions

- Esta funcionalidade é a primeira fatia do MVP e cobre um reservatório com medições simuladas;
  integração com sensores físicos está fora do escopo.
- O valor simulado representa o nível em metros em relação a um referencial definido para o
  cenário. Na ausência de referencial físico cadastrado, a interface identifica o valor como
  leitura simulada e não o descreve como profundidade absoluta ou volume.
- O simulador declara os limites do cenário que utiliza. Esses limites servem à validação dos
  dados simulados e não são apresentados como limites científicos universais do reservatório.
- A variação do período compara a primeira e a última medição válida no intervalo selecionado;
  não é calculada taxa por hora nem interpolação nesta funcionalidade.
- A precipitação simulada é contexto opcional. Temperatura, correlação estatística, modelos
  hidrológicos, alertas, limites operacionais e diagnóstico de risco ficam fora desta
  funcionalidade.
- Os critérios exatos de recência visual, cadência da simulação e períodos predefinidos do
  histórico serão estabelecidos no planejamento, sem alterar a regra de seleção e cálculo aqui
  definida.
- As recomendações de perspectiva do cliente são hipóteses de produto para validação e não
  resultados de pesquisa com usuários.
