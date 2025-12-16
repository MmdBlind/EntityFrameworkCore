using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Relationship_AspUser_Customer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "Fk_Customers_AspNetUser_CustomerID",
                table: "Customers",
                column: "CustomerID",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Fk_Customers_AspNetUser_CustomerID",
                table: "Customers"
            );

        }
    }
}
