using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using DoAnNote.Models;
using DoAnNote.Static;
using DoAnNote.ViewModel.Auth;


namespace DoAnNote
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private bool _registerMode = false;
        private readonly LoginValidator _auth = new LoginValidator();

        public AuthWindow()
        {
            InitializeComponent();
            UpdateModeVisual();
            Loaded += (s, e) =>
            {
                UpdatePasswordWatermark(Pwd1);
                UpdatePasswordWatermark(Pwd2);
            };
        }

        private void UpdateModeVisual()
        {
            if (_registerMode)
            {
                TxtTitle.Text = "Đăng ký";
                TxtSubtitle.Text = "Tạo tài khoản mới để sử dụng";
                TxtFullName.Visibility = Visibility.Visible;
                Pwd2.Visibility = Visibility.Visible;
                BtnPrimaryText.Text = "Đăng ký";
                TxtSwitch.Text = "Bạn đã có tài khoản? Đăng nhập";
                BtnBack.ToolTip = "Về đăng nhập";
            }
            else
            {
                TxtTitle.Text = "Đăng nhập";
                TxtSubtitle.Text = "Nhập tài khoản để tiếp tục";
                TxtFullName.Visibility = Visibility.Collapsed;
                Pwd2.Visibility = Visibility.Collapsed;
                BtnPrimaryText.Text = "Đăng nhập";
                TxtSwitch.Text = "Bạn chưa có tài khoản? Đăng ký";
                BtnBack.ToolTip = "Về màn hình chính";
            }

            PasswordBox_Loaded(Pwd1, null);
            PasswordBox_Loaded(Pwd2, null);

        }


        // back theo ngữ cảnh
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_registerMode)
            {
                _registerMode = false;
                UpdateModeVisual();
                Pwd2.Password = string.Empty;
                return;
            }

            new MainWindow().Show();
            this.Close();
        }

        // Event đổi giao diện đăng nhập/đăng ký
        private void TxtSwitch_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _registerMode = !_registerMode;
            UpdateModeVisual();
        }

        // Event đăng nhập/đăng ký
        private void BtnPrimary_Click(object sender, RoutedEventArgs e)
        {
            string user = (TxtUser.Text ?? "").Trim();
            string pass = Pwd1.Password ?? "";
            string pass2 = Pwd2.Password ?? "";
            string fullName = (TxtFullName.Text ?? "").Trim();

            if (_registerMode)
            {
                SignUpViewModel viewModel = new SignUpViewModel
                {
                    Username = user,
                    DisplayName = fullName,
                    Password = pass,
                    ConfirmPassword = pass2
                };

                var (ok, err) = viewModel.CheckValid();
                
                if (!ok)
                {
                    MessageBox.Show(err, "Đăng ký", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                viewModel.ToModel();

                MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay.", "NOTE.me",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                _registerMode = false;
                UpdateModeVisual();
                Pwd1.Password = Pwd2.Password = string.Empty;
                return;
            }
            else
            {
                var (ok, err, displayName, userId) = _auth.Login(user, pass);
                if (!ok)
                {
                    MessageBox.Show(err, "Đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AppSession.BeginSession(userId, user, displayName);

                var alias = string.IsNullOrWhiteSpace(displayName) ? user : displayName;
                var notesWindow = new Notes(alias);

                MessageBox.Show($"Xin chào, {alias}!\nĐăng nhập thành công.", "NOTE.me",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                notesWindow.Show();
                this.Close();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                BtnBack_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }


        private void UpdatePasswordWatermark(PasswordBox pb)
        {
            var wm = pb.Template?.FindName("Watermark", pb) as TextBlock;
            if (wm == null) return;
            bool show = string.IsNullOrEmpty(pb.Password) && !pb.IsKeyboardFocused;
            wm.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
            => UpdatePasswordWatermark((PasswordBox)sender);

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
            => UpdatePasswordWatermark((PasswordBox)sender);

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
            => UpdatePasswordWatermark((PasswordBox)sender);

        private void PasswordBox_Loaded(object sender, RoutedEventArgs e)
            => UpdatePasswordWatermark((PasswordBox)sender);

    }
    //---------------------------------------------------------------------------------------------

    public class LoginValidator
    {
        public (bool ok, string error, string displayName, int userID) Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Vui lòng nhập đầy đủ thông tin.", null, -1);

            using (var db = new AppDatabaseEntities())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username.Trim());
                if (user == null)
                    return (false, "Tài khoản không tồn tại.", null, -1);

                if (user.PasswordHash != StringHash.GetSHA256Hash(password))
                    return (false, "Mật khẩu không đúng.", null, -1);

                return (true, null, user.DisplayName, user.UserID);
            }
        }

    }

}
