using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeRate.Data.Migrations
{
    /// <inheritdoc />
    public partial class Indexesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
               name: "IX_CustomerApiLogs_ApiKey_CreatedDate_HttpStatusCode_Direction",
               table: "CustomerApiLogs",
               columns: new[] { "ApiKey", "CreatedDate", "HttpStatusCode", "Direction" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                 name: "IX_CustomerApiLogs_ApiKey_CreatedDate_HttpStatusCode_Direction",
                 table: "CustomerApiLogs");
        }
    }
}
