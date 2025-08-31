using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeFormTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById1",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFJjRbZEXPZEValXhOFI8q2on3I2C72VMGIsYTecOiy95dN34wnJabsVwbeKBNgClg==");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CreatedById1",
                table: "Employees",
                column: "CreatedById1");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById1",
                table: "Employees",
                column: "CreatedById1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById1",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CreatedById1",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedById1",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOqRxu4LPOmV5P2Tms3rsSqMJdo/lEnyFnta4oqVsOhg30l5QnLSgFoUOEVLCExEbQ==");
        }
    }
}
