<!--
Sync Impact Report
- Version change: scaffold → 1.0.0 (initial adoption; original ratification date unknown)
- Modified principles: none (initial constitution)
- Added sections: Core Principles (14 principles), Domain and Operational Constraints,
  Development and Governance Workflow
- Removed sections: none
- Follow-up TODOs: confirm the original ratification date
-->
# AquaGuard Constitution

## Core Principles

### I. Engenharia orientada ao problema real
O desenvolvimento MUST começar pela compreensão do problema real, antes da escolha de
tecnologias. Toda funcionalidade MUST ter justificativa de negócio ou técnica. O sistema MUST
transformar dados físicos em informações confiáveis e úteis, seguindo o fluxo: mundo físico,
medição, coleta, comunicação, validação, processamento, informação e decisão. Essa ordem mantém
as escolhas técnicas ligadas ao uso real do AquaGuard.

### II. Arquitetura modular e responsabilidades explícitas
O sistema MUST manter responsabilidades claras entre Domain (entidades e regras), Application
(casos de uso), Infrastructure (persistência e integrações), API (serviços e contratos HTTP),
Frontend (apresentação e interação) e Edge (sensores e dispositivos físicos). Regras de negócio
MUST NOT depender diretamente de frameworks, bancos de dados ou tecnologias de comunicação.
Abstrações MUST ter necessidade demonstrável; isso preserva a independência do domínio sem criar
complexidade prematura.

### III. Integridade e confiabilidade dos dados
Medições de sensores MUST ser tratadas como dados físicos sujeitos a erro. O sistema MUST
distinguir dados recebidos, validados, processados e apresentados. Validações e processamento
MUST considerar ruído, valores fisicamente impossíveis, duplicidade, atraso, perda de comunicação,
falhas intermitentes e calibração. Receber uma leitura MUST NOT, por si só, torná-la confiável.

### IV. Fundamentação física e científica
Regras relativas a sensores, hidrologia, meteorologia e matemática MUST ter fundamentação técnica.
Fórmulas, conversões e cálculos MUST ser documentados com variáveis, unidades e limitações. O
sistema MUST NOT inventar parâmetros físicos, coeficientes, limites ou características de sensores.
Fatos conhecidos, hipóteses e valores provisórios MUST ser identificados explicitamente, e
correlações MUST NOT ser apresentadas como causalidade comprovada.

### V. Regras de domínio testáveis
Regras de negócio MUST ser testáveis sem depender da infraestrutura. A cobertura MUST priorizar
cálculos físicos, conversões de unidades, validação e processamento de medições, detecção de
anomalias, alertas e controle de acesso. Testes de integração MUST verificar interações com bancos
de dados e serviços externos quando esses contratos forem alterados ou introduzidos. Isso reduz o
risco de falhas silenciosas no domínio e nas integrações.

### VI. Segurança desde a concepção
As soluções MUST aplicar autenticação e autorização, isolamento de dados entre clientes,
validação de entradas, comunicação segura, proteção de credenciais, controle de acesso a
dispositivos e integridade das mensagens recebidas, conforme aplicável. Toda operação que leia ou
altere dados de uma propriedade MUST verificar autorização para essa propriedade. Nenhum usuário
MUST acessar informações de propriedades sem autorização.

### VII. Comunicação tolerante a falhas
O sistema MUST considerar conectividade limitada em ambientes rurais. Comunicação entre
componentes MUST tratar timeouts, reconexões, retries limitados, backoff, duplicatas, mensagens
atrasadas e perda de conexão, quando aplicável. A coleta e o funcionamento local MUST NOT depender
da disponibilidade contínua da internet; períodos offline e sincronização posterior devem ser
considerados no desenho dos fluxos envolvidos.

### VIII. Dispositivos e protocolos substituíveis
Drivers MUST encapsular particularidades de equipamentos e protocolos. Substituir sensor ou
protocolo MUST NOT exigir alterações desnecessárias nas regras de negócio. MQTT, HTTP, Serial,
RS-485 e Modbus podem ser considerados quando compatíveis com os requisitos do dispositivo e do
ambiente; a escolha MUST ser justificada, sem presumir que todos se aplicam a cada integração.

### IX. Simplicidade e decisões justificadas
Decisões técnicas MUST considerar complexidade, custo, confiabilidade, manutenção, escala,
aprendizado e aplicabilidade real. Tecnologias MUST NOT ser introduzidas apenas por popularidade.
Microsserviços, mensageria e abstrações distribuídas MUST ter necessidade demonstrável. A solução
deve atender aos requisitos atuais e permitir evolução sem antecipar complexidade não requerida.

### X. Experiência orientada à operação
O frontend MUST ajudar o usuário a compreender a situação de suas propriedades e reservatórios.
Indicadores, tendências, históricos, alertas e estado dos dispositivos MUST ser apresentados de
forma clara e acionável. Visualizações MUST ter propósito operacional; informação decorativa não
deve substituir contexto necessário para interpretar uma medição ou um alerta.

### XI. Automação explicável e baseada em evidências
Detecção inicial de anomalias MUST priorizar regras determinísticas e explicáveis. Machine
Learning MUST ser introduzido somente quando houver dados suficientes e justificativa técnica.
Informações geradas por modelos MUST apresentar contexto, limitações e incerteza relevantes. IA
MUST NOT substituir prematuramente modelos físicos ou regras determinísticas validadas.

### XII. Documentação e rastreabilidade
Requisitos, decisões arquiteturais e regras importantes MUST ser documentados. Implementações
MUST permanecer alinhadas às especificações aprovadas, e mudanças relevantes MUST atualizar a
documentação correspondente. A documentação deve permitir rastrear por que uma regra ou decisão
foi adotada.

### XIII. Desenvolvimento assistido por IA com responsabilidade
Agentes de IA MUST respeitar os limites de responsabilidade dos domínios e seguir especificações
e decisões arquiteturais aprovadas. Agentes MUST NOT inventar requisitos, características físicas
ou decisões de negócio. Mudanças arquiteturais propostas por agentes MUST incluir justificativa.
Implementações assistidas por IA MUST manter compreensíveis os conceitos técnicos relevantes.

### XIV. Evolução guiada por entendimento
A evolução do AquaGuard MUST seguir esta sequência: compreender o problema, definir requisitos,
compreender os dados físicos, definir regras de negócio, projetar a solução, implementar, validar
e evoluir. O objetivo de cada etapa é construir um sistema sólido, confiável, compreensível e
potencialmente utilizável em propriedades rurais.

## Domain and Operational Constraints

O domínio inclui reservatórios e propriedades rurais, sensores e estações, telemetria, medições
ambientais e interpretação de dados físicos. Nível de água MUST NOT ser tratado como volume sem
uma relação nível-volume fundamentada na geometria do reservatório. Alertas e correlações
meteorológicas MUST ser apresentados como indicadores com evidência e limitações, e MUST NOT
constituir diagnóstico definitivo sem base suficiente. Limites, calibrações e parâmetros MUST
ser fornecidos por especificação técnica, medição ou decisão de domínio documentada.

## Development and Governance Workflow

Mudanças MUST começar com requisitos e critérios de validação proporcionais ao risco. Revisões
MUST verificar aderência aos princípios aplicáveis, segurança, qualidade dos dados e documentação.
O primeiro MVP pode simular medições antes da adoção de hardware real; essa simulação MUST ser
identificada como tal e MUST NOT ser confundida com observação física. Sugestões não aprovadas
MUST NOT ser convertidas silenciosamente em requisitos.

## Governance

Esta constituição governa decisões de produto e engenharia do AquaGuard. Em caso de conflito, os
princípios aqui definidos prevalecem sobre práticas locais incompatíveis, salvo alteração formal
desta constituição. Uma emenda MUST documentar a motivação, os princípios e seções afetados, a
avaliação de compatibilidade e a migração necessária. A equipe responsável pelo projeto MUST
revisar e aprovar emendas antes de tratá-las como vigentes.

O versionamento segue Semantic Versioning: MAJOR para remoção ou redefinição incompatível de
princípios; MINOR para novos princípios ou expansão material de obrigações; PATCH para
esclarecimentos e correções sem mudança normativa. Revisões de mudanças MUST avaliar conformidade
com esta constituição e registrar exceções com justificativa, escopo e prazo de revisão. A
constituição MUST ser revisada quando mudanças de domínio ou arquitetura tornarem suas regras
inadequadas.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): confirmar a data original de adoção | **Last Amended**: 2026-11-09
