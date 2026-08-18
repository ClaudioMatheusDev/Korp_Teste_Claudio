using Estoque.Domain.Entities;

namespace Estoque.Application.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> BuscarProdutoPorIDAsync(int IDProduto);
        Task<List<Produto>> BuscarTodosProdutosAsync();
        Task AdicionarProdutoAsync(Produto produto);
        void Atualizar(Produto produto);
        void Deletar(Produto produto);
        Task SalvarAlteracoesAsync();
        Task<bool> ExisteProdutoComCodigoAsync(int codigo, int? idProdutoIgnorar = null);

    }
}
