# HƯỚNG DẪN THIẾT LẬP CƠ SỞ DỮ LIỆU - WEBSITE CHIA SẺ KHÓA HỌC TRỰC TUYẾN

Hệ thống sử dụng **SQL Server** cùng với **Entity Framework 6 Database First (`Model1.edmx`)**.

---

## 1. Phân loại các Script SQL trong Repository

| Tên File | Vai trò | Khi nào cần chạy? |
| :--- | :--- | :--- |
| **`DatabaseKLTN.sql`** | Script khởi tạo Database và 13 bảng ban đầu (hệ thống trung tâm tiếng Anh) | **Cài đặt mới hoàn toàn** (khi chưa có database trên SQL Server). |
| **`Insert.sql`** | Dữ liệu khởi tạo ban đầu cho các bảng danh mục tài khoản, giảng viên, phòng học | **Cài đặt mới hoàn toàn** ngay sau khi chạy `DatabaseKLTN.sql`. |
| **`UpdateDatabase_KhoaHocTrucTuyen.sql`** | Nâng cấp hệ thống Khóa học trực tuyến: tạo các bảng `DanhMucKhoaHoc`, `ChuongHoc`, `BaiHoc`, `DangKyKhoaHoc`, `TienDoHoc`, mở rộng `KhoaHoc` và `ThanhToan` | **Cài đặt mới** hoặc **nâng cấp từ database cũ**. Script được thiết kế kiểm tra an toàn (Idempotent). |
| **`update_vietnamese.sql`** | Dữ liệu mẫu chuẩn hóa tiếng Việt cho các khóa học công nghệ thông tin (ASP.NET MVC 5, C# .NET, SQL Server, HTML/CSS/JS) và đề cương chương/bài chi tiết | Chạy sau khi nâng cấp để có đầy đủ dữ liệu demo khóa học trực tuyến. |
| **`01_SetupDatabase_KhoaHocTrucTuyen.sql`** | Script chuẩn hóa duy nhất: Đảm bảo độ rộng cột `MatKhau` (`VARCHAR(256)` cho SHA-256 + salt), tạo các Unique Constraint (`UQ_HocVien_KhoaHoc`, `UQ_HocVien_BaiHoc`) và kích hoạt đầy đủ ràng buộc khóa ngoại | **Bắt buộc chạy** trên mọi môi trường để đảm bảo toàn vẹn dữ liệu và mã hóa mật khẩu an toàn. |

> **Lưu ý:** Các file PowerShell `.ps1` là các file script tự động hóa phục vụ dev/test nội bộ, không bắt buộc người dùng cuối phải chạy.

---

## 2. Quy trình Setup Database Khuyến nghị (Duy Nhất)

### Trường hợp A: Cài mới hoàn toàn từ đầu (Fresh Install)
Mở SQL Server Management Studio (SSMS), kết nối tới SQL Server của bạn và thực thi tuần tự 4 bước:

1. **Bước 1**: Mở và chạy `DatabaseKLTN.sql` (tạo CSDL `trungtamtienganh` hoặc tên chỉ định).
2. **Bước 2**: Mở và chạy `Insert.sql` (chèn các tài khoản, giảng viên mặc định).
3. **Bước 3**: Mở và chạy `UpdateDatabase_KhoaHocTrucTuyen.sql` (bổ sung các bảng khóa học online).
4. **Bước 4**: Mở và chạy `01_SetupDatabase_KhoaHocTrucTuyen.sql` (chuẩn hóa schema, mật khẩu, unique constraints).
*(Tùy chọn: Chạy thêm `update_vietnamese.sql` nếu muốn có dữ liệu bài giảng video CNTT mẫu phong phú).*

### Trường hợp B: Đã có database hiện tại
Chỉ cần mở và chạy:
1. `01_SetupDatabase_KhoaHocTrucTuyen.sql`

---

## 3. Cấu hình Connection String trong `Web.config`

Kiểm tra chuỗi kết nối trong file `DuAnEnglish/Web.config`:
```xml
<connectionStrings>
  <add name="trungtamtienganhEntities" 
       connectionString="metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=localhost;initial catalog=KhoaHocTrucTuyenDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```
*(Thay thế `data source` và `initial catalog` cho khớp với tên instance SQL Server và tên database của máy bạn).*

---

## 4. Tài khoản Kiểm thử Mặc định

| Vai trò | Tên đăng nhập | Mật khẩu mặc định | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `123456` | Quản trị toàn bộ hệ thống |
| **Giảng viên** | `gv_huynhphuoc` | `123456` | Giảng viên ThS. Huỳnh Phước |
| **Giảng viên** | `gv_nguyenvana` | `123456` | Giảng viên ThS. Nguyễn Văn A |
| **Học viên** | `hocvien_test` | `123456` | Học viên demo (đã có khóa học) |
| **Học viên** | `tranvanb` | `123456` | Học viên Trần Văn B |
