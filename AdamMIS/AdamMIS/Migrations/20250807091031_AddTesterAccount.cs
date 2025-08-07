using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class AddTesterAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "RCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                columns: new[] { "NormalizedUserName", "PasswordHash", "UserName" },
                values: new object[] { "MOHAMEDKANDIL", "AQAAAAIAAYagAAAAEGVyqLk2t9TzWUyhKs/vbFmndUolhxALXlgBLDLyL+W6iEkwQ92u8lZYH+GpHaXMgQ==", "MohamedKandil" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DepartmentId", "Email", "EmailConfirmed", "IsDisabled", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoPath", "SecurityStamp", "Title", "TwoFactorEnabled", "UserName" },
                values: new object[] { "03174B27-D47B-4C12-94AD-676B3BF14BV2", 0, "07C2800E-4457-4360-8102-A99EC48944VD", null, null, true, false, false, null, null, "TESTER", "AQAAAAIAAYagAAAAEDXpfNVnGnTEf99RD0t5l+TDA4O9ES7gWokTvGl3m4U1jrsO3KsvV8b90Vpk8r2n5w==", null, false, null, "75B0F3ACD7DE4D088DA0594E3ACDC1EF", null, false, "Tester" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "35904F06-0FD5-47F3-ACF2-23B77C14F947", "03174B27-D47B-4C12-94AD-676B3BF14BV2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "35904F06-0FD5-47F3-ACF2-23B77C14F947", "03174B27-D47B-4C12-94AD-676B3BF14BV2" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BV2");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "RCategories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RCategories");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                columns: new[] { "NormalizedUserName", "PasswordHash", "UserName" },
                values: new object[] { "MOHAMEDKANDO", "AQAAAAIAAYagAAAAEGMzp1ZFzDRh6IjvRxCo+KZtAvlHkVtFDLweQCOCbHE4++7adc+huUHBfUo6LiX3yQ==", "MohamedKando" });
        }
    }
}
