using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLTTDT.Migrations
{
    /// <inheritdoc />
    public partial class Initialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CapDo",
                columns: table => new
                {
                    MaCapDo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCapDo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CapDo__40B881FC34CD938B", x => x.MaCapDo);
                });

            migrationBuilder.CreateTable(
                name: "ChuDe",
                columns: table => new
                {
                    MaChuDe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuDe = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    UrlAnhChuDe = table.Column<string>(type: "varchar(1024)", unicode: false, maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChuDe__358545113217E9F1", x => x.MaChuDe);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlAnhDaiDien = table.Column<string>(type: "varchar(1024)", unicode: false, maxLength: 1024, nullable: true),
                    HoVaTen = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: false),
                    ThoiGianCompute = table.Column<string>(type: "varchar(10)", nullable: true, computedColumnSql: "CONVERT(varchar(10), [NgaySinh], 103)", stored: true),
                    SoDienThoai = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiDun__C539D762DEB4CD35", x => x.MaNguoiDung);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VaiTro__C24C41CFAB44AE39", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "KhoaHoc",
                columns: table => new
                {
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGiangVien = table.Column<int>(type: "int", nullable: false),
                    MaChuDe = table.Column<int>(type: "int", nullable: false),
                    MaCapDo = table.Column<int>(type: "int", nullable: false),
                    UrlAnh = table.Column<string>(type: "varchar(1024)", unicode: false, maxLength: 1024, nullable: true),
                    TenKhoaHoc = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ThoiGianKhaiGiang = table.Column<DateTime>(type: "datetime", nullable: false),
                    ThoiGianCompute = table.Column<string>(type: "varchar(19)", nullable: true, computedColumnSql: "CONVERT(varchar(8), [ThoiGianKhaiGiang], 108) + ' ' + CONVERT(varchar(10), [ThoiGianKhaiGiang], 103)", stored: true),
                    HocPhi = table.Column<int>(type: "int", nullable: false),
                    SoLuongHocVienToiDa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhoaHoc__48F0FF9810CE4F86", x => x.MaKhoaHoc);
                    table.ForeignKey(
                        name: "FK_KhoaHoc_CapDo",
                        column: x => x.MaCapDo,
                        principalTable: "CapDo",
                        principalColumn: "MaCapDo");
                    table.ForeignKey(
                        name: "FK_KhoaHoc_ChuDe",
                        column: x => x.MaChuDe,
                        principalTable: "ChuDe",
                        principalColumn: "MaChuDe");
                    table.ForeignKey(
                        name: "FK_KhoaHoc_NguoiDung",
                        column: x => x.MaGiangVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    TenDangNhap = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TaiKhoan__AD7C65292DD8A25F", x => x.MaTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TaiKhoan_NguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_TaiKhoan_VaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro");
                });

            migrationBuilder.CreateTable(
                name: "DangKiKhoaHoc",
                columns: table => new
                {
                    MaDangKi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: false),
                    ThoiGianDangKi = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ThoiGianCompute = table.Column<string>(type: "varchar(19)", nullable: true, computedColumnSql: "CONVERT(varchar(8), [ThoiGianDangKi], 108) + ' ' + CONVERT(varchar(10), [ThoiGianDangKi], 103)", stored: true),
                    HocPhi = table.Column<int>(type: "int", nullable: false),
                    TienDo = table.Column<int>(type: "int", nullable: false),
                    DaHuy = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DangKiKh__BA90F03DA183D1E4", x => x.MaDangKi);
                    table.ForeignKey(
                        name: "FK_DangKiKhoaHoc_KhoaHoc",
                        column: x => x.MaKhoaHoc,
                        principalTable: "KhoaHoc",
                        principalColumn: "MaKhoaHoc");
                    table.ForeignKey(
                        name: "FK_DangKiKhoaHoc_NguoiDung",
                        column: x => x.MaHocVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DangKiKhoaHoc_MaHocVien",
                table: "DangKiKhoaHoc",
                column: "MaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_DangKiKhoaHoc_MaKhoaHoc",
                table: "DangKiKhoaHoc",
                column: "MaKhoaHoc");

            migrationBuilder.CreateIndex(
                name: "IX_KhoaHoc_MaCapDo",
                table: "KhoaHoc",
                column: "MaCapDo");

            migrationBuilder.CreateIndex(
                name: "IX_KhoaHoc_MaChuDe",
                table: "KhoaHoc",
                column: "MaChuDe");

            migrationBuilder.CreateIndex(
                name: "IX_KhoaHoc_MaGiangVien",
                table: "KhoaHoc",
                column: "MaGiangVien");

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__0389B7BDB0A369DA",
                table: "NguoiDung",
                column: "SoDienThoai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__A9D10534C87B5570",
                table: "NguoiDung",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_MaNguoiDung",
                table: "TaiKhoan",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_MaVaiTro",
                table: "TaiKhoan",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "UQ__TaiKhoan__55F68FC0D1D67C41",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__TaiKhoan__A152BCCEA20E915F",
                table: "TaiKhoan",
                column: "Salt",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DangKiKhoaHoc");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "KhoaHoc");

            migrationBuilder.DropTable(
                name: "VaiTro");

            migrationBuilder.DropTable(
                name: "CapDo");

            migrationBuilder.DropTable(
                name: "ChuDe");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
