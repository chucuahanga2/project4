# HƯỚNG DẪN THIẾT LẬP CƠ SỞ DỮ LIỆU - WEBSITE CHIA SẺ KHÓA HỌC TRỰC TUYẾN

Hệ thống sử dụng **SQL Server** cùng với **Entity Framework 6 Database First (`Model1.edmx`)**.

---

## 1. Tên Database Thống Nhất

> [!IMPORTANT]
> Toàn bộ hệ thống (Script SQL, EDMX, Web.config, Unit Tests) thống nhất sử dụng duy nhất một tên Database:
> **`KhoaHocTrucTuyenDB`**

---

## 2. Quy trình Cài đặt Nhanh Nhất (Chỉ 1 Bước Duy Nhất)

File **`01_SetupDatabase_KhoaHocTrucTuyen.sql`** đã được thiết kế hoàn chỉnh dạng **All-in-one & Idempotent** (tự tạo Database nếu chưa có, tự tạo tất cả 18 bảng, tự thiết lập quan hệ / ràng buộc Unique, băm mật khẩu PBKDF2 chuẩn công nghiệp và chèn sẵn đầy đủ dữ liệu mẫu).

### Các bước thực hiện:
1. Mở **SQL Server Management Studio (SSMS)** hoặc **Azure Data Studio**.
2. Kết nối tới instance SQL Server của bạn (ví dụ: `.` hoặc `localhost` hoặc `.\SQLEXPRESS` hoặc `127.0.0.1`).
3. Mở file **`01_SetupDatabase_KhoaHocTrucTuyen.sql`** trong thư mục gốc dự án.
4. Bấm **Execute (F5)**.
5. Thông báo hoàn tất: `[THÀNH CÔNG] Cơ sở dữ liệu KhoaHocTrucTuyenDB đã được thiết lập hoàn chỉnh 100%!`.

*(Nếu muốn có thêm nhiều bài giảng video CNTT nâng cao bằng tiếng Việt, bạn có thể chạy thêm script tùy chọn `update_vietnamese.sql`).*

---

## 3. Cấu hình Chuỗi Kết Nối (`Web.config`)

Repo đã chuẩn bị sẵn file mẫu **`DuAnEnglish/Web.config.example`**. 

Khi clone repo lần đầu:
1. Sao chép `DuAnEnglish/Web.config.example` thành `DuAnEnglish/Web.config` (nếu chưa có).
2. Kiểm tra phần `<connectionStrings>`:
```xml
<connectionStrings>
  <add name="trungtamtienganhEntities" 
       connectionString="metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=127.0.0.1;initial catalog=KhoaHocTrucTuyenDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```
3. Thay đổi giá trị `data source` thành tên SQL Server trên máy của bạn (ví dụ: `127.0.0.1`, `localhost`, `.` hoặc `.\SQLEXPRESS`).

---

## 4. Cấu hình Cổng Thanh Toán VNPay Sandbox

Trong `Web.config`:
```xml
<appSettings>
  <!-- Cấu hình VNPay Sandbox -->
  <add key="Vnp_Url" value="https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" />
  <add key="Vnp_TmnCode" value="YOUR_VNPAY_TMN_CODE" />
  <add key="Vnp_HashSecret" value="YOUR_VNPAY_HASH_SECRET" />
  <add key="Vnp_ReturnUrl" value="http://localhost:52000/ThanhToan/ReturnVnpay" />
</appSettings>
```

---

## 5. Tài khoản Kiểm thử Mặc định

| Vai trò | Tên đăng nhập | Mật khẩu mặc định | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `admin@123` | Quản trị toàn bộ danh mục, khóa học, học viên, doanh thu |
| **Giảng viên** | `giangvien1` | `123456` | ThS. Nguyễn Văn Toàn (quản lý chương, bài, học viên) |
| **Giảng viên** | `gv_huynhphuoc` | `123456` | ThS. Huỳnh Phước |
| **Giảng viên** | `gv_nguyenvana` | `123456` | ThS. Nguyễn Văn A |
| **Học viên** | `hocvien1` | `123456` | Trần Thị Mai (học viên tiêu biểu) |
| **Học viên** | `hocvien_test` | `123456` | Học viên demo (đã ghi danh khóa MVC2026) |
| **Học viên** | `tranvanb` | `123456` | Trần Văn B |
| **Khóa** | `thanh` | `123456` | Tài khoản đã bị khóa (dùng kiểm thử bảo mật) |
