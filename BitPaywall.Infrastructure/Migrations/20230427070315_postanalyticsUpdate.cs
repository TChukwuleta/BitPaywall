using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitPaywall.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class postanalyticsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RatedBy",
                table: "PostRatings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PostAnalytics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatedBy",
                table: "PostRatings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PostAnalytics");
        }
    }
}
