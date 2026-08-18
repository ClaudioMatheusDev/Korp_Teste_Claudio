using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<NotaFiscal> NotasFiscais { get; set; }
        public DbSet<ItemNotaFiscal> ItensNotaFiscal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NotaFiscal>()
                .HasKey(n => n.IDNotaFiscal);

            modelBuilder.Entity<NotaFiscal>()
                .HasIndex(n => n.Numero)
                .IsUnique();

            modelBuilder.Entity<ItemNotaFiscal>()
                .HasKey(i => i.IDItemNotaFiscal);

            modelBuilder.Entity<ItemNotaFiscal>()
                .HasOne(i => i.NotaFiscal)
                .WithMany(n => n.Itens)
                .HasForeignKey(i => i.IDNotaFiscal)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
