using Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<Produto> Produtos { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacaoesEstoque { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>()
                .HasKey(p => p.IDProduto);

            modelBuilder.Entity<Produto>()
                .Property(p => p.ValorProduto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasKey(m => m.IDMovimentacaoEstoque);

            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasOne(m => m.Produto)
                .WithMany(p => p.MovimentacoesEstoque)
                .HasForeignKey(m => m.IDProduto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
