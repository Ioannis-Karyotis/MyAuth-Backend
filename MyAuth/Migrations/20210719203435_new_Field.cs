using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyAuth.Migrations
{
    public partial class new_Field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MyAuth");

            migrationBuilder.CreateTable(
                name: "MyAuthUsers",
                schema: "MyAuth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Surname = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyAuthUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalApps",
                schema: "MyAuth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MyAuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    ClientSecret = table.Column<string>(type: "text", nullable: true),
                    CallbackUrl = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalApps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalApps_MyAuthUsers_MyAuthUserId",
                        column: x => x.MyAuthUserId,
                        principalSchema: "MyAuth",
                        principalTable: "MyAuthUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAppsAuthUsers",
                schema: "MyAuth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MyAuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalAppId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAppsAuthUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalAppsAuthUsers_ExternalApps_ExternalAppId",
                        column: x => x.ExternalAppId,
                        principalSchema: "MyAuth",
                        principalTable: "ExternalApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalAppsAuthUsers_MyAuthUsers_MyAuthUserId",
                        column: x => x.MyAuthUserId,
                        principalSchema: "MyAuth",
                        principalTable: "MyAuthUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalApps_CallbackUrl",
                schema: "MyAuth",
                table: "ExternalApps",
                column: "CallbackUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalApps_ClientId",
                schema: "MyAuth",
                table: "ExternalApps",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalApps_ClientSecret",
                schema: "MyAuth",
                table: "ExternalApps",
                column: "ClientSecret",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalApps_MyAuthUserId",
                schema: "MyAuth",
                table: "ExternalApps",
                column: "MyAuthUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAppsAuthUsers_ExternalAppId",
                schema: "MyAuth",
                table: "ExternalAppsAuthUsers",
                column: "ExternalAppId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAppsAuthUsers_MyAuthUserId",
                schema: "MyAuth",
                table: "ExternalAppsAuthUsers",
                column: "MyAuthUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalAppsAuthUsers",
                schema: "MyAuth");

            migrationBuilder.DropTable(
                name: "ExternalApps",
                schema: "MyAuth");

            migrationBuilder.DropTable(
                name: "MyAuthUsers",
                schema: "MyAuth");
        }
    }
}
