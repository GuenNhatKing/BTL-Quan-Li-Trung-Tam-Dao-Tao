using QLTTDT.Models;

namespace QLTTDT.ViewModels
{
    public class TopicDescription
    {
        public int MaChuDe { get; set; }
        public string TenChuDe { get; set; } = null!;
        public string? UrlAnhChuDe { get; set; }
        public string MoTa { get; set; } = null!;
        public int SoKhoaHocDaDangKi { get; set; }
        public List<CourseCard> DanhSachKhoaHoc { get; set; } = new List<CourseCard>();
    }
}
