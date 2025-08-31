using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeFormTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeadId",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameArabic = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEnglish = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PersonalEmail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PayrollNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    IsMedical = table.Column<bool>(type: "bit", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Specialty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MedicalServiceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DoctorStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SeniorDoctorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MedicalProfileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SystemPermissions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternetAccess = table.Column<bool>(type: "bit", nullable: true),
                    ExternalEmail = table.Column<bool>(type: "bit", nullable: true),
                    InternalEmail = table.Column<bool>(type: "bit", nullable: true),
                    FilesSharing = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NetworkId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CurrentStep = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "HR"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                    HRApproved = table.Column<bool>(type: "bit", nullable: false),
                    HRApprovedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HRApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartmentApproved = table.Column<bool>(type: "bit", nullable: false),
                    DepartmentApprovedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DepartmentApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ITApproved = table.Column<bool>(type: "bit", nullable: false),
                    ITApprovedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ITApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CEOApproved = table.Column<bool>(type: "bit", nullable: false),
                    CEOApprovedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CEOApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CEOSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_AspNetUsers_CEOApprovedById",
                        column: x => x.CEOApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_AspNetUsers_DepartmentApprovedById",
                        column: x => x.DepartmentApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_AspNetUsers_HRApprovedById",
                        column: x => x.HRApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_AspNetUsers_ITApprovedById",
                        column: x => x.ITApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPFzRjS70f6WEkMHr5YR+dFrzr5JzaHW6v3sOD1vUx7pZWfKp+M2zHubJzjPAT8eew==");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "HeadId",
                value: "b1855db6-6ac2-4750-a279-b4f52e2dc05f");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "HeadId",
                value: "b1855db6-6ac2-4750-a279-b4f52e2dc05f");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                column: "HeadId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                column: "HeadId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CEOApprovedById",
                table: "Employee",
                column: "CEOApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CreatedById",
                table: "Employee",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentApprovedById",
                table: "Employee",
                column: "DepartmentApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeNumber",
                table: "Employee",
                column: "EmployeeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_HRApprovedById",
                table: "Employee",
                column: "HRApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ITApprovedById",
                table: "Employee",
                column: "ITApprovedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropColumn(
                name: "HeadId",
                table: "Departments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMeyw3Vzpt4hoPeoQPzEQPeWSd7YU1YAgeiAN6NgJmLfCNUZz3KqF23Ux8K2nmbyRQ==");
        }
    }
}
