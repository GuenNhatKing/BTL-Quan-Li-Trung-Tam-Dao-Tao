using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace QLTTDT.Services
{
    public class StrToSlug
    {
        public static string Convert(string str)
        {
            str = RemoveAccent(str).ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); 
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }
        private static string RemoveAccent(string str)
        {
            var normalizedStr = str.Normalize(NormalizationForm.FormD);
            var strbd = new StringBuilder();

            foreach (var c in normalizedStr)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    // đ, Đ là kí tự đặc biệt không thể phân thành dạng tổ hợp
                    if ((int)c == 273 || (int)c == 272)
                        strbd.Append('d');
                    else strbd.Append(c);
                }
            }
            return strbd.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
