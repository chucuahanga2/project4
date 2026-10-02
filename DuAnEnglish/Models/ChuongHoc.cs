namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;

    public partial class ChuongHoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChuongHoc()
        {
            this.BaiHocs = new HashSet<BaiHoc>();
        }

        public int IDChuong { get; set; }
        public string IDKhoaHoc { get; set; }
        public string TenChuong { get; set; }
        public string MoTa { get; set; }
        public Nullable<int> ThuTu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaiHoc> BaiHocs { get; set; }
        public virtual KhoaHoc KhoaHoc { get; set; }
    }
}
