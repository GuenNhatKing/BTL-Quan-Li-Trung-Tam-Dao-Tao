namespace QLTTDT.ViewModels
{
    public class Statistics
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = null!;
        public DateOnly ThoiGian { get; set; }
        public string? ThoiGianStr { get; set; }
        public int? SoHocVienDangKi { get; set; }
        public int? DoanhThu { get; set; }
    }
}
