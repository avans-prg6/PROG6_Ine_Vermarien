using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6_2425.Migrations
{
    public partial class addBeestjeBoekingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boekingen_Beestjes_BeestjeId",
                table: "Boekingen");

            migrationBuilder.DropIndex(
                name: "IX_Boekingen_BeestjeId",
                table: "Boekingen");

            migrationBuilder.DropColumn(
                name: "BeestjeId",
                table: "Boekingen");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Boekingen",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Adres",
                table: "Boekingen",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Boekingen",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Naam",
                table: "Boekingen",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefoonnummer",
                table: "Boekingen",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BeestjeBoekingen",
                columns: table => new
                {
                    BeestjeBoekingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeestjeId = table.Column<int>(type: "int", nullable: false),
                    BoekingId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeestjeBoekingen", x => x.BeestjeBoekingId);
                    table.ForeignKey(
                        name: "FK_BeestjeBoekingen_Beestjes_BeestjeId",
                        column: x => x.BeestjeId,
                        principalTable: "Beestjes",
                        principalColumn: "BeestjeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BeestjeBoekingen_Boekingen_BoekingId",
                        column: x => x.BoekingId,
                        principalTable: "Boekingen",
                        principalColumn: "BoekingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeestjeBoekingen_BeestjeId",
                table: "BeestjeBoekingen",
                column: "BeestjeId");

            migrationBuilder.CreateIndex(
                name: "IX_BeestjeBoekingen_BoekingId",
                table: "BeestjeBoekingen",
                column: "BoekingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Boekingen",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BeestjeId",
                table: "Boekingen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Boekingen_BeestjeId",
                table: "Boekingen",
                column: "BeestjeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boekingen_Beestjes_BeestjeId",
                table: "Boekingen",
                column: "BeestjeId",
                principalTable: "Beestjes",
                principalColumn: "BeestjeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
