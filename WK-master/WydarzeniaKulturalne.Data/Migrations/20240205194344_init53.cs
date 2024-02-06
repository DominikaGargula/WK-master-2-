using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WydarzeniaKulturalne.Data.Migrations
{
    /// <inheritdoc />
    public partial class init53 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KategoriaWydarzenia",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KategoriaWydarzenia", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "LokalizacjaWydarzenia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Miejscowosc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KodPocztowy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ulica = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumerDomu = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    NazwaMiejsca = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LokalizacjaWydarzenia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rola",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aktywna = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rola", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecjalnyTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecjalnyTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uzytkownik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Haslo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RolaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzytkownik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Uzytkownik_Rola_RolaId",
                        column: x => x.RolaId,
                        principalTable: "Rola",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WydarzenieKulturalne",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZdjecieUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataUwtorzenia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KategoriaWydarzeniaId = table.Column<int>(type: "int", nullable: false),
                    Promowane = table.Column<bool>(type: "bit", nullable: false),
                    SpecjalnyTagId = table.Column<int>(type: "int", nullable: true),
                    LokalizacjaWydarzeniaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WydarzenieKulturalne", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WydarzenieKulturalne_KategoriaWydarzenia_KategoriaWydarzeniaId",
                        column: x => x.KategoriaWydarzeniaId,
                        principalTable: "KategoriaWydarzenia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WydarzenieKulturalne_LokalizacjaWydarzenia_LokalizacjaWydarzeniaId",
                        column: x => x.LokalizacjaWydarzeniaId,
                        principalTable: "LokalizacjaWydarzenia",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WydarzenieKulturalne_SpecjalnyTag_SpecjalnyTagId",
                        column: x => x.SpecjalnyTagId,
                        principalTable: "SpecjalnyTag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Zamowienie",
                columns: table => new
                {
                    IdZamowienie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    Suma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zamowienie", x => x.IdZamowienie);
                    table.ForeignKey(
                        name: "FK_Zamowienie_Uzytkownik_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bilety",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WydarzenieKulturalneId = table.Column<int>(type: "int", nullable: false),
                    LokalizacjaWydarzeniaId = table.Column<int>(type: "int", nullable: false),
                    IloscBiletow = table.Column<int>(type: "int", nullable: false),
                    DataWydarzenia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CzyDostepne = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bilety", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bilety_LokalizacjaWydarzenia_LokalizacjaWydarzeniaId",
                        column: x => x.LokalizacjaWydarzeniaId,
                        principalTable: "LokalizacjaWydarzenia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bilety_WydarzenieKulturalne_WydarzenieKulturalneId",
                        column: x => x.WydarzenieKulturalneId,
                        principalTable: "WydarzenieKulturalne",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElementKoszyka",
                columns: table => new
                {
                    IdElementuKoszyka = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSesjiKoszyka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdBilet = table.Column<int>(type: "int", nullable: false),
                    BiletyId = table.Column<int>(type: "int", nullable: false),
                    Ilosc = table.Column<int>(type: "int", nullable: false),
                    DataUtworzenia = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementKoszyka", x => x.IdElementuKoszyka);
                    table.ForeignKey(
                        name: "FK_ElementKoszyka_Bilety_BiletyId",
                        column: x => x.BiletyId,
                        principalTable: "Bilety",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    BiletId = table.Column<int>(type: "int", nullable: true),
                    ZamowienieIdZamowienie = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZamowienieSzczegoly", x => x.IdZamowienieSzczegoly);
                    table.ForeignKey(
                        name: "FK_ZamowienieSzczegoly_Bilety_BiletId",
                        column: x => x.BiletId,
                        principalTable: "Bilety",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ZamowienieSzczegoly_Zamowienie_ZamowienieIdZamowienie",
                        column: x => x.ZamowienieIdZamowienie,
                        principalTable: "Zamowienie",
                        principalColumn: "IdZamowienie");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bilety_LokalizacjaWydarzeniaId",
                table: "Bilety",
                column: "LokalizacjaWydarzeniaId");

            migrationBuilder.CreateIndex(
                name: "IX_Bilety_WydarzenieKulturalneId",
                table: "Bilety",
                column: "WydarzenieKulturalneId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementKoszyka_BiletyId",
                table: "ElementKoszyka",
                column: "BiletyId");

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownik_RolaId",
                table: "Uzytkownik",
                column: "RolaId");

            migrationBuilder.CreateIndex(
                name: "IX_WydarzenieKulturalne_KategoriaWydarzeniaId",
                table: "WydarzenieKulturalne",
                column: "KategoriaWydarzeniaId");

            migrationBuilder.CreateIndex(
                name: "IX_WydarzenieKulturalne_LokalizacjaWydarzeniaId",
                table: "WydarzenieKulturalne",
                column: "LokalizacjaWydarzeniaId");

            migrationBuilder.CreateIndex(
                name: "IX_WydarzenieKulturalne_SpecjalnyTagId",
                table: "WydarzenieKulturalne",
                column: "SpecjalnyTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Zamowienie_UzytkownikId",
                table: "Zamowienie",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_ZamowienieSzczegoly_BiletId",
                table: "ZamowienieSzczegoly",
                column: "BiletId");

            migrationBuilder.CreateIndex(
                name: "IX_ZamowienieSzczegoly_ZamowienieIdZamowienie",
                table: "ZamowienieSzczegoly",
                column: "ZamowienieIdZamowienie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElementKoszyka");

            migrationBuilder.DropTable(
                name: "ZamowienieSzczegoly");

            migrationBuilder.DropTable(
                name: "Bilety");

            migrationBuilder.DropTable(
                name: "Zamowienie");

            migrationBuilder.DropTable(
                name: "WydarzenieKulturalne");

            migrationBuilder.DropTable(
                name: "Uzytkownik");

            migrationBuilder.DropTable(
                name: "KategoriaWydarzenia");

            migrationBuilder.DropTable(
                name: "LokalizacjaWydarzenia");

            migrationBuilder.DropTable(
                name: "SpecjalnyTag");

            migrationBuilder.DropTable(
                name: "Rola");
        }
    }
}
