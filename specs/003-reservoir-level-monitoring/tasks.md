# Tasks: Monitoramento de nível do reservatório com dados simulados

**Input**: Design documents from `specs/003-reservoir-level-monitoring/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Tests**: Incluídos conforme o plano e a Constitution: testes unitários de domínio e testes de integração com PostgreSQL. Não há runner frontend; validação do frontend usa lint/build e o quickstart.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar os projetos e o ambiente local do MVP.

- [X] T001 Criar o projeto Worker .NET 10 `backend/src/AquaGuard.Simulator/AquaGuard.Simulator.csproj` e adicioná-lo a `backend/AquaGuard.slnx`.
- [X] T002 [P] Configurar TypeScript estrito e checagem JSX para a aplicação em `frontend/aquaguard-web/tsconfig.json`, `frontend/aquaguard-web/package.json` e `frontend/aquaguard-web/eslint.config.js`, mantendo a tela da feature em `.tsx`.
- [X] T003 [P] Criar `backend/docker-compose.yml` com serviços locais PostgreSQL 18, API, `aquaguard-simulator` e frontend, health checks e portas de host limitadas a localhost.
- [X] T004 Criar `backend/.env.example` com nomes e valores locais não secretos requeridos por `backend/docker-compose.yml`; não incluir credenciais reais.
- [X] T005 Criar o projeto de testes xUnit v3 `backend/tests/AquaGuard.Tests/AquaGuard.Tests.csproj` e adicioná-lo a `backend/AquaGuard.slnx`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Conectar as camadas, persistência e configuração compartilhadas antes de implementar qualquer história.

**⚠️ CRITICAL**: Histórias de usuário dependem desta fase.

- [X] T006 Adicionar referências entre `backend/src/AquaGuard.Api/AquaGuard.Api.csproj`, `backend/src/AquaGuard.Application/AquaGuard.Application.csproj`, `backend/src/AquaGuard.Domain/AquaGuard.Domain.csproj`, `backend/src/AquaGuard.Infrastructure/AquaGuard.Infrastructure.csproj`, `backend/src/AquaGuard.Simulator/AquaGuard.Simulator.csproj` e `backend/tests/AquaGuard.Tests/AquaGuard.Tests.csproj`, respeitando Domain sem dependências de framework/persistência.
- [X] T007 [P] Criar `backend/src/AquaGuard.Infrastructure/Persistence/AquaGuardDbContext.cs` e configuração de DI em `backend/src/AquaGuard.Infrastructure/DependencyInjection.cs` para PostgreSQL/Npgsql, sem colocar detalhes do provedor em Domain.
- [X] T008 [P] Ajustar `backend/src/AquaGuard.Domain/Entities/Property.cs` e substituir o stub `backend/src/AquaGuard.Domain/Entities/Reservoir.cs` pelo modelo do MVP; `Reservoir` deve referenciar `Property` e exigir `levelReferenceLabel`, sem incluir geometria ou curva nível-volume.
- [X] T009 Mapear `Property` e `Reservoir` em `backend/src/AquaGuard.Infrastructure/Persistence/PropertyConfiguration.cs` e `backend/src/AquaGuard.Infrastructure/Persistence/ReservoirConfiguration.cs`, depois adicionar seed idempotente em `backend/src/AquaGuard.Infrastructure/Persistence/DemoDataSeeder.cs`; identificar o referencial como sintético e não declarar limite físico.
- [X] T010 Criar a migração inicial de propriedade e reservatório em `backend/src/AquaGuard.Infrastructure/Persistence/Migrations/`; os índices de medições serão adicionados junto à migração de recebimentos da US1.
- [X] T011 Configurar aplicação de migrações e seed somente no ambiente de desenvolvimento em `backend/src/AquaGuard.Api/Program.cs`; obter a conexão de `backend/src/AquaGuard.Api/appsettings.Development.json` ou variáveis de ambiente, sem adicionar segredos versionados.
- [X] T012 Adicionar tratamento consistente de erros de entrada/consulta e logging estruturado nos pontos de composição de `backend/src/AquaGuard.Api/Program.cs` e configuração de `backend/src/AquaGuard.Api/appsettings.json`.

**Checkpoint**: Projetos conectados, armazenamento local inicializável e reservatório de demonstração disponível.

---

## Phase 3: User Story 1 - Acompanhar o nível e a variação recente (Priority: P1) 🎯 MVP

**Goal**: Exibir o nível simulado mais recente e a variação entre medições válidas, preservando origem e horário.

**Independent Test**: Iniciar o ambiente local, aguardar dois ciclos, e identificar no painel nível em metros, origem simulada, horário e Δh com os dois horários comparados. Parar o simulador e confirmar que o último horário permanece visível enquanto API e frontend continuam disponíveis.

### Tests for User Story 1

> Criar os testes primeiro e confirmar que falham antes da implementação correspondente.

- [X] T013 [P] [US1] Criar testes de validade para número finito, unidade, horário e limites somente quando configurados em `backend/tests/AquaGuard.Tests/ReservoirMonitoring/MeasurementValidationTests.cs`.
- [X] T014 [P] [US1] Criar testes do cálculo Δh = h_final − h_inicial, direção por sinal, ausência de variação, timestamps iguais e histórico insuficiente em `backend/tests/AquaGuard.Tests/ReservoirMonitoring/LevelVariationCalculatorTests.cs`.
- [X] T015 [US1] Criar testes de integração do recebimento, persistência, idempotência e seleção da última medição por `measuredAt` em `backend/tests/AquaGuard.Tests/Api/SimulatedMeasurementIngestionTests.cs`, usando PostgreSQL do Compose.

### Implementation for User Story 1

- [X] T016 [US1] Implementar estados e regras de validação da medição em `backend/src/AquaGuard.Domain/Entities/Measurement.cs` e `backend/src/AquaGuard.Domain/ReservoirMonitoring/MeasurementValidator.cs`; preservar recebimento inválido e impedir seu uso em cálculos.
- [X] T017 [US1] Implementar `LevelVariation` e cálculo puro em `backend/src/AquaGuard.Domain/ReservoirMonitoring/LevelVariationCalculator.cs`, usando somente medições válidas, timestamps distintos e delta em metros.
- [X] T018 [US1] Mapear recebimentos, payload, resultado de validação e chave idempotente em `backend/src/AquaGuard.Infrastructure/Persistence/MeasurementConfiguration.cs` e `backend/src/AquaGuard.Infrastructure/Persistence/AquaGuardDbContext.cs`; criar migração da tabela e índices em `backend/src/AquaGuard.Infrastructure/Persistence/Migrations/`, aplicando as restrições de `data-model.md`.
- [X] T019 [US1] Implementar gravação e consulta do último nível válido em `backend/src/AquaGuard.Application/ReservoirMonitoring/RecordSimulatedMeasurement.cs` e `backend/src/AquaGuard.Application/ReservoirMonitoring/GetLatestReservoirLevel.cs`.
- [X] T020 [US1] Expor ingestão e resumo inicial conforme `contracts/reservoir-monitoring.openapi.yaml` em `backend/src/AquaGuard.Api/Endpoints/ReservoirMonitoringEndpoints.cs`; retornar status de validação e impedir chave idempotente repetida com payload divergente.
- [X] T021 [P] [US1] Implementar perfil determinístico sintético e cadência configurável com padrão de 60 segundos em `backend/src/AquaGuard.Simulator/Simulation/ReservoirScenario.cs` e `backend/src/AquaGuard.Simulator/appsettings.json`; os valores devem variar sem pretensão de representar condições físicas reais.
- [X] T022 [US1] Implementar Worker que envia leituras à API usando chave de evento, health check e retries limitados com backoff em `backend/src/AquaGuard.Simulator/Simulation/SimulationWorker.cs` e `backend/src/AquaGuard.Simulator/Program.cs`.
- [X] T023 [US1] Criar cliente TypeScript para o resumo de monitoramento em `frontend/aquaguard-web/src/services/reservoirMonitoring.ts`, incluindo estados HTTP de carregamento, indisponibilidade e erro.
- [X] T024 [US1] Substituir a tela inicial por painel acessível com nome do reservatório, nível, unidade, horários, origem simulada, variação e estado sem dados em `frontend/aquaguard-web/src/App.tsx` e `frontend/aquaguard-web/src/features/reservoir-monitoring/CurrentLevelPanel.tsx`.
- [X] T025 [US1] Integrar API, simulator, health checks e interface no Compose em `backend/docker-compose.yml`, aguardando disponibilidade do banco/API antes do envio de dados.

**Checkpoint**: User Story 1 demonstra nível e variação sem história visual completa; parar o simulador não derruba o painel/API.

---

## Phase 4: User Story 2 - Consultar o histórico de nível (Priority: P2)

**Goal**: Permitir selecionar um período e revisar medições ordenadas e variação do intervalo.

**Independent Test**: Consultar períodos predefinidos e personalizado; confirmar medição, horário, origem, ordem temporal, delta primeira/última e estado vazio quando não existem dados.

### Tests for User Story 2

> Criar os testes primeiro e confirmar que falham antes da implementação correspondente.

- [X] T026 [P] [US2] Adicionar casos de consulta inclusiva `[from, to]`, ordenação por `measuredAt`, pontos inválidos/duplicados e resultado sem duas medições em `backend/tests/AquaGuard.Tests/ReservoirMonitoring/LevelHistoryQueryTests.cs`.
- [X] T027 [US2] Adicionar teste de integração de intervalos, ordenação e histórico vazio em `backend/tests/AquaGuard.Tests/Api/ReservoirLevelHistoryTests.cs`, usando PostgreSQL do Compose.

### Implementation for User Story 2

- [X] T028 [US2] Implementar consulta do período inclusivo e variação primeira/última em `backend/src/AquaGuard.Application/ReservoirMonitoring/GetReservoirLevelHistory.cs` e `backend/src/AquaGuard.Infrastructure/Persistence/MeasurementRepository.cs`.
- [X] T029 [US2] Completar o contrato GET com `from`, `to`, default de 24h, validação `from <= to`, histórico e variação em `backend/src/AquaGuard.Api/Endpoints/ReservoirMonitoringEndpoints.cs` e `specs/003-reservoir-level-monitoring/contracts/reservoir-monitoring.openapi.yaml`.
- [X] T030 [P] [US2] Implementar presets de 1h, 6h, 24h, 7d e seleção personalizada em `frontend/aquaguard-web/src/features/reservoir-monitoring/HistoryPeriodSelector.tsx`.
- [X] T031 [P] [US2] Implementar gráfico SVG com resumo textual/tabela equivalente, timestamps no fuso local identificado e estados de lacuna/sem dados em `frontend/aquaguard-web/src/features/reservoir-monitoring/LevelHistoryChart.tsx` e `frontend/aquaguard-web/src/features/reservoir-monitoring/MeasurementHistoryTable.tsx`.
- [X] T032 [US2] Conectar seleção de período, histórico e variação ao endpoint em `frontend/aquaguard-web/src/features/reservoir-monitoring/ReservoirMonitoringPage.tsx` e `frontend/aquaguard-web/src/services/reservoirMonitoring.ts`.

**Checkpoint**: User Story 2 pode ser demonstrada em qualquer intervalo suportado, inclusive sem medições.

---

## Phase 5: User Story 3 - Ver precipitação como contexto (Priority: P3)

**Goal**: Mostrar precipitação simulada como uma série opcional e temporalmente explícita, sem atribuir causa à mudança de nível.

**Independent Test**: Habilitar a série de precipitação e identificar mm, origem e período de acumulação; comparar período ausente e valor zero; confirmar ausência de linguagem causal.

### Tests for User Story 3

> Criar os testes primeiro e confirmar que falham antes da implementação correspondente.

- [X] T033 [P] [US3] Criar testes para precipitação zero versus ausente, unidade mm e intervalo de acumulação válido em `backend/tests/AquaGuard.Tests/ReservoirMonitoring/PrecipitationMeasurementTests.cs`.
- [X] T034 [US3] Criar teste de integração para carregar precipitação opcional no intervalo solicitado em `backend/tests/AquaGuard.Tests/Api/ReservoirPrecipitationContextTests.cs`.

### Implementation for User Story 3

- [X] T035 [US3] Estender o perfil sintético com precipitação independente e intervalos explícitos em `backend/src/AquaGuard.Simulator/Simulation/ReservoirScenario.cs`; não derivar nível de chuva.
- [X] T036 [US3] Validar e persistir precipitação zero, ausência, unidade e início/fim do acúmulo em `backend/src/AquaGuard.Domain/ReservoirMonitoring/MeasurementValidator.cs`, `backend/src/AquaGuard.Infrastructure/Persistence/MeasurementConfiguration.cs` e migração em `backend/src/AquaGuard.Infrastructure/Persistence/Migrations/`.
- [X] T037 [US3] Adicionar seleção opcional da série, preservando intervalos e status ausente versus zero, no retorno de `backend/src/AquaGuard.Api/Endpoints/ReservoirMonitoringEndpoints.cs` conforme `contracts/reservoir-monitoring.openapi.yaml`.
- [X] T038 [P] [US3] Renderizar precipitação em série SVG separada e tabela/resumo textual, com período de acumulação e origem, em `frontend/aquaguard-web/src/features/reservoir-monitoring/PrecipitationContext.tsx`.
- [X] T039 [US3] Integrar alternância da precipitação e atualizar mensagens de contexto não causal em `frontend/aquaguard-web/src/features/reservoir-monitoring/ReservoirMonitoringPage.tsx`.

**Checkpoint**: User Story 3 apresenta contexto meteorológico opcional sem misturar chuva e nível ou afirmar causalidade.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Consolidar documentação, qualidade visual e critérios transversais para a demonstração.

- [X] T040 [P] Atualizar roteiro executável e resultados esperados em `specs/003-reservoir-level-monitoring/quickstart.md` após validar nomes de serviços, portas e comandos reais.
- [X] T041 [P] Revisar contraste, labels, navegação por teclado e alternativas textuais do gráfico em `frontend/aquaguard-web/src/features/reservoir-monitoring/` e `frontend/aquaguard-web/src/App.css`.
- [X] T042 Confirmar consistência de status simulado, timestamps UTC, janela inclusiva e ausência de inferência de volume/causalidade entre `specs/003-reservoir-level-monitoring/spec.md`, `data-model.md`, `contracts/reservoir-monitoring.openapi.yaml` e os arquivos de domínio correspondentes em `backend/src/AquaGuard.Domain/ReservoirMonitoring/`.
- [X] T043 Executar `dotnet test backend/AquaGuard.slnx` com PostgreSQL do Compose disponível e corrigir falhas nos arquivos de teste afetados em `backend/tests/AquaGuard.Tests/`.
- [X] T044 Executar `npm run lint` e `npm run build` em `frontend/aquaguard-web/` e corrigir falhas nos arquivos frontend correspondentes.
- [ ] T045 Executar os passos de `specs/003-reservoir-level-monitoring/quickstart.md` e registrar no próprio roteiro quaisquer pré-requisitos ou resultados que precisem de ajuste.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: independente; prepara projetos, frontend TypeScript, Compose e testes.
- **Foundational (Phase 2)**: depende da configuração; estabelece relações entre camadas, persistência e seed. Bloqueia histórias.
- **User Stories (Phase 3+)**: dependem da fase foundational e seguem a prioridade P1 → P2 → P3, pois história e precipitação usam o armazenamento/contrato base.
- **Polish (Phase 6)**: após as histórias desejadas.

### User Story Dependencies

- **US1 (P1)**: independente após Phase 2; entrega nível atual e variação em janela default.
- **US2 (P2)**: depende do registro e consulta base da US1; adiciona seleção de período, série completa e visualização histórica.
- **US3 (P3)**: depende da ingestão e consulta temporal da US1/US2; adiciona a série opcional de precipitação.

### Parallel Opportunities

- Phase 1: T002 e T003 podem ocorrer em paralelo. T004 depende dos nomes de configuração definidos em T003. T001 e T005 editam o mesmo arquivo de solução e devem ser sequenciais.
- Phase 2: T007 e T008 podem ocorrer em paralelo em arquivos independentes; T009 depende dos modelos e contexto. T010 depende dos mapeamentos. T011 depende do DbContext/migration.
- US1: T013 e T014 são testes independentes e paralelos; T021 pode avançar em paralelo com testes/domain, desde que o contrato de entrada permaneça estável. API, frontend e worker integram após contratos/tipos compartilhados.
- US2: T026, T030 e T031 podem começar em paralelo após US1; T028/T029 precedem integração do seletor e gráficos em T032.
- US3: T033 e T038 podem começar em paralelo após contrato acordado; persistência, endpoint e série UI integram em sequência.
- Histórias não devem ser paralelizadas entre especialistas até a conclusão da fase anterior, pois compartilham migrations, endpoint consolidado e `reservoirMonitoring.ts`.

### Parallel Example: User Story 1

```text
Task: T013 MeasurementValidationTests.cs
Task: T014 LevelVariationCalculatorTests.cs
Task: T021 ReservoirScenario.cs
```

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Phase 1 e Phase 2.
2. Completar US1: dado sintético de nível, validação, persistência, última leitura e delta na tela.
3. Demonstrar API/painel ainda disponíveis ao interromper o simulador.
4. Parar no checkpoint e validar o fluxo antes de iniciar gráfico de histórico e precipitação.

### Incremental Delivery

1. Setup + Foundation.
2. US1 oferece o primeiro valor utilizável.
3. US2 acrescenta período selecionável, histórico e gráfico.
4. US3 acrescenta precipitação como contexto opcional.
5. Polish executa quickstart, testes backend e verificações frontend.

## Notes

- Todas as tasks usam `- [ ] Tnnn`, IDs sequenciais, labels [USn] nas fases de história e caminho de arquivo explícito.
- [P] está reservado a arquivos/atividades sem dependência de tarefas incompletas.
- Valores sintéticos e cadência não representam leitura, limite ou dinâmica física de um reservatório real.
- Testes estão no plano em conformidade com as obrigações da Constitution e os critérios da feature.
