using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corporate_Banking_Payment_Application.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceReportSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Reports",
                type: "nvarchar(350)",
                maxLength: 350,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Reports",
                type: "int",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "OutputFormat",
            //    table: "Reports",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ClientId",
                table: "Reports",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Clients_ClientId",
                table: "Reports",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Clients_ClientId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ClientId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "OutputFormat",
                table: "Reports");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Reports",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(350)",
                oldMaxLength: 350);
        }
    }
}
