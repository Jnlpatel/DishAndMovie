using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DishAndMovie.Data.Migrations
{
    /// <inheritdoc />
    public partial class addrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Movies_OriginId",
                table: "Movies",
                column: "OriginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Origins_OriginId",
                table: "Movies",
                column: "OriginId",
                principalTable: "Origins",
                principalColumn: "OriginId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Origins_OriginId",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_OriginId",
                table: "Movies");
        }
    }
}
