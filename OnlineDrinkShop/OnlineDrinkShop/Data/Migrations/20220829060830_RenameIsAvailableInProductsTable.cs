using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineDrinkShop.Data.Migrations
{
    public partial class RenameIsAvailableInProductsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "lIsAvailable",
                table: "Products",
                newName: "IsAvailable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "Products",
                newName: "lIsAvailable");
        }
    }
}
