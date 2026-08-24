using Faturamento.Application.Dtos;
using Faturamento.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Faturamento.API.Controllers.Faturamento
{
    [ApiController]
    [Route("api/notas")]
    public class NotaFiscalController : ControllerBase
    {
        private readonly INotaFiscalService _notaService;

        public NotaFiscalController(INotaFiscalService notaService)
        {
            _notaService = notaService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarNota(
            [FromBody] NotaFiscalCriarDto notaFiscalCriarDto)
        {
            var idNota =
                await _notaService.CriarNotaFiscalAsync(notaFiscalCriarDto);

            return Ok(new
            {
                IDNotaFiscal = idNota,
                Message = "Nota fiscal criada com sucesso."
            });
        }

        [HttpGet]
        public async Task<IActionResult> ListarNotas()
        {
            var notas = await _notaService.ListarNotasFiscaisAsync();

            return Ok(notas);
        }

        [HttpGet("{IDNotaFiscal:int}")]
        public async Task<IActionResult> BuscarNota(int IDNotaFiscal)
        {
            var nota =
                await _notaService.BuscarNotaFiscalAsync(IDNotaFiscal);

            return Ok(nota);
        }

        [HttpPost("{IDNotaFiscal:int}/imprimir")]
        public async Task<IActionResult> ImprimirNota(int IDNotaFiscal)
        {
            var impressaAgora = await _notaService.ImprimirNotaFiscal(IDNotaFiscal);

            return Ok(new
            {
                Message = impressaAgora
                    ? "Nota fiscal impressa e fechada com sucesso."
                    : "Nota fiscal já estava fechada. Nenhuma impressão foi realizada."
            });
        }
    }
}