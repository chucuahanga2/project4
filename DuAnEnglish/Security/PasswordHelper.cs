using System;
using System.Security.Cryptography;
using System.Text;

namespace DuAnEnglish.Security
{
    /// <summary>
    /// Lớp tiện ích mã hóa và xác thực mật khẩu chuẩn công nghiệp.
    /// Sử dụng PBKDF2 (Rfc2898DeriveBytes) kèm Salt ngẫu nhiên riêng biệt cho từng tài khoản (10.000 vòng lặp).
    /// Đồng thời tương thích ngược để xác thực và tự động nâng cấp mật khẩu từ SHA-256 hoặc plain text cũ.
    /// </summary>
    public static class PasswordHelper
    {
        // Salt hệ thống cũ dùng để đối chiếu mật khẩu SHA-256 tạm thời
        private const string LegacyAppSalt = "TikiCourse_OnlineEdu_Salt@2026";
        private const int SaltByteSize = 16;       // 128-bit Salt ngẫu nhiên
        private const int HashByteSize = 32;       // 256-bit Hash
        private const int Pbkdf2Iterations = 10000; // 10.000 vòng lặp chuẩn OWASP / NIST

        /// <summary>
        /// Băm mật khẩu bằng thuật toán PBKDF2 (HMAC-SHA1) với Salt ngẫu nhiên riêng cho từng user.
        /// Định dạng lưu trữ: PBKDF2$10000${saltHex}${hashHex}
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            // 1. Tạo Salt ngẫu nhiên 16 bytes (128-bit) bằng CSP an toàn mật mã
            byte[] salt = new byte[SaltByteSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // 2. Tính toán PBKDF2 derivation
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Pbkdf2Iterations))
            {
                hash = pbkdf2.GetBytes(HashByteSize);
            }

            // 3. Đóng gói kết quả dưới định dạng chuẩn: PBKDF2$<iterations>$<saltHex>$<hashHex>
            return string.Format("PBKDF2${0}${1}${2}", 
                Pbkdf2Iterations, 
                ToHexString(salt), 
                ToHexString(hash));
        }

        /// <summary>
        /// Xác thực mật khẩu nhập vào với mật khẩu lưu trong cơ sở dữ liệu.
        /// Hỗ trợ cả 3 cấp độ:
        /// 1. PBKDF2 với Salt ngẫu nhiên (chuẩn mới nhất)
        /// 2. SHA-256 với Salt hệ thống (chuẩn trung gian)
        /// 3. Plain text (tài khoản cũ ban đầu chưa nâng cấp)
        /// </summary>
        public static bool VerifyPassword(string inputPassword, string storedPassword)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedPassword))
            {
                return false;
            }

            // 1. Xác thực định dạng chuẩn PBKDF2
            if (storedPassword.StartsWith("PBKDF2$", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string[] parts = storedPassword.Split('$');
                    if (parts.Length == 4)
                    {
                        int iterations = int.Parse(parts[1]);
                        byte[] salt = FromHexString(parts[2]);
                        byte[] expectedHash = FromHexString(parts[3]);

                        using (var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, iterations))
                        {
                            byte[] actualHash = pbkdf2.GetBytes(expectedHash.Length);
                            return SlowEquals(actualHash, expectedHash);
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }

            // 2. Kiểm tra nếu khớp mã băm SHA-256 (hệ thống trung gian)
            if (storedPassword.Length == 64)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword + LegacyAppSalt));
                    string sha256Hex = ToHexString(bytes);
                    if (string.Equals(sha256Hex, storedPassword, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            // 3. Kiểm tra nếu khớp mật khẩu plain text (tài khoản ban đầu của đồ án cũ)
            if (string.Equals(inputPassword, storedPassword, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra xem mật khẩu hiện tại trong CSDL có cần tự động nâng cấp lên chuẩn PBKDF2 hay không.
        /// </summary>
        public static bool IsLegacyPassword(string storedPassword)
        {
            if (string.IsNullOrEmpty(storedPassword)) return false;
            // Nếu chưa được băm theo chuẩn PBKDF2 thì coi là legacy để tự động nâng cấp khi đăng nhập
            return !storedPassword.StartsWith("PBKDF2$", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// So sánh 2 mảng byte với thời gian không đổi (Constant-time comparison) để phòng chống tấn công Timing Attack.
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        private static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private static byte[] FromHexString(string hex)
        {
            if (hex == null || hex.Length % 2 != 0) return new byte[0];

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }
}
