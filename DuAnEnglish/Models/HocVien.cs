namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HocVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HocVien()
        {
            this.HocVienLopHocs = new HashSet<HocVienLopHoc>();
            this.DangKyKhoaHocs = new HashSet<DangKyKhoaHoc>();
            this.TienDoHocs = new HashSet<TienDoHoc>();
        }
    
        public int IDHocVien { get; set; }
        public string IDTenDangNhap { get; set; }
        public string TenHV { get; set; }
        public Nullable<System.DateTime> NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
    
        public virtual TaiKhoan TaiKhoan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HocVienLopHoc> HocVienLopHocs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TienDoHoc> TienDoHocs { get; set; }
    }
}
