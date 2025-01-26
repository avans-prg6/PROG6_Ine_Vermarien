using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6_2425.Migrations
{
    public partial class addalltables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adres",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Naam",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoonNummer",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Beestjes",
                columns: table => new
                {
                    BeestjeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AfbeeldingUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beestjes", x => x.BeestjeId);
                });

            migrationBuilder.CreateTable(
                name: "KlantenKaartTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlantenKaartTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Boekingen",
                columns: table => new
                {
                    BoekingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UiteindelijkePrijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KortingPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    BeestjeId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boekingen", x => x.BoekingId);
                    table.ForeignKey(
                        name: "FK_Boekingen_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Boekingen_Beestjes_BeestjeId",
                        column: x => x.BeestjeId,
                        principalTable: "Beestjes",
                        principalColumn: "BeestjeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KlantenKaarten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KlantenKaartTypeId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlantenKaarten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KlantenKaarten_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KlantenKaarten_KlantenKaartTypes_KlantenKaartTypeId",
                        column: x => x.KlantenKaartTypeId,
                        principalTable: "KlantenKaartTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "KlantenKaartTypes",
                columns: new[] { "Id", "Naam" },
                values: new object[,]
                {
                    { 1, "Geen" },
                    { 2, "Zilver" },
                    { 3, "Goud" },
                    { 4, "Platina" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boekingen_AccountId",
                table: "Boekingen",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Boekingen_BeestjeId",
                table: "Boekingen",
                column: "BeestjeId");

            migrationBuilder.CreateIndex(
                name: "IX_KlantenKaarten_AccountId",
                table: "KlantenKaarten",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KlantenKaarten_KlantenKaartTypeId",
                table: "KlantenKaarten",
                column: "KlantenKaartTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boekingen");

            migrationBuilder.DropTable(
                name: "KlantenKaarten");

            migrationBuilder.DropTable(
                name: "Beestjes");

            migrationBuilder.DropTable(
                name: "KlantenKaartTypes");

            migrationBuilder.DropColumn(
                name: "Adres",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Naam",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TelefoonNummer",
                table: "AspNetUsers");
        }
    }
}
