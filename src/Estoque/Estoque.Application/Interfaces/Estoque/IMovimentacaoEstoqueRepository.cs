using Estoque.Domain.Entities;

namespace Estoque.Application.Interfaces
{
    public interface IMovimentacaoEstoqueRepository
    {
        Task AdicionarAsync(MovimentacaoEstoque movimentacao);
        Task<List<MovimentacaoEstoque>> BuscarProdutoAsync(int IDProduto);
        Task SalvarAsync();
    }
}
