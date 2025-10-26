using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketTransactionDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTransactions_Tickets_TicketId",
                table: "TicketTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTransactions_Users_UserSourceId",
                table: "TicketTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTransactions_Users_UserTargetId",
                table: "TicketTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTransactions_Tickets_TicketId",
                table: "TicketTransactions",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTransactions_Users_UserSourceId",
                table: "TicketTransactions",
                column: "UserSourceId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTransactions_Users_UserTargetId",
                table: "TicketTransactions",
                column: "UserTargetId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTransactions_Tickets_TicketId",
                table: "TicketTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTransactions_Users_UserSourceId",
                table: "TicketTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTransactions_Users_UserTargetId",
                table: "TicketTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTransactions_Tickets_TicketId",
                table: "TicketTransactions",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTransactions_Users_UserSourceId",
                table: "TicketTransactions",
                column: "UserSourceId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTransactions_Users_UserTargetId",
                table: "TicketTransactions",
                column: "UserTargetId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
