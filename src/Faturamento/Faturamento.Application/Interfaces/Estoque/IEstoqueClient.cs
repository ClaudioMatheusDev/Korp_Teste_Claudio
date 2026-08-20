using Faturamento.Application.Dtos;

namespace Faturamento.Application.Interfaces
{
    public interface IEstoqueClient
    {
        Task BaixarEstoqueLoteAsync(BaixaEstoqueLoteDto dto);
    }
}
