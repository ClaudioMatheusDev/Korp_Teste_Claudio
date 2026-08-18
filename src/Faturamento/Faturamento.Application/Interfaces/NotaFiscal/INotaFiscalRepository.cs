using Faturamento.Domain.Entities;

namespace Faturamento.Application.Interfaces
{
    public interface INotaFiscalRepository
    {
        Task AdicionarNotaAsync(NotaFiscal notaFiscal);
        Task<NotaFiscal?> BuscarNotaFiscalPorIDAsync(int IDNotaFiscal);
        Task<List<NotaFiscal>> ListarTodasNotasFiscais();
        Task SalvarAlteracoesAsync();
        Task<int> BuscarProximoNumeroAsync();
    }
}
