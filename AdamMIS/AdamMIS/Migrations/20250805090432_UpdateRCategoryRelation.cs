using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRCategoryRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_RCategories_CategoryId",
                table: "Reports");

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
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIMlamsozskdH61UM5CwcHHZXYjLJJ2EwwTS7/pNCshHIozPgrQPsSAi04X6Iu89tg==");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_RCategories_CategoryId",
                table: "Reports",
                column: "CategoryId",
                principalTable: "RCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_RCategories_CategoryId",
                table: "Reports");

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
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGMzp1ZFzDRh6IjvRxCo+KZtAvlHkVtFDLweQCOCbHE4++7adc+huUHBfUo6LiX3yQ==");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_RCategories_CategoryId",
                table: "Reports",
                column: "CategoryId",
                principalTable: "RCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
