using Estoque.Domain.Enums;

namespace Estoque.Application.DTOs
{
    public class MovimentacaoResponseDto
    {
        public int IDMovimentacaoEstoque { get; set; }

        public int IDProduto { get; set; }

        public TipoMovimentacaoEstoque Tipo { get; set; }

        public int Quantidade { get; set; }

        public int SaldoAnterior { get; set; }

        public int SaldoPosterior { get; set; }

        public string? Motivo { get; set; }

        public DateTime DataMovimentacao { get; set; }
    }
}
