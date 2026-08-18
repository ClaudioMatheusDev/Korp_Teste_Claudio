using System.ComponentModel.DataAnnotations;

namespace Estoque.Application.DTOs
{
    public class EntradaEstoqueDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "O ID do produto deve ser maior que zero.")]
        public int IDProduto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [StringLength(200, ErrorMessage = "O motivo deve ter no máximo 200 caracteres.")]
        public string? Motivo { get; set; }
    }
}
