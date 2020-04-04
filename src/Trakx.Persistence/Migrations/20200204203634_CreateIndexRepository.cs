using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trakx.Persistence.Migrations
{
    public partial class CreateIndexRepository : Migration
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
                    Decimals = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentDefinitions", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "IndexDefinitions",
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
                    table.PrimaryKey("PK_IndexDefinitions", x => x.Symbol);
                });

            migrationBuilder.CreateTable(
                name: "ComponentWeights",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ComponentDefinitionDaoAddress = table.Column<string>(nullable: false),
                    IndexDefinitionDaoSymbol = table.Column<string>(nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10, 10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentWeights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentWeights_ComponentDefinitions_ComponentDefinitionDaoAddress",
                        column: x => x.ComponentDefinitionDaoAddress,
                        principalTable: "ComponentDefinitions",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentWeights_IndexDefinitions_IndexDefinitionDaoSymbol",
                        column: x => x.IndexDefinitionDaoSymbol,
                        principalTable: "IndexDefinitions",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexCompositions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IndexDefinitionDaoSymbol = table.Column<string>(nullable: false),
                    Version = table.Column<long>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexCompositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndexCompositions_IndexDefinitions_IndexDefinitionDaoSymbol",
                        column: x => x.IndexDefinitionDaoSymbol,
                        principalTable: "IndexDefinitions",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentQuantities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ComponentDefinitionDaoAddress = table.Column<string>(nullable: false),
                    IndexCompositionDaoId = table.Column<string>(nullable: false),
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
                        name: "FK_ComponentQuantities_IndexCompositions_IndexCompositionDaoId",
                        column: x => x.IndexCompositionDaoId,
                        principalTable: "IndexCompositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexValuations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    QuoteCurrency = table.Column<string>(nullable: false),
                    NetAssetValue = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    IndexCompositionDaoId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexValuations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndexValuations_IndexCompositions_IndexCompositionDaoId",
                        column: x => x.IndexCompositionDaoId,
                        principalTable: "IndexCompositions",
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
                    Value = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    IndexValuationDaoId = table.Column<string>(nullable: true)
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
                        name: "FK_ComponentValuations_IndexValuations_IndexValuationDaoId",
                        column: x => x.IndexValuationDaoId,
                        principalTable: "IndexValuations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentQuantities_ComponentDefinitionDaoAddress",
                table: "ComponentQuantities",
                column: "ComponentDefinitionDaoAddress");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentQuantities_IndexCompositionDaoId",
                table: "ComponentQuantities",
                column: "IndexCompositionDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentValuations_ComponentQuantityDaoId",
                table: "ComponentValuations",
                column: "ComponentQuantityDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentValuations_IndexValuationDaoId",
                table: "ComponentValuations",
                column: "IndexValuationDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentWeights_ComponentDefinitionDaoAddress",
                table: "ComponentWeights",
                column: "ComponentDefinitionDaoAddress");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentWeights_IndexDefinitionDaoSymbol",
                table: "ComponentWeights",
                column: "IndexDefinitionDaoSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_IndexCompositions_IndexDefinitionDaoSymbol",
                table: "IndexCompositions",
                column: "IndexDefinitionDaoSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_IndexValuations_IndexCompositionDaoId",
                table: "IndexValuations",
                column: "IndexCompositionDaoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentValuations");

            migrationBuilder.DropTable(
                name: "ComponentWeights");

            migrationBuilder.DropTable(
                name: "ComponentQuantities");

            migrationBuilder.DropTable(
                name: "IndexValuations");

            migrationBuilder.DropTable(
                name: "ComponentDefinitions");

            migrationBuilder.DropTable(
                name: "IndexCompositions");

            migrationBuilder.DropTable(
                name: "IndexDefinitions");
        }
    }
}
