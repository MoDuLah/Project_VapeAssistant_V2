using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Security.Principal;
using Vape_Assistant.Properties;
using System.Runtime.InteropServices;
using Vape_Assistant.Views;
using System.Threading.Tasks;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for ShellViews.xaml
    /// </summary>
    public partial class ShellsView : Window
    {
        Version version = Assembly.GetExecutingAssembly().GetName().Version;
        public string CurrentCulture = Settings.Default.Culture;
        public string message, errmsg;
        public string caption;
        public string title;
        public int autotimeout = 5000;
        string eng = "en-US";
        string gr = "el-GR";
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        public bool IsAdmin { get; set; } = false;

        public ShellsView()
        {
            InitializeComponent();

            Settings.Default.Run = Settings.Default.Run + 1;
            Settings.Default.LoginSuccess = false;
            int Run = Settings.Default.Run;
            VapeAssistant.Title = string.Format(Properties.Resources.SoftwareTitle + " - " + Properties.Resources.SoftwareVersion + " {0}.{1}.{2}",
              version.Major, version.Minor, version.Build) + " Run: " + Run;

            MyNotifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "favicon.ico")
            };
            MyNotifyIcon.MouseDoubleClick +=
                new System.Windows.Forms.MouseEventHandler
                    (MyNotifyIcon_MouseDoubleClick);
            if (IsAdministrator == true)
            {
                VapeAssistant.Title = VapeAssistant.Title + " [Administrator]";
            }
            else
            {
                if (CurrentCulture == eng)
                {
                    message = "To avoid any problems you should nrun this program as Administrator.";
                    message += "\n\nTo do that do:\nRight-Click on the shortcut and select [Run As Administrator]";
                    caption = "Error";
                }
                if (CurrentCulture == gr)
                {
                    message = "Για την αποφυγή προβλημάτων το πρόγραμμα θα πρέπει να εκτελέστει σαν Διαχειριστής.";
                    message += "\n\nΓια να το κάνετε αυτό πατήστε:\nΔεξί Click στη συντόμευση και επιλέξτε";
                    message += "\n[Εκτέλεση ως Διαχειριστής].";
                    caption = "Σφάλμα";
                }
                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
                return;
            }
            string CustomVersion = VapeAssistant.Title;

            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                //Handler attach - will not be done if not needed
                PreviewKeyDown += new KeyEventHandler(ShellView_PreviewKeyDown);
            }
        }

        void MyNotifyIcon_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
        {
            WindowState = WindowState.Normal;
            MyNotifyIcon.Visible = false;
            ShowInTaskbar = true;
        }

        private void Window_StateChanged(object sender, SizeChangedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    break;
                case WindowState.Minimized:
                    ShowInTaskbar = false;
                    MyNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                    MyNotifyIcon.BalloonTipText = "Minimized the app ";
                    MyNotifyIcon.ShowBalloonTip(autotimeout);
                    MyNotifyIcon.Visible = true;
                    break;
                case WindowState.Normal:
                    MyNotifyIcon.Visible = false;
                    ShowInTaskbar = true;
                    break;
            }
        }
        public static bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.Save();
        }

        public static void ShellView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;

                if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Length > 0)
                {
                    Keyboard.FocusedElement.RaiseEvent(
                        new TextCompositionEventArgs(
                            InputManager.Current.PrimaryKeyboardDevice,
                            new TextComposition(InputManager.Current,
                                Keyboard.FocusedElement,
                                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                            )
                        {
                            RoutedEvent = TextCompositionManager.TextInputEvent
                        });
                }
            }
        }

        private void VapeAssistant_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Donate sw = new Donate();
            Settings.Default.Save();
            GC.Collect();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ActiveItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MainW_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
            MyNotifyIcon.BalloonTipTitle = Properties.Resources.SoftwareTitle;
            MyNotifyIcon.BalloonTipText = "Minimized to tray.";
            MyNotifyIcon.ShowBalloonTip(autotimeout);
            MyNotifyIcon.Visible = true;
        }

        private void _exit_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            GC.Collect();
            Close();
        }
    }
}

class AutoClosingMessageBox
{
    System.Threading.Timer _timeoutTimer;
    string _caption;
    AutoClosingMessageBox(string text, string caption, int timeout)
    {
        _caption = caption;
        _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
            null, timeout, System.Threading.Timeout.Infinite);
        MessageBox.Show(text, caption);
    }
    public static void Show(string text, string caption, int timeout)
    {
        new AutoClosingMessageBox(text, caption, timeout);

    }
    void OnTimerElapsed(object state)
    {
        IntPtr mbWnd = FindWindow(null, _caption);
        if (mbWnd != IntPtr.Zero)
            SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        _timeoutTimer.Dispose();
    }
    const int WM_CLOSE = 0x0010;
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
}