using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntDorSys.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "user_info",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "user_info");
        }
    }
}
