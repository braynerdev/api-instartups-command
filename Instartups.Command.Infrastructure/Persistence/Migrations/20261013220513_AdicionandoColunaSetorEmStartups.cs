using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instartups.Command.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoColunaSetorEmStartups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "setor",
                table: "startups",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "setor",
                table: "startups");
        }
    }
}
