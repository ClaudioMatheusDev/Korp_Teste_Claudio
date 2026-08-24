using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure
{
    public class ProdutoRepository : IProdutoRepository
    {

        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarProdutoAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
        }

        public async Task<Produto?> BuscarProdutoPorIDAsync(int IDProduto)
        {
            return await _context.Produtos.FirstOrDefaultAsync(p => p.IDProduto == IDProduto);
        }

        public async Task<List<Produto>> BuscarTodosProdutosAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public void Atualizar(Produto produto)
        {
            _context.Produtos.Update(produto);
        }

        public void Deletar(Produto produto)
        {
            _context.Remove(produto);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteProdutoComCodigoAsync(int codigo, int? idProdutoIgnorar = null)
        {
            return await _context.Produtos.AnyAsync(p => p.Codigo == codigo && p.IDProduto != idProdutoIgnorar);
        }

        public async Task RecarregarAsync(Produto produto)
        {
            await _context.Entry(produto).ReloadAsync();
        }

    }
}
