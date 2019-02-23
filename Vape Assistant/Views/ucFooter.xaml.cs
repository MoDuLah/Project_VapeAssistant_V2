using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Linq;
using System.IO;
using Vape_Assistant.Properties;
using System.Diagnostics;
using System.Windows.Navigation;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for ucFooter.xaml
    /// </summary>
    public partial class ucFooter : UserControl
    {
        private DispatcherTimer timerImageChange;
        private Image[] ImageControls;
        private List<ImageSource> Images = new List<ImageSource>();
        private static string[] ValidImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
        private static string[] TransitionEffects = new[] { "Fade" };
        private string TransitionType, strImagePath = "";
        private int CurrentSourceIndex, CurrentCtrlIndex, EffectIndex = 0, IntervalTimer = 1;
        public string removeString = "";
        public int index = 0;
        public string MyURL;
        public int y = 0;
        public bool clicked = false;
        public bool pause = false;
        DirectoryInfo ch;

        public ucFooter()
        {
            InitializeComponent();
            //Initialize Image control, Image directory path and Image timer.
            IntervalTimer = Convert.ToInt32(Settings.Default.IntervalTime);
            strImagePath = Settings.Default.ImagePath;
            ImageControls = new[] { myImage, myImage2 };

            LoadImageFolder(strImagePath);

            timerImageChange = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, IntervalTimer)
            };
            timerImageChange.Tick += new EventHandler(timerImageChange_Tick);
        }

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            PlaySlideShow();
            timerImageChange.IsEnabled = true;
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            pause = true;
            string s = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));
            clickedToday(e);
            if (!string.IsNullOrEmpty(MyURL) && clicked == false) { 
            Process.Start(new ProcessStartInfo(MyURL));
                //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                switch (y)
                {
                    case 1:
                        Settings.Default.link1clickeddate = s;
                        Settings.Default.Save();
                        break;
                    case 2:
                        Settings.Default.link2clickeddate = s;
                        Settings.Default.Save();
                        break;
                    case 3:
                        Settings.Default.link3clickeddate = s;
                        Settings.Default.Save();
                        break;
                    case 4:
                        Settings.Default.link4clickeddate = s;
                        Settings.Default.Save();
                        break;
                    case 5:
                        Settings.Default.link5clickeddate = s;
                        Settings.Default.Save();
                        break;
                    case 9999:
                        MessageBox.Show("Error");
                        return;
                    default:
                        MessageBox.Show("Error");
                        return;
                }
                
                e.Handled = true;
                pause = false;
            }
            else
            {
                if (clicked == true)
                {
                    MessageBox.Show("Error");
                    e.Handled = false;
                    pause = false;
                    return;
                }
                else
                {
                    e.Handled = false;
                    pause = false;
                }
            }
        }
        private void clickedToday(object sender)
        {
            if (y == 9999) { return; }
            string date1 = Settings.Default.link1clickeddate;
            string date2 = Settings.Default.link2clickeddate;
            string date3 = Settings.Default.link3clickeddate;
            string date4 = Settings.Default.link4clickeddate;
            string date5 = Settings.Default.link5clickeddate;
            string[] date = { date1, date2, date3, date4,date5 };
            int index = y - 1;
            string s = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));
            string test = date[index];
            bool result = s.Equals(test, StringComparison.Ordinal);
            if (!string.IsNullOrEmpty(MyURL) && result == false)
            {
                clicked = false;
            }
            else
            {
                clicked = true;
            }
        }

        private void LoadImageFolder(string folder)
        {
            ErrorText.Visibility = Visibility.Collapsed;
            var sw = Stopwatch.StartNew();
            if (!Path.IsPathRooted(folder))
                folder = Path.Combine(Environment.CurrentDirectory, folder);
            if (!Directory.Exists(folder))
            {
                ErrorText.Text = "The specified folder does not exist: " + Environment.NewLine + Environment.CurrentDirectory + folder;
                ErrorText.Visibility = Visibility.Visible;
                y = 9999;
                return;
            }
            //Random r = new Random();
            //orderby r.Next()
            string path = folder; // or whatever 
            if (!Directory.Exists(path))
            {
                ch = Directory.CreateDirectory(path);
                ch.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            else
            {
                ch = new DirectoryInfo(path)
                {
                    Attributes = FileAttributes.Directory | FileAttributes.Hidden
                };

            }
            var sources = from file in new DirectoryInfo(folder).GetFiles().AsParallel()
                          where ValidImageExtensions.Contains(file.Extension, StringComparer.InvariantCultureIgnoreCase)
                          orderby file.FullName
                          select CreateImageSource(file.FullName, true);
            Images.Clear();
            Images.AddRange(sources);
            sw.Stop();
            Console.WriteLine("Total time to load {0} images: {1}ms", Images.Count, sw.ElapsedMilliseconds);
        }
        public static string TruncateLongString(string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        private ImageSource CreateImageSource(string file, bool forcePreLoad)
        {
            if (forcePreLoad)
            {

                var src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(file, UriKind.Absolute);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                src.Freeze();
                return src;
            }
            else
            {
                var src = new BitmapImage(new Uri(file, UriKind.Absolute));
                src.Freeze();
                return src;
            }
        }

        private void timerImageChange_Tick(object sender, EventArgs e)
        {
            if (pause == false)
            {
                PlaySlideShow();
            }
        }

        private void PlaySlideShow()
        {
            try
            {

                if (Images.Count == 0)
                    return;
                var oldCtrlIndex = CurrentCtrlIndex;
                CurrentCtrlIndex = (CurrentCtrlIndex + 1) % 2;
                CurrentSourceIndex = (CurrentSourceIndex + 1) % Images.Count;

                Image imgFadeOut = ImageControls[oldCtrlIndex];
                Image imgFadeIn = ImageControls[CurrentCtrlIndex];
                ImageSource newSource = Images[CurrentSourceIndex];
                imgFadeIn.Source = newSource;

                TransitionType = TransitionEffects[EffectIndex].ToString();

                Storyboard StboardFadeOut = (Resources[string.Format("{0}Out", TransitionType.ToString())] as Storyboard).Clone();
                StboardFadeOut.Begin(imgFadeOut);
                Storyboard StboardFadeIn = Resources[string.Format("{0}In", TransitionType.ToString())] as Storyboard;
                StboardFadeIn.Begin(imgFadeIn);
                if (y < Images.Count && y > 0) { 
                y++;
                }
                else
                {
                    y = 1;
                }
                switch (y)
                {
                    case 1:
                        MyURL = "http://Link1.com";//"http://bit.ly/2XiJBJV"; // #Atmology
                        break;
                    case 2:
                        MyURL = "http://Link2.com";
                        break;
                    case 3:
                        MyURL = "http://Link3.com"; //"http://vape-assistant.com";
                        break; 
                    case 4:
                        MyURL = "http://Link4.com"; //"http://facebook.com";
                        break;
                    case 5:
                        MyURL = "http://Link5.com"; //"http://facebook.com";
                        break;
                    case 6:
                        MyURL = "https://www.facebook.com/messages/t/VapeAssistant"; // #PM
                        break; 
                    default:
                        break;
                }
                
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}