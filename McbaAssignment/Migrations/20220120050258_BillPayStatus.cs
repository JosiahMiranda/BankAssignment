using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McbaExampleWithLogin.Migrations
{
    public partial class BillPayStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillPayStatus",
                table: "BillPays",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillPayStatus",
                table: "BillPays");
        }
    }
}
