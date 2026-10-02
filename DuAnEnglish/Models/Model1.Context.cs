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
            throw new UnintentionalCodeFirstException();
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
        public virtual DbSet<DiemKhoaHoc> DiemKhoaHocs { get; set; }
    }
}
