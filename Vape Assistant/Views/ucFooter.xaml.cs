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
using System.Net;
using System.Reflection;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for ucFooter.xaml
    /// </summary>
    public partial class ucFooter : UserControl
    {
        Version version = Assembly.GetExecutingAssembly().GetName().Version;
        public static List<string> listA = new List<string>();
        public static List<string> listB = new List<string>();
        private DispatcherTimer timerImageChange;
        private Image[] ImageControls;
        private List<ImageSource> Images = new List<ImageSource>();
        private static string[] ValidImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
        private static string[] TransitionEffects = new[] { "Fade" };
        private string TransitionType, strImagePath = "";
        private int CurrentSourceIndex, CurrentCtrlIndex, EffectIndex = 0, IntervalTimer = 1;
        public string removeString = "";
        public int index = 0;
        public string MyURL,MyURL2;
        public int y = 0;
        public bool clicked = false;
        public bool pause = false;
        DirectoryInfo ch;

        public ucFooter()
        {
            InitializeComponent();

            //Download Ads
            GetImagesInFolder();

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

        private void GetImagesInFolder()
        {
            int i = 0;
            string v = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            TextBox[] adlink = { adlink1, adlink2, adlink3, adlink4, adlink5, adlink6, adlink7, adlink8, adlink9, adlink10 };
            string BannerPath = AppDomain.CurrentDomain.BaseDirectory + @"Images\banners\";
            string fileName = BannerPath + "update." + v + ".txt";
            string timeStamp = DateTime.Now.ToString("yyyy.MM.dd");
            string remoteaddress = "https://vapeassistant.000webhostapp.com/updates/" + @"update.txt";
            string localpath = BannerPath + @"update." + v + ".txt";
            string extension = ".png";
            WebClient Client = new WebClient();
            if (!File.Exists(fileName))
            {
                try
                {
                    Client.DownloadFile(remoteaddress, localpath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            using (var reader = new StreamReader(localpath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        i++;

                        var values = line.Split(';');
                        foreach (var value in values)
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (value.StartsWith("https://vapeassistant"))
                                {
                                    if (!File.Exists(BannerPath + i + extension))
                                    {
                                        try
                                        {
                                            Client.DownloadFile(value, BannerPath + i + extension);
                                        }
                                        catch (Exception dx)
                                        {
                                            MessageBox.Show(dx.Message);
                                            return;
                                        }
                                    }
                                }
                                else if (value.StartsWith("https://bit.ly"))
                                {
                                    listA.Add(value);
                                }
                                else
                                {
                                    listB.Add(value);
                                }
                            }
                        }

                    }
                }
            }
            Client.Dispose();
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
            clickedToday(y);
            if (!string.IsNullOrEmpty(MyURL) && clicked == false) { 
            Process.Start(new ProcessStartInfo(MyURL));
                //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                switch (y)
                {
                    case 0:
                        Settings.Default.link0clickeddate = s;
                        Settings.Default.Save();
                        break;
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
                    case 6:
                        Settings.Default.link6clickeddate = s;
                        Settings.Default.Save();
                        break;
                    case 7:
                        Settings.Default.link7clickeddate = s;
                        Settings.Default.Save();
                        break;
                    case 8:
                        Settings.Default.link8clickeddate = s;
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
                    Process.Start(new ProcessStartInfo(MyURL2));
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
        private void clickedToday(int y)
        {
            if (y == 9999) { return; }
            string date0 = Settings.Default.link0clickeddate;
            string date1 = Settings.Default.link1clickeddate;
            string date2 = Settings.Default.link2clickeddate;
            string date3 = Settings.Default.link3clickeddate;
            string date4 = Settings.Default.link4clickeddate;
            string date5 = Settings.Default.link5clickeddate;
            string date6 = Settings.Default.link6clickeddate;
            string date7 = Settings.Default.link7clickeddate;
            string date8 = Settings.Default.link8clickeddate;
            string date9 = Settings.Default.link9clickeddate;
            string date10 = Settings.Default.link10clickeddate;
            string[] date = { date0, date1, date2, date3, date4,date5, date6, date7, date8, date9, date10 };

            string s = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));
            string test = date[y];
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

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            pause = true;
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            pause = false;
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
                if (Images.Count == 0) { 
                    return;
                }
                int oldCtrlIndex = CurrentCtrlIndex;
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
                if (y < Images.Count) { 
                y++;
                }
                else
                {
                    y = 0;
                }
                if (y == Images.Count) { y = 0; }
                MyURL = listA[y];
                MyURL2 = listB[y];

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}