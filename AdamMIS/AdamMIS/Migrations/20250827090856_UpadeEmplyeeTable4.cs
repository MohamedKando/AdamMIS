using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpadeEmplyeeTable4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_CEOApprovedById",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_DepartmentApprovedById",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_HRApprovedById",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_ITApprovedById",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CEOApprovedById",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentApprovedById",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_HRApprovedById",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ITApprovedById",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CEOApproved",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CEOApprovedById",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentApproved",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentApprovedById",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "HRApproved",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "HRApprovedById",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ITApproved",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ITApprovedById",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "ITApprovedAt",
                table: "Employees",
                newName: "ITCompletedAt");

            migrationBuilder.RenameColumn(
                name: "HRApprovedAt",
                table: "Employees",
                newName: "HRCompletedAt");

            migrationBuilder.RenameColumn(
                name: "DepartmentApprovedAt",
                table: "Employees",
                newName: "DepartmentCompletedAt");

            migrationBuilder.RenameColumn(
                name: "CEOApprovedAt",
                table: "Employees",
                newName: "CEOCompletedAt");

            migrationBuilder.AddColumn<string>(
                name: "CEOCompletedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCompletedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HRCompletedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ITCompletedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOuYM6esEUb50N/H98tyTAbJiA7Up8/GSZ9p6aShwKe8hcUwWxF/UFj7Tg+2AxIn2A==");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "HeadId",
                value: "03174B27-D47B-4C12-94AD-676B3BF14BC2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CEOCompletedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentCompletedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "HRCompletedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ITCompletedBy",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "ITCompletedAt",
                table: "Employees",
                newName: "ITApprovedAt");

            migrationBuilder.RenameColumn(
                name: "HRCompletedAt",
                table: "Employees",
                newName: "HRApprovedAt");

            migrationBuilder.RenameColumn(
                name: "DepartmentCompletedAt",
                table: "Employees",
                newName: "DepartmentApprovedAt");

            migrationBuilder.RenameColumn(
                name: "CEOCompletedAt",
                table: "Employees",
                newName: "CEOApprovedAt");

            migrationBuilder.AddColumn<bool>(
                name: "CEOApproved",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CEOApprovedById",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DepartmentApproved",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentApprovedById",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HRApproved",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HRApprovedById",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ITApproved",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ITApprovedById",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEM4TUd1hxXhd1JFOD5PEbk1AObQ8geEBFS9Hnyxl4Oepd38XzMusanH9I/fsoewzhw==");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "HeadId",
                value: "b1855db6-6ac2-4750-a279-b4f52e2dc05f");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CEOApprovedById",
                table: "Employees",
                column: "CEOApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentApprovedById",
                table: "Employees",
                column: "DepartmentApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_HRApprovedById",
                table: "Employees",
                column: "HRApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ITApprovedById",
                table: "Employees",
                column: "ITApprovedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_CEOApprovedById",
                table: "Employees",
                column: "CEOApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_DepartmentApprovedById",
                table: "Employees",
                column: "DepartmentApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_HRApprovedById",
                table: "Employees",
                column: "HRApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_ITApprovedById",
                table: "Employees",
                column: "ITApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
