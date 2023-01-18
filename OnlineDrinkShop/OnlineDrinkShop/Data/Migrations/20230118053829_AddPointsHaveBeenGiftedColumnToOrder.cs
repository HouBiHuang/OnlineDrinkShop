using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineDrinkShop.Data.Migrations
{
    public partial class AddPointsHaveBeenGiftedColumnToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PointsHaveBeenGifted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsHaveBeenGifted",
                table: "Orders");
        }
    }
}
