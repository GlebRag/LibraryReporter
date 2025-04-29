using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUserDataIdFromUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_UserDataId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_UserDataId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "UserDataId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_UserDataId",
                table: "Books",
                column: "UserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_UserDataId",
                table: "Books",
                column: "UserDataId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
