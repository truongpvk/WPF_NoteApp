using DoAnNote.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnNote.ViewModel.Auth
{
    public class SignUpViewModel
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public (bool ok, string err) CheckValid()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(DisplayName) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                return (false, "Bạn chưa điền đủ thông tin!");
            }
            if (Password != ConfirmPassword)
            {
                return (false, "Mật khẩu không trùng khớp!");
            }

            using (var db = new AppDatabaseEntities())
            {
                if (db.Users.Any(u => u.Username == Username))
                {
                    return (false, "Tên đăng nhập đã tồn tại!");
                }
            }

            return (true, null);
        }

        public void ToModel()
        {
            Username = Username.Trim();
            DisplayName = DisplayName.Trim();
            Password = Password.Trim();
            ConfirmPassword = ConfirmPassword.Trim();

            using (var db = new AppDatabaseEntities())
            {
                var user = new User
                {
                    Username = Username,
                    DisplayName = DisplayName,
                    PasswordHash = StringHash.GetSHA256Hash(Password),
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
        }
    }
}
