using Faturamento.Application.Dtos;
using Faturamento.Domain.Enums;

public class NotaFiscalDetalhesDto
{
    public int IDNotaFiscal { get; set; }
    public int Numero { get; set; }
    public StatusNotaFiscal Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataFechamento { get; set; }

    public List<ItemNotaFiscalResponseDto> Itens { get; set; } = [];
}