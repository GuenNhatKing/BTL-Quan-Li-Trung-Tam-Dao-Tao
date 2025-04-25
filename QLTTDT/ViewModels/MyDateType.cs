using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QLTTDT.ViewModels
{
    public enum DateTypes
    {
        DATE,   // dd/MM/yyyy
        MONTH,  // MM/yyyy
        YEAR    // yyyy
    }
    [ModelBinder(BinderType = typeof(MyDateTypeBinder))]
    public class MyDateType
    {
        public DateTypes Type { get; set; } = DateTypes.DATE;
        public int Day { get; set; } = 1;
        public int Month { get; set; } = 1;
        public int Year { get; set; } = 1;
        public MyDateType() { }
        public MyDateType(string str)
        {
            // str: yyyy-MM-dd
            var parts = str.Split('-');
            if (parts.Length == 3)
            {
                Year = Convert.ToInt32(parts[0]);
                Month = Convert.ToInt32(parts[1]);
                Day = Convert.ToInt32(parts[2]);
                Type = DateTypes.DATE;
            }
            else if (parts.Length == 2)
            {
                Year = Convert.ToInt32(parts[0]);
                Month = Convert.ToInt32(parts[1]);
                Day = 1;
                Type = DateTypes.MONTH;
            }
            else
            {
                Year = Convert.ToInt32(parts[0]);
                Month = Day = 1;
                Type = DateTypes.YEAR;
            }
            // Thêm vào để kiểm tra dữ liệu, nếu không hợp lệ sẽ nhận exception
            var date = new DateOnly(Year, Month, Day);
        }
        public override string ToString()
        {
            if (Type == DateTypes.DATE)
            {
                return string.Format($"{Day}/{Month}/{Year}");
            }
            if (Type == DateTypes.MONTH)
            {
                return string.Format($"{Month}/{Year}");
            }
            if (Type == DateTypes.YEAR)
            {
                return string.Format($"{Year}");
            }
            return "";
        }
        public static explicit operator DateTime(MyDateType mdt)
        {
            return new DateTime(mdt.Year, mdt.Month, mdt.Day);
        }
    }
}
