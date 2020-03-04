using Microsoft.EntityFrameworkCore.Migrations;

namespace Trakx.Data.Persistence.Migrations
{
    public partial class AddCompositionAddressAndSymbol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "IndexCompositions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "IndexCompositions",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "IndexCompositions");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "IndexCompositions");
        }
    }
}
