using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for NinthChildView.xaml
    /// </summary>
    public partial class About_ChildView : UserControl
    {
        Version version = Assembly.GetExecutingAssembly().GetName().Version;
        public About_ChildView()
        {
            InitializeComponent();
            Versioned_Text.Text = string.Format(Properties.Resources.SoftwareVersion + " {0}.{1}.{2}",
              version.Major, version.Minor, version.Build);

        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void BtnDonate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string url = "";

            string business = "uf1r_nitrous@hotmail.com";  // your paypal email
            string description = "Beer%20Donation";            // '%20' represents a space. remember HTML!
            string country = "GR";                  // AU, US, etc.
            string currency = "EUR";                 // AUD, USD, etc.

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + description +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";

            Process.Start(url);

        }
    }
}
