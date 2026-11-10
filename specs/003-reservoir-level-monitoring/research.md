# Research: Monitoramento de nível do reservatório com dados simulados

**Feature**: `003-reservoir-level-monitoring`

**Status**: Decisões da fase de pesquisa consolidadas; sem NEEDS CLARIFICATION pendente.

## Decisões

### Simulador executado de forma independente

- **Decision**: Criar `AquaGuard.Simulator` como Worker separado no ambiente local Compose. Ele
  envia medições simuladas ao contrato interno da API; não escreve diretamente no banco.
- **Rationale**: A definição do MVP pede um serviço independente e a falha simulada deve poder
  parar a geração sem desligar a API/painel. A API continua sendo a fronteira de validação e
  gravação comum a todas as fontes.
- **Alternatives considered**: Um `BackgroundService` dentro da API reduziria um processo, mas
  acoplaria ciclo de vida da simulação e disponibilidade do painel. Um broker/fila aumentaria a
  infraestrutura sem necessidade para um único produtor local.

### Banco relacional e mapeamento

- **Decision**: PostgreSQL 18 para execução local e EF Core 10 com o provedor Npgsql 10, dentro de
  Infrastructure. A versão de patch deve seguir a mais recente compatível no momento da
  implementação.
- **Rationale**: PostgreSQL e EF Core foram escolhidos explicitamente no contexto e MVP do projeto;
  o backend já usa .NET 10. EF Core 10 é a linha alinhada ao .NET 10, e o provedor Npgsql tem uma
  linha 10 compatível com EF Core 10. PostgreSQL 18 está suportado pelo projeto PostgreSQL.
- **Alternatives considered**: Persistência em memória não entrega o histórico requerido. Acesso
  SQL direto adicionaria código específico de provedor à camada de persistência sem benefício para
  esse escopo.
- **Sources**: [EF Core releases](https://learn.microsoft.com/en-us/ef/core/what-is-new/index),
  [Npgsql EF Core 10 release notes](https://www.npgsql.org/efcore/release-notes/10.0.html),
  [PostgreSQL versioning policy](https://www.postgresql.org/support/versioning/).

### Fonte sintética determinística

- **Decision**: Perfil de sequência configurável, repetível e explicitamente sintético; cadência
  padrão local de 60 segundos. A série contém valores que variam para que a funcionalidade possa
  demonstrar elevação e redução.
- **Default profile**: Nível `[2.30, 2.34, 2.39, 2.35, 2.29, 2.27, 2.32] m`; precipitação
  `[0, 0, 3.2, ausente, 0, 1.4, ausente] mm`. A sequência delimita o cenário sintético repetível,
  não limites físicos do reservatório.
- **Rationale**: Repetibilidade facilita reproduzir critérios e testar cálculo. Perfil é dado de
  demonstração, sem pretensão de representar um açude, sensor ou processo hidrológico real.
- **Alternatives considered**: Aleatoriedade sem seed dificulta validação repetível. Modelo
  hidrológico está fora do escopo e exigiria parâmetros físicos não fornecidos.

### Horário e limites do histórico

- **Decision**: Instantes transmitidos e persistidos em UTC; interface localiza o horário e mostra
  o fuso. Janela inicial de 24 horas; opções 1h, 6h, 24h, 7d e período personalizado. Limites do
  período são inclusivos.
- **Rationale**: Instantes UTC ordenam medições sem depender da configuração local. Fuso visível
  permite ao usuário interpretar horários. Presets fornecem leitura rápida sem impedir seleção
  própria.
- **Alternatives considered**: Armazenar horário local cria ambiguidade em mudanças de fuso e
  ordenação. Intervalos exclusivos seriam menos intuitivos para seleção por datas no MVP.

### Regra de variação

- **Decision**: Para intervalo escolhido, Δh = h_final − h_inicial, usando primeira/última medição
  válida com horários distintos. Resultado em metros e horários dos dois pontos. Sem taxa temporal,
  interpolação ou limiar para “estável”.
- **Rationale**: É a regra explícita da especificação e não exige inventar uma janela de taxa ou
  banda de estabilidade. Uma amostra uniforme não é presumida.
- **Alternatives considered**: Taxa por hora exigiria tratar intervalos, lacunas e janelas de
  agregação; uma classe “estável” requer limiar documentado.

### Séries meteorológicas

- **Decision**: Precipitação simulada é uma série opcional, separada do nível, com mm e início/fim
  do período de acumulação. Valores de chuva e nível são gerados independentemente; intervalos
  ausentes são diferentes de zero.
- **Rationale**: A série contextualiza temporalmente sem sugerir causalidade. A avaliação
  meteorológica ressalta que períodos e origem devem ser visíveis.
- **Alternatives considered**: Derivar elevação do nível a partir de precipitação simularia uma
  causalidade física sem parâmetros. Excluir toda chuva não atenderia ao contexto opcional previsto
  na spec.

### Frontend e visualização

- **Decision**: TypeScript para a tela da feature, alinhado a `docs/context.md` e à definição do
  MVP. Gráfico SVG simples, acompanhado por resumo textual/tabela acessível; nenhuma biblioteca de
  gráficos nesta fase.
- **Rationale**: A stack TypeScript foi indicada pelo responsável do produto. A feature possui
  uma série primária e uma opcional, que não justificam dependência de visualização dedicada. O
  frontend-specialist recomenda texto equivalente e estados acessíveis.
- **Alternatives considered**: Manter JSX contradiz a stack declarada pelo projeto. Biblioteca de
  gráficos adicionaria dependência para uma visualização simples.

### Testes

- **Decision**: Novo projeto backend com xUnit v3. Cobrir cálculo/validação no domínio e
  persistência/consulta com PostgreSQL de teste disponibilizado pelo Compose. Frontend usa lint,
  build e verificações manuais do roteiro; não há runner frontend configurado.
- **Rationale**: A Constitution exige regras de domínio testáveis e testes de integração para
  persistência. O repositório não possui projeto de testes. A documentação do xUnit v3 cobre .NET
  8 ou posterior, portanto .NET 10.
- **Alternatives considered**: Testes unitários apenas não cobrem a integração com PostgreSQL. Um
  novo runner de teste frontend não é necessário para a regra de variação, que pertence ao domínio
  backend.
- **Source**: [xUnit v3 getting started](https://xunit.net/docs/getting-started/v3/cmdline).

### Execução de tarefas em segundo plano

- **Decision**: O processo simulador é um Worker separado; a API não hospeda a cadência de geração.
- **Rationale**: Um hosted service é suportado pelo ASP.NET Core, mas o processo independente
  atende melhor ao requisito de parar a fonte sem indisponibilizar o painel. A documentação .NET
  confirma hosted services como alternativa válida dentro do processo, considerada e rejeitada
  por esse motivo funcional.
- **Source**: [Background tasks in ASP.NET Core](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-10.0).

## Consultas especializadas

- **Backend specialist**: manter EF e provedor no Infrastructure; separar horário de medição e
  recebimento; preservar resultado de validação; não acoplar regras de domínio a armazenamento.
- **Frontend specialist**: cumprir TypeScript explicitado pelo projeto apesar do starter JSX;
  gráfico SVG e representação textual; expor estados sem cor como único sinal.
- **Inland-water science**: não inferir volume sem geometria/curva; não inventar limites ou
  classificar estabilidade sem critério; variação é indicador entre pontos observados.
- **Meteorology**: manter precipitação, intervalo e unidade explícitos; ausência não é zero; não
  inferir causalidade.
- **Rural-customer perspective**: valor, horário, origem e variação precisam ser identificáveis
  rapidamente; recomendações são hipóteses, não pesquisa com usuários.
