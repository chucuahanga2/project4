namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;

    public partial class BaiHoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BaiHoc()
        {
            this.TienDoHocs = new HashSet<TienDoHoc>();
        }

        public int IDBaiHoc { get; set; }
        public int IDChuong { get; set; }
        public string TenBaiHoc { get; set; }
        public string MoTa { get; set; }
        public string NoiDung { get; set; }
        public string VideoUrl { get; set; }
        public string TaiLieuUrl { get; set; }
        public Nullable<int> ThuTu { get; set; }
        public Nullable<int> ThoiLuong { get; set; }
        public Nullable<bool> ChoXemThu { get; set; }

        public virtual ChuongHoc ChuongHoc { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TienDoHoc> TienDoHocs { get; set; }
    }
}
