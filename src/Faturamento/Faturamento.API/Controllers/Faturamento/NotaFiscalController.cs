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
            try
            {
                var idNota =
                    await _notaService.CriarNotaFiscalAsync(notaFiscalCriarDto);

                return Ok(new
                {
                    IDNotaFiscal = idNota,
                    Message = "Nota fiscal criada com sucesso."
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

        [HttpGet]
        public async Task<IActionResult> ListarNotas()
        {
            try
            {
                var notas = await _notaService.ListarNotasFiscaisAsync();

                return Ok(notas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{IDNotaFiscal:int}")]
        public async Task<IActionResult> BuscarNota(int IDNotaFiscal)
        {
            try
            {
                var nota =
                    await _notaService.BuscarNotaFiscalAsync(IDNotaFiscal);

                return Ok(nota);
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