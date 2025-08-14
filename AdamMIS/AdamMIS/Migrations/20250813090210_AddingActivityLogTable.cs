using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class AddingActivityLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "35904F06-0FD5-47F3-ACF2-23B77C14F947", "03174B27-D47B-4C12-94AD-676B3BF14BV2" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BV2");

            migrationBuilder.CreateTable(
                name: "acivityLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastActivityTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acivityLogs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -20, "permissions", "View Audits Logs", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -19, "permissions", "View Admin Manager", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -18, "permissions", "View Report Manager", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -17, "permissions", "Read Result", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -16, "permissions", "Delete Roles", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -15, "permissions", "Update Roles", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -14, "permissions", "Add Roles", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -13, "permissions", "Read Roles", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -12, "permissions", "Delete Categories", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -11, "permissions", "Update Categories", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -10, "permissions", "Add Categories", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -9, "permissions", "Read Categories", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -8, "permissions", "Delete Reports", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -7, "permissions", "Update Reports", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -6, "permissions", "Add Reports", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -5, "permissions", "Read Reports", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -4, "permissions", "Read Users", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -3, "permissions", "Update Users", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -2, "permissions", "Delete Users", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -1, "permissions", "Register Users", "35904F06-0FD5-47F3-ACF2-23B77C14F947" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELbW9ANXI1UIr+xdPaRgym8rddPjsCDAoe6s6qmiCUVvzFX1b48LpNcfsrU+e6D55g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acivityLogs");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -20);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -19);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -18);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -17);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -16);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -15);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -14);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "Register Users", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 2, "permissions", "Delete Users", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 3, "permissions", "Update Users", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 4, "permissions", "Read Users", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 5, "permissions", "Read Reports", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 6, "permissions", "Add Reports", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 7, "permissions", "Update Reports", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 8, "permissions", "Delete Reports", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 9, "permissions", "Read Categories", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 10, "permissions", "Add Categories", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 11, "permissions", "Update Categories", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 12, "permissions", "Delete Categories", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 13, "permissions", "Read Roles", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 14, "permissions", "Add Roles", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 15, "permissions", "Update Roles", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 16, "permissions", "Delete Roles", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 17, "permissions", "Read Result", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 18, "permissions", "View Report Manager", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 19, "permissions", "View Admin Manager", "35904F06-0FD5-47F3-ACF2-23B77C14F947" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDRQCciCSSdu9SyHUj6PpoPY7JV+G1j89INj5uZbBkaazP99fesf4Q84mDHiaGBJyQ==");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DepartmentId", "Email", "EmailConfirmed", "InternalPhone", "IsDisabled", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoPath", "SecurityStamp", "Title", "TwoFactorEnabled", "UserName" },
                values: new object[] { "03174B27-D47B-4C12-94AD-676B3BF14BV2", 0, "07C2800E-4457-4360-8102-A99EC48944VD", null, null, true, null, false, false, null, null, "TESTER", "AQAAAAIAAYagAAAAEGe9bClB0OKYMEEGouhmn8YVq5H+2/U4cy9WgO/dJ7Xw1WBp7tJk+zLXT49zpUZjbw==", null, false, null, "75B0F3ACD7DE4D088DA0594E3ACDC1EF", null, false, "Tester" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "35904F06-0FD5-47F3-ACF2-23B77C14F947", "03174B27-D47B-4C12-94AD-676B3BF14BV2" });
        }
    }
}
