using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueProjectTitlePerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_PortfolioUserId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PortfolioUserId_Title",
                table: "Projects",
                columns: new[] { "PortfolioUserId", "Title" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_PortfolioUserId_Title",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PortfolioUserId",
                table: "Projects",
                column: "PortfolioUserId");
        }
    }
}
