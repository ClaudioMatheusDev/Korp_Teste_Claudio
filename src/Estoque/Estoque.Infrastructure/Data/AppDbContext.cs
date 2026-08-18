using Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {

      public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        } 


        public DbSet<Produto> produtos { get; set; }
        //public DbSet<Estoque> estoques { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasKey(produto => produto.IDProduto);
            modelBuilder.Entity<Produto>().Property(produto => produto.ValorProduto).HasPrecision(18, 2);
        }
        }
}
