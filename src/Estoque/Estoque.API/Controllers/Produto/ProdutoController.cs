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
            var idProduto = await _produtoService.CriarProdutoAsync(produtoCriarDto);
            return Ok(new
            {
                IDProduto = idProduto,
            });
        }

        [HttpGet("{IDProduto:int}")]
        public async Task<IActionResult> BuscarProdutoPorID(int IDProduto)
        {
            var produto = await _produtoService.BuscarProdutoPorIDAsync(IDProduto);
            return Ok(produto);
        }


        [HttpGet]
        public async Task<IActionResult> BuscarTodosProdutos()
        {
            var produto = await _produtoService.BuscarTodosProdutosAsync();
            return Ok(produto);
        }

        [HttpDelete("{IDProduto:int}")]
        public async Task<IActionResult> DeletarProduto(int IDProduto)
        {
            await _produtoService.ApagarProdutoAsync(IDProduto);
            return Ok(new { Mensagem = "Produto deletado com sucesso." });
        }

        [HttpPut("{IDProduto:int}")]
        public async Task<IActionResult> AtualizarProduto(int IDProduto, ProdutoAtualizarDto produtoAtualizarDto)
        {
            await _produtoService.AtualizarProdutoAsync(IDProduto, produtoAtualizarDto);
            return Ok(new { Mensagem = "Produto atualizado com sucesso." });
        }

    }
}
