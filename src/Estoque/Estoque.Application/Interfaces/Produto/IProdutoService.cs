using Estoque.Application.DTOs;

namespace Estoque.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<int> CriarProdutoAsync(ProdutoCriarDto produtoCriarDto);
        Task<ProdutoResponseDto> BuscarProdutoPorIDAsync(int IDProduto);
        Task<List<ProdutoResponseDto>> BuscarTodosProdutosAsync();
        Task<bool> ApagarProdutoAsync(int IDProduto);
        Task<bool> AtualizarProdutoAsync(int IDProduto,ProdutoAtualizarDto produtoAtualizarDto);
    }
}