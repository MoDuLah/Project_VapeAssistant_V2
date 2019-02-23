using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Vape_Assistant.Properties;

namespace Vape_Assistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Among other settings, this code may be used
            CultureInfo ci = CultureInfo.CurrentUICulture;

            try
            {
                //Override the default culture with something from app settings
                ci = new CultureInfo(Settings.Default.Culture);
                ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            }
            catch { }
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            //MessageBox.Show(ci.ToString());
 
            //Here is the important part for databinding default converters
            FrameworkElement.LanguageProperty.OverrideMetadata(
                    typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(ci.IetfLanguageTag)));
            //Other initialization things
            //string clt = 
            //MessageBox.Show(CultureInfo.CurrentCulture.ToString());
            //VapeAssistant.Properties.Resources.Culture = new CultureInfo();
            //VapeAssistant.Properties.Resources.Culture = new CultureInfo("en-US");
            //VapeAssistant.Properties.Resources.Culture = new CultureInfo("el-GR");
        }
        //rest of the part in this method remained same 
    }
}

