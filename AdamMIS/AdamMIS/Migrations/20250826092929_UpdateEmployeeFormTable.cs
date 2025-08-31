using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeFormTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers_CEOApprovedById",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers_CreatedById",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers_DepartmentApprovedById",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers_HRApprovedById",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers_ITApprovedById",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Departments_DepartmentId",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_ITApprovedById",
                table: "Employees",
                newName: "IX_Employees_ITApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_HRApprovedById",
                table: "Employees",
                newName: "IX_Employees_HRApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_EmployeeNumber",
                table: "Employees",
                newName: "IX_Employees_EmployeeNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employees",
                newName: "IX_Employees_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_DepartmentApprovedById",
                table: "Employees",
                newName: "IX_Employees_DepartmentApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_CreatedById",
                table: "Employees",
                newName: "IX_Employees_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_CEOApprovedById",
                table: "Employees",
                newName: "IX_Employees_CEOApprovedById");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId1",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOqRxu4LPOmV5P2Tms3rsSqMJdo/lEnyFnta4oqVsOhg30l5QnLSgFoUOEVLCExEbQ==");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId1",
                table: "Employees",
                column: "DepartmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_CEOApprovedById",
                table: "Employees",
                column: "CEOApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById",
                table: "Employees",
                column: "CreatedById",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId1",
                table: "Employees",
                column: "DepartmentId1",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_CEOApprovedById",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_CreatedById",
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

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId1",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId1",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employee");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_ITApprovedById",
                table: "Employee",
                newName: "IX_Employee_ITApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_HRApprovedById",
                table: "Employee",
                newName: "IX_Employee_HRApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_EmployeeNumber",
                table: "Employee",
                newName: "IX_Employee_EmployeeNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employee",
                newName: "IX_Employee_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_DepartmentApprovedById",
                table: "Employee",
                newName: "IX_Employee_DepartmentApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_CreatedById",
                table: "Employee",
                newName: "IX_Employee_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_CEOApprovedById",
                table: "Employee",
                newName: "IX_Employee_CEOApprovedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPFzRjS70f6WEkMHr5YR+dFrzr5JzaHW6v3sOD1vUx7pZWfKp+M2zHubJzjPAT8eew==");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AspNetUsers_CEOApprovedById",
                table: "Employee",
                column: "CEOApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AspNetUsers_CreatedById",
                table: "Employee",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AspNetUsers_DepartmentApprovedById",
                table: "Employee",
                column: "DepartmentApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AspNetUsers_HRApprovedById",
                table: "Employee",
                column: "HRApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AspNetUsers_ITApprovedById",
                table: "Employee",
                column: "ITApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Departments_DepartmentId",
                table: "Employee",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
