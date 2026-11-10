# Implementation Plan: Monitoramento de nível do reservatório com dados simulados

**Branch**: `003-reservoir-level-monitoring` | **Date**: 2026-11-09 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/003-reservoir-level-monitoring/spec.md`

## Summary

Entregar o primeiro fluxo operacional do AquaGuard: um simulador independente gera medições
sintéticas, a API as valida e registra, e o usuário acompanha o nível e seu histórico. A variação
é calculada a partir das medições válidas do período selecionado. A precipitação pode ser mostrada
como série contextual independente, sem alegação causal. A implementação reutiliza os projetos
backend existentes, acrescenta somente o processo simulador solicitado para o MVP e mantém as
regras de domínio isoladas da persistência e do transporte.

## Technical Context

**Language/Version**: C# / .NET 10 no backend; TypeScript no frontend, conforme a stack definida
para o MVP.

**Primary Dependencies**: ASP.NET Core existente; EF Core 10 com provedor Npgsql 10; React 19 e
Vite 8 existentes; TypeScript e configuração de compilação/lint a acrescentar; xUnit v3 para os
testes backend; SVG nativo para a série temporal, sem dependência de gráficos.

**Storage**: PostgreSQL 18 em ambiente local Compose, com mapeamento e migrações EF Core sob
Infrastructure.

**Testing**: Projeto backend dedicado com xUnit v3. Testes unitários cobrem validade e cálculo de
variação; testes de integração usam o PostgreSQL local iniciado pelo Compose. Frontend validado
por lint/build e roteiro funcional manual; não há runner frontend no repositório.

**Target Platform**: Linux local via Docker Compose para a demonstração; navegador desktop moderno.

**Project Type**: Aplicação web com API backend, frontend, banco relacional e processo simulador
independente.

**Performance Goals**: Não há meta de throughput ou baixa latência definida para este MVP de um
reservatório. A consulta do painel deve retornar o conjunto do período selecionado sem exigir
carregamento de todo o histórico armazenado.

**Constraints**: Todos os instantes são armazenados e trafegados como UTC; a interface apresenta
horário local com fuso identificável. Medições simuladas permanecem marcadas como simuladas. A
variação é Δh = h_final − h_inicial; sem limiar de estabilidade, interpolação, taxa temporal,
volume inferido, alerta ou diagnóstico. A demonstração é local e não inclui autenticação nem
implantação de produção.

**Scale/Scope**: Um usuário local, uma propriedade, um reservatório e uma estação; medições
sintéticas periódicas e histórico suficiente para a demonstração. Cadência, valores sintéticos e
janela de visualização são configuráveis, não parâmetros físicos.

## Constitution Check

| Princípio / gate | Resultado | Evidência de desenho |
|---|---|---|
| Problema real antes da tecnologia | PASS | O fluxo implementa nível, variação e histórico definidos na spec. |
| Separação de responsabilidades | PASS | Domain calcula e valida; Application orquestra; Infrastructure persiste; API expõe contratos; Simulator gera entradas; Frontend apresenta. |
| Integridade dos dados | PASS | Horário de medição é distinto do recebimento; inválidos não atualizam o último valor válido; origem e validação são preservadas. |
| Fundamentação física | PASS | Perfil sintético é identificado como demonstração; nenhum limite universal ou volume é inferido. |
| Regras testáveis | PASS | Cálculo e validação pertencem ao domínio e têm testes unitários; persistência tem testes de integração. |
| Segurança e privacidade | PASS WITH SCOPE | MVP local de usuário único, conforme a spec-pai; serviços internos na rede Compose e portas de desenvolvimento limitadas a localhost. Produção/autenticação ficam fora deste escopo. |
| Tolerância a falhas | PASS | API e simulador têm health checks; o simulador aplica retries limitados com backoff e a UI preserva horário da última medição. |
| Dispositivos substituíveis | PASS | Simulator é uma fonte de entrada; validação e domínio não dependem do processo que gerou a medição. |
| Simplicidade | PASS WITH JUSTIFICATION | O processo simulador separado é pedido no MVP e permite interromper comunicação sem derrubar API/painel. Não será introduzido broker ou fila distribuída. |
| Experiência operacional | PASS | Valor, unidade, horário, origem, período e estado indisponível também são apresentados em texto, não apenas em cores/gráfico. |
| Automação explicável | PASS | O escopo não tem ML, inferência causal nem regra de alerta. |
| Documentação e rastreabilidade | PASS | Contrato, modelo e quickstart ficam junto à spec. |

**Gate result**: aprovado. As duas ressalvas registram explicitamente o escopo de demonstração
local e a razão para o processo simulador separado; não são desvios não justificados.

## Design Decisions

- O simulador roda como processo Worker independente e envia leituras à API por HTTP interno no
  Compose. A API conduz todas as fontes pelo mesmo fluxo de validação e gravação. Isso atende à
  decisão do MVP e permite simular perda de comunicação sem indisponibilizar o painel.
- O simulador usa perfil determinístico configurável. Seus valores são dados de demonstração,
  independentes de um modelo hidrológico, e incluem trechos de subida e descida para a variação ser
  observável. Chuva sintética é gerada separadamente, sem determinar o nível.
- A cadência local padrão é 60 segundos e pode ser substituída por configuração. A janela inicial
  do histórico é de 24 horas, com opções de 1 hora, 6 horas, 24 horas, 7 dias e intervalo
  personalizado. São escolhas de demonstração/UX, não escalas físicas.
- Os limites de consulta são inclusivos (`from <= measuredAt <= to`). A primeira e a última
  medição válida com instantes distintos nesse intervalo definem Δh.
- Instantes de medição e recebimento são UTC no domínio, API e banco. O browser converte para o
  fuso local e o identifica na interface. A API usa ISO 8601 com offset `Z`.
- O nível e a precipitação são armazenados em uma entidade de medição tipada; precipitação guarda
  início/fim da acumulação. A variação é derivada na consulta, não persistida como medição.
- Registros de recebimento preservam origem, payload e resultado de validação. Recebimentos
  semanticamente inválidos são rastreáveis, mas não entram no histórico validado nem no cálculo.
  JSON malformado ou chave de idempotência ausente é rejeitado no limite da API.
- O contrato oferece uma leitura de painel consolidada e um endpoint de ingestão apenas para
  dados simulados. A chave de evento garante idempotência; reutilizá-la com conteúdo diferente é
  conflito.
- A tela usa SVG nativo para o gráfico simples, com tabela/texto equivalente e resumo acessível.
  Isso evita dependência de gráficos para uma série de nível e uma série opcional de precipitação.
- O frontend da feature usa TypeScript: `docs/context.md` e a definição do MVP especificam
  React + TypeScript. A tela inicial atual é JSX puro; a configuração necessária será limitada à
  aplicação, sem migrar arquivos não relacionados.
- Nível e variação são exibidos com duas casas decimais; a API conserva os valores sem
  arredondamento e Δh é calculado antes da formatação da interface.
- Migrações são aplicadas pela rotina de inicialização do ambiente local de desenvolvimento. A
  documentação deixa claro que isso não constitui estratégia de migração para produção.

## Project Structure

### Documentation (this feature)

```text
specs/003-reservoir-level-monitoring/
├── plan.md
├── research.md
├── data-model.md
├── contracts/
│   └── reservoir-monitoring.openapi.yaml
├── quickstart.md
└── tasks.md                 # será criado por $speckit-tasks
```

### Source Code (repository root)

```text
backend/
├── docker-compose.yml
├── .env.example
├── AquaGuard.slnx
├── src/
│   ├── AquaGuard.Api/
│   │   ├── Endpoints/ReservoirMonitoringEndpoints.cs
│   │   └── Program.cs
│   ├── AquaGuard.Application/
│   │   └── ReservoirMonitoring/
│   ├── AquaGuard.Domain/
│   │   ├── Entities/Measurement.cs
│   │   └── ReservoirMonitoring/
│   ├── AquaGuard.Infrastructure/
│   │   └── Persistence/
│   ├── AquaGuard.Edge/         # permanece para futuras fontes físicas
│   └── AquaGuard.Simulator/    # novo processo Worker do simulador
└── tests/
    └── AquaGuard.Tests/

frontend/aquaguard-web/
├── tsconfig.json
├── vite.config.js
└── src/
    ├── features/reservoir-monitoring/
    ├── services/reservoirMonitoring.ts
    └── App.tsx
```

**Structure Decision**: Aplicação web já organizada em cinco projetos backend e um frontend. O
novo projeto `AquaGuard.Simulator` é justificado pela exigência de um simulador independente e
pela necessidade de poder interrompê-lo sem derrubar o painel/API. Não será adicionado broker. O
Compose fica em `backend/`, junto à solução e serviços .NET, e inicia PostgreSQL, API, simulador e
frontend para a demonstração local.

## Complexity Tracking

| Added element | Justification | Simpler alternative not selected |
|---|---|---|
| Processo `AquaGuard.Simulator` | Requisito explícito do MVP; permite simular desconexão enquanto API e painel continuam disponíveis. | `BackgroundService` dentro da API não pode ser interrompido independentemente da API e não atende tão bem ao cenário de falha de comunicação. |
| Contrato HTTP interno entre simulator e API | Reutiliza validação/ingestão da API e mantém a fonte substituível. | Escrita direta do simulador no PostgreSQL acoplaria a geração ao schema e contornaria o fluxo de validação. |
