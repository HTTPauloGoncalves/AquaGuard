# Quickstart: Monitoramento de nível do reservatório

Este roteiro define a validação manual ponta a ponta após a implementação. Todos os dados deste
ambiente são simulados e não representam medições ou limites de um reservatório real.

## Pré-requisitos

- Docker Engine com Docker Compose plugin.
- Arquivo de ambiente local criado a partir de `backend/.env.example`; valores e credenciais são
  locais e não devem ser versionados.
- Portas locais documentadas no Compose livres.

O Compose publica o PostgreSQL em `127.0.0.1:5432` por padrão. Se essa porta já estiver ocupada,
defina `POSTGRES_PORT=5433` (ou outra porta livre) em `backend/.env` antes de iniciar os serviços.
API e simulador comunicam-se pela porta interna `5432` do serviço `postgres`; somente conexões
iniciadas no host usam a porta configurada. Ao executar os testes no host com uma porta alternativa,
configure `AQUAGUARD_TEST_CONNECTION` para apontar à porta escolhida e ao banco `aquaguard_test`.

Se o volume PostgreSQL já existia antes da inclusão do banco `aquaguard_test`, os scripts de
inicialização não serão executados novamente. Depois que o serviço `postgres` estiver saudável,
crie somente a base de testes com:

```sh
docker compose --env-file backend/.env -f backend/docker-compose.yml exec postgres sh -c \
  'createdb --username "$POSTGRES_USER" --maintenance-db "$POSTGRES_DB" --owner "$POSTGRES_USER" "$POSTGRES_TEST_DB"'
```

## Iniciar a demonstração

1. Na raiz do repositório, iniciar os serviços definidos em `backend/docker-compose.yml`:

   ```sh
   docker compose --env-file backend/.env -f backend/docker-compose.yml up --build -d
   ```

2. Aguardar health checks de PostgreSQL, API, simulador e frontend.
3. Abrir o endereço local do frontend indicado no Compose.
4. Confirmar que o painel identifica o reservatório de demonstração, mostra nível em metros,
   horário com fuso, origem simulada e série histórica.
5. Aguardar ao menos dois ciclos do simulador. Confirmar que a variação mostra Δh = h_final −
   h_inicial e os horários comparados.

## Validar histórico e dados ausentes

1. Selecionar cada período disponível (1h, 6h, 24h, 7d e intervalo personalizado).
2. Confirmar a ordenação pelo horário de medição e que o intervalo escolhido está visível.
3. Consultar um intervalo anterior ao início da simulação; o painel deve mostrar ausência de dados,
   sem fabricar nível ou variação.
4. Comparar medições em horários irregulares; a interface deve mostrar os horários reais e não
   calcular taxa por hora ou interpolar pontos.

## Validar perda e recuperação de comunicação

1. Parar somente o serviço simulador:

   ```sh
   docker compose --env-file backend/.env -f backend/docker-compose.yml stop aquaguard-simulator
   ```

2. Confirmar que API e frontend permanecem disponíveis, a última medição conserva seu horário e
   não é apresentada como recém-observada.
3. Reiniciar o simulador:

   ```sh
   docker compose --env-file backend/.env -f backend/docker-compose.yml start aquaguard-simulator
   ```

4. Confirmar que novas medições aparecem sem reordenar o histórico pelo horário de recebimento.

## Validar precipitação contextual

1. Habilitar a série de precipitação no histórico.
2. Confirmar milímetros, origem simulada e intervalo de acumulação.
3. Confirmar que zero e ausência de dados têm estados diferentes e que a interface não afirma que
   chuva causou variação do nível.

## Testes automatizados

- Com os serviços PostgreSQL do Compose em execução, executar `dotnet test backend/AquaGuard.slnx`.
  O projeto de testes usa `AQUAGUARD_TEST_CONNECTION` quando definido; sem ele, conecta ao banco
  isolado `aquaguard_test` / `aquaguard-local`, criado pelo script de inicialização do PostgreSQL.
  Se `POSTGRES_PORT` diferir de `5432`, defina a conexão de teste com a porta publicada antes de
  executar o comando.
- Executar `npm run lint` e `npm run build` em `frontend/aquaguard-web/`.

## Resultado esperado

- O painel e a API permanecem disponíveis quando o processo simulador é parado.
- Leituras válidas aparecem com origem, unidade e horário consistentes.
- Leituras atrasadas não substituem a medição mais recente pelo horário de recebimento.
- Variações são reproduzíveis e seguem a regra definida na especificação.
- Nenhum volume, causalidade meteorológica ou limite científico é inferido dos dados sintéticos.
