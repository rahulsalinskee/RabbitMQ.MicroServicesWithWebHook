using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop the primary key constraint first
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            // 2. Drop the existing ID column
            migrationBuilder.DropColumn(
                name: "ID",
                table: "Products");

            // 3. Recreate the ID column WITHOUT the Identity property
            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 4. Re-establish the Primary Key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the process if you ever need to rollback
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1"); // Adds Identity back

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "ID");
        }
    }
}
