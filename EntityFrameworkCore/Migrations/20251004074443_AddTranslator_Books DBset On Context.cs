using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslator_BooksDBsetOnContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translator_Book_BookInfo_BookID",
                table: "Translator_Book");

            migrationBuilder.DropForeignKey(
                name: "FK_Translator_Book_Translator_TranslatorID",
                table: "Translator_Book");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Translator_Book",
                table: "Translator_Book");

            migrationBuilder.RenameTable(
                name: "Translator_Book",
                newName: "Translator_Books");

            migrationBuilder.RenameIndex(
                name: "IX_Translator_Book_TranslatorID",
                table: "Translator_Books",
                newName: "IX_Translator_Books_TranslatorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translator_Books",
                table: "Translator_Books",
                columns: new[] { "BookID", "TranslatorID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Translator_Books_BookInfo_BookID",
                table: "Translator_Books",
                column: "BookID",
                principalTable: "BookInfo",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Translator_Books_Translator_TranslatorID",
                table: "Translator_Books",
                column: "TranslatorID",
                principalTable: "Translator",
                principalColumn: "TranslatorID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translator_Books_BookInfo_BookID",
                table: "Translator_Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Translator_Books_Translator_TranslatorID",
                table: "Translator_Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Translator_Books",
                table: "Translator_Books");

            migrationBuilder.RenameTable(
                name: "Translator_Books",
                newName: "Translator_Book");

            migrationBuilder.RenameIndex(
                name: "IX_Translator_Books_TranslatorID",
                table: "Translator_Book",
                newName: "IX_Translator_Book_TranslatorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translator_Book",
                table: "Translator_Book",
                columns: new[] { "BookID", "TranslatorID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Translator_Book_BookInfo_BookID",
                table: "Translator_Book",
                column: "BookID",
                principalTable: "BookInfo",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Translator_Book_Translator_TranslatorID",
                table: "Translator_Book",
                column: "TranslatorID",
                principalTable: "Translator",
                principalColumn: "TranslatorID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
