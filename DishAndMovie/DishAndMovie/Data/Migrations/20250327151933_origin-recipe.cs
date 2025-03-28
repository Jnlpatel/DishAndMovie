using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DishAndMovie.Data.Migrations
{
    /// <inheritdoc />
    public partial class originrecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginId",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_OriginId",
                table: "Recipes",
                column: "OriginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Origins_OriginId",
                table: "Recipes",
                column: "OriginId",
                principalTable: "Origins",
                principalColumn: "OriginId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Origins_OriginId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_OriginId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "Recipes");
        }
    }
}
