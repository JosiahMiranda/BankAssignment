using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McbaExampleWithLogin.Migrations
{
    public partial class RemoveTransactionTypeFromBillPay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "BillPays");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "BillPays",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
