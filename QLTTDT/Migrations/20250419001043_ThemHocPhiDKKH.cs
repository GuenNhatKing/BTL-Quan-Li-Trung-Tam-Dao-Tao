using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLTTDT.Migrations
{
    /// <inheritdoc />
    public partial class ThemHocPhiDKKH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HocPhi",
                table: "DangKiKhoaHoc",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HocPhi",
                table: "DangKiKhoaHoc");
        }
    }
}
