using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfMVVMEfApp.Migrations
{
    public partial class BookFilesDescriptionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookFilesDescription_BookFiles_FileId",
                table: "BookFilesDescription");

            migrationBuilder.DropIndex(
                name: "IX_BookFilesDescription_FileId",
                table: "BookFilesDescription");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "BookFilesDescription");

            migrationBuilder.AddColumn<int>(
                name: "BookFileDescriptionId",
                table: "BookFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookFiles_BookFileDescriptionId",
                table: "BookFiles",
                column: "BookFileDescriptionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookFiles_BookFilesDescription_BookFileDescriptionId",
                table: "BookFiles",
                column: "BookFileDescriptionId",
                principalTable: "BookFilesDescription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookFiles_BookFilesDescription_BookFileDescriptionId",
                table: "BookFiles");

            migrationBuilder.DropIndex(
                name: "IX_BookFiles_BookFileDescriptionId",
                table: "BookFiles");

            migrationBuilder.DropColumn(
                name: "BookFileDescriptionId",
                table: "BookFiles");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "BookFilesDescription",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookFilesDescription_FileId",
                table: "BookFilesDescription",
                column: "FileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookFilesDescription_BookFiles_FileId",
                table: "BookFilesDescription",
                column: "FileId",
                principalTable: "BookFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
