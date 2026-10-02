namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;

    public partial class DiemKhoaHoc
    {
        public int IDDiem { get; set; }
        public int IDDangKy { get; set; }
        public Nullable<decimal> Diem { get; set; }
        public string NhanXet { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }

        public virtual DangKyKhoaHoc DangKyKhoaHoc { get; set; }
    }
}
