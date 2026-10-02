namespace DuAnEnglish.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class trungtamtienganhEntities : DbContext
    {
        public trungtamtienganhEntities()
            : base("name=trungtamtienganhEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

            modelBuilder.Entity<KhoaHoc>().HasKey(k => k.IDKhoaHoc);
            modelBuilder.Entity<DanhMucKhoaHoc>().HasKey(d => d.IDDanhMuc);
            modelBuilder.Entity<ChuongHoc>().HasKey(c => c.IDChuong);
            modelBuilder.Entity<BaiHoc>().HasKey(b => b.IDBaiHoc);
            modelBuilder.Entity<DangKyKhoaHoc>().HasKey(d => d.IDDangKy);
            modelBuilder.Entity<TienDoHoc>().HasKey(t => t.IDTienDo);

            modelBuilder.Entity<TaiKhoan>().HasKey(t => t.TenDangNhap);
            modelBuilder.Entity<LoaiTaiKhoan>().HasKey(l => l.LoaiTK);
            modelBuilder.Entity<HocVien>().HasKey(h => h.IDHocVien);
            modelBuilder.Entity<GiangVien>().HasKey(g => g.IDGiangVien);
            modelBuilder.Entity<ThanhToan>().HasKey(t => t.IDThanhToan);
            modelBuilder.Entity<GiaoDichVNPAY>().HasKey(g => g.IDGiaoDich);
            modelBuilder.Entity<LopHoc>().HasKey(l => l.IDLopHoc);
            modelBuilder.Entity<PhongHoc>().HasKey(p => p.IDPhongHoc);
            modelBuilder.Entity<ThongBao>().HasKey(t => t.IDThongBao);
            modelBuilder.Entity<ChatBotNoiDung>().HasKey(c => c.MaChat);
            modelBuilder.Entity<HocVienLopHoc>().HasKey(h => new { h.IDHocVien, h.IDLopHoc });
            modelBuilder.Entity<DiemIELT>().HasKey(d => new { d.IDHocVien, d.IDLopHoc });
            modelBuilder.Entity<DiemTOEIC>().HasKey(d => new { d.IDHocVien, d.IDLopHoc });
            modelBuilder.Entity<sysdiagram>().HasKey(s => s.diagram_id);

            modelBuilder.Entity<HocVienLopHoc>()
                .HasOptional(h => h.DiemIELT)
                .WithRequired(d => d.HocVienLopHoc);

            modelBuilder.Entity<HocVienLopHoc>()
                .HasOptional(h => h.DiemTOEIC)
                .WithRequired(d => d.HocVienLopHoc);

            // Foreign Key Mappings
            modelBuilder.Entity<HocVien>()
                .HasOptional(h => h.TaiKhoan)
                .WithMany(t => t.HocViens)
                .HasForeignKey(h => h.IDTenDangNhap);

            modelBuilder.Entity<GiangVien>()
                .HasOptional(g => g.TaiKhoan)
                .WithMany(t => t.GiangViens)
                .HasForeignKey(g => g.IDTenDangNhap);

            modelBuilder.Entity<ThongBao>()
                .HasOptional(t => t.TaiKhoan)
                .WithMany(tk => tk.ThongBaos)
                .HasForeignKey(t => t.IDNguoiGui);

            modelBuilder.Entity<ThanhToan>()
                .HasOptional(t => t.TaiKhoan)
                .WithMany(tk => tk.ThanhToans)
                .HasForeignKey(t => t.TenDangNhap);

            modelBuilder.Entity<KhoaHoc>()
                .HasOptional(k => k.DanhMucKhoaHoc)
                .WithMany(d => d.KhoaHocs)
                .HasForeignKey(k => k.IDDanhMuc);

            modelBuilder.Entity<KhoaHoc>()
                .HasOptional(k => k.GiangVien)
                .WithMany(g => g.KhoaHocs)
                .HasForeignKey(k => k.IDGiangVien);

            modelBuilder.Entity<ChuongHoc>()
                .HasRequired(c => c.KhoaHoc)
                .WithMany(k => k.ChuongHocs)
                .HasForeignKey(c => c.IDKhoaHoc);

            modelBuilder.Entity<BaiHoc>()
                .HasRequired(b => b.ChuongHoc)
                .WithMany(c => c.BaiHocs)
                .HasForeignKey(b => b.IDChuong);

            modelBuilder.Entity<DangKyKhoaHoc>()
                .HasRequired(d => d.HocVien)
                .WithMany(h => h.DangKyKhoaHocs)
                .HasForeignKey(d => d.IDHocVien);

            modelBuilder.Entity<DangKyKhoaHoc>()
                .HasRequired(d => d.KhoaHoc)
                .WithMany(k => k.DangKyKhoaHocs)
                .HasForeignKey(d => d.IDKhoaHoc);

            modelBuilder.Entity<TienDoHoc>()
                .HasRequired(t => t.HocVien)
                .WithMany(h => h.TienDoHocs)
                .HasForeignKey(t => t.IDHocVien);

            modelBuilder.Entity<TienDoHoc>()
                .HasRequired(t => t.BaiHoc)
                .WithMany(b => b.TienDoHocs)
                .HasForeignKey(t => t.IDBaiHoc);

            modelBuilder.Entity<TaiKhoan>()
                .HasOptional(t => t.LoaiTaiKhoan)
                .WithMany(l => l.TaiKhoans)
                .HasForeignKey(t => t.LoaiTK);

            modelBuilder.Entity<ThanhToan>()
                .HasOptional(t => t.KhoaHoc)
                .WithMany(k => k.ThanhToans)
                .HasForeignKey(t => t.IDKhoaHoc);

            modelBuilder.Entity<ThanhToan>()
                .HasOptional(t => t.DangKyKhoaHoc)
                .WithMany(d => d.ThanhToans)
                .HasForeignKey(t => t.IDDangKy);
        }
    
        public virtual DbSet<ChatBotNoiDung> ChatBotNoiDungs { get; set; }
        public virtual DbSet<DiemIELT> DiemIELTS { get; set; }
        public virtual DbSet<DiemTOEIC> DiemTOEICs { get; set; }
        public virtual DbSet<GiangVien> GiangViens { get; set; }
        public virtual DbSet<GiaoDichVNPAY> GiaoDichVNPAYs { get; set; }
        public virtual DbSet<HocVien> HocViens { get; set; }
        public virtual DbSet<HocVienLopHoc> HocVienLopHocs { get; set; }
        public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }
        public virtual DbSet<LoaiTaiKhoan> LoaiTaiKhoans { get; set; }
        public virtual DbSet<LopHoc> LopHocs { get; set; }
        public virtual DbSet<PhongHoc> PhongHocs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
        public virtual DbSet<ThanhToan> ThanhToans { get; set; }
        public virtual DbSet<ThongBao> ThongBaos { get; set; }
        public virtual DbSet<DanhMucKhoaHoc> DanhMucKhoaHocs { get; set; }
        public virtual DbSet<ChuongHoc> ChuongHocs { get; set; }
        public virtual DbSet<BaiHoc> BaiHocs { get; set; }
        public virtual DbSet<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }
        public virtual DbSet<TienDoHoc> TienDoHocs { get; set; }
    }
}
