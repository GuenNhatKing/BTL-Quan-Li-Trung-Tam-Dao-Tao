using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace QLTTDT.Models;

[Table("KhoaHoc")]
public partial class KhoaHoc
{
    [Key]
    public int MaKhoaHoc { get; set; }

    public int MaGiangVien { get; set; }

    public int MaChuDe { get; set; }

    public int MaCapDo { get; set; }

    [StringLength(1024)]
    [Unicode(false)]
    public string? UrlAnh { get; set; }

    [StringLength(512)]
    public string TenKhoaHoc { get; set; } = null!;

    [StringLength(2048)]
    public string MoTa { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ThoiGianKhaiGiang { get; set; }
    public string? ThoiGianCompute { get; set; }

    public int HocPhi { get; set; }

    public int SoLuongHocVienToiDa { get; set; }

    [InverseProperty("MaKhoaHocNavigation")]
    public virtual ICollection<DangKiKhoaHoc> DangKiKhoaHocs { get; set; } = new List<DangKiKhoaHoc>();

    [ForeignKey("MaCapDo")]
    [InverseProperty("KhoaHocs")]
    [ValidateNever]
    public virtual CapDo MaCapDoNavigation { get; set; } = null!;

    [ForeignKey("MaChuDe")]
    [InverseProperty("KhoaHocs")]
    [ValidateNever]
    public virtual ChuDe MaChuDeNavigation { get; set; } = null!;

    [ForeignKey("MaGiangVien")]
    [InverseProperty("KhoaHocs")]
    [ValidateNever]
    public virtual NguoiDung MaGiangVienNavigation { get; set; } = null!;
}
