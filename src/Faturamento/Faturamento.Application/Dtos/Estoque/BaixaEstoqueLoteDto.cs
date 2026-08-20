namespace Faturamento.Application.Dtos
{
    public class BaixaEstoqueLoteDto
    {
        public int IDNotaFiscal { get; set; }
        public List<ItemBaixaEstoqueDto> Itens { get; set; } = [];
    }

}
