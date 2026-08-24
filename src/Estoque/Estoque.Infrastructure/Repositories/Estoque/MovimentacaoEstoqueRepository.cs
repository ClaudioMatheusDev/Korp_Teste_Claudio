using Estoque.Application.Exceptions;
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

        public async Task<bool> ExisteBaixaParaNotaFiscalAsync(int idNotaFiscal)
        {
            return await _context.MovimentacaoesEstoque
                .AnyAsync(m => m.IDNotaFiscalOrigem == idNotaFiscal);
        }

        public async Task SalvarAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyConflictException(
                    "Os dados foram alterados por outra operação simultânea.", ex);
            }
        }
    }
}
