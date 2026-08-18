using Faturamento.Domain.Enums;

namespace Faturamento.Application.Dtos
{
    public class NotaFiscalResponseDto
    {
        public int IDNotaFiscal { get; set; }
        public int Numero { get; set; }
        public StatusNotaFiscal Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataFechamento { get; set; }

    }
}
