using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WydarzeniaKulturalne.Data.Migrations
{
    /// <inheritdoc />
    public partial class init123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
 




            migrationBuilder.CreateTable(
                name: "ZamowienieSzczegoly",
                columns: table => new
                {
                    IdZamowienieSzczegoly = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdZamowienie = table.Column<int>(type: "int", nullable: false),
                    IdBilet = table.Column<int>(type: "int", nullable: false),
                    Ilosc = table.Column<int>(type: "int", nullable: false),
                    Cena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ZamowienieIdZamowienie = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZamowienieSzczegoly", x => x.IdZamowienieSzczegoly);
                    table.ForeignKey(
                        name: "FK_ZamowienieSzczegoly_Zamowienie_ZamowienieIdZamowienie",
                        column: x => x.ZamowienieIdZamowienie,
                        principalTable: "Zamowienie",
                        principalColumn: "IdZamowienie");
                });

          
            migrationBuilder.CreateIndex(
                name: "IX_ZamowienieSzczegoly_ZamowienieIdZamowienie",
                table: "ZamowienieSzczegoly",
                column: "ZamowienieIdZamowienie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
   

            migrationBuilder.DropTable(
                name: "ZamowienieSzczegoly");

        }
    }
}
