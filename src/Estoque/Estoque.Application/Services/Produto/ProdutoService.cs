using Estoque.Application.DTOs;
using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;

namespace Estoque.Application.Services
{
    public class ProdutoService : IProdutoService
    {

        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<int> CriarProdutoAsync(ProdutoCriarDto produtoCriarDto)
        {
            var produto = new Produto
            {
                Codigo = produtoCriarDto.Codigo,
                Descricao = produtoCriarDto.Descricao,
                ValorProduto = produtoCriarDto.ValorProduto,
                QuantidadeEstoque = produtoCriarDto.QuantidadeEstoque,
                DataCriacao = DateTime.Now
            };
            await _produtoRepository.AdicionarProdutoAsync(produto);
            await _produtoRepository.SalvarAlteracoesAsync();
            return produto.IDProduto;
        }

        public async Task<ProdutoResponseDto> BuscarProdutoPorIDAsync(int IDProduto)
        {
            var produto = await _produtoRepository.BuscarProdutoPorIDAsync(IDProduto);


            if (produto == null)
            {
                throw new Exception("Nenhum produto encontrado com esse IDProduto .");
            }

            return new ProdutoResponseDto
            {
                IDProduto = produto.IDProduto,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                ValorProduto = produto.ValorProduto,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                DataCriacao = produto.DataCriacao,
                DataAtualizacao = produto.DataAtualizacao
            };
        }

        public async Task<List<ProdutoResponseDto>> BuscarTodosProdutosAsync()
        {
            var produtos = await _produtoRepository.BuscarTodosProdutosAsync();

            if (produtos == null || produtos.Count == 0)
            {
                throw new Exception("Nenhum produto encontrado.");
            }

            return produtos.Select(p => new ProdutoResponseDto
            {
                IDProduto = p.IDProduto,
                Codigo = p.Codigo,
                Descricao = p.Descricao,
                ValorProduto = p.ValorProduto,
                QuantidadeEstoque = p.QuantidadeEstoque,
                DataCriacao = p.DataCriacao,
                DataAtualizacao = p.DataAtualizacao
            }).ToList();
        }

        public async Task<bool> AtualizarProdutoAsync(int IDProduto, ProdutoAtualizarDto produtoAtualizarDto)
        {
            var produto = await _produtoRepository.BuscarProdutoPorIDAsync(IDProduto);
            if (produto == null)
            {
                throw new Exception("Nenhum produto encontrado com esse IDProduto.");
            }
            produto.Codigo = produtoAtualizarDto.Codigo;
            produto.Descricao = produtoAtualizarDto.Descricao;
            produto.ValorProduto = produtoAtualizarDto.ValorProduto;
            produto.QuantidadeEstoque = produtoAtualizarDto.QuantidadeEstoque;
            produto.DataAtualizacao = DateTime.Now;

            _produtoRepository.Atualizar(produto);
            await _produtoRepository.SalvarAlteracoesAsync();

            return true;
        }

        public async Task<bool> ApagarProdutoAsync(int IDProduto)
        {
            var produto = await _produtoRepository.BuscarProdutoPorIDAsync(IDProduto);
            if (produto == null)
            {
                throw new Exception("Nenhum produto encontrado com esse IDProduto.");
            }
            _produtoRepository.Deletar(produto);
            await _produtoRepository.SalvarAlteracoesAsync();

            return true;    
        }
    }
}


