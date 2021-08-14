using Microsoft.EntityFrameworkCore.Migrations;

namespace MyAuth.Migrations
{
    public partial class FaceDescriptor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaceDescriptor",
                schema: "MyAuth",
                table: "MyAuthUsers",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasFaceRegistered",
                schema: "MyAuth",
                table: "MyAuthUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaceDescriptor",
                schema: "MyAuth",
                table: "MyAuthUsers");

            migrationBuilder.DropColumn(
                name: "HasFaceRegistered",
                schema: "MyAuth",
                table: "MyAuthUsers");
        }
    }
}
