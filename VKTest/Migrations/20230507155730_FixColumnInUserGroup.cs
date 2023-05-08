using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VKTest.Migrations
{
    /// <inheritdoc />
    public partial class FixColumnInUserGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Decription",
                table: "UserGroups",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "UserGroups",
                newName: "Decription");
        }
    }
}
