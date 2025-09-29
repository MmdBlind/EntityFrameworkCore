using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Add_Translator_TB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Translator",
                columns: table => new
                {
                    TranslatorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translator", x => x.TranslatorID);
                });

            migrationBuilder.CreateTable(
                name: "Translator_Book",
                columns: table => new
                {
                    BookID = table.Column<int>(type: "int", nullable: false),
                    TranslatorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translator_Book", x => new { x.BookID, x.TranslatorID });
                    table.ForeignKey(
                        name: "FK_Translator_Book_BookInfo_BookID",
                        column: x => x.BookID,
                        principalTable: "BookInfo",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Translator_Book_Translator_TranslatorID",
                        column: x => x.TranslatorID,
                        principalTable: "Translator",
                        principalColumn: "TranslatorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translator_Book_TranslatorID",
                table: "Translator_Book",
                column: "TranslatorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translator_Book");

            migrationBuilder.DropTable(
                name: "Translator");
        }
    }
}
