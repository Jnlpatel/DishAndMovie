using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DishAndMovie.Data.Migrations
{
    /// <inheritdoc />
    public partial class addRecipexIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecipesXIngredients",
                columns: table => new
                {
                    RecipexIngredientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipesXIngredients", x => x.RecipexIngredientId);
                    table.ForeignKey(
                        name: "FK_RecipesXIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "IngredientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipesXIngredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipesXIngredients_IngredientId",
                table: "RecipesXIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipesXIngredients_RecipeId",
                table: "RecipesXIngredients",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipesXIngredients");
        }
    }
}
