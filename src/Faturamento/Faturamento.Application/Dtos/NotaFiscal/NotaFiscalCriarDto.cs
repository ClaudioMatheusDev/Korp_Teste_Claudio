using Faturamento.Domain.Enums;

namespace Faturamento.Application.Dtos
{
    public class NotaFiscalCriarDto
    {
        public List<ItemNotaFiscalCriarDto> Itens { get; set; } = [];
    }
}
