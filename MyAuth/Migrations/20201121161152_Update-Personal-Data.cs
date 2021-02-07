using Microsoft.EntityFrameworkCore.Migrations;

namespace MyAuth.Migrations
{
    public partial class UpdatePersonalData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "MyAuth",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "MyAuth",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                schema: "MyAuth",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "MyAuth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "MyAuth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                schema: "MyAuth",
                table: "AspNetUsers");
        }
    }
}
