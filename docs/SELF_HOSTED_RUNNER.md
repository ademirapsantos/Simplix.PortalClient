# Self-Hosted Runner (Linux)

Este projeto agora usa jobs com:

- `self-hosted`
- `Linux`
- `X64`

## 1. Pre-requisitos no servidor runner

Instalar:

- `git`
- `curl`
- `tar`
- `docker` + `docker compose`
- `.NET SDK 9`

## 2. Criar usuario de runner (recomendado)

```bash
sudo useradd -m -s /bin/bash actions
sudo usermod -aG docker actions
sudo su - actions
```

## 3. Baixar e configurar o runner

No GitHub:

- Repositorio -> `Settings` -> `Actions` -> `Runners` -> `New self-hosted runner`
- Escolher `Linux` / `x64`
- Copiar comandos de download/config

Executar no servidor (como usuario `actions`), exemplo:

```bash
mkdir -p ~/actions-runner && cd ~/actions-runner
curl -o actions-runner-linux-x64.tar.gz -L <URL_FORNECIDA_PELO_GITHUB>
tar xzf actions-runner-linux-x64.tar.gz
./config.sh --url <URL_REPO> --token <TOKEN_TEMPORARIO> --labels Linux,X64
```

Observacao:

- O label `self-hosted` ja e adicionado automaticamente.
- Garantir que os labels finais fiquem: `self-hosted`, `Linux`, `X64`.

## 4. Instalar como servico

```bash
sudo ./svc.sh install
sudo ./svc.sh start
sudo ./svc.sh status
```

## 5. Validar

No GitHub Actions, rodar workflow manual (`workflow_dispatch`) e confirmar que os jobs iniciam no runner local.

## 6. Operacao

- Para atualizar o runner:
```bash
cd ~/actions-runner
./svc.sh stop
./svc.sh start
```
- Para logs do servico:
```bash
journalctl -u actions.runner.* -f
```
