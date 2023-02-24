using Microsoft.Identity.Core.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace QuickEntry
{

    public class TaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            try { return ((Project)value).tasks; }
            catch { return null; }
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class MeetingRule
    {
        public string MeetingName { get; set; }

        public Project Project { get; set; }

        public ProjectTask Task { get; set; }

    }

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window, INotifyPropertyChanged
    {
        CacheManager cache;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Settings()
        {
            InitializeComponent();
            var cachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QuickEntry");
            cache = new CacheManager(cachePath);

            if (!cache.TryReadCache("user.txt", out string email)) Email.BorderBrush = new SolidColorBrush(Colors.Red); else Email.Text = email;
            if (!cache.TryReadCache("url.txt", out string url)) CrmURL.BorderBrush = new SolidColorBrush(Colors.Red); else CrmURL.Text = url;

            var rules = new List<MeetingRule>();

            if (!cache.TryReadCache("projects.txt", out Dictionary<Guid, Project> allProjects)) allProjects = new Dictionary<Guid, Project>();
            if (!cache.TryReadCache("rules.txt", out rules)) rules = new List<MeetingRule>();


            var projects = allProjects.Values.ToList();

            foreach (var rule in rules)
            {
                var projId = rule.Project.id;
                var taskId = rule.Task.id;
                rule.Project = projects.Where(Project=> Project.id == projId).FirstOrDefault();
                rule.Task = rule.Project.tasks.Where(Task => Task.id == taskId).FirstOrDefault();
            }


            RuleGrid.ItemsSource = rules;
            Project.ItemsSource = projects;
            RuleGrid.CurrentCellChanged += RuleGrid_CurrentCellChanged;

        }

        private void RuleGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged();
            try
            {
                RuleGrid.Items.Refresh();
            }
            catch { }
        }

        private void On_Save(object sender, RoutedEventArgs e)
        {
            cache.WriteCache("user.txt", Email.Text);
            cache.WriteCache("url.txt", CrmURL.Text);

            var rules = RuleGrid.Items.SourceCollection;

            cache.WriteCache("rules.txt", rules);

            Close();

        }

        private void RuleGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotifyPropertyChanged();
            RuleGrid.Items.Refresh();
        }
    }
}
