using Estoque.Domain.Enums;

namespace Estoque.Domain.Entities
{
    public class MovimentacaoEstoque
    {
        public int IDMovimentacaoEstoque { get; set; }

        public int IDProduto { get; set; }

        public TipoMovimentacaoEstoque Tipo { get; set; }

        public int Quantidade { get; set; }

        public int SaldoAnterior { get; set; }

        public int SaldoPosterior { get; set; }

        public string? Motivo { get; set; }

        public int? IDNotaFiscalOrigem { get; set; }

        public DateTime DataMovimentacao { get; set; }

        public Produto Produto { get; set; } = null!;
    }
}
