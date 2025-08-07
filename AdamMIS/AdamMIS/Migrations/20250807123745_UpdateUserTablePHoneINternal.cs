using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTablePHoneINternal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalPhone",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                columns: new[] { "InternalPhone", "PasswordHash" },
                values: new object[] { null, "AQAAAAIAAYagAAAAEIptpj2DNbSuQ/lgQoZ9lf1yTvp89EkhQj0zs6atj0jbW7jMgsToVJE7y80uN4VhtA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BV2",
                columns: new[] { "InternalPhone", "PasswordHash" },
                values: new object[] { null, "AQAAAAIAAYagAAAAEL2pCSZhUek2+eQqmCV1OOn//z6zm5DK0JgUNZSLqsdYQ4zKFU0o8EzVUTggOQ960g==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalPhone",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGVyqLk2t9TzWUyhKs/vbFmndUolhxALXlgBLDLyL+W6iEkwQ92u8lZYH+GpHaXMgQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BV2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDXpfNVnGnTEf99RD0t5l+TDA4O9ES7gWokTvGl3m4U1jrsO3KsvV8b90Vpk8r2n5w==");
        }
    }
}
