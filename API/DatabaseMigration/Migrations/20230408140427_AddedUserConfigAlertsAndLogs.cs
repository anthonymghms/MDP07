using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseMigration.Migrations
{
    public partial class AddedUserConfigAlertsAndLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70ef66f3-66b4-428f-9662-589809b4c0eb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c58a8a40-0f9f-4c8d-8fc1-647e0c1b22c7");

            migrationBuilder.RenameColumn(
                name: "EmergencyContactId",
                table: "EmergencyContact",
                newName: "Id");

            migrationBuilder.CreateTable(
                name: "UserAlert",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AlertSeverity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlertMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlertDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAlert_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AlertType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlertVolume = table.Column<float>(type: "real", nullable: false),
                    LocationSharing = table.Column<bool>(type: "bit", nullable: false),
                    DarkMode = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConfig_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0f702d9e-e59d-48c4-bfec-8511a2999e69", "1", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "26856ff8-839e-483c-9988-d0972300ffd3", "2", "User", "User" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAlert_UserId",
                table: "UserAlert",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConfig_UserId",
                table: "UserConfig",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAlert");

            migrationBuilder.DropTable(
                name: "UserConfig");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0f702d9e-e59d-48c4-bfec-8511a2999e69");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26856ff8-839e-483c-9988-d0972300ffd3");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EmergencyContact",
                newName: "EmergencyContactId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "70ef66f3-66b4-428f-9662-589809b4c0eb", "2", "User", "User" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c58a8a40-0f9f-4c8d-8fc1-647e0c1b22c7", "1", "Admin", "Admin" });
        }
    }
}
