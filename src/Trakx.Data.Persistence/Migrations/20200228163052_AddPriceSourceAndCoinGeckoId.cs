using Microsoft.EntityFrameworkCore.Migrations;

namespace Trakx.Data.Persistence.Migrations
{
    public partial class AddPriceSourceAndCoinGeckoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PriceSource",
                table: "ComponentValuations",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoinGeckoId",
                table: "ComponentDefinitions",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceSource",
                table: "ComponentValuations");

            migrationBuilder.DropColumn(
                name: "CoinGeckoId",
                table: "ComponentDefinitions");
        }
    }
}
