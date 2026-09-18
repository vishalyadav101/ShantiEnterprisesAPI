using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShantiEnterprises.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteBusinessDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerCare",
                table: "WebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GSTIN",
                table: "WebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Proprietor",
                table: "WebsiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SinceYear",
                table: "WebsiteSettings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCare",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "GSTIN",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Proprietor",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "SinceYear",
                table: "WebsiteSettings");
        }
    }
}
