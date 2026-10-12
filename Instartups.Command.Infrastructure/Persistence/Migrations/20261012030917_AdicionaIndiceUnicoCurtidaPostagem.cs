using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instartups.Command.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaIndiceUnicoCurtidaPostagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_curtidas_postagens_postagem_id",
                table: "curtidas_postagens");

            migrationBuilder.CreateIndex(
                name: "IX_curtidas_postagens_postagem_id_perfil_autor_curtida_id",
                table: "curtidas_postagens",
                columns: new[] { "postagem_id", "perfil_autor_curtida_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_curtidas_postagens_postagem_id_perfil_autor_curtida_id",
                table: "curtidas_postagens");

            migrationBuilder.CreateIndex(
                name: "IX_curtidas_postagens_postagem_id",
                table: "curtidas_postagens",
                column: "postagem_id");
        }
    }
}
