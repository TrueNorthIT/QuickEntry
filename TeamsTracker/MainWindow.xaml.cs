using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;


namespace TeamsTracker
{

    public class TextBoxStreamWriter: StringWriter
    {
        private TextBox textBoxOutput;
        protected StreamWriter writer;
        protected MemoryStream mem;

        public TextBoxStreamWriter(TextBox _output)
        {
            textBoxOutput = _output;
            mem = new MemoryStream(1000000);
            writer = new StreamWriter(mem);
            writer.AutoFlush = true;
        }

        public override void Write(char value)
        {
            Application.Current.Dispatcher.Invoke(() => { 
                textBoxOutput.AppendText(value.ToString());
            });
        }
        public override void Write(string value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                textBoxOutput.AppendText(value.ToString());
            });
        }

        public override void WriteLine(string value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                textBoxOutput.AppendText(value.ToString() + Environment.NewLine);
            });
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged  
    {
        private CacheManager cache;
        private string exePath;
        private string argsString;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }
        private string GetMeetingTime(TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        public MainWindow()
        {

            InitializeComponent();


            var cachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TeamsTracker");
            cache = new CacheManager(cachePath);

            cache.TryReadCache("exe.txt", out exePath);
            cache.TryReadCache("args.txt", out argsString);


            var debugTB = new TextBoxStreamWriter(debugOut);
            var consoleTB = new TextBoxStreamWriter(consoleOut);
            Console.SetOut(debugTB);
            Console.WriteLine("DEBUG");

            HashSet<IntPtr> trackedWindows = new HashSet<IntPtr>();

            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Children, (sender, e) =>
            {
                var element = (AutomationElement)sender;
                if (trackedWindows.Contains((IntPtr)element.Current.NativeWindowHandle)) return;
                trackedWindows.Add((IntPtr)element.Current.NativeWindowHandle);
                var name = element.Current.Name;
                string? fullName = null;
                string? meetingName = null;
                bool isMeeting = false;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();


                Console.WriteLine("open: " + name + " hwnd:" + element.Current.NativeWindowHandle);
                

                // Teams does not show the full title straight away, just "Teams" so wait a few seconds before querying
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(5000);
                    fullName = GetWindowTitle((IntPtr)element.Current.NativeWindowHandle);
                    Console.WriteLine($"Full title = {fullName}");
                    if (fullName.Contains("| Microsoft Teams"))
                    {
                        isMeeting = true;
                        meetingName = fullName.Split("| Microsoft Teams")[0];
                        consoleTB.WriteLine($"In a meeting: {meetingName}");
                    }
                    else stopWatch.Stop(); // Might as well stop the stopwatch since it's not a meeting
                });



                Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent, element, TreeScope.Element, (s, e2) =>
                {
                    trackedWindows.Remove((IntPtr)element.Current.NativeWindowHandle);
                    Console.WriteLine("close: " + fullName == null ? name : fullName + " hwnd:" + element.Current.NativeWindowHandle);
                    if (isMeeting)
                    {
                        stopWatch.Stop();
                        Console.WriteLine($"MEETING ENDED!");
                        meetingName = "Test Meeting";
                        consoleTB.WriteLine($"Meeting: {meetingName} ended. {GetMeetingTime(stopWatch.Elapsed)}");

                        if (exePath != null & exePath != "") 
                        {
                            using Process pProcess = new Process();
                            pProcess.StartInfo.FileName = exePath;

                            if (argsString != null & argsString != "")
                            {
                                argsString = argsString.Replace("{time}", Math.Ceiling(stopWatch.Elapsed.TotalMinutes).ToString());
                                argsString = argsString.Replace("{name}", $"\"{meetingName}\"");
                                pProcess.StartInfo.Arguments = argsString;
                            }
                            pProcess.StartInfo.UseShellExecute = false;
                            pProcess.Start();

                        }



                    }
                });
            });

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new Settings();
            settingsDialog.ShowDialog();
            cache.TryReadCache("exe.txt", out exePath);
            cache.TryReadCache("args.txt", out argsString);
        }
    }
}
