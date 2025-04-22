using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTTDT.Models;

[Table("NguoiDung")]
[Index("SoDienThoai", Name = "UQ__NguoiDun__0389B7BDB0A369DA", IsUnique = true)]
[Index("Email", Name = "UQ__NguoiDun__A9D10534C87B5570", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    public int MaNguoiDung { get; set; }

    [StringLength(1024)]
    [Unicode(false)]
    public string? UrlAnhDaiDien { get; set; }

    [StringLength(512)]
    public string HoVaTen { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }
    public string? ThoiGianCompute { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string SoDienThoai { get; set; } = null!;

    [StringLength(512)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [InverseProperty("MaHocVienNavigation")]
    public virtual ICollection<DangKiKhoaHoc> DangKiKhoaHocs { get; set; } = new List<DangKiKhoaHoc>();

    [InverseProperty("MaGiangVienNavigation")]
    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
