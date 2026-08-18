using Estoque.Application.DTOs;
using Estoque.Application.Exceptions;
using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;

namespace Estoque.Application.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IMovimentacaoEstoqueRepository _movimentacao;
        private readonly IProdutoRepository _produtoRepository;

        public EstoqueService(IMovimentacaoEstoqueRepository movimentacao, IProdutoRepository produtoRepository)
        {
            _movimentacao = movimentacao;
            _produtoRepository = produtoRepository;
        }

        public async Task<int> EntradaAsync(EntradaEstoqueDto dto)
        {
            var produto = await _produtoRepository.BuscarProdutoPorIDAsync(dto.IDProduto);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado.");

            if (dto.Quantidade <= 0)
                throw new BusinessRuleException("A quantidade deve ser maior que zero.");

            var saldoAnterior = produto.QuantidadeEstoque;

            produto.QuantidadeEstoque += dto.Quantidade;

            var movimentacao = new MovimentacaoEstoque
            {
                IDProduto = produto.IDProduto,
                Tipo = TipoMovimentacaoEstoque.Entrada,
                Quantidade = dto.Quantidade,
                SaldoAnterior = saldoAnterior,
                SaldoPosterior = produto.QuantidadeEstoque,
                Motivo = dto.Motivo,
                DataMovimentacao = DateTime.Now
            };

            await _movimentacao.AdicionarAsync(movimentacao);
            await _movimentacao.SalvarAsync();

            return produto.IDProduto;
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
            var produto = await _produtoRepository.BuscarProdutoPorIDAsync(dto.IDProduto);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado.");

            if (dto.Quantidade <= 0)
                throw new BusinessRuleException("A quantidade deve ser maior que zero.");

            if (produto.QuantidadeEstoque < dto.Quantidade)
                throw new BusinessRuleException("Saldo insuficiente.");

            var saldoAnterior = produto.QuantidadeEstoque;

            produto.QuantidadeEstoque -= dto.Quantidade;

            var movimentacao = new MovimentacaoEstoque
            {
                IDProduto = produto.IDProduto,
                Tipo = TipoMovimentacaoEstoque.Saida,
                Quantidade = dto.Quantidade,
                SaldoAnterior = saldoAnterior,
                SaldoPosterior = produto.QuantidadeEstoque,
                Motivo = dto.Motivo,
                DataMovimentacao = DateTime.Now
            };

            await _movimentacao.AdicionarAsync(movimentacao);
            await _movimentacao.SalvarAsync();

            return produto.IDProduto;
        }

    }
}
