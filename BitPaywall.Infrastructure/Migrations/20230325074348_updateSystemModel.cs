using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitPaywall.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateSystemModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "PostAnalytics");

            migrationBuilder.AlterColumn<string>(
                name: "DebitAccount",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountGenerated",
                table: "PostAnalytics",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReadCount",
                table: "PostAnalytics",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountGenerated",
                table: "PostAnalytics");

            migrationBuilder.DropColumn(
                name: "ReadCount",
                table: "PostAnalytics");

            migrationBuilder.AlterColumn<string>(
                name: "DebitAccount",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "PostAnalytics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
