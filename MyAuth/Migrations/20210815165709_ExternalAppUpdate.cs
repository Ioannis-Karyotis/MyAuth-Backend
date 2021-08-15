using Microsoft.EntityFrameworkCore.Migrations;

namespace MyAuth.Migrations
{
    public partial class ExternalAppUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppName",
                schema: "MyAuth",
                table: "ExternalApps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                schema: "MyAuth",
                table: "ExternalApps",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppName",
                schema: "MyAuth",
                table: "ExternalApps");

            migrationBuilder.DropColumn(
                name: "BaseUrl",
                schema: "MyAuth",
                table: "ExternalApps");
        }
    }
}
