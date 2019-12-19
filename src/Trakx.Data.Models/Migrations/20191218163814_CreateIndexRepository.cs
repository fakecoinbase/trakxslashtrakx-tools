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
                    Id = table.Column<Guid>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    QuoteCurrency = table.Column<string>(nullable: false),
                    NetAssetValue = table.Column<decimal>(type: "decimal(38, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexValuation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentValuation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    QuoteCurrency = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    IndexValuationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentValuation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentValuation_IndexValuation_IndexValuationId",
                        column: x => x.IndexValuationId,
                        principalTable: "IndexValuation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndexDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Symbol = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Address = table.Column<string>(maxLength: 256, nullable: false),
                    InitialValuationId = table.Column<Guid>(nullable: false),
                    NaturalUnit = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndexDefinitions_IndexValuation_InitialValuationId",
                        column: x => x.InitialValuationId,
                        principalTable: "IndexValuation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentDefinition",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(maxLength: 256, nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Symbol = table.Column<string>(maxLength: 50, nullable: false),
                    Decimals = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    ComponentDefinition = table.Column<Guid>(nullable: true),
                    IndexDefinitionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentDefinition_ComponentValuation_ComponentDefinition",
                        column: x => x.ComponentDefinition,
                        principalTable: "ComponentValuation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComponentDefinition_IndexDefinitions_IndexDefinitionId",
                        column: x => x.IndexDefinitionId,
                        principalTable: "IndexDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentDefinition_ComponentDefinition",
                table: "ComponentDefinition",
                column: "ComponentDefinition",
                unique: true,
                filter: "[ComponentDefinition] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentDefinition_IndexDefinitionId",
                table: "ComponentDefinition",
                column: "IndexDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentValuation_IndexValuationId",
                table: "ComponentValuation",
                column: "IndexValuationId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexDefinitions_InitialValuationId",
                table: "IndexDefinitions",
                column: "InitialValuationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentDefinition");

            migrationBuilder.DropTable(
                name: "ComponentValuation");

            migrationBuilder.DropTable(
                name: "IndexDefinitions");

            migrationBuilder.DropTable(
                name: "IndexValuation");
        }
    }
}
