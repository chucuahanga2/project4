using System;
using System.Security.Cryptography;
using System.Text;

namespace DuAnEnglish.Security
{
    public static class PasswordHelper
    {
        // Salt hệ thống để chống tấn công Rainbow Table
        private const string AppSalt = "TikiCourse_OnlineEdu_Salt@2026";

        /// <summary>
        /// Băm mật khẩu bằng thuật toán SHA-256 kèm Salt
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + AppSalt));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Xác thực mật khẩu: Hỗ trợ cả mật khẩu băm mới và mật khẩu cũ dạng plain text
        /// </summary>
        public static bool VerifyPassword(string inputPassword, string storedPassword)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedPassword))
            {
                return false;
            }

            // 1. Kiểm tra nếu khớp mật khẩu plain text (tài khoản cũ ban đầu chưa migration)
            if (string.Equals(inputPassword, storedPassword, StringComparison.Ordinal))
            {
                return true;
            }

            // 2. Kiểm tra nếu khớp mã băm SHA-256
            string hashedInput = HashPassword(inputPassword);
            return string.Equals(hashedInput, storedPassword, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra xem mật khẩu trong CSDL có phải là dạng cũ (cần tự động nâng cấp sang Hash) hay không
        /// </summary>
        public static bool IsLegacyPassword(string storedPassword)
        {
            if (string.IsNullOrEmpty(storedPassword)) return false;
            // Chuỗi SHA-256 hex có đúng 64 ký tự. Nếu không đủ 64 ký tự hex thì là mật khẩu plain text cũ
            return storedPassword.Length != 64;
        }
    }
}
