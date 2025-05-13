using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EcomerceMVC.Data;

public partial class DbecomerceContext : DbContext
{
    public DbecomerceContext()
    {
    }

    public DbecomerceContext(DbContextOptions<DbecomerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietHd> ChiTietHds { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HangHoa> HangHoas { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=Admin\\SQLEXPRESS;Initial Catalog=DBEcomerce;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietHd>(entity =>
        {
            entity.HasKey(e => e.MaCt).HasName("PK__ChiTietH__27258E745E56F52B");

            entity.ToTable("ChiTietHD");

            entity.Property(e => e.MaCt).HasColumnName("MaCT");
            entity.Property(e => e.DonGia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaHh).HasColumnName("MaHH");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHds)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHD__MaHD__59063A47");

            entity.HasOne(d => d.MaHhNavigation).WithMany(p => p.ChiTietHds)
                .HasForeignKey(d => d.MaHh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHD__MaHH__59FA5E80");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGh).HasName("PK__GioHang__2725AE8585FE630C");

            entity.ToTable("GioHang");

            entity.Property(e => e.MaGh).HasColumnName("MaGH");
            entity.Property(e => e.MaHh).HasColumnName("MaHH");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");

            entity.HasOne(d => d.MaHhNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaHh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaHH__5DCAEF64");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaKH__5CD6CB2B");
        });

        modelBuilder.Entity<HangHoa>(entity =>
        {
            entity.HasKey(e => e.MaHh).HasName("PK__HangHoa__2725A6E4FDCBDFAA");

            entity.ToTable("HangHoa");

            entity.Property(e => e.MaHh).HasColumnName("MaHH");
            entity.Property(e => e.DonGia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Hinh).HasMaxLength(255);
            entity.Property(e => e.MoTa).HasColumnType("ntext");
            entity.Property(e => e.TenHh)
                .HasMaxLength(255)
                .HasColumnName("TenHH");

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.HangHoas)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HangHoa__MaLoai__4BAC3F29");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E054C3F4A7");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.CachThanhToan).HasMaxLength(50);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("SDT");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xử lý");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaKH__5629CD9C");
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.Maloai).HasName("PK__Loai__3E1DB46D7753B0D8");

            entity.ToTable("Loai");

            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__User__2725CF1E0C36E067");

            entity.ToTable("User");

            entity.HasIndex(e => e.TaiKhoan, "UQ__User__D5B8C7F01C612DEB").IsUnique();

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.GgId)
                .HasMaxLength(255)
                .HasColumnName("GG_ID");
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("SDT");
            entity.Property(e => e.TaiKhoan).HasMaxLength(100);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .HasDefaultValue("KhachHang");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
