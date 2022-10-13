using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rpg_combat.Migrations
{
    public partial class AddLifeLogToCharacter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LifeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Log = table.Column<string>(type: "TEXT", nullable: true),
                    HappenedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsBattleLog = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVictory = table.Column<bool>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LifeLogs_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifeLogs_CharacterId",
                table: "LifeLogs",
                column: "CharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifeLogs");
        }
    }
}
