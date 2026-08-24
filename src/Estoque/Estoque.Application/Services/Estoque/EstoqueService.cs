using Estoque.Application.DTOs;
using Estoque.Application.Exceptions;
using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;

namespace Estoque.Application.Services
{
    public class EstoqueService : IEstoqueService
    {
        private const int MaxTentativasConcorrencia = 3;

        private readonly IMovimentacaoEstoqueRepository _movimentacao;
        private readonly IProdutoRepository _produtoRepository;

        public EstoqueService(IMovimentacaoEstoqueRepository movimentacao, IProdutoRepository produtoRepository)
        {
            _movimentacao = movimentacao;
            _produtoRepository = produtoRepository;
        }


        public async Task<int> EntradaAsync(EntradaEstoqueDto dto)
        {
            if (dto.Quantidade <= 0)
                throw new BusinessRuleException("A quantidade deve ser maior que zero.");

            var produto = await _produtoRepository.BuscarProdutoPorIDAsync(dto.IDProduto);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado.");

            var movimentacao = new MovimentacaoEstoque
            {
                IDProduto = produto.IDProduto,
                Tipo = TipoMovimentacaoEstoque.Entrada,
                Quantidade = dto.Quantidade,
                Motivo = dto.Motivo,
                DataMovimentacao = DateTime.Now
            };

            await _movimentacao.AdicionarAsync(movimentacao);

            // Concorrência otimista: se outro processo alterar o saldo desse
            // produto entre a leitura e o salvamento, o EF Core detecta o
            // conflito (via RowVersion) e recarregamos os dados para tentar
            // de novo com o saldo atual, em vez de sobrescrever às cegas.
            for (var tentativa = 1; ; tentativa++)
            {
                movimentacao.SaldoAnterior = produto.QuantidadeEstoque;
                produto.QuantidadeEstoque += dto.Quantidade;
                movimentacao.SaldoPosterior = produto.QuantidadeEstoque;

                try
                {
                    await _movimentacao.SalvarAsync();
                    return produto.IDProduto;
                }
                catch (ConcurrencyConflictException)
                {
                    if (tentativa == MaxTentativasConcorrencia)
                        throw new ConflictException(
                            "Não foi possível concluir a entrada de estoque: o produto foi alterado por outra operação simultânea. Tente novamente.");

                    await _produtoRepository.RecarregarAsync(produto);
                }
            }
        }

        public async Task<MovimentacoesDetalhesDto> MovimentacoesDetalhesAsync(int IDProduto)
        {
            var produto = await _produtoRepository.BuscarProdutoPorIDAsync(IDProduto);

            if (produto == null)
            {
                throw new NotFoundException("Nenhum produto para esse IDProduto.");
            }

            var movimentacao = await _movimentacao.BuscarProdutoAsync(IDProduto);


            var movimentacaoDto = movimentacao
                .Select(t => new MovimentacaoResponseDto
                {
                    IDMovimentacaoEstoque = t.IDMovimentacaoEstoque,
                    IDProduto = t.IDProduto,
                    Tipo = t.Tipo,
                    Quantidade = t.Quantidade,
                    SaldoAnterior = t.SaldoAnterior,
                    SaldoPosterior = t.SaldoPosterior,
                    Motivo = t.Motivo,
                    DataMovimentacao = t.DataMovimentacao,
                }).ToList();

            return new MovimentacoesDetalhesDto
            {
                IDProduto = produto.IDProduto,
                SaldoAtual = produto.QuantidadeEstoque,
                Movimentacoes = movimentacaoDto

            };

        }

        public async Task<int> SaidaAsync(SaidaEstoqueDto dto)
        {
            if (dto.Quantidade <= 0)
                throw new BusinessRuleException("A quantidade deve ser maior que zero.");

            var produto = await _produtoRepository.BuscarProdutoPorIDAsync(dto.IDProduto);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado.");

            var movimentacao = new MovimentacaoEstoque
            {
                IDProduto = produto.IDProduto,
                Tipo = TipoMovimentacaoEstoque.Saida,
                Quantidade = dto.Quantidade,
                Motivo = dto.Motivo,
                DataMovimentacao = DateTime.Now
            };

            await _movimentacao.AdicionarAsync(movimentacao);

            for (var tentativa = 1; ; tentativa++)
            {
                if (produto.QuantidadeEstoque < dto.Quantidade)
                    throw new BusinessRuleException("Saldo insuficiente.");

                movimentacao.SaldoAnterior = produto.QuantidadeEstoque;
                produto.QuantidadeEstoque -= dto.Quantidade;
                movimentacao.SaldoPosterior = produto.QuantidadeEstoque;

                try
                {
                    await _movimentacao.SalvarAsync();
                    return produto.IDProduto;
                }
                catch (ConcurrencyConflictException)
                {
                    if (tentativa == MaxTentativasConcorrencia)
                        throw new ConflictException(
                            "Não foi possível concluir a saída de estoque: o produto foi alterado por outra operação simultânea. Tente novamente.");

                    await _produtoRepository.RecarregarAsync(produto);
                }
            }
        }


        public async Task BaixarLoteAsync(BaixaEstoqueLoteDto dto)
        {
            if (dto.Itens == null || dto.Itens.Count == 0)
                throw new BusinessRuleException("Nenhum item informado para baixa.");

            // Idempotência: se essa nota fiscal já gerou baixa, não repete
            // (protege contra reenvio por retry do lado do Faturamento).
            if (await _movimentacao.ExisteBaixaParaNotaFiscalAsync(dto.IDNotaFiscal))
                return;

            var itensParaBaixa = new List<(Produto Produto, int Quantidade)>();

            foreach (var item in dto.Itens)
            {
                if (item.Quantidade <= 0)
                    throw new BusinessRuleException(
                        $"Quantidade inválida para o produto {item.IDProduto}."
                    );

                var produto =
                    await _produtoRepository.BuscarProdutoPorIDAsync(item.IDProduto);

                if (produto == null)
                    throw new NotFoundException(
                        $"Produto {item.IDProduto} não encontrado."
                    );

                itensParaBaixa.Add((produto, item.Quantidade));
            }

            var movimentacoes = new List<MovimentacaoEstoque>();

            foreach (var item in itensParaBaixa)
            {
                var movimentacao = new MovimentacaoEstoque
                {
                    IDProduto = item.Produto.IDProduto,
                    Tipo = TipoMovimentacaoEstoque.Saida,
                    Quantidade = item.Quantidade,
                    Motivo = $"Saída referente à Nota Fiscal {dto.IDNotaFiscal}",
                    IDNotaFiscalOrigem = dto.IDNotaFiscal,
                    DataMovimentacao = DateTime.Now
                };

                await _movimentacao.AdicionarAsync(movimentacao);
                movimentacoes.Add(movimentacao);
            }

            // Concorrência otimista: se outra requisição alterar o saldo de
            // algum desses produtos entre a leitura e o salvamento (ex: duas
            // notas fiscais dando baixa no mesmo produto ao mesmo tempo), o
            // EF Core detecta o conflito pelo RowVersion e recarregamos os
            // produtos envolvidos para reavaliar as regras com o saldo
            // atualizado, em vez de sobrescrever às cegas.
            for (var tentativa = 1; ; tentativa++)
            {
                for (var i = 0; i < itensParaBaixa.Count; i++)
                {
                    var (produto, quantidade) = itensParaBaixa[i];

                    if (produto.QuantidadeEstoque < quantidade)
                        throw new BusinessRuleException(
                            $"Saldo insuficiente para o produto {produto.IDProduto}."
                        );

                    movimentacoes[i].SaldoAnterior = produto.QuantidadeEstoque;
                    produto.QuantidadeEstoque -= quantidade;
                    movimentacoes[i].SaldoPosterior = produto.QuantidadeEstoque;
                }

                try
                {
                    await _movimentacao.SalvarAsync();
                    return;
                }
                catch (ConcurrencyConflictException)
                {
                    if (tentativa == MaxTentativasConcorrencia)
                        throw new ConflictException(
                            "Não foi possível concluir a baixa de estoque: um ou mais produtos foram alterados por outra operação simultânea. Tente novamente.");

                    foreach (var (produto, _) in itensParaBaixa)
                        await _produtoRepository.RecarregarAsync(produto);
                }
            }
        }

    }
}
