using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevenueManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUserAndRoles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d692693e-cfae-4d82-a3cd-0712c73ea0d2", "AQAAAAIAAYagAAAAEKdlAjF1vpMNXZUk8SH3tc5yUqRzioo6YiJjV6UbIsej6GGc0n4Qr/Yc0cpFpZ69Hw==", "STATIC-SECURITY-STAMP-12345" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e4c0d3de-f5ea-4d81-88e6-e01c49982b13", "AQAAAAIAAYagAAAAEAyOt9ERlv/kyorSoAkDq/7lgm92mG7pgfmyNdELTcwyoC9uZM35R1oskzUnfsuLYg==", "f6d3f8f3-7ee0-4dce-8623-979f5d38a3e5" });
        }
    }
}
