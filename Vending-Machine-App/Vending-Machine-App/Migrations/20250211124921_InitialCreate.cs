using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vending_Machine_App.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    item_name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    item_price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    item_quantity = table.Column<int>(type: "int", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__items__52020FDD8B472EAF", x => x.item_id);
                });

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    purchase_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    item_name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    amount_paid = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    change = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    purchase_date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__purchase__87071CB95534AE18", x => x.purchase_id);
                    table.ForeignKey(
                        name: "FK_purchases_items",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "item_id");
                });

            migrationBuilder.InsertData(
                table: "items",
                columns: new[] { "item_id", "Category", "item_name", "item_price", "item_quantity" },
                values: new object[,]
                {
                    { 1, "Drink", "Sprite", 20.00m, 10 },
                    { 2, "Drink", "Coke", 5.00m, 10 },
                    { 3, "Drink", "Water", 20.00m, 10 },
                    { 4, "Snack", "Oreo", 5.00m, 1 },
                    { 5, "Snack", "Chips", 20.00m, 10 },
                    { 6, "Drink", "Twist", 20.00m, 10 },
                    { 7, "Drink", "Pepsi", 10.00m, 10 },
                    { 8, "Drink", "Stoney", 10.00m, 10 },
                    { 9, "Snack", "BarOne", 5.00m, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_purchases_item_id",
                table: "purchases",
                column: "item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchases");

            migrationBuilder.DropTable(
                name: "items");
        }
    }
}
