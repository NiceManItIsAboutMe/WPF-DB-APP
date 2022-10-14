using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfMVVMEfApp.Migrations
{
    public partial class UniqueCategoryAndAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_categories_Name",
                table: "categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Surname_Name_Patronymic",
                table: "Authors",
                columns: new[] { "Surname", "Name", "Patronymic" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_categories_Name",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_Authors_Surname_Name_Patronymic",
                table: "Authors");
        }
    }
}
