using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McbaExampleWithLogin.Migrations
{
    public partial class ChangePayeeProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mobile",
                table: "Payees",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Payees",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Payees");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Payees",
                newName: "Mobile");
        }
    }
}
