using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QLTTDT.Models;

[Table("CapDo")]
public partial class CapDo
{
    [Key]
    public int MaCapDo { get; set; }

    [StringLength(512)]
    public string TenCapDo { get; set; } = null!;

    [InverseProperty("MaCapDoNavigation")]
    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
}
