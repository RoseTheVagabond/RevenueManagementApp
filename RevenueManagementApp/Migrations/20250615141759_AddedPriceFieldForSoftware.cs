using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RevenueManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedPriceFieldForSoftware : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "price",
                table: "Software",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "Cathegory",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Business Software" },
                    { 2, "Gaming" },
                    { 3, "Design" }
                });

            migrationBuilder.InsertData(
                table: "Software",
                columns: new[] { "id", "Cathegory_id", "currentVersion", "description", "name", "price" },
                values: new object[,]
                {
                    { 1, 1, "2024", "Complete office productivity suite", "Office Suite Pro", 0m },
                    { 2, 2, "5.1.2", "Advanced 3D game development engine", "Game Engine X", 0m },
                    { 3, 3, "12.3", "Professional graphic design software", "Design Studio", 0m },
                    { 4, 1, "4.8.1", "Integrated development environment", "Code Builder", 0m },
                    { 6, 3, "3.5.1", "Design templates library", "DesignMaster", 0m },
                    { 7, 1, "2.1.4", "Online management system", "BusinessPlatform", 0m },
                    { 8, 2, "6.0.2", "Advanced multimedia player", "MediaPlayer Pro", 0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cathegory",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cathegory",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cathegory",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "price",
                table: "Software");
        }
    }
}
