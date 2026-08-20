using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIDNotaFiscalOrigemToMovimentacaoEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDNotaFiscalOrigem",
                table: "MovimentacaoesEstoque",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDNotaFiscalOrigem",
                table: "MovimentacaoesEstoque");
        }
    }
}
