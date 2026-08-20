namespace Estoque.Application.DTOs
{
    public class BaixaEstoqueLoteDto
    {
        public int IDNotaFiscal { get; set; }
        public List<ItemBaixaEstoqueDto> Itens { get; set; } = [];
    }
}
