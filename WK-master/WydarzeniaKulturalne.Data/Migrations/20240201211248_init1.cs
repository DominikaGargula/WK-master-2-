using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WydarzeniaKulturalne.Data.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElementKoszyka_Bilety_BiletyId",
                table: "ElementKoszyka");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Zamowienie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BiletyId",
                table: "ElementKoszyka",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ElementKoszyka_Bilety_BiletyId",
                table: "ElementKoszyka",
                column: "BiletyId",
                principalTable: "Bilety",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElementKoszyka_Bilety_BiletyId",
                table: "ElementKoszyka");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Zamowienie",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "BiletyId",
                table: "ElementKoszyka",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ElementKoszyka_Bilety_BiletyId",
                table: "ElementKoszyka",
                column: "BiletyId",
                principalTable: "Bilety",
                principalColumn: "Id");
        }
    }
}
