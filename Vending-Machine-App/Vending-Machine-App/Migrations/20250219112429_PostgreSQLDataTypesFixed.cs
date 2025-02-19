using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vending_Machine_App.Migrations
{
    /// <inheritdoc />
    public partial class PostgreSQLDataTypesFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_purchases_items",
                table: "purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK__purchase__87071CB95534AE18",
                table: "purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK__items__52020FDD8B472EAF",
                table: "items");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "items",
                newName: "category");

            migrationBuilder.AlterColumn<DateTime>(
                name: "purchase_date",
                table: "purchases",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "item_name",
                table: "purchases",
                type: "varchar",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "item_id",
                table: "purchases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "purchase_id",
                table: "purchases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "item_quantity",
                table: "items",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "item_name",
                table: "items",
                type: "varchar",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "category",
                table: "items",
                type: "varchar",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "item_id",
                table: "items",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_purchases",
                table: "purchases",
                column: "purchase_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_items",
                table: "items",
                column: "item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_purchases_items",
                table: "purchases",
                column: "item_id",
                principalTable: "items",
                principalColumn: "item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_purchases_items",
                table: "purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_purchases",
                table: "purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_items",
                table: "items");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "items",
                newName: "Category");

            migrationBuilder.AlterColumn<DateTime>(
                name: "purchase_date",
                table: "purchases",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "item_name",
                table: "purchases",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "item_id",
                table: "purchases",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "purchase_id",
                table: "purchases",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "item_quantity",
                table: "items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "item_name",
                table: "items",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "items",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "item_id",
                table: "items",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK__purchase__87071CB95534AE18",
                table: "purchases",
                column: "purchase_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__items__52020FDD8B472EAF",
                table: "items",
                column: "item_id");

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_items",
                table: "purchases",
                column: "item_id",
                principalTable: "items",
                principalColumn: "item_id");
        }
    }
}
