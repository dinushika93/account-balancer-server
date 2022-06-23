using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountBalancer.Migrations
{
    public partial class AddAccountBalanceToDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountBalances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RDBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CanteenBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CarBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MarketingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParkingFinesBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBalances", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBalances");
        }
    }
}
