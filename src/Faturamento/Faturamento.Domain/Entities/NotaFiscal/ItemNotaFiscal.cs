using System.ComponentModel.DataAnnotations;

namespace Faturamento.Domain.Entities
{
    public class ItemNotaFiscal
    {
        [Key]
        public int IDItemNotaFiscal { get; set; }
        public int IDNotaFiscal { get; set; }
        public int IDProduto { get; set; }
        public int Quantidade { get; set; }
        public NotaFiscal NotaFiscal { get; set; } = null!;
    }
}
