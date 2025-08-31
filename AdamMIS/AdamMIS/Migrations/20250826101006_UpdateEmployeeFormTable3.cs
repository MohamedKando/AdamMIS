using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeFormTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById1",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId1",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CreatedById",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CreatedById1",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId1",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedById1",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
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
                value: "AQAAAAIAAYagAAAAEM4TUd1hxXhd1JFOD5PEbk1AObQ8geEBFS9Hnyxl4Oepd38XzMusanH9I/fsoewzhw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById1",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId1",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFJjRbZEXPZEValXhOFI8q2on3I2C72VMGIsYTecOiy95dN34wnJabsVwbeKBNgClg==");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CreatedById",
                table: "Employees",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CreatedById1",
                table: "Employees",
                column: "CreatedById1");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId1",
                table: "Employees",
                column: "DepartmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById",
                table: "Employees",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById1",
                table: "Employees",
                column: "CreatedById1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId1",
                table: "Employees",
                column: "DepartmentId1",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
