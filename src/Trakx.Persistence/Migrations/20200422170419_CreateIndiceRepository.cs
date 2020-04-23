using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trakx.Persistence.Migrations
{
    public partial class CreateIndiceRepository : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComponentDefinitions",
                columns: table => new
                {
                    Address = table.Column<string>(maxLength: 256, nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Symbol = table.Column<string>(maxLength: 50, nullable: false),
                    Decimals = table.Column<int>(nullable: false),
                    CoinGeckoId = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentDefinitions", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "IndiceDefinitions",
                columns: table => new
                {
                    Symbol = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Address = table.Column<string>(maxLength: 256, nullable: false),
                    NaturalUnit = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndiceDefinitions", x => x.Symbol);
                });

            migrationBuilder.CreateTable(
                name: "IndiceCompositions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Symbol = table.Column<string>(nullable: false),
                    IndiceDefinitionDaoSymbol = table.Column<string>(nullable: false),
                    Version = table.Column<long>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndiceCompositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndiceCompositions_IndiceDefinitions_IndiceDefinitionDaoSymbol",
                        column: x => x.IndiceDefinitionDaoSymbol,
                        principalTable: "IndiceDefinitions",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentQuantities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ComponentDefinitionDaoAddress = table.Column<string>(nullable: false),
                    IndiceCompositionDaoId = table.Column<string>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(38, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentQuantities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentQuantities_ComponentDefinitions_ComponentDefinitionDaoAddress",
                        column: x => x.ComponentDefinitionDaoAddress,
                        principalTable: "ComponentDefinitions",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentQuantities_IndiceCompositions_IndiceCompositionDaoId",
                        column: x => x.IndiceCompositionDaoId,
                        principalTable: "IndiceCompositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndiceValuations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    QuoteCurrency = table.Column<string>(nullable: false),
                    NetAssetValue = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    IndiceCompositionDaoId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndiceValuations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndiceValuations_IndiceCompositions_IndiceCompositionDaoId",
                        column: x => x.IndiceCompositionDaoId,
                        principalTable: "IndiceCompositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentValuations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10, 10)", nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    ComponentQuantityDaoId = table.Column<string>(nullable: false),
                    QuoteCurrency = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    PriceSource = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    IndiceValuationDaoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentValuations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentValuations_ComponentQuantities_ComponentQuantityDaoId",
                        column: x => x.ComponentQuantityDaoId,
                        principalTable: "ComponentQuantities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentValuations_IndiceValuations_IndiceValuationDaoId",
                        column: x => x.IndiceValuationDaoId,
                        principalTable: "IndiceValuations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentQuantities_ComponentDefinitionDaoAddress",
                table: "ComponentQuantities",
                column: "ComponentDefinitionDaoAddress");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentQuantities_IndiceCompositionDaoId",
                table: "ComponentQuantities",
                column: "IndiceCompositionDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentValuations_ComponentQuantityDaoId",
                table: "ComponentValuations",
                column: "ComponentQuantityDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentValuations_IndiceValuationDaoId",
                table: "ComponentValuations",
                column: "IndiceValuationDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_IndiceCompositions_IndiceDefinitionDaoSymbol",
                table: "IndiceCompositions",
                column: "IndiceDefinitionDaoSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_IndiceValuations_IndiceCompositionDaoId",
                table: "IndiceValuations",
                column: "IndiceCompositionDaoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentValuations");

            migrationBuilder.DropTable(
                name: "ComponentQuantities");

            migrationBuilder.DropTable(
                name: "IndiceValuations");

            migrationBuilder.DropTable(
                name: "ComponentDefinitions");

            migrationBuilder.DropTable(
                name: "IndiceCompositions");

            migrationBuilder.DropTable(
                name: "IndiceDefinitions");
        }
    }
}
