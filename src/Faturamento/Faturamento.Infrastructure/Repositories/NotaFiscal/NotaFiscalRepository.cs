using Faturamento.Application.Interfaces;
using Faturamento.Domain.Entities;
using Faturamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Infrastructure
{
    public class NotaFiscalRepository : INotaFiscalRepository
    {
        private readonly AppDbContext _context;

        public NotaFiscalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarNotaAsync(NotaFiscal notaFiscal)
        {
            await _context.NotasFiscais.AddAsync(notaFiscal);
        }

        public async Task<NotaFiscal?> BuscarNotaFiscalPorIDAsync(int IDNotaFiscal)
        {
            return await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.IDNotaFiscal == IDNotaFiscal);
        }

        public async Task<List<NotaFiscal>> ListarTodasNotasFiscais()
        {
            return await _context.NotasFiscais.Include(n => n.Itens).OrderByDescending(n => n.DataCriacao).ToListAsync();
        }
        public async Task<int> BuscarProximoNumeroAsync()
        {
            var ultimoNumero = await _context.NotasFiscais
                .MaxAsync(n => (int?)n.Numero);

            return (ultimoNumero ?? 0) + 1;
        }
        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}