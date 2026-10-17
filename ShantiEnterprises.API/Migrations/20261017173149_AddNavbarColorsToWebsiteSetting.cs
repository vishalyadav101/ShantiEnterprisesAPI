using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShantiEnterprises.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNavbarColorsToWebsiteSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NavbarButtonColor",
                table: "WebsiteSettings",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavbarColor1",
                table: "WebsiteSettings",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavbarColor2",
                table: "WebsiteSettings",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavbarGlowColor",
                table: "WebsiteSettings",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NavbarButtonColor",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "NavbarColor1",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "NavbarColor2",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "NavbarGlowColor",
                table: "WebsiteSettings");
        }
    }
}
