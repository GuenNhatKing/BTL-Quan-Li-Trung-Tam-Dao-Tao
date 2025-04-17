using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLTTDT.Migrations
{
    /// <inheritdoc />
    public partial class addTienDoDKKH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TienDo",
                table: "DangKiKhoaHoc",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TienDo",
                table: "DangKiKhoaHoc");
        }
    }
}
