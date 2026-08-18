namespace Estoque.Application.DTOs
{
    public class MovimentacoesDetalhesDto
    {
        public int IDProduto { get; set; }
        public int SaldoAtual { get; set; }
        public List<MovimentacaoResponseDto> Movimentacoes { get; set; } = [];
    }
}
