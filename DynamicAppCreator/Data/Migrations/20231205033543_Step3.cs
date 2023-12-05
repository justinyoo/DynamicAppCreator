using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicAppCreator.Migrations
{
    /// <inheritdoc />
    public partial class Step3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Table",
                table: "Modules",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Table",
                table: "Modules");
        }
    }
}
