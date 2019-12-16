using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trakx.Data.Models.Migrations
{
    public partial class CreateIndexRepository : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndexValuation",
                columns: table => new
                {
                    IndexValuationId = table.Column<Guid>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    QuoteCurrency = table.Column<string>(nullable: false),
                    NetAssetValue = table.Column<decimal>(type: "decimal(38, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexValuation", x => x.IndexValuationId);
                });

            migrationBuilder.CreateTable(
                name: "ComponentValuation",
                columns: table => new
                {
                    ComponentValuationId = table.Column<Guid>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    QuoteCurrency = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    IndexValuationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentValuation", x => x.ComponentValuationId);
                    table.ForeignKey(
                        name: "FK_ComponentValuation_IndexValuation_IndexValuationId",
                        column: x => x.IndexValuationId,
                        principalTable: "IndexValuation",
                        principalColumn: "IndexValuationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndexDefinitions",
                columns: table => new
                {
                    Symbol = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(512)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR(256)", nullable: false),
                    InitialValuationIndexValuationId = table.Column<Guid>(nullable: false),
                    NaturalUnit = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexDefinitions", x => x.Symbol);
                    table.ForeignKey(
                        name: "FK_IndexDefinitions_IndexValuation_InitialValuationIndexValuationId",
                        column: x => x.InitialValuationIndexValuationId,
                        principalTable: "IndexValuation",
                        principalColumn: "IndexValuationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentDefinition",
                columns: table => new
                {
                    ComponentDefinitionId = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR(256)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(512)", nullable: false),
                    Symbol = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Decimals = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    InitialValuationComponentValuationId = table.Column<Guid>(nullable: false),
                    IndexDefinitionSymbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentDefinition", x => x.ComponentDefinitionId);
                    table.ForeignKey(
                        name: "FK_ComponentDefinition_IndexDefinitions_IndexDefinitionSymbol",
                        column: x => x.IndexDefinitionSymbol,
                        principalTable: "IndexDefinitions",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComponentDefinition_ComponentValuation_InitialValuationComponentValuationId",
                        column: x => x.InitialValuationComponentValuationId,
                        principalTable: "ComponentValuation",
                        principalColumn: "ComponentValuationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentDefinition_IndexDefinitionSymbol",
                table: "ComponentDefinition",
                column: "IndexDefinitionSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentDefinition_InitialValuationComponentValuationId",
                table: "ComponentDefinition",
                column: "InitialValuationComponentValuationId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentValuation_IndexValuationId",
                table: "ComponentValuation",
                column: "IndexValuationId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexDefinitions_InitialValuationIndexValuationId",
                table: "IndexDefinitions",
                column: "InitialValuationIndexValuationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentDefinition");

            migrationBuilder.DropTable(
                name: "IndexDefinitions");

            migrationBuilder.DropTable(
                name: "ComponentValuation");

            migrationBuilder.DropTable(
                name: "IndexValuation");
        }
    }
}
