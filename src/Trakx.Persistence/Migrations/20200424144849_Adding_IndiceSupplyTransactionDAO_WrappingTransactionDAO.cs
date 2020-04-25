using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trakx.Persistence.Migrations
{
    public partial class Adding_IndiceSupplyTransactionDAO_WrappingTransactionDAO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndiceSupplyTransaction_IndiceCompositions_IndiceCompositionDaoId",
                table: "IndiceSupplyTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WrappingTransaction",
                table: "WrappingTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IndiceSupplyTransaction",
                table: "IndiceSupplyTransaction");

            migrationBuilder.DropColumn(
                name: "TimeOfTransaction",
                table: "WrappingTransaction");

            migrationBuilder.DropColumn(
                name: "AmountOfIndices",
                table: "IndiceSupplyTransaction");

            migrationBuilder.DropColumn(
                name: "AmountUsdc",
                table: "IndiceSupplyTransaction");

            migrationBuilder.DropColumn(
                name: "ReceiverAddress",
                table: "IndiceSupplyTransaction");

            migrationBuilder.DropColumn(
                name: "TimestampCreation",
                table: "IndiceSupplyTransaction");

            migrationBuilder.DropColumn(
                name: "TypeOfTx",
                table: "IndiceSupplyTransaction");

            migrationBuilder.RenameTable(
                name: "WrappingTransaction",
                newName: "WrappingTransactions");

            migrationBuilder.RenameTable(
                name: "IndiceSupplyTransaction",
                newName: "IndiceSupplyTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_IndiceSupplyTransaction_IndiceCompositionDaoId",
                table: "IndiceSupplyTransactions",
                newName: "IX_IndiceSupplyTransactions_IndiceCompositionDaoId");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "WrappingTransactions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "WrappingTransactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "IndiceSupplyTransactions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTimestamp",
                table: "IndiceSupplyTransactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "IndiceSupplyTransactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "IndiceSupplyTransactions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WrappingTransactions",
                table: "WrappingTransactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IndiceSupplyTransactions",
                table: "IndiceSupplyTransactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IndiceSupplyTransactions_IndiceCompositions_IndiceCompositionDaoId",
                table: "IndiceSupplyTransactions",
                column: "IndiceCompositionDaoId",
                principalTable: "IndiceCompositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndiceSupplyTransactions_IndiceCompositions_IndiceCompositionDaoId",
                table: "IndiceSupplyTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WrappingTransactions",
                table: "WrappingTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IndiceSupplyTransactions",
                table: "IndiceSupplyTransactions");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "WrappingTransactions");

            migrationBuilder.DropColumn(
                name: "CreationTimestamp",
                table: "IndiceSupplyTransactions");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "IndiceSupplyTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "IndiceSupplyTransactions");

            migrationBuilder.RenameTable(
                name: "WrappingTransactions",
                newName: "WrappingTransaction");

            migrationBuilder.RenameTable(
                name: "IndiceSupplyTransactions",
                newName: "IndiceSupplyTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_IndiceSupplyTransactions_IndiceCompositionDaoId",
                table: "IndiceSupplyTransaction",
                newName: "IX_IndiceSupplyTransaction_IndiceCompositionDaoId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "WrappingTransaction",
                type: "int",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOfTransaction",
                table: "WrappingTransaction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "IndiceSupplyTransaction",
                type: "int",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountOfIndices",
                table: "IndiceSupplyTransaction",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountUsdc",
                table: "IndiceSupplyTransaction",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverAddress",
                table: "IndiceSupplyTransaction",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimestampCreation",
                table: "IndiceSupplyTransaction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TypeOfTx",
                table: "IndiceSupplyTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WrappingTransaction",
                table: "WrappingTransaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IndiceSupplyTransaction",
                table: "IndiceSupplyTransaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IndiceSupplyTransaction_IndiceCompositions_IndiceCompositionDaoId",
                table: "IndiceSupplyTransaction",
                column: "IndiceCompositionDaoId",
                principalTable: "IndiceCompositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
