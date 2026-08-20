using Estoque.Application.DTOs;

namespace Estoque.Application.Interfaces
{
    public interface IEstoqueService
    {
        Task<int> EntradaAsync(EntradaEstoqueDto dto);
        Task<int> SaidaAsync(SaidaEstoqueDto dto);
        Task<MovimentacoesDetalhesDto> MovimentacoesDetalhesAsync(int IDProduto);
        Task BaixarLoteAsync(BaixaEstoqueLoteDto dto);
    }
}
