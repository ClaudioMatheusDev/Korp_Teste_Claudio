# Detalhamento Técnico

Este documento cobre os pontos técnicos pedidos no edital para o desafio
"Sistema de emissão de Notas Fiscais". Para visão geral do projeto e como
rodá-lo, veja o [README.md](README.md).

## 1. Ciclos de vida do Angular utilizados

O único hook de ciclo de vida usado é o **`ngOnInit`**, em `ProdutosComponent`
e `NotasComponent`, para carregar a listagem assim que o componente é
inicializado:

```ts
// frontend/src/app/pages/notas/notas.component.ts
export class NotasComponent implements OnInit {
  ngOnInit(): void {
    this.carregarNotas();
  }
}
```

Não há `ngOnDestroy` porque não existem *subscriptions* de longa duração para
cancelar: toda chamada HTTP feita com `HttpClient` retorna um `Observable`
que **completa sozinho** após emitir a resposta (sucesso ou erro), então não
há vazamento de memória a se preocupar nesses componentes. Também não há
`ngOnChanges` nem `ngAfterViewInit`, pois nenhum dos componentes recebe
`@Input()` nem precisa inspecionar a view depois de renderizada.

## 2. Uso de RxJS

Sim. Todo o `ProdutoService` e `NotaFiscalService` retornam `Observable`
(via `HttpClient`), consumidos nos componentes com `.subscribe({ next, error })`.

Além do uso básico, o operador **`finalize()`** é usado para centralizar a
limpeza do estado de carregamento/salvamento — ele roda tanto no caminho de
sucesso quanto no de erro, eliminando a duplicação de `this.salvando = false`
em cada branch:

```ts
// frontend/src/app/pages/notas/notas.component.ts
this.notaFiscalService
  .imprimir(nota.idNotaFiscal)
  .pipe(finalize(() => (this.imprimindoId = null)))
  .subscribe({
    next: (resposta) => { /* ... */ },
    error: (err) => { /* ... */ },
  });
```

## 3. Outras bibliotecas utilizadas

**Frontend**
- `@angular/forms` (Reactive Forms) — construção e validação dos formulários
  de Produto e Nota Fiscal (`FormBuilder`, `FormGroup`, `FormArray` para a
  lista dinâmica de itens da nota).
- `rxjs` — ver item 2.

**Backend (C#)**
- `Microsoft.EntityFrameworkCore` + `Microsoft.EntityFrameworkCore.SqlServer`
  — ORM e migrations para os dois bancos (Estoque e Faturamento).
- `Microsoft.Extensions.Http.Resilience` — política padrão de retry e
  circuit breaker no `HttpClient` que o Faturamento usa para chamar o
  Estoque, para tolerar falhas transitórias de rede.

## 4. Bibliotecas de componentes visuais

**Angular Material** (`@angular/material` + `@angular/cdk`): `mat-toolbar`
(navegação), `mat-card` (formulários), `mat-form-field`/`mat-input`
(campos), `mat-table` (listagens de Produtos e Notas), `mat-chip`
(indicador de status Aberta/Fechada), `mat-progress-spinner` (indicador de
carregamento e de impressão em andamento) e `mat-snack-bar` (feedback de
sucesso/erro).

## 5. Gerenciamento de dependências no Golang

Não aplicável — o backend foi implementado em **C#/.NET**, não em Golang.
O gerenciamento de dependências do backend é feito via **NuGet**
(`PackageReference` nos arquivos `.csproj` de cada projeto), e o do
frontend via **npm** (`package.json`).

## 6. Frameworks utilizados (C#)

- **ASP.NET Core Web API** (.NET 9) — nos dois microsserviços
  (`Estoque.API` e `Faturamento.API`).
- **Entity Framework Core** — ORM usado como camada de acesso a dados em
  ambos os serviços, com SQL Server como banco físico.

## 7. Tratamento de erros e exceções no backend

- **Exceções de domínio customizadas**, lançadas pelas camadas de
  Application: `NotFoundException`, `BusinessRuleException`,
  `ConflictException` (Estoque) e `NotFoundException`,
  `BusinessRuleException`, `EstoqueIndisponivelException` (Faturamento).
- **Handler global de exceção** em cada API, via `IExceptionHandler`
  (`GlobalExceptionHandler`, registrado com `AddExceptionHandler` +
  `AddProblemDetails`), que mapeia cada tipo de exceção para o status HTTP
  correto (404, 400, 409, 503 ou 500) e devolve um `ProblemDetails`
  consistente — sem vazar detalhes internos em erros 500. Isso eliminou a
  necessidade de `try/catch` repetido em cada endpoint dos controllers.
- **Diferenciação de causa de falha na chamada entre serviços**: o
  `EstoqueClient` (Faturamento → Estoque) trata separadamente falha de
  rede/timeout (`HttpRequestException`/`TaskCanceledException`, mapeada
  para `EstoqueIndisponivelException` → 503) de um erro de negócio
  devolvido pelo próprio Estoque (400/404/409, mapeado para
  `BusinessRuleException`).
- **Resiliência**: a chamada HTTP do Faturamento para o Estoque usa a
  política padrão do `Microsoft.Extensions.Http.Resilience` (retry com
  backoff + circuit breaker + timeout), para se recuperar sozinha de
  falhas transitórias antes de reportar erro ao usuário.
- **Idempotência como parte do tratamento de falha**: a baixa de estoque é
  identificada pela nota fiscal de origem (`IDNotaFiscalOrigem`), então se
  uma chamada for repetida após uma falha parcial (ex: Estoque confirma a
  baixa mas o Faturamento falha ao salvar o fechamento da nota), o
  reprocessamento não duplica o efeito.

## Tratamento de concorrência (opcional)

Cenário do edital: produto com saldo 1 sendo usado simultaneamente por duas
notas fiscais. Sem tratamento, as duas requisições leem saldo = 1, ambas
passam na validação e ambas gravam — o saldo final fica incorreto (ou
negativo), dependendo da ordem de escrita.

A solução usa **concorrência otimista do EF Core**: a entidade `Produto`
ganhou uma coluna `RowVersion` (`[Timestamp]`, tipo `rowversion` do SQL
Server, atualizada automaticamente pelo banco a cada `UPDATE`). Quando duas
requisições carregam o mesmo produto e uma delas salva primeiro, a segunda
tenta salvar com um `RowVersion` desatualizado — o EF Core detecta isso e
lança `DbUpdateConcurrencyException` em vez de sobrescrever o dado.

`EstoqueService` (`EntradaAsync`, `SaidaAsync`, `BaixarLoteAsync`) captura
esse conflito, **recarrega o produto do banco** (`RecarregarAsync`) e
**reavalia a regra de negócio com o saldo atual**, tentando de novo — até 3
vezes. Se o saldo já foi consumido por outra requisição, a segunda nota
recebe `BusinessRuleException` ("Saldo insuficiente"), não um erro genérico
nem uma sobrescrita silenciosa. Testado ao vivo: produto com saldo 1 +
duas notas simultâneas → uma dá baixa com sucesso, a outra recebe 400 com
mensagem clara, saldo final = 0 (nunca negativo).

Para manter a camada de Application livre de dependência do EF Core, a
exceção é traduzida na borda do repositório
(`Estoque.Infrastructure/Repositories/Estoque/MovimentacaoEstoqueRepository.cs`)
para uma exceção própria da Application (`ConcurrencyConflictException`).

## 8. Uso de LINQ

Usado extensivamente na camada de Application e nos repositórios do EF
Core, principalmente para:

- **Mapear entidade → DTO**: `produtos.Select(p => new ProdutoResponseDto {...})`
  (`Estoque.Application/Services/Produto/ProdutoService.cs`).
- **Consultas com EF Core** (traduzidas para SQL pelo provider): `.Include()`
  para carregar itens da nota fiscal, `.FirstOrDefaultAsync()`,
  `.OrderByDescending()` para listar notas por data de criação
  (`Faturamento.Infrastructure/Repositories/NotaFiscal/NotaFiscalRepository.cs`),
  `.Where()` para filtrar movimentações por produto
  (`Estoque.Infrastructure/Repositories/Estoque/MovimentacaoEstoqueRepository.cs`).
- **Validações de negócio**: `.Any()` para checar se já existe produto com
  o mesmo código ou se uma nota já teve baixa de estoque registrada;
  `.MaxAsync()` para calcular o próximo número sequencial da nota fiscal
  (`BuscarProximoNumeroAsync`).
