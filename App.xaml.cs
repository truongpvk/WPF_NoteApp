using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace DoAnNote
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // trỏ DataDirectory tới thư mục gốc project
            string projectDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\");
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(projectDir));
        }
    }
}
