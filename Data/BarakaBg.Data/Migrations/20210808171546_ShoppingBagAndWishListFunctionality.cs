using Microsoft.EntityFrameworkCore.Migrations;

namespace BarakaBg.Data.Migrations
{
    public partial class ShoppingBagAndWishListFunctionality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ShoppingBagId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ShoppingBags",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBags_UserId",
                table: "ShoppingBags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ShoppingBagId",
                table: "AspNetUsers",
                column: "ShoppingBagId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingBags_AspNetUsers_UserId",
                table: "ShoppingBags",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingBags_AspNetUsers_UserId",
                table: "ShoppingBags");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingBags_UserId",
                table: "ShoppingBags");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ShoppingBagId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShoppingBags");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ShoppingBagId",
                table: "AspNetUsers",
                column: "ShoppingBagId",
                unique: true,
                filter: "[ShoppingBagId] IS NOT NULL");
        }
    }
}
