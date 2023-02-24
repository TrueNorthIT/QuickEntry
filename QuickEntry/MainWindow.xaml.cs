using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickEntry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        string meetingName = "";
        string mins = "";
        Dictionary<Guid, Project> projects; 
        string cachePath;
        string email;
        string crmUrl;
        CacheManager cache;
        List<MeetingRule> meetingRules;
        DynamicsAPI dynamicsAPI;


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleArgs()
        {
            // Parse the arguments passed to the exe, we are looking for the meeting name and time
            string[] args = Environment.GetCommandLineArgs();
            var argsDict = new Dictionary<string, string>();
            for (int index = 1; index < args.Length; index += 2) argsDict.Add(args[index], args[index + 1]);

            // Arg flags -name "SQT Planning" -time "120" 
            argsDict.TryGetValue("-name", out meetingName);
            argsDict.TryGetValue("-time", out mins);
        }


        private void GetProjectDetails()
        {
            // Query dynamics for all the projects that the user is registered for.
            var result = dynamicsAPI.GetUsersProjects();
            projects = new Dictionary<Guid, Project>();

            foreach (var e in result.Entities)
            {
                var taskName = DynamicsAPI.GetAliasedValue<string>(e, "msdyn_projecttask2.msdyn_subject");
                var taskId = DynamicsAPI.GetAliasedValue<Guid>(e, "msdyn_projecttask2.msdyn_projecttaskid");
                var task = new ProjectTask() { name = taskName, id = taskId };

                if (projects.ContainsKey(e.Id)) projects[e.Id].tasks.Add(task);
                else
                {
                    projects.Add(e.Id, new Project() {
                        id = e.Id,
                        name = e.GetAttributeValue<string>("msdyn_subject"),
                        bookableResource = DynamicsAPI.GetAliasedValue<EntityReference>(e, "msdyn_projectteam1.msdyn_bookableresourceid").Id,
                        tasks = new List<ProjectTask>() { task }
                    });
                }
            }
        }


        
        private void PopulateTasks ()
        {
            // Get the selected index, if it's -1 then return early
            var selectedIndex = Project.SelectedIndex;
            if (selectedIndex == -1) return;

            // Grab the selected combo box item and from there get the Project
            var selected = (ComboBoxItem)Project.Items[selectedIndex];
            var selectedProject = projects[new Guid(selected.Uid)];

            // Clear out the old task list
            Task.Items.Clear();

            // For each task of the project add a new combo box item to the dropdown
            foreach (var task in selectedProject.tasks) Task.Items.Add(new ComboBoxItem() { Content = task.name, Uid = task.id.ToString() });
            
            // Notify the UI that the options have changed
            NotifyPropertyChanged();
        }

        private void SetTimeLabel()
        {
            // Format the mins provided as hours and mins, if the parse fails just don't update the label yet
            // The user may have accidently typed a letter
            try
            {
                var span = TimeSpan.FromMinutes(double.Parse(Time.Text));
                TimeDisplay.Content = string.Format("{0:00}:{1:00}", span.Hours, span.Minutes);
                NotifyPropertyChanged();
            }
            catch { }
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // The event handeler for the add button, gather all the data then instruct the dynamics API 
            // to add a new timesheet entry. Once done quit the program

            var selectedIndex = Project.SelectedIndex;
            var selected = (ComboBoxItem)Project.Items[selectedIndex];
            var selectedProject = projects[new Guid(selected.Uid)];

            var taskIndex = Task.SelectedIndex;
            var task = (ComboBoxItem)Task.Items[taskIndex];


            var project = new EntityReference("msdyn_project", new Guid(selected.Uid));
            var projecttask = new EntityReference("msdyn_projecttask", new Guid(task.Uid));
            var bookableresource = new EntityReference("bookableresource", selectedProject.bookableResource);

            dynamicsAPI.AddTimesheetEntry(int.Parse(Time.Text), Title.Text, Comments.Text, project, projecttask, bookableresource);

            Close();
        }



        public MainWindow()
        {
            InitializeComponent();
            HandleArgs();


            #region Load data from disk
            cachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QuickEntry");
            cache = new CacheManager(cachePath);

            // Try read the users email
            if (!cache.TryReadCache("user.txt", out email))
            {
                var settings = new Settings();
                var diag = settings.ShowDialog();
                if (!cache.TryReadCache("user.txt", out email)) Close(); // If still unset just quit
            }

            if (!cache.TryReadCache("url.txt", out crmUrl))
            {
                var settings = new Settings();
                var diag = settings.ShowDialog();
                if (!cache.TryReadCache("url.txt", out crmUrl)) Close(); // If still unset just quit
            }

            cache.TryReadCache("rules.txt", out meetingRules);

            #endregion

            dynamicsAPI = new DynamicsAPI(cachePath, email, crmUrl);


            // Try read the project details, if not set then pull them from CRM and store
            var gotProjects = cache.TryReadCache("projects.txt", out projects);
            if (projects == null || !gotProjects)
            {
                GetProjectDetails();
                cache.WriteCache("projects.txt", projects);
            }

            foreach (var kvp in projects) Project.Items.Add(new ComboBoxItem() { Content = kvp.Value.name, Uid = kvp.Key.ToString() });
            Project.SelectedIndex = 0;
            PopulateTasks();

            #region Setup event handelers

            Project.SelectionChanged += (object sender, SelectionChangedEventArgs e) => { PopulateTasks(); };
            Time.KeyUp += (object sender, KeyEventArgs e) => { SetTimeLabel(); };
            Add.Click += Add_Click;
            Refresh.Click += (object sender, RoutedEventArgs e) =>
            {
                GetProjectDetails();
                Project.SelectedIndex = 0;
                PopulateTasks();
            };

            Settings.Click += (object sender, RoutedEventArgs e) =>
            {
                var settings = new Settings();
                settings.ShowDialog();
            };
            #endregion

            Time.Text = mins == "" || mins == null ? "0" : mins;
            if (meetingName != null && meetingName != "") Title.Text = meetingName;
            SetTimeLabel();

            // Finally, let's check the rules

            if (meetingName != null)
            {
                MeetingRule matchedRule = meetingRules.Where(MeetingRule => MeetingRule.MeetingName.Trim() == meetingName.Trim()).FirstOrDefault();
                if (matchedRule != null)
                {
                    for (int i = 0; i < Project.Items.Count; i++)
                    {
                        object item = Project.Items[i];
                        if (new Guid(((ComboBoxItem)item).Uid) == matchedRule.Project.id)
                        {
                            Project.SelectedIndex = i;
                            PopulateTasks();
                            break;
                        }
                    }
                    for (int i = 0; i < Task.Items.Count; i++)
                    {
                        object item = Task.Items[i];
                        if (new Guid(((ComboBoxItem)item).Uid) == matchedRule.Task.id)
                        {
                            Task.SelectedIndex = i;
                            break;
                        }
                    }

                }
            }
        }

    }
}
