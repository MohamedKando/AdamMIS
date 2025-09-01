using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentDeadTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadId",
                table: "Departments");

            migrationBuilder.CreateTable(
                name: "DepartmentHeads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeadId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentHeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentHeads_Departments_DepartmentId",
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
                value: "AQAAAAIAAYagAAAAEHrxE0rGEdO0HsJ+mGW1zNVVjE/EZhSi0MJ57NSgGGMllxXmidtiXZ1JiRFjXfHsKA==");

            migrationBuilder.InsertData(
                table: "DepartmentHeads",
                columns: new[] { "Id", "DepartmentId", "HeadId" },
                values: new object[,]
                {
                    { 1, 1, "03174B27-D47B-4C12-94AD-676B3BF14BC2" },
                    { 2, 2, "9427f54e-4ca9-4662-b07b-ce078f19b4b9" },
                    { 3, 3, "f2cb2e80-c4d8-432d-95b5-09ae5ab13069" },
                    { 4, 4, "e43c1183-1abd-4b65-bc5a-64fa7b06d66e" },
                    { 5, 5, "9023369e-85f8-4389-85d0-d765caa0e1f9" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentHeads_DepartmentId",
                table: "DepartmentHeads",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentHeads");

            migrationBuilder.AddColumn<string>(
                name: "HeadId",
                table: "Departments",
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

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5,
                column: "HeadId",
                value: "9023369e-85f8-4389-85d0-d765caa0e1f9");
        }
    }
}
