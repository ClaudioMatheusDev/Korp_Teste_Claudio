# Sistema de Emissão de Notas Fiscais

Projeto técnico desenvolvido para o desafio da Korp: sistema de emissão de notas
fiscais com baixa de estoque, estruturado como dois microsserviços .NET e um
frontend em Angular.

> Detalhamento técnico completo (ciclos de vida, RxJS, bibliotecas, LINQ,
> tratamento de erros etc.): [DETALHAMENTO_TECNICO.md](DETALHAMENTO_TECNICO.md)

## Arquitetura

```text
frontend/                     Angular 19 (Material) — telas de Produtos e Notas Fiscais
src/
  Estoque/                    Microsserviço de Estoque (produtos e saldos)
    Estoque.API               ASP.NET Core Web API — porta 5268
    Estoque.Application       Casos de uso, DTOs, exceções de domínio
    Estoque.Domain            Entidades (Produto, MovimentacaoEstoque)
    Estoque.Infrastructure    EF Core + SQL Server (banco Korp_Produto)
  Faturamento/                Microsserviço de Faturamento (notas fiscais)
    Faturamento.API           ASP.NET Core Web API — porta 5237
    Faturamento.Application   Casos de uso, DTOs, exceções de domínio
    Faturamento.Domain        Entidades (NotaFiscal, ItemNotaFiscal)
    Faturamento.Infrastructure EF Core + SQL Server (banco Korp_Faturamento)
                               + cliente HTTP para o Estoque
```

O Faturamento não acessa o banco do Estoque diretamente: ao imprimir uma nota,
ele chama o Estoque via HTTP (`POST /api/estoque/baixar-lote`) para dar baixa
nos produtos. Essa chamada usa retry/circuit-breaker padrão do .NET
(`Microsoft.Extensions.Http.Resilience`) e é idempotente por nota fiscal.

## Pré-requisitos

- .NET SDK 9 (ou superior)
- Node.js 22+ e npm
- SQL Server (local ou LocalDB) — as connection strings em
  `appsettings.json` apontam para `(localdb)\MinhaInstancia` por padrão

## Como rodar

### Opção 1 — Docker Compose (sobe tudo de uma vez)

```bash
docker compose up --build
```

Sobe os dois bancos SQL Server, as duas APIs (com migrations aplicadas
automaticamente ao iniciar) e o frontend via Nginx em `http://localhost:4200`.

### Opção 2 — manualmente, em 3 terminais

```bash
# 1. Estoque.API (porta 5268)
cd src/Estoque/Estoque.API
dotnet ef database update 
dotnet run --urls http://localhost:5268

# 2. Faturamento.API (porta 5237)
cd src/Faturamento/Faturamento.API
dotnet ef database update
dotnet run --urls http://localhost:5237

# 3. Frontend Angular (porta 4200)
cd frontend
npm install
npm start
```

Acesse `http://localhost:4200`.

## Funcionalidades

- Cadastro de Produtos (código, descrição, saldo)
- Cadastro de Notas Fiscais com numeração sequencial e múltiplos produtos
- Impressão de nota fiscal: dá baixa no estoque dos produtos utilizados e
  fecha a nota — com indicador de carregamento e tratamento de falha do
  Estoque na tela
- Idempotência: reprocessar uma baixa de estoque ou uma impressão já
  concluída não duplica o efeito
- Concorrência: baixa de estoque usa concurrency token do EF Core
  (`RowVersion`) para impedir saldo negativo quando duas notas fiscais dão
  baixa no mesmo produto ao mesmo tempo
