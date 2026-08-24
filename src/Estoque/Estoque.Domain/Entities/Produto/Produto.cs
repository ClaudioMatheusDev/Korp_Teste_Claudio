using System.ComponentModel.DataAnnotations;

namespace Estoque.Domain.Entities
{
    public class Produto
    {
        public int IDProduto { get; set; }
        public int Codigo { get; set; }
        public required string Descricao { get; set; }
        public decimal ValorProduto { get; set; }
        public int QuantidadeEstoque { get; set; }
        public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }

        /// <summary>
        /// Token de concorrência otimista (SQL Server rowversion, gerado e
        /// atualizado automaticamente pelo banco a cada UPDATE). Usado pelo
        /// EF Core para detectar quando dois processos tentam alterar o
        /// mesmo produto ao mesmo tempo (ex: duas notas fiscais dando baixa
        /// no mesmo produto simultaneamente).
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
