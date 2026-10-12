using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instartups.Command.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlterandoOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_midias_postagens_postagens_postagem_id",
                table: "midias_postagens");

            migrationBuilder.AddForeignKey(
                name: "FK_midias_postagens_postagens_postagem_id",
                table: "midias_postagens",
                column: "postagem_id",
                principalTable: "postagens",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_midias_postagens_postagens_postagem_id",
                table: "midias_postagens");

            migrationBuilder.AddForeignKey(
                name: "FK_midias_postagens_postagens_postagem_id",
                table: "midias_postagens",
                column: "postagem_id",
                principalTable: "postagens",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
