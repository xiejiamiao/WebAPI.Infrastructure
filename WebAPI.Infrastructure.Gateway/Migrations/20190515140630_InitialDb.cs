using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Infrastructure.Gateway.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrderNo = table.Column<string>(nullable: true),
                    ReciverName = table.Column<string>(nullable: true),
                    ReciverMobile = table.Column<string>(nullable: true),
                    ReciverProvince = table.Column<string>(nullable: true),
                    ReciverCity = table.Column<string>(nullable: true),
                    ReciverDistrict = table.Column<string>(nullable: true),
                    ReciverDetailAddress = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    ShippingAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
