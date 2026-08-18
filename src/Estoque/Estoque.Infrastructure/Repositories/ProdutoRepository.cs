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
            await _context.produtos.AddAsync(produto);
        }

        public async Task<Produto?> BuscarProdutoPorIDAsync(int IDProduto)
        {
            return await _context.produtos.FirstOrDefaultAsync(p => p.IDProduto == IDProduto);
        }

        public async Task<List<Produto>> BuscarTodosProdutosAsync()
        {
            return await _context.produtos.ToListAsync();
        }

        public void Atualizar(Produto produto)
        {
            _context.produtos.Update(produto);
        }

        public void Deletar(Produto produto)
        {
            _context.Remove(produto);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
