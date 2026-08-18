namespace Estoque.Application.DTOs
{
    public class ProdutoCriarDto
    {
        public int Codigo { get; set; }
        public required string Descricao { get; set; }
        public decimal ValorProduto { get; set; }
        public int QuantidadeEstoque { get; set; }
    }
}
