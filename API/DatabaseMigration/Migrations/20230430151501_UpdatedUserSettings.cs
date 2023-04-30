using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseMigration.Migrations
{
    public partial class UpdatedUserSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55757225-655a-45ff-9016-98cebde5781f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90a28dec-63b8-4c42-95b0-192d326f70f0");

            migrationBuilder.AddColumn<string>(
                name: "IpEspAddress",
                table: "UserConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3172442b-76e6-43a9-b444-1ae5e8923140", "2", "User", "User" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "af80ebc3-264f-4ca5-a116-f399c6716641", "1", "Admin", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3172442b-76e6-43a9-b444-1ae5e8923140");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "af80ebc3-264f-4ca5-a116-f399c6716641");

            migrationBuilder.DropColumn(
                name: "IpEspAddress",
                table: "UserConfig");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "55757225-655a-45ff-9016-98cebde5781f", "1", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "90a28dec-63b8-4c42-95b0-192d326f70f0", "2", "User", "User" });
        }
    }
}
