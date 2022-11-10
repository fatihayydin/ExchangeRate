using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeRate.Data.Migrations
{
    /// <inheritdoc />
    public partial class Httpstatuscodeadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerApiLogs_ApiKey",
                table: "CustomerApiLogs");

            migrationBuilder.AddColumn<int>(
                name: "HttpStatusCode",
                table: "CustomerApiLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HttpStatusCode",
                table: "CustomerApiLogs");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerApiLogs_ApiKey",
                table: "CustomerApiLogs",
                column: "ApiKey");
        }
    }
}
