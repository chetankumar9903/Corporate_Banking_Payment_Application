using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corporate_Banking_Payment_Application.Migrations
{
    /// <inheritdoc />
    public partial class initial4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Beneficiaries");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "Beneficiaries",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_AccountNumber",
                table: "Beneficiaries",
                column: "AccountNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Beneficiaries_AccountNumber",
                table: "Beneficiaries");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "Beneficiaries",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Beneficiaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
