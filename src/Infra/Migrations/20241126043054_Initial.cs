using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Pedidos",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroPedido = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 2, nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PedidosItens",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosItens_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalSchema: "dbo",
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidosItens_PedidoId",
                schema: "dbo",
                table: "PedidosItens",
                column: "PedidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidosItens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Pedidos",
                schema: "dbo");
        }
    }
}
