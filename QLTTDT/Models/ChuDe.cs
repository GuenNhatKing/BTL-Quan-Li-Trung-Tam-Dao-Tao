using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTTDT.Models;

[Table("ChuDe")]
public partial class ChuDe
{
    [Key]
    public int MaChuDe { get; set; }

    [StringLength(512)]
    public string TenChuDe { get; set; } = null!;

    [StringLength(2048)]
    public string MoTa { get; set; } = null!;

    [InverseProperty("MaChuDeNavigation")]
    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
}
