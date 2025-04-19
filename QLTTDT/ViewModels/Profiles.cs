namespace QLTTDT.ViewModels
{
    public class Profiles
    {
        public int MaNguoiDung { get; set; }
        public string? UrlAnhDaiDien { get; set; }
        public string HoVaTen { get; set; } = null!;
        public DateOnly NgaySinh { get; set; }
        public string SoDienThoai { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int SoKhoaHocDaDangKi { get; set; }
        public int SoKhoaHocDangHoc { get; set; }
        public int SoKhoaHocDaHoanThanh { get; set; }
    }
}
