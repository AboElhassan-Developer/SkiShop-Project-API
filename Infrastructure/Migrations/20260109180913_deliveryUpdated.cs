using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deliveryUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4493341a-3643-4377-a282-82ad545312e3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "960bc3bb-9573-4649-b372-0a62c828baf1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "836e2c59-22d9-4e33-a51c-dcb9b9b06c20", null, "Admin", "ADMIN" },
                    { "88cd2c66-e30e-47d6-bf47-0c20434f1d32", null, "Customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "836e2c59-22d9-4e33-a51c-dcb9b9b06c20");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88cd2c66-e30e-47d6-bf47-0c20434f1d32");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4493341a-3643-4377-a282-82ad545312e3", null, "Customer", "CUSTOMER" },
                    { "960bc3bb-9573-4649-b372-0a62c828baf1", null, "Admin", "ADMIN" }
                });
        }
    }
}
