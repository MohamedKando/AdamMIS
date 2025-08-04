using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoPathToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[] { 19, "permissions", "View Admin Manager", "35904F06-0FD5-47F3-ACF2-23B77C14F947" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                columns: new[] { "PasswordHash", "PhotoPath" },
                values: new object[] { "AQAAAAIAAYagAAAAEGMzp1ZFzDRh6IjvRxCo+KZtAvlHkVtFDLweQCOCbHE4++7adc+huUHBfUo6LiX3yQ==", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHIzA+DGXse4tNQDdJp1il7BNOmptq1ZnwMeds1ZHE6X8R0ssRhVku4cIEOnqU1xow==");
        }
    }
}
