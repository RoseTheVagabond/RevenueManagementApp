using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevenueManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class RemovedClientClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Company_Client",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "Contract_Client",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "Individual_Client",
                table: "Individual");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropPrimaryKey(
                name: "Individual_pk",
                table: "Individual");

            migrationBuilder.DropIndex(
                name: "IX_Contract_Client_id",
                table: "Contract");

            migrationBuilder.DropPrimaryKey(
                name: "Company_pk",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Client_id",
                table: "Individual");

            migrationBuilder.DropColumn(
                name: "Client_id",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "Client_id",
                table: "Company");

            migrationBuilder.AlterColumn<decimal>(
                name: "toPay",
                table: "Contract",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start",
                table: "Contract",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "softwareDeadline",
                table: "Contract",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<decimal>(
                name: "paid",
                table: "Contract",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");

            migrationBuilder.AlterColumn<bool>(
                name: "isSigned",
                table: "Contract",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "isPaid",
                table: "Contract",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Software_id",
                table: "Contract",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Discount_id",
                table: "Contract",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Company_KRS",
                table: "Contract",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Individual_PESEL",
                table: "Contract",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "Individual_pk",
                table: "Individual",
                column: "PESEL");

            migrationBuilder.AddPrimaryKey(
                name: "Company_pk",
                table: "Company",
                column: "KRS");

            migrationBuilder.CreateIndex(
                name: "IX_Individual_PESEL",
                table: "Individual",
                column: "PESEL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contract_Company_KRS",
                table: "Contract",
                column: "Company_KRS");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_Individual_PESEL",
                table: "Contract",
                column: "Individual_PESEL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Contract_ClientType",
                table: "Contract",
                sql: "([Individual_PESEL] IS NOT NULL AND [Company_KRS] IS NULL) OR ([Individual_PESEL] IS NULL AND [Company_KRS] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Company_KRS",
                table: "Company",
                column: "KRS",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "Contract_Company",
                table: "Contract",
                column: "Company_KRS",
                principalTable: "Company",
                principalColumn: "KRS");

            migrationBuilder.AddForeignKey(
                name: "Contract_Individual",
                table: "Contract",
                column: "Individual_PESEL",
                principalTable: "Individual",
                principalColumn: "PESEL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Contract_Company",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "Contract_Individual",
                table: "Contract");

            migrationBuilder.DropPrimaryKey(
                name: "Individual_pk",
                table: "Individual");

            migrationBuilder.DropIndex(
                name: "IX_Individual_PESEL",
                table: "Individual");

            migrationBuilder.DropIndex(
                name: "IX_Contract_Company_KRS",
                table: "Contract");

            migrationBuilder.DropIndex(
                name: "IX_Contract_Individual_PESEL",
                table: "Contract");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Contract_ClientType",
                table: "Contract");

            migrationBuilder.DropPrimaryKey(
                name: "Company_pk",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_KRS",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Company_KRS",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "Individual_PESEL",
                table: "Contract");

            migrationBuilder.AddColumn<int>(
                name: "Client_id",
                table: "Individual",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "toPay",
                table: "Contract",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start",
                table: "Contract",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "softwareDeadline",
                table: "Contract",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "paid",
                table: "Contract",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "isSigned",
                table: "Contract",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "isPaid",
                table: "Contract",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Software_id",
                table: "Contract",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Discount_id",
                table: "Contract",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Client_id",
                table: "Contract",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Client_id",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "Individual_pk",
                table: "Individual",
                column: "Client_id");

            migrationBuilder.AddPrimaryKey(
                name: "Company_pk",
                table: "Company",
                column: "Client_id");

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    clientType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Client_pk", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contract_Client_id",
                table: "Contract",
                column: "Client_id");

            migrationBuilder.AddForeignKey(
                name: "Company_Client",
                table: "Company",
                column: "Client_id",
                principalTable: "Client",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "Contract_Client",
                table: "Contract",
                column: "Client_id",
                principalTable: "Client",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "Individual_Client",
                table: "Individual",
                column: "Client_id",
                principalTable: "Client",
                principalColumn: "id");
        }
    }
}
