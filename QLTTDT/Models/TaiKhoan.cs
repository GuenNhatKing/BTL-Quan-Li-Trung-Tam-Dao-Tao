using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace QLTTDT.Models;

[Table("TaiKhoan")]
[Index("TenDangNhap", Name = "UQ__TaiKhoan__55F68FC0D1D67C41", IsUnique = true)]
[Index("Salt", Name = "UQ__TaiKhoan__A152BCCEA20E915F", IsUnique = true)]
public partial class TaiKhoan
{
    [Key]
    public int MaTaiKhoan { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaVaiTro { get; set; }

    [StringLength(512)]
    [Unicode(false)]
    public string TenDangNhap { get; set; } = null!;

    [MaxLength(32)]
    public byte[] Salt { get; set; } = null!;

    [StringLength(512)]
    [Unicode(false)]
    public string MatKhau { get; set; } = null!;

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("TaiKhoans")]
    [ValidateNever]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("MaVaiTro")]
    [InverseProperty("TaiKhoans")]
    [ValidateNever]
    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}
