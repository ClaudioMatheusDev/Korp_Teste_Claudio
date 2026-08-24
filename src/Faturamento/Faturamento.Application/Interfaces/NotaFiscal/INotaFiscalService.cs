using Faturamento.Application.Dtos;

namespace Faturamento.Application.Interfaces
{
    public interface INotaFiscalService
    {
        Task<int> CriarNotaFiscalAsync(NotaFiscalCriarDto dto);
        Task<NotaFiscalDetalhesDto> BuscarNotaFiscalAsync(int IDNotaFiscal);
        Task<List<NotaFiscalResponseDto>> ListarNotasFiscaisAsync();
        Task<bool> ImprimirNotaFiscal(int IDNotaFiscal);
    }
}
