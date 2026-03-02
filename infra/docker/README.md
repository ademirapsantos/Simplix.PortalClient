# Infraestrutura Docker

Base Linux local do ecossistema K.M.K.K.M com quatro servicos:

- `app`: aplicacao principal ASP.NET Core
- `postgres`: banco PostgreSQL
- `update`: servico de validacao de versoes e aprovacao administrativa
- `openclaw-agent`: agente de IA para atendimento e triagem

## Subir o ambiente

```powershell
docker compose -f infra/docker/docker-compose.yml up --build
```

## Endpoints locais

- Aplicacao: `http://localhost:8080`
- Update service: `http://localhost:8081`
- OpenClaw agent: `http://localhost:8082`
- PostgreSQL: `localhost:5434`

## Observacoes

- `update` e `openclaw-agent` foram criados como mocks funcionais para preparar a comunicacao entre os servicos.
- O proximo passo natural e trocar esses mocks por implementacoes reais ou projetos dedicados na solution.
