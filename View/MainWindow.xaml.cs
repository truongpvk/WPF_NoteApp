using DoAnNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoAnNote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLoginAccount_Click(object sender, RoutedEventArgs e)
        {
            var win = new AuthWindow();
            win.Show();
            this.Close();
        }


        private void BtnAndanh_Click(object sender, RoutedEventArgs e)
        {
            string alias = TxtAlias.Text == null ? "" : TxtAlias.Text.Trim();
            if (alias.Length == 0)
            {
                MessageBox.Show("Hãy nhập mật danh trước khi đăng nhập ẩn danh.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Xin chào, " + alias + "!\nĐăng nhập ẩn danh thành công.",
                "NOTE.me", MessageBoxButton.OK, MessageBoxImage.Information);
            // new MainNotesWindow().Show(); this.Close();
            AppSession.SetAnonymous(alias);

            Notes notesWindow = new Notes(alias);
            notesWindow.Show();
            this.Close();
        }
    }
}
