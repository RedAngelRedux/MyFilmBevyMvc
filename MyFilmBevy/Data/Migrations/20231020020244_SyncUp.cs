using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFilmBevy.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviesCollection_Collection_CollectionId",
                table: "MoviesCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_MoviesCollection_Movies_MovieId",
                table: "MoviesCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoviesCollection",
                table: "MoviesCollection");

            migrationBuilder.RenameTable(
                name: "MoviesCollection",
                newName: "MovieCollection");

            migrationBuilder.RenameIndex(
                name: "IX_MoviesCollection_MovieId",
                table: "MovieCollection",
                newName: "IX_MovieCollection_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MoviesCollection_CollectionId",
                table: "MovieCollection",
                newName: "IX_MovieCollection_CollectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieCollection",
                table: "MovieCollection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCollection_Collection_CollectionId",
                table: "MovieCollection",
                column: "CollectionId",
                principalTable: "Collection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCollection_Movies_MovieId",
                table: "MovieCollection",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieCollection_Collection_CollectionId",
                table: "MovieCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCollection_Movies_MovieId",
                table: "MovieCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieCollection",
                table: "MovieCollection");

            migrationBuilder.RenameTable(
                name: "MovieCollection",
                newName: "MoviesCollection");

            migrationBuilder.RenameIndex(
                name: "IX_MovieCollection_MovieId",
                table: "MoviesCollection",
                newName: "IX_MoviesCollection_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieCollection_CollectionId",
                table: "MoviesCollection",
                newName: "IX_MoviesCollection_CollectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoviesCollection",
                table: "MoviesCollection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviesCollection_Collection_CollectionId",
                table: "MoviesCollection",
                column: "CollectionId",
                principalTable: "Collection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoviesCollection_Movies_MovieId",
                table: "MoviesCollection",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
