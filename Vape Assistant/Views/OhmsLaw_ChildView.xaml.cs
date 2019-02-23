using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for FirstChildView.xaml
    /// </summary>
    public partial class OhmsLaw_ChildView : UserControl
    {
        public int autotimeout = 5000;
        string CurrentCulture = Settings.Default.Culture;

        public OhmsLaw_ChildView()
        {
            InitializeComponent();
            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                //Handler attach - will not be done if not needed
                PreviewKeyDown += new KeyEventHandler(ShellView.ShellView_PreviewKeyDown);
            }
        }


        private void ohm_reset_Click(object sender, RoutedEventArgs e)
        {
            ampsslider.Value = 0;
            voltslider.Value = 0;
            resslider.Value = 0;
            wattslider.Value = 0;
            ampsslider.IsEnabled = true;
            voltslider.IsEnabled = true;
            resslider.IsEnabled = true;
            wattslider.IsEnabled = true;
            ampsv.IsEnabled = true;
            voltv.IsEnabled = true;
            resv.IsEnabled = true;
            wattv.IsEnabled = true;
            ohm_submit.IsEnabled = true;
            ohm_reset.IsEnabled = false;
        }

        private void ohm_submit_Click(object sender, RoutedEventArgs e)
        {
            string title = "";
            string message = "";
            {
                if (string.IsNullOrEmpty(ampsv.Text))
                {
                    ampsslider.Value = 0;
                }
                if (string.IsNullOrEmpty(voltv.Text))
                {
                    voltslider.Value = 0;
                }
                if (string.IsNullOrEmpty(resv.Text))
                {
                    resslider.Value = 0;
                }
                if (string.IsNullOrEmpty(wattv.Text))
                {
                    wattslider.Value = 0;
                }

                double ampsvar, voltvar, resvar, wattvar;

                ampsvar = ampsslider.Value;
                voltvar = voltslider.Value;
                resvar = resslider.Value;
                wattvar = wattslider.Value;

                if ((resvar > 0) && (voltvar > 0) ||
                    (voltvar > 0) && (wattvar > 0) ||
                    (wattvar > 0) && (resvar > 0) ||
                    (ampsvar > 0) && (resvar > 0) ||
                    (ampsvar > 0) && (voltvar > 0) ||
                    (ampsvar > 0) && (wattvar > 0))
                {
                    if ((resvar > 0) && (voltvar > 0))
                    {
                        wattvar = Math.Pow(voltvar, 2) / resvar;
                        ampsvar = wattvar / voltvar;
                        wattslider.Value = wattvar;
                        ampsslider.Value = ampsvar;
                    }
                    if ((voltvar > 0) && (wattvar > 0))
                    {
                        ampsvar = wattvar / voltvar;
                        resvar = Math.Pow(voltvar, 2) / wattvar;
                        ampsslider.Value = ampsvar;
                        resslider.Value = resvar;
                    }
                    if ((wattvar > 0) && (resvar > 0))
                    {

                        voltvar = Math.Sqrt(wattvar * resvar);
                        ampsvar = wattvar / voltvar;
                        voltslider.Value = voltvar;
                        ampsslider.Value = ampsvar;
                    }
                    if ((ampsvar > 0) && (resvar > 0))
                    {
                        voltvar = ampsvar * resvar;
                        wattvar = Math.Pow(ampsvar, 2) * resvar;
                        voltslider.Value = voltvar;
                        wattslider.Value = wattvar;
                    }
                    if ((ampsvar > 0) && (voltvar > 0))
                    {
                        resvar = Math.Round(voltvar / ampsvar, 2);
                        wattvar = Math.Round(voltvar * ampsvar, 2);
                        resslider.Value = resvar;
                        wattslider.Value = wattvar;
                    }
                    else if ((ampsvar > 0) && (wattvar > 0))
                    {
                        voltvar = Math.Round(wattvar / ampsvar, 2);
                        resvar = Math.Round(wattvar / Math.Pow(ampsvar, 2), 2);
                        resslider.Value = resvar;
                        voltslider.Value = voltvar;
                    }
                    if (CurrentCulture == "en-US")
                    {
                        title = "Caution!";
                        message = "Ampere exceed the 20A (continuous discharge rate) limit.\nBeyond this point most batteries will not cope well.\n#VapeSafe";
                    }
                    if (CurrentCulture == "el-GR")
                    {
                        title = "Προσοχή!";
                        message = "Τα αμπέρ ξεπερνούν το όριο των 20A (Συνεχούς Ρυθμού Αποφόρτισης).\nΠέρα από αυτό το σημείο οι περισσότερες μπαταρίες" +
                            " θα δυσκολευτούν να ανταπεξέλθουν.";
                    }
                    if (ampsvar > 20.0)
                    {
                        AutoClosingMessageBox.Show(message, title, autotimeout);
                    }
                    ohm_submit.IsEnabled = false;
                    ampsslider.IsEnabled = false;
                    voltslider.IsEnabled = false;
                    resslider.IsEnabled = false;
                    wattslider.IsEnabled = false;
                    ampsv.IsEnabled = false;
                    voltv.IsEnabled = false;
                    resv.IsEnabled = false;
                    wattv.IsEnabled = false;
                    ohm_reset.IsEnabled = true;
                }
            }
        }

        private void ampsv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ampsv.Text))
            {
                ampsslider.Value = 0;
            }
        }

        private void voltv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(voltv.Text))
            {
                voltslider.Value = 0;
            }
        }

        private void resv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(resv.Text))
            {
                resslider.Value = 0;
            }
        }

        private void wattv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(wattv.Text))
            {
                wattslider.Value = 0;
            }
        }

        public void onlynumwithsinglepoint(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Back || e.Key == Key.Decimal || e.Key == Key.OemComma))
            { e.Handled = true; }
            TextBox txtDecimal = sender as TextBox;
            if (e.Key == Key.Decimal && txtDecimal.Text.Contains("."))
            {
                e.Handled = true;

            }
        }

        private static bool IsTextAllowed(string text)
        {
            string CurrentCulture = Settings.Default.Culture;
            if (CurrentCulture == "en-US")
            {
                Regex regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
                return !regex.IsMatch(text);
            }
            else
            {
                Regex regex = new Regex("[^0-9,]+"); //regex that matches disallowed text
                return !regex.IsMatch(text);
            }
        }


        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }


        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
