using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vending_Machine_App.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    item_price = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
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
                columns: new[] { "item_id", "item_name", "item_price" },
                values: new object[,]
                {
                    { 1, "Item 1", 2.09m },
                    { 2, "Item 2", 2.19m },
                    { 3, "Item 3", 2.29m },
                    { 4, "Item 4", 2.39m },
                    { 5, "Item 5", 2.49m },
                    { 6, "Item 6", 2.59m },
                    { 7, "Item 7", 2.69m },
                    { 8, "Item 8", 2.79m },
                    { 9, "Item 9", 2.89m },
                    { 10, "Item 10", 2.99m },
                    { 11, "Item 11", 3.09m },
                    { 12, "Item 12", 3.19m },
                    { 13, "Item 13", 3.29m },
                    { 14, "Item 14", 3.39m },
                    { 15, "Item 15", 3.49m },
                    { 16, "Item 16", 3.59m },
                    { 17, "Item 17", 3.69m },
                    { 18, "Item 18", 3.79m },
                    { 19, "Item 19", 3.89m },
                    { 20, "Item 20", 3.99m }
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
