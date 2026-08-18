using Estoque.Application.DTOs;
using Estoque.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers.Estoque
{
    [ApiController]
    [Route("api/estoque")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _estoqueService;

        public EstoqueController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> CriarEntrada(
            [FromBody] EntradaEstoqueDto entradaEstoqueDto)
        {
            try
            {
                var idProduto =
                    await _estoqueService.EntradaAsync(entradaEstoqueDto);

                return Ok(new
                {
                    IDProduto = idProduto,
                    Message = "Entrada de estoque realizada com sucesso."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost("saida")]
        public async Task<IActionResult> CriarSaida(
            [FromBody] SaidaEstoqueDto saidaEstoqueDto)
        {
            try
            {
                var idProduto =
                    await _estoqueService.SaidaAsync(saidaEstoqueDto);

                return Ok(new
                {
                    IDProduto = idProduto,
                    Message = "Saída de estoque realizada com sucesso."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("produtos/{IDProduto:int}/movimentacoes")]
        public async Task<IActionResult> Movimentacoes(int IDProduto)
        {
            try
            {
                var movimentacoes =
                    await _estoqueService.MovimentacoesDetalhesAsync(IDProduto);

                return Ok(movimentacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }
    }
}