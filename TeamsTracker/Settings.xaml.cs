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
using System.Windows.Shapes;

namespace TeamsTracker
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private CacheManager cache;
        public Settings()
        {
            InitializeComponent();
            var cachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TeamsTracker");
            cache = new CacheManager(cachePath);

            cache.TryReadCache("exe.txt", out string exePath);
            cache.TryReadCache("args.txt", out string argsString);

            if (exePath != null) exe.Text = exePath;
            if (argsString != null) args.Text = argsString;


        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            cache.WriteCache("exe.txt", exe.Text);
            cache.WriteCache("args.txt", args.Text);

            this.Close();

        }
    }
}
