using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBankingApp.Migrations
{
    public partial class AddedEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Auth",
                table: "Logins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Period",
                table: "BillPays",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth",
                table: "Logins");

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                table: "BillPays",
                type: "nvarchar(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
