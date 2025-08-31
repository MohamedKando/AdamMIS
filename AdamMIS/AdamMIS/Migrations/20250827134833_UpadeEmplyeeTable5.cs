using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpadeEmplyeeTable5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFNbXhWRotgZ0/uud4ilwB6Ux9F68RZpo/Yct2mjEvzO2kAbeIPT2cNfx1BBuoHRVQ==");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "HeadId",
                value: "9427f54e-4ca9-4662-b07b-ce078f19b4b9");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                column: "HeadId",
                value: "f2cb2e80-c4d8-432d-95b5-09ae5ab13069");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                column: "HeadId",
                value: "e43c1183-1abd-4b65-bc5a-64fa7b06d66e");

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "HeadId", "Name" },
                values: new object[] { 5, "080be5c4-43e2-4521-a80d-79936ed598df", "CEO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5);

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
        }
    }
}
