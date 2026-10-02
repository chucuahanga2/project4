namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;

    public partial class DanhMucKhoaHoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DanhMucKhoaHoc()
        {
            this.KhoaHocs = new HashSet<KhoaHoc>();
        }

        public int IDDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public string MoTa { get; set; }
        public string TrangThai { get; set; }
        public Nullable<int> ThuTu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KhoaHoc> KhoaHocs { get; set; }
    }
}
