using Estoque.Application.DTOs;
using Estoque.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers.Produto
{
    [ApiController]
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarProduto([FromBody] ProdutoCriarDto produtoCriarDto)
        {
            try
            {
                var idProduto = await _produtoService.CriarProdutoAsync(produtoCriarDto);
                return Ok(new
                {
                    IDProduto = idProduto,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{IDProduto:int}")]
        public async Task<IActionResult> BuscarProdutoPorID(int IDProduto)
        {
            try
            {
                var produto = await _produtoService.BuscarProdutoPorIDAsync(IDProduto);
                return Ok(produto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> BuscarTodosProdutos()
        {
            try
            {
                var produto = await _produtoService.BuscarTodosProdutosAsync();
                return Ok(produto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpDelete("{IDProduto:int}")]
        public async Task<IActionResult> DeletarProduto(int IDProduto)
        {
            try
            {
                var resultado = await _produtoService.ApagarProdutoAsync(IDProduto);
                if (resultado)
                {
                    return Ok(new { Mensagem = "Produto deletado com sucesso." });
                }
                else
                {
                    return NotFound(new { Mensagem = "Produto não encontrado." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{IDProduto:int}")]
        public async Task<IActionResult> AtualizarProduto(int IDProduto, ProdutoAtualizarDto produtoAtualizarDto)
        {
            try
            {
                var resultado = await _produtoService.AtualizarProdutoAsync(IDProduto,produtoAtualizarDto);
                if (resultado)
                {
                    return Ok(new { Mensagem = "Produto atualizado com sucesso." });
                }
                else
                {
                    return NotFound(new { Mensagem = "Produto não encontrado." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
