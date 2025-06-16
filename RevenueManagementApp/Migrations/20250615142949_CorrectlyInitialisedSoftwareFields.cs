using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevenueManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class CorrectlyInitialisedSoftwareFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 1,
                column: "price",
                value: 4999.99m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 2,
                column: "price",
                value: 7999.99m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 3,
                column: "price",
                value: 2999.99m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 4,
                column: "price",
                value: 3499.99m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 6,
                column: "price",
                value: 1999.99m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 7,
                column: "price",
                value: 5999.99m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 8,
                column: "price",
                value: 1499.99m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 1,
                column: "price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 2,
                column: "price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 3,
                column: "price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 4,
                column: "price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 6,
                column: "price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 7,
                column: "price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Software",
                keyColumn: "id",
                keyValue: 8,
                column: "price",
                value: 0m);
        }
    }
}
