namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;

    public partial class TienDoHoc
    {
        public int IDTienDo { get; set; }
        public int IDHocVien { get; set; }
        public int IDBaiHoc { get; set; }
        public Nullable<bool> DaHoanThanh { get; set; }
        public Nullable<System.DateTime> NgayHoanThanh { get; set; }
        public Nullable<System.DateTime> ThoiDiemXemGanNhat { get; set; }

        public virtual BaiHoc BaiHoc { get; set; }
        public virtual HocVien HocVien { get; set; }
    }
}
