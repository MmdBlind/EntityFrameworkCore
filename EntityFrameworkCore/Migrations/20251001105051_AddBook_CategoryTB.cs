using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddBook_CategoryTB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookInfo_Categories_CategoryID",
                table: "BookInfo");

            migrationBuilder.DropIndex(
                name: "IX_BookInfo_CategoryID",
                table: "BookInfo");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "BookInfo",
                newName: "PublishYear");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "BookInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublish",
                table: "BookInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "BookInfo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Book_Category",
                columns: table => new
                {
                    BookID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book_Category", x => new { x.BookID, x.CategoryID });
                    table.ForeignKey(
                        name: "FK_Book_Category_BookInfo_BookID",
                        column: x => x.BookID,
                        principalTable: "BookInfo",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Book_Category_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Book_Category_CategoryID",
                table: "Book_Category",
                column: "CategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Book_Category");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "BookInfo");

            migrationBuilder.DropColumn(
                name: "IsPublish",
                table: "BookInfo");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "BookInfo");

            migrationBuilder.RenameColumn(
                name: "PublishYear",
                table: "BookInfo",
                newName: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_BookInfo_CategoryID",
                table: "BookInfo",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_BookInfo_Categories_CategoryID",
                table: "BookInfo",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
