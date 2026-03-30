using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModuloMVC.Migrations
{
    /// <inheritdoc />
    public partial class CreateadTarefa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tarefa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Vencimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefa", x => x.Id);
                    table.CheckConstraint("CK_Tarefa_TituloOuDescricao_Requerido", "([Titulo] IS NOT NULL AND [Titulo] <> '') OR ([Descricao] IS NOT NULL AND [Descricao] <> '')");
                });

            migrationBuilder.CreateTable(
                name: "ContatoTarefa",
                columns: table => new
                {
                    ContatosEnvolvidosId = table.Column<int>(type: "int", nullable: false),
                    TarefasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContatoTarefa", x => new { x.ContatosEnvolvidosId, x.TarefasId });
                    table.ForeignKey(
                        name: "FK_ContatoTarefa_Contato_ContatosEnvolvidosId",
                        column: x => x.ContatosEnvolvidosId,
                        principalTable: "Contato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContatoTarefa_Tarefa_TarefasId",
                        column: x => x.TarefasId,
                        principalTable: "Tarefa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContatoTarefa_TarefasId",
                table: "ContatoTarefa",
                column: "TarefasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContatoTarefa");

            migrationBuilder.DropTable(
                name: "Tarefa");
        }
    }
}
