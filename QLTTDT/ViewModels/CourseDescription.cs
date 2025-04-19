namespace QLTTDT.ViewModels
{
    public class CourseDescription
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string MoTa { get; set; }
        public int MaChuDe { get; set; }
        public string TenChuDe { get; set; } 
        public int HocPhi { get; set; }
        public int MaCapDo { get; set; }
        public string TenCapDo { get; set; }
        public int MaGiangVien { get; set; }
        public string TenGiangVien { get; set; }
        public string? UrlAnh { get; set; }
        public DateTime ThoiGianKhaiGiang { get; set; }
        public int SoLuongHocVienDangKi { get; set; }
        public int SoLuongHocVienToiDa { get; set; }
        public bool DaDangKi { get; set; }
        public int TienDo { get; set; }
    }
}
