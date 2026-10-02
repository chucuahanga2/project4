namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ThanhToan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ThanhToan()
        {
            this.GiaoDichVNPAYs = new HashSet<GiaoDichVNPAY>();
        }
    
        public int IDThanhToan { get; set; }
        public string IDLopHoc { get; set; }
        public string TenDangNhap { get; set; }
        public string IDKhoaHoc { get; set; }
        public Nullable<decimal> SoTien { get; set; }
        public string PhuongThucTT { get; set; }
        public Nullable<System.DateTime> NgayThanhToan { get; set; }
        public string TrangThai { get; set; }
        public Nullable<System.DateTime> NgayXacNhan { get; set; }
        public Nullable<int> IDDangKy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoDichVNPAY> GiaoDichVNPAYs { get; set; }
        public virtual KhoaHoc KhoaHoc { get; set; }
        public virtual LopHoc LopHoc { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; }
        public virtual DangKyKhoaHoc DangKyKhoaHoc { get; set; }
    }
}
