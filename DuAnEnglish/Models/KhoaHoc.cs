namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class KhoaHoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhoaHoc()
        {
            this.LopHocs = new HashSet<LopHoc>();
            this.ThanhToans = new HashSet<ThanhToan>();
            this.ChuongHocs = new HashSet<ChuongHoc>();
            this.DangKyKhoaHocs = new HashSet<DangKyKhoaHoc>();
        }
    
        public string IDKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string DanhMuc { get; set; }
        public string MoTa { get; set; }
        public Nullable<decimal> HocPhi { get; set; }
        public string HinhAnhKH { get; set; }
        public Nullable<int> IDDanhMuc { get; set; }
        public Nullable<int> IDGiangVien { get; set; }
        public string NoiDung { get; set; }
        public string TrangThai { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LopHoc> LopHocs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanhToan> ThanhToans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChuongHoc> ChuongHocs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }
        public virtual DanhMucKhoaHoc DanhMucKhoaHoc { get; set; }
        public virtual GiangVien GiangVien { get; set; }
    }
}
