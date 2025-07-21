using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityRoleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDeafult", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "35904F06-0FD5-47F3-ACF2-23B77C14F947", "F851A5D6-1A5C-48BA-8C07-23DD5239319B", false, "SuperAdmin", "SUPERADMIN" },
                    { "7DFA4DBB-644A-4759-BC51-FB23455DB7C1", "62263B17-1587-4F52-9077-976245E626EA", true, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "03174B27-D47B-4C12-94AD-676B3BF14BC2", 0, "07C2800E-4457-4360-8102-A99EC489446D", null, true, "", "", false, null, null, "MOHAMEDKANDO", "AQAAAAIAAYagAAAAEB3jiNdDXPnqEtU7ce5yMXdYBDecRPmpJvdTuiKQ3/A56NhzBSRVtyse04xc2sc3/w==", null, false, "75B0F3ACD7DE4D088DA0594E3ACDC1EF", false, "MohamedKando" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "auth:add", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 2, "permissions", "user:delete", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 3, "permissions", "user:update", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 4, "permissions", "user:read", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 5, "permissions", "report:read", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 6, "permissions", "report:add", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 7, "permissions", "report:update", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 8, "permissions", "report:delete", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 9, "permissions", "category:read", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 10, "permissions", "category:add", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 11, "permissions", "category:update", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 12, "permissions", "category:delete", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 13, "permissions", "roles:read", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 14, "permissions", "roles:add", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 15, "permissions", "Roles:update", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 16, "permissions", "Roles:delete", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { 17, "permissions", "results:read", "35904F06-0FD5-47F3-ACF2-23B77C14F947" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "35904F06-0FD5-47F3-ACF2-23B77C14F947", "03174B27-D47B-4C12-94AD-676B3BF14BC2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7DFA4DBB-644A-4759-BC51-FB23455DB7C1");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "35904F06-0FD5-47F3-ACF2-23B77C14F947", "03174B27-D47B-4C12-94AD-676B3BF14BC2" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "35904F06-0FD5-47F3-ACF2-23B77C14F947");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
