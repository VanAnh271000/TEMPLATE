using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Discriminator", "Name", "NormalizedName" },
                values: new object[] { "11111111-1111-1111-1111-111111111111", null, "Full system access", "ApplicationRole", "Admin", null });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Action", "Description", "Module", "Name", "Resource" },
                values: new object[,]
                {
                    { 1, "read", "Get user", "User", "user.read", "user" },
                    { 2, "write", "Create/ update user", "User", "user.write", "user" },
                    { 3, "delete", "Delete user", "User", "user.delete", "user" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId", "AssignedAt" },
                values: new object[] { 1, "11111111-1111-1111-1111-111111111111", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, "11111111-1111-1111-1111-111111111111" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11111111-1111-1111-1111-111111111111");

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
