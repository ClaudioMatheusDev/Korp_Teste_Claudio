namespace Estoque.Application.DTOs
{
    public class EntradaEstoqueDto
    {
        public int IDProduto { get; set; }
        public int Quantidade { get; set; }
        public string? Motivo { get; set; }
    }
}
