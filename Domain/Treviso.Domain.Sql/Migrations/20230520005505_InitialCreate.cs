using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Treviso.Domain.Sql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Badged = table.Column<int>(type: "int", nullable: false),
                    Bws = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RangeLower = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RangeUpper = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TeamSize = table.Column<int>(type: "int", nullable: false),
                    Vs = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    User = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Round = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Date = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Time = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Team1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CaptainDiscord1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Team2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CaptainDiscord2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Referee = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RefereeDiscord = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Streamer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Commentator1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Commentator2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PingSent = table.Column<int>(type: "int", nullable: false),
                    MatchFinished = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchToTournament",
                        column: x => x.TournamentId,
                        principalTable: "tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sheets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Main = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ref = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RefType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Pool = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Admin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    User = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheetToTournament",
                        column: x => x.TournamentId,
                        principalTable: "tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_TournamentId",
                table: "matches",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_sheets_TournamentId",
                table: "sheets",
                column: "TournamentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "sheets");

            migrationBuilder.DropTable(
                name: "tournaments");
        }
    }
}
