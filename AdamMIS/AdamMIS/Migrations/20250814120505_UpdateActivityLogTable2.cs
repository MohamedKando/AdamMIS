using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActivityLogTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SesesionTime",
                table: "acivityLogs",
                newName: "SessionTime");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIShK5at1rABQ5u7MkanNGx2ymIN9YSUBcrPe/mjHwGUDTnvBbaiqLlhx7sdIGWTOw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionTime",
                table: "acivityLogs",
                newName: "SesesionTime");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMLnHQUtg/L5ESybG+lN4/eGg7Ay22lGbHB2sXGkGx2mKFuB4z4z8oqkj8x8oRtBBg==");
        }
    }
}
