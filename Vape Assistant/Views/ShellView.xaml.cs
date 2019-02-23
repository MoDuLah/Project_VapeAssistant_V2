using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Security.Principal;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        Version version = Assembly.GetExecutingAssembly().GetName().Version;
        public string CurrentCulture = Settings.Default.Culture;
        public string message, errmsg;
        public string caption;
        public string title;
        public int autotimeout = 5000;
        string eng = "en-US";
        string gr = "el-GR";
        bool isAdmin = false;

        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }

        public ShellView()
        {
            InitializeComponent();

            Settings.Default.Run = Settings.Default.Run + 1;
            Settings.Default.LoginSuccess = false;
            int Run = Settings.Default.Run;
            VapeAssistant.Title = string.Format(Properties.Resources.SoftwareTitle + " - " + Properties.Resources.SoftwareVersion + " {0}.{1}.{2}",
              version.Major, version.Minor, version.Build) + " Run: " + Run;
            if (IsAdministrator == true) {
                VapeAssistant.Title = VapeAssistant.Title + " [Administrator]";
            }
            else
            {
                VapeAssistant.Title = VapeAssistant.Title + " [Not Administrator]";
                if (CurrentCulture == eng)
                {
                    message = "To avoid any problems you should run this program as Administrator.";
                    message += "\n\nTo do that do:\nRight-Click on the shortcut and select [Run As Administrator]";
                    caption = "Error";
                }
                if (CurrentCulture == gr)
                {
                    message = "Για την αποφυγή προβλημάτων το πρόγραμμα θα πρέπει να εκτελέστει σαν Διαχειριστής.";
                    message += "\n\nΓια να το κάνετε αυτό πατήστε:\nΔεξί Click στη συντόμευση και επιλέξτε [Εκτέλεση ως Διαχειριστής]";
                    caption = "Σφάλμα";
                }
                MessageBox.Show(message, caption,MessageBoxButton.OK,MessageBoxImage.Error);
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

        private void _exit_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            GC.Collect();
            Close();
        }
    }
}