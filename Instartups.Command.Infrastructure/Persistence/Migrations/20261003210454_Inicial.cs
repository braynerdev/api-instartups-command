using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Instartups.Command.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "perfis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    tipo_perfil = table.Column<short>(type: "smallint", nullable: false),
                    imagem_perfil_url = table.Column<string>(type: "varchar(400)", nullable: true),
                    imagem_fundo_url = table.Column<string>(type: "varchar(400)", nullable: true),
                    coordenada = table.Column<NpgsqlPoint>(type: "geography(Point, 4326)", nullable: false),
                    total_curtidas = table.Column<int>(type: "integer", nullable: false),
                    total_seguidores = table.Column<int>(type: "integer", nullable: false),
                    total_seguindo = table.Column<int>(type: "integer", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    data_atualizacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "investidores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tese_investimento = table.Column<string>(type: "varchar(1000)", nullable: false),
                    ticket_minimo = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    ticket_maximo = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    perfil_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    data_atualizacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investidores", x => x.id);
                    table.ForeignKey(
                        name: "FK_investidores_perfis_perfil_id",
                        column: x => x.perfil_id,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "perfil_seguidores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seguidor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seguido_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    data_atualizacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfil_seguidores", x => x.id);
                    table.ForeignKey(
                        name: "FK_perfil_seguidores_perfis_seguido_id",
                        column: x => x.seguido_id,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_perfil_seguidores_perfis_seguidor_id",
                        column: x => x.seguidor_id,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "postagens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "varchar(500)", nullable: false),
                    total_curtidas = table.Column<long>(type: "bigint", nullable: false),
                    autor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    data_atualizacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postagens", x => x.id);
                    table.ForeignKey(
                        name: "FK_postagens_perfis_autor_id",
                        column: x => x.autor_id,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "startups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pitch = table.Column<string>(type: "varchar(1000)", nullable: false),
                    data_fundacao = table.Column<DateOnly>(type: "date", nullable: false),
                    tamanho_equipe = table.Column<short>(type: "smallint", nullable: false),
                    valor_buscado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    perfil_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    data_atualizacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_startups", x => x.id);
                    table.ForeignKey(
                        name: "FK_startups_perfis_perfil_id",
                        column: x => x.perfil_id,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "curtidas_postagens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    postagem_id = table.Column<Guid>(type: "uuid", nullable: false),
                    perfil_autor_curtida_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    data_atualizacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_curtidas_postagens", x => x.id);
                    table.ForeignKey(
                        name: "FK_curtidas_postagens_perfis_perfil_autor_curtida_id",
                        column: x => x.perfil_autor_curtida_id,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_curtidas_postagens_postagens_postagem_id",
                        column: x => x.postagem_id,
                        principalTable: "postagens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "midias_postagens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    url_midia = table.Column<string>(type: "varchar(400)", nullable: false),
                    tipo_midia = table.Column<int>(type: "int", nullable: false),
                    postagem_id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostagemEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    data_atualizacao = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_midias_postagens", x => x.id);
                    table.ForeignKey(
                        name: "FK_midias_postagens_postagens_PostagemEntityId",
                        column: x => x.PostagemEntityId,
                        principalTable: "postagens",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_midias_postagens_postagens_postagem_id",
                        column: x => x.postagem_id,
                        principalTable: "postagens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_curtidas_postagens_data_atualizacao",
                table: "curtidas_postagens",
                column: "data_atualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_curtidas_postagens_data_criacao",
                table: "curtidas_postagens",
                column: "data_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_curtidas_postagens_perfil_autor_curtida_id",
                table: "curtidas_postagens",
                column: "perfil_autor_curtida_id");

            migrationBuilder.CreateIndex(
                name: "IX_curtidas_postagens_postagem_id",
                table: "curtidas_postagens",
                column: "postagem_id");

            migrationBuilder.CreateIndex(
                name: "IX_investidores_data_atualizacao",
                table: "investidores",
                column: "data_atualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_investidores_data_criacao",
                table: "investidores",
                column: "data_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_investidores_perfil_id",
                table: "investidores",
                column: "perfil_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_midias_postagens_data_atualizacao",
                table: "midias_postagens",
                column: "data_atualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_midias_postagens_data_criacao",
                table: "midias_postagens",
                column: "data_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_midias_postagens_postagem_id",
                table: "midias_postagens",
                column: "postagem_id");

            migrationBuilder.CreateIndex(
                name: "IX_midias_postagens_PostagemEntityId",
                table: "midias_postagens",
                column: "PostagemEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_perfil_seguidores_data_atualizacao",
                table: "perfil_seguidores",
                column: "data_atualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_perfil_seguidores_data_criacao",
                table: "perfil_seguidores",
                column: "data_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_perfil_seguidores_seguido_id",
                table: "perfil_seguidores",
                column: "seguido_id");

            migrationBuilder.CreateIndex(
                name: "IX_perfil_seguidores_seguidor_id",
                table: "perfil_seguidores",
                column: "seguidor_id");

            migrationBuilder.CreateIndex(
                name: "IX_perfis_data_atualizacao",
                table: "perfis",
                column: "data_atualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_perfis_data_criacao",
                table: "perfis",
                column: "data_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_postagens_autor_id",
                table: "postagens",
                column: "autor_id");

            migrationBuilder.CreateIndex(
                name: "IX_postagens_data_atualizacao",
                table: "postagens",
                column: "data_atualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_postagens_data_criacao",
                table: "postagens",
                column: "data_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_startups_data_atualizacao",
                table: "startups",
                column: "data_atualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_startups_data_criacao",
                table: "startups",
                column: "data_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_startups_perfil_id",
                table: "startups",
                column: "perfil_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "curtidas_postagens");

            migrationBuilder.DropTable(
                name: "investidores");

            migrationBuilder.DropTable(
                name: "midias_postagens");

            migrationBuilder.DropTable(
                name: "perfil_seguidores");

            migrationBuilder.DropTable(
                name: "startups");

            migrationBuilder.DropTable(
                name: "postagens");

            migrationBuilder.DropTable(
                name: "perfis");
        }
    }
}
