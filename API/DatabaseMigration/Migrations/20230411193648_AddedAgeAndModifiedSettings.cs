using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseMigration.Migrations
{
    public partial class AddedAgeAndModifiedSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0f702d9e-e59d-48c4-bfec-8511a2999e69");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26856ff8-839e-483c-9988-d0972300ffd3");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "UserConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "UserConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationsEnabled",
                table: "UserConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "UserConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorAuthEnabled",
                table: "UserConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "UserConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthday",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "35285870-0b9d-4979-8618-6972a2924437", "1", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f0330bab-c756-4e65-8fdc-f28b79962a4b", "2", "User", "User" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "35285870-0b9d-4979-8618-6972a2924437");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0330bab-c756-4e65-8fdc-f28b79962a4b");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserConfig");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "UserConfig");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "UserConfig");

            migrationBuilder.DropColumn(
                name: "NotificationsEnabled",
                table: "UserConfig");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "UserConfig");

            migrationBuilder.DropColumn(
                name: "TwoFactorAuthEnabled",
                table: "UserConfig");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "UserConfig");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0f702d9e-e59d-48c4-bfec-8511a2999e69", "1", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "26856ff8-839e-483c-9988-d0972300ffd3", "2", "User", "User" });
        }
    }
}
