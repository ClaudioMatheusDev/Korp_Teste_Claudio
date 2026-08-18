using Faturamento.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Faturamento.Domain.Entities
{
    public class NotaFiscal
    {
        [Key]
        public int IDNotaFiscal { get; set; }
        public int Numero { get; set; }
        public StatusNotaFiscal Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataFechamento { get; set; }  
        public ICollection<ItemNotaFiscal> Itens { get; set; } = new List<ItemNotaFiscal>();
    }
}
