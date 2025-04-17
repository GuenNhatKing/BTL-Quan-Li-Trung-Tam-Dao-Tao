using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTTDT.Models;

[Table("DangKiKhoaHoc")]
public partial class DangKiKhoaHoc
{
    [Key]
    public int MaDangKi { get; set; }

    public int MaHocVien { get; set; }

    public int MaKhoaHoc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianDangKi { get; set; }
    public int TienDo { get; set; }

    public bool? DaHuy { get; set; }

    [ForeignKey("MaHocVien")]
    [InverseProperty("DangKiKhoaHocs")]
    public virtual NguoiDung MaHocVienNavigation { get; set; } = null!;

    [ForeignKey("MaKhoaHoc")]
    [InverseProperty("DangKiKhoaHocs")]
    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}
