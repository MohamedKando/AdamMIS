using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdamMIS.Migrations
{
    /// <inheritdoc />
    public partial class AddingMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RecipientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -25, "permissions", "View AIMS", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -24, "permissions", "View H DMS", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -23, "permissions", "View F DMS", "35904F06-0FD5-47F3-ACF2-23B77C14F947" },
                    { -22, "permissions", "View MRM", "35904F06-0FD5-47F3-ACF2-23B77C14F947" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMeyw3Vzpt4hoPeoQPzEQPeWSd7YU1YAgeiAN6NgJmLfCNUZz3KqF23Ux8K2nmbyRQ==");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Conversation",
                table: "Messages",
                columns: new[] { "SenderId", "RecipientId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Recipient",
                table: "Messages",
                column: "RecipientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -25);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -24);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -23);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -22);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "03174B27-D47B-4C12-94AD-676B3BF14BC2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDWc3Aao79vDyzfZcVlCJGem32culS+LD80gAqtPkS9TE+SVWSTsJChusBkLQvj9nA==");
        }
    }
}
