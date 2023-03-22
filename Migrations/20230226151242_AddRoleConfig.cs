using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppointmentScheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "29399ca8-67a6-4cd4-9b19-877267cd51b1", null, "Employee", "EMPLOYEE" },
                    { "79545fb1-1663-4210-91cf-e8281c9bfd92", null, "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "29399ca8-67a6-4cd4-9b19-877267cd51b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "79545fb1-1663-4210-91cf-e8281c9bfd92");
        }
    }
}
