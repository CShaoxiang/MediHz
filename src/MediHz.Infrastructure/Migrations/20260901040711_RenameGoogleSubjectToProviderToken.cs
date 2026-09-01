using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediHz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameGoogleSubjectToProviderToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GoogleSubject",
                table: "Users",
                newName: "ProviderToken");

            migrationBuilder.RenameIndex(
                name: "IX_Users_GoogleSubject",
                table: "Users",
                newName: "IX_Users_ProviderToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderToken",
                table: "Users",
                newName: "GoogleSubject");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ProviderToken",
                table: "Users",
                newName: "IX_Users_GoogleSubject");
        }
    }
}
