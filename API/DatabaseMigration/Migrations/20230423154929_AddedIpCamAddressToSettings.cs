using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseMigration.Migrations
{
    public partial class AddedIpCamAddressToSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "969e2786-f441-423f-8418-bd0dcb074557");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ab7e99a7-c0b3-4f8c-8f3b-51cac8eea44e");

            migrationBuilder.AddColumn<string>(
                name: "IpCamAddress",
                table: "UserConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "25f28476-b9b3-4313-824f-3916239bf069", "2", "User", "User" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c4d2e3d7-bf2b-4a8e-b8ac-33e32a9c98b2", "1", "Admin", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25f28476-b9b3-4313-824f-3916239bf069");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c4d2e3d7-bf2b-4a8e-b8ac-33e32a9c98b2");

            migrationBuilder.DropColumn(
                name: "IpCamAddress",
                table: "UserConfig");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "969e2786-f441-423f-8418-bd0dcb074557", "1", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ab7e99a7-c0b3-4f8c-8f3b-51cac8eea44e", "2", "User", "User" });
        }
    }
}
