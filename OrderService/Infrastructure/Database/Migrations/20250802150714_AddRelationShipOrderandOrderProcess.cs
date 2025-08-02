using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationShipOrderandOrderProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_Name",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId_Name",
                table: "Orders",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcesses_Orders_OrderId",
                table: "OrderProcesses",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcesses_Orders_OrderId",
                table: "OrderProcesses");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId_Name",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Name",
                table: "Orders",
                column: "Name");
        }
    }
}
