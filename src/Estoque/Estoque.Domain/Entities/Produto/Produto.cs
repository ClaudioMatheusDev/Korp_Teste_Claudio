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
    }
}
