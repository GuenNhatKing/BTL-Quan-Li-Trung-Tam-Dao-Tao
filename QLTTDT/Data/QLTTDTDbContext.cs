using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Models;

namespace QLTTDT.Data;

public partial class QLTTDTDbContext : DbContext
{
    public QLTTDTDbContext(DbContextOptions<QLTTDTDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CapDo> CapDos { get; set; }

    public virtual DbSet<ChuDe> ChuDes { get; set; }

    public virtual DbSet<DangKiKhoaHoc> DangKiKhoaHocs { get; set; }

    public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CapDo>(entity =>
        {
            entity.HasKey(e => e.MaCapDo).HasName("PK__CapDo__40B881FC34CD938B");
        });

        modelBuilder.Entity<ChuDe>(entity =>
        {
            entity.HasKey(e => e.MaChuDe).HasName("PK__ChuDe__358545113217E9F1");
        });

        modelBuilder.Entity<DangKiKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.MaDangKi).HasName("PK__DangKiKh__BA90F03DA183D1E4");

            entity.Property(e => e.DaHuy).HasDefaultValue(false);
            entity.Property(e => e.ThoiGianDangKi).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.DangKiKhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DangKiKhoaHoc_NguoiDung");

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.DangKiKhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DangKiKhoaHoc_KhoaHoc");
            entity.HasQueryFilter(i => i.DaHuy == null || i.DaHuy == false);
        });

        modelBuilder.Entity<KhoaHoc>(entity =>
        {
            entity.HasKey(e => e.MaKhoaHoc).HasName("PK__KhoaHoc__48F0FF9810CE4F86");

            entity.HasOne(d => d.MaCapDoNavigation).WithMany(p => p.KhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhoaHoc_CapDo");

            entity.HasOne(d => d.MaChuDeNavigation).WithMany(p => p.KhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhoaHoc_ChuDe");

            entity.HasOne(d => d.MaGiangVienNavigation).WithMany(p => p.KhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhoaHoc_NguoiDung");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762DEB4CD35");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C65292DD8A25F");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.TaiKhoans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_NguoiDung");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.TaiKhoans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_VaiTro");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CFAB44AE39");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
