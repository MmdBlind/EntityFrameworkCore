using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Category_BookInfo_BookID",
                table: "Book_Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Book_Category_Categories_CategoryID",
                table: "Book_Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Book_Category",
                table: "Book_Category");

            migrationBuilder.RenameTable(
                name: "Book_Category",
                newName: "Book_Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Book_Category_CategoryID",
                table: "Book_Categories",
                newName: "IX_Book_Categories_CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Book_Categories",
                table: "Book_Categories",
                columns: new[] { "BookID", "CategoryID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Categories_BookInfo_BookID",
                table: "Book_Categories",
                column: "BookID",
                principalTable: "BookInfo",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Categories_Categories_CategoryID",
                table: "Book_Categories",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Categories_BookInfo_BookID",
                table: "Book_Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Book_Categories_Categories_CategoryID",
                table: "Book_Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Book_Categories",
                table: "Book_Categories");

            migrationBuilder.RenameTable(
                name: "Book_Categories",
                newName: "Book_Category");

            migrationBuilder.RenameIndex(
                name: "IX_Book_Categories_CategoryID",
                table: "Book_Category",
                newName: "IX_Book_Category_CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Book_Category",
                table: "Book_Category",
                columns: new[] { "BookID", "CategoryID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Category_BookInfo_BookID",
                table: "Book_Category",
                column: "BookID",
                principalTable: "BookInfo",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Category_Categories_CategoryID",
                table: "Book_Category",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
