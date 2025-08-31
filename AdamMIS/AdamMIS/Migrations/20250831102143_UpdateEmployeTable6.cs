using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeTable6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELqy8DDy5S0BipoINGIZhGrOUT6V/uGAWVp/c9Yv+Kkck5GQVCu5GYOven5dC3/EJQ==");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "HeadId",
                value: "03174B27-D47B-4C12-94AD-676B3BF14BC2");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5,
                column: "HeadId",
                value: "9023369e-85f8-4389-85d0-d765caa0e1f9");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFNbXhWRotgZ0/uud4ilwB6Ux9F68RZpo/Yct2mjEvzO2kAbeIPT2cNfx1BBuoHRVQ==");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "HeadId",
                value: "b1855db6-6ac2-4750-a279-b4f52e2dc05f");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5,
                column: "HeadId",
                value: "080be5c4-43e2-4521-a80d-79936ed598df");
        }
    }
}
