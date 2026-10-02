namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;

    public partial class DangKyKhoaHoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DangKyKhoaHoc()
        {
            this.ThanhToans = new HashSet<ThanhToan>();
            this.DiemKhoaHocs = new HashSet<DiemKhoaHoc>();
        }

        public int IDDangKy { get; set; }
        public int IDHocVien { get; set; }
        public string IDKhoaHoc { get; set; }
        public Nullable<System.DateTime> NgayDangKy { get; set; }
        public string TrangThai { get; set; }
        public Nullable<System.DateTime> NgayHoanThanh { get; set; }

        public virtual HocVien HocVien { get; set; }
        public virtual KhoaHoc KhoaHoc { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanhToan> ThanhToans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiemKhoaHoc> DiemKhoaHocs { get; set; }
        public virtual DiemKhoaHoc DiemKhoaHoc
        {
            get
            {
                if (DiemKhoaHocs != null && DiemKhoaHocs.Count > 0)
                {
                    return System.Linq.Enumerable.FirstOrDefault(DiemKhoaHocs);
                }
                return null;
            }
        }
    }
}
