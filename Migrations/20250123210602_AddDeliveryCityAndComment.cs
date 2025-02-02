using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hazinDNS_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryCityAndComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryCity",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Wishlist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlist_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_ProductId",
                table: "Wishlist",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wishlist");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryCity",
                table: "Orders");
        }
    }
}
