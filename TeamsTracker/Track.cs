
//using System;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Automation;

//namespace TeamsTracker
//{
//    public class Track
//    {



//        [STAThread]
//        public static void Start()
//        {
//            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Children, (sender, e) =>
//            {
//                var element = (AutomationElement)sender;
//                var name = element.Current.Name;
//                string? fullName = null;
//                bool isMeeting = false;
//                Stopwatch stopWatch = new Stopwatch();
//                stopWatch.Start();


//                Console.WriteLine("open: " + name + " hwnd:" + element.Current.NativeWindowHandle);


//                // Teams does not show the full title straight away, just "Teams" so wait a few seconds before querying
//                Task.Factory.StartNew(() =>
//                {
//                    Thread.Sleep(5000);
//                    fullName = GetWindowTitle(element.Current.NativeWindowHandle);
//                    Console.WriteLine($"Full title = {fullName}");
//                    if (fullName.Contains("| Microsoft Teams")) isMeeting = true;
//                    else stopWatch.Stop(); // Might as well stop the stopwatch since it's not a meeting
//                });

            

//                Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent, element, TreeScope.Element, (s, e2) =>
//                {
//                    Console.WriteLine("close: " + fullName == null ? name : fullName + " hwnd:" + element.Current.NativeWindowHandle);
//                    if (isMeeting)
//                    {
//                        TimeSpan ts = stopWatch.Elapsed;
//                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
//                        Console.WriteLine($"MEETING ENDED! {elapsedTime}");

//                        Dialog d = new Dialog();
//                        d.ShowDialog();
//                    }
//                });
//            });

         
//            Console.ReadLine();
//            Automation.RemoveAllEventHandlers();
//        }
//    }
//}