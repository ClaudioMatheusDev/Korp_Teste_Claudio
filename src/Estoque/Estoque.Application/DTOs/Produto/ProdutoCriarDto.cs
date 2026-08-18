using System.ComponentModel.DataAnnotations;

namespace Estoque.Application.DTOs
{
    public class ProdutoCriarDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "O código deve ser maior que zero.")]
        public int Codigo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        public required string Descricao { get; set; }

        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor do produto deve ser maior que zero.")]
        public decimal ValorProduto { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa.")]
        public int QuantidadeEstoque { get; set; }
    }
}
