using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseMigration.Migrations
{
    public partial class UpdatedAlertMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25f28476-b9b3-4313-824f-3916239bf069");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c4d2e3d7-bf2b-4a8e-b8ac-33e32a9c98b2");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "AlertMessage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ToUserId",
                table: "AlertMessage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "55757225-655a-45ff-9016-98cebde5781f", "1", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "90a28dec-63b8-4c42-95b0-192d326f70f0", "2", "User", "User" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55757225-655a-45ff-9016-98cebde5781f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90a28dec-63b8-4c42-95b0-192d326f70f0");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "AlertMessage");

            migrationBuilder.DropColumn(
                name: "ToUserId",
                table: "AlertMessage");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "25f28476-b9b3-4313-824f-3916239bf069", "2", "User", "User" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c4d2e3d7-bf2b-4a8e-b8ac-33e32a9c98b2", "1", "Admin", "Admin" });
        }
    }
}
