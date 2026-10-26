using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZapatillasProject.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "Marca",
                columns: table => new
                {
                    id_marca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    pais = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marca", x => x.id_marca);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "Zapatilla",
                columns: table => new
                {
                    id_zapatilla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    modelo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    precio = table.Column<int>(type: "int", nullable: false),
                    foto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    id_marca = table.Column<int>(type: "int", nullable: false),
                    descripcion = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zapatilla", x => x.id_zapatilla);
                    table.ForeignKey(
                        name: "FK_Zapatilla_Categoria",
                        column: x => x.id_categoria,
                        principalTable: "Categoria",
                        principalColumn: "id_categoria");
                    table.ForeignKey(
                        name: "FK_Zapatilla_Marca",
                        column: x => x.id_marca,
                        principalTable: "Marca",
                        principalColumn: "id_marca");
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    password = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    id_rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id_user);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol",
                        column: x => x.id_rol,
                        principalTable: "Rol",
                        principalColumn: "id_rol");
                });

            migrationBuilder.CreateTable(
                name: "StockTalla",
                columns: table => new
                {
                    id_stock = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_zapatilla = table.Column<int>(type: "int", nullable: false),
                    talla = table.Column<decimal>(type: "numeric(4,1)", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTalla", x => x.id_stock);
                    table.ForeignKey(
                        name: "FK_StockTalla_Zapatilla",
                        column: x => x.id_zapatilla,
                        principalTable: "Zapatilla",
                        principalColumn: "id_zapatilla");
                });

            migrationBuilder.CreateTable(
                name: "Resena",
                columns: table => new
                {
                    id_resena = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    resena = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    valoracion = table.Column<int>(type: "int", nullable: false),
                    id_zapatilla = table.Column<int>(type: "int", nullable: false),
                    id_user = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resena", x => x.id_resena);
                    table.ForeignKey(
                        name: "FK_Resena_Usuario",
                        column: x => x.id_user,
                        principalTable: "Usuario",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "FK_Resena_Zapatilla",
                        column: x => x.id_zapatilla,
                        principalTable: "Zapatilla",
                        principalColumn: "id_zapatilla");
                });

            migrationBuilder.CreateTable(
                name: "Venta",
                columns: table => new
                {
                    id_venta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numero_venta = table.Column<int>(type: "int", nullable: false),
                    tipo_pago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    monto = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    id_user = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venta", x => x.id_venta);
                    table.ForeignKey(
                        name: "FK_Venta_Usuario",
                        column: x => x.id_user,
                        principalTable: "Usuario",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "DetalleVenta",
                columns: table => new
                {
                    id_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_zapatilla = table.Column<int>(type: "int", nullable: false),
                    precio = table.Column<int>(type: "int", nullable: false),
                    id_venta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleVenta", x => x.id_detalle);
                    table.ForeignKey(
                        name: "FK_DetalleVenta_Venta",
                        column: x => x.id_venta,
                        principalTable: "Venta",
                        principalColumn: "id_venta");
                    table.ForeignKey(
                        name: "FK_DetalleVenta_Zapatilla",
                        column: x => x.id_zapatilla,
                        principalTable: "Zapatilla",
                        principalColumn: "id_zapatilla");
                });

            migrationBuilder.InsertData(
                table: "Categoria",
                columns: new[] { "id_categoria", "nombre" },
                values: new object[,]
                {
                    { 1, "Running" },
                    { 2, "Urbana" },
                    { 3, "Basketball" },
                    { 4, "Skate" }
                });

            migrationBuilder.InsertData(
                table: "Marca",
                columns: new[] { "id_marca", "nombre", "pais" },
                values: new object[,]
                {
                    { 1, "Nike", "Estados Unidos" },
                    { 2, "Adidas", "Alemania" },
                    { 3, "Puma", "Alemania" },
                    { 4, "Vans", "Estados Unidos" }
                });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "id_rol", "nombre" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Cliente" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVenta_id_venta",
                table: "DetalleVenta",
                column: "id_venta");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVenta_id_zapatilla",
                table: "DetalleVenta",
                column: "id_zapatilla");

            migrationBuilder.CreateIndex(
                name: "IX_Resena_id_user",
                table: "Resena",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_Resena_id_zapatilla",
                table: "Resena",
                column: "id_zapatilla");

            migrationBuilder.CreateIndex(
                name: "IX_StockTalla_id_zapatilla",
                table: "StockTalla",
                column: "id_zapatilla");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_id_rol",
                table: "Usuario",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "UQ_Usuario_Email",
                table: "Usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venta_id_user",
                table: "Venta",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_Zapatilla_id_categoria",
                table: "Zapatilla",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Zapatilla_id_marca",
                table: "Zapatilla",
                column: "id_marca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleVenta");

            migrationBuilder.DropTable(
                name: "Resena");

            migrationBuilder.DropTable(
                name: "StockTalla");

            migrationBuilder.DropTable(
                name: "Venta");

            migrationBuilder.DropTable(
                name: "Zapatilla");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Marca");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
