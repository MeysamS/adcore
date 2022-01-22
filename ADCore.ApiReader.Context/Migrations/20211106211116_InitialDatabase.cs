using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ADCore.ApiReader.Context.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketName = table.Column<string>(nullable: true),
                    CoinMarketId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Trading = table.Column<bool>(nullable: false),
                    Etf = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoinPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CoinId = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Created_At = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoinPrices_Coins_CoinId",
                        column: x => x.CoinId,
                        principalTable: "Coins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoinPrices_CoinId",
                table: "CoinPrices",
                column: "CoinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoinPrices");

            migrationBuilder.DropTable(
                name: "Coins");
        }
    }
}
