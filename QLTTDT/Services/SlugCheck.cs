using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;

namespace QLTTDT.Services
{
    public class SlugCheck
    {
        private QLTTDTDbContext _context;
        public SlugCheck(QLTTDTDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckTopicCourseSlug(string? topicSlug, int? topicId, string? courseSlug, int? courseId)
        {
            return (await CheckCourseSlug(courseSlug, courseId)) && (await CheckTopicSlug(topicSlug, topicId));
        }
        public async Task<bool> CheckTopicSlug(string? topicSlug, int? id)
        {
            if(topicSlug == null || id == null)
            {
                return false;
            }
            else
            {
                string slugFirst = topicSlug + id;
                string? topicName = (await _context.ChuDes.FirstOrDefaultAsync(i => i.MaChuDe == id))?.TenChuDe;
                if (topicName == null) return false;
                string slugSecond = StrToSlug.Convert(topicName) + id;
                return slugFirst == slugSecond;
            }
        }
        public async Task<bool> CheckCourseSlug(string? courseSlug, int? id)
        {
            if(courseSlug == null || id == null)
            {
                return false;
            }
            else
            {
                string slugFirst = courseSlug + id;
                string? courseName = (await _context.KhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == id))?.TenKhoaHoc;
                if (courseName == null) return false;
                string slugSecond = StrToSlug.Convert(courseName) + id;
                return slugFirst == slugSecond;
            }
        }
    }
}
