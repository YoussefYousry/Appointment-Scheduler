using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppointmentScheduler.Migrations
{
    /// <inheritdoc />
    public partial class PK_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5cc916e3-b158-440d-b532-3889fe57c7b9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8df84c13-5ecb-43f9-94b2-a4e543cec720");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0b9ca3c-1db5-4b14-93cc-2c862831a8c0");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2872eecb-f7c6-476e-9d44-56f71d7ed62b", null, "Admin", "ADMIN" },
                    { "8553d17d-fbf8-4619-a130-d8c3746a80c3", null, "Employee", "EMPLOYEE" },
                    { "c5b89840-975d-4690-bd60-5e53ead87008", null, "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2872eecb-f7c6-476e-9d44-56f71d7ed62b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8553d17d-fbf8-4619-a130-d8c3746a80c3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5b89840-975d-4690-bd60-5e53ead87008");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5cc916e3-b158-440d-b532-3889fe57c7b9", null, "Admin", "ADMIN" },
                    { "8df84c13-5ecb-43f9-94b2-a4e543cec720", null, "Manager", "MANAGER" },
                    { "f0b9ca3c-1db5-4b14-93cc-2c862831a8c0", null, "Employee", "EMPLOYEE" }
                });
        }
    }
}
