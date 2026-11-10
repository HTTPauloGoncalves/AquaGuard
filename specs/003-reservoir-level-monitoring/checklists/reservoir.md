# Reservoir Requirements Quality Checklist: Monitoramento de nível do reservatório com dados simulados

**Purpose**: Revisar completude, clareza e consistência dos requisitos de domínio e apresentação do nível, variação e contexto de precipitação.
**Created**: 2026-11-09
**Feature**: [spec.md](../spec.md)

**Note**: Esta checklist customizada foi gerada pelo `$speckit-checklist` a partir do contexto da funcionalidade.
**Review Ownership**: Artefato de revisão de requisitos pertencente ao revisor. Marque `[x]` somente quando o critério de qualidade do requisito tiver sido revisado e satisfeito.
**Marker Semantics**: `[x]` indica revisão e aprovação da qualidade do requisito; não indica que a implementação foi concluída.

## Completude dos requisitos

- [ ] CHK001 Os requisitos identificam claramente o referencial associado ao nível em metros e distinguem uma leitura simulada de profundidade ou altitude física? [Completeness, Spec §FR-001, Assumptions]
- [ ] CHK002 Os requisitos definem quais metadados acompanham cada medição para que origem, horário, unidade e estado de validação permaneçam interpretáveis? [Completeness, Spec §FR-002–FR-005]
- [ ] CHK003 Os requisitos delimitam explicitamente o que esta funcionalidade não conclui sobre volume, causa hidrológica, risco ou recomendação operacional? [Completeness, Spec §FR-017–FR-019]

## Clareza das regras de domínio

- [ ] CHK004 A regra de variação deixa claro quais medições válidas são os pontos inicial e final, qual fórmula é aplicada e em que unidade o resultado aparece? [Clarity, Spec §FR-009–FR-012]
- [ ] CHK005 Os critérios de validade distinguem verificações estruturais das faixas físicas específicas do reservatório ou cenário, sem sugerir limites universais não documentados? [Clarity, Spec §FR-004–FR-005]
- [ ] CHK006 A seleção do período esclarece se medições exatamente nos horários inicial e final fazem parte do intervalo? [Ambiguity, Spec §FR-007, §FR-009]
- [ ] CHK007 A apresentação de dado antigo explica sua recência sem depender de um limite de “desatualizado” ainda não definido? [Clarity, Spec §FR-014, Assumptions]

## Consistência dos requisitos

- [ ] CHK008 A regra de ordenação por horário de medição é consistente com o tratamento de dados atrasados, horários repetidos e escolha da última medição válida? [Consistency, Spec §FR-006, §FR-013]
- [ ] CHK009 O uso de “ausência de variação observada” permanece distinto de “estável” em histórias, requisitos e critérios de sucesso? [Consistency, Spec §FR-011, SC-003]
- [ ] CHK010 A precipitação aparece apenas como contexto opcional e não contradiz a proibição de afirmar causalidade entre chuva e variação do nível? [Consistency, Spec §FR-015–FR-017]

## Qualidade dos critérios de aceitação

- [ ] CHK011 Os critérios de sucesso permitem avaliar objetivamente cálculo, indisponibilidade de variação, ordem temporal e identificação da origem simulada? [Measurability, Spec §SC-001–SC-006]
- [ ] CHK012 Os critérios de sucesso definem como avaliar a compreensão do usuário sem tratar recomendações ainda não validadas como pesquisa concluída? [Acceptance Criteria, Spec §SC-001, §SC-003, Assumptions]

## Cobertura de cenários e casos-limite

- [ ] CHK013 Os cenários cobrem ausência de medições, somente uma medição válida, dados inválidos, atrasos e horários iguais ou ambíguos? [Coverage, Spec §User Stories 1–2, Edge Cases]
- [ ] CHK014 Os requisitos distinguem precipitação igual a zero de dado ausente, intervalo desconhecido e período de acumulação incompatível? [Coverage, Spec §FR-015–FR-017, Edge Cases]
- [ ] CHK015 Os requisitos explicam como a série histórica apresenta lacunas e intervalos irregulares sem interpolação ou taxa temporal enganosa? [Edge Cases, Spec §FR-013, User Story 2]

## Requisitos não funcionais e dependências

- [ ] CHK016 Os requisitos de apresentação especificam como horários e intervalos são interpretados para evitar ambiguidade temporal ao comparar nível e precipitação? [Gap, Spec §FR-002, §FR-010, §FR-015]
- [ ] CHK017 Há requisitos de acessibilidade para interpretar valores, variações, histórico e estados indisponíveis sem depender apenas de cor ou forma gráfica? [Gap, User Stories 1–3]
- [ ] CHK018 As dependências de disponibilidade da precipitação simulada e do referencial do nível estão documentadas, com comportamento definido quando cada uma faltar? [Dependency, Assumptions, Edge Cases]

## Ambiguidades e conflitos

- [ ] CHK019 A diferença entre “última medição registrada” e “nível atual” é consistente em toda a especificação e não promete observação em tempo real? [Ambiguity, Spec §FR-006, §FR-014]
- [ ] CHK020 As decisões deixadas para planejamento — cadência, recência e períodos predefinidos — permanecem separadas das regras funcionais já fixadas? [Assumption, Assumptions]

## Notes

- Esta checklist avalia a qualidade do texto dos requisitos; não verifica código nem funcionamento da implementação.
- Os itens foram gerados a partir da especificação; `plan.md` e `tasks.md` ainda não existem.
- Marque os itens após revisão. `$speckit-implement` lê o estado da checklist, mas não altera seus marcadores.
- `checklists/requirements.md` é uma checklist distinta, mantida por `$speckit-specify` e `$speckit-clarify`.
