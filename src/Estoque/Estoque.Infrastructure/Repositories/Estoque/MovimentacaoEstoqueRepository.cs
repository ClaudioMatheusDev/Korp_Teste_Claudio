using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Repositories
{
    public class MovimentacaoEstoqueRepository : IMovimentacaoEstoqueRepository
    {
        private readonly AppDbContext _context;

        public MovimentacaoEstoqueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(MovimentacaoEstoque movimentacao)
        {
            await _context.MovimentacaoesEstoque.AddAsync(movimentacao);
        }

        public async Task<List<MovimentacaoEstoque>> BuscarProdutoAsync(int IDProduto)
        {
            return await _context.MovimentacaoesEstoque.Where(p => p.IDProduto == IDProduto).OrderByDescending(p => p.DataMovimentacao).ToListAsync();
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
