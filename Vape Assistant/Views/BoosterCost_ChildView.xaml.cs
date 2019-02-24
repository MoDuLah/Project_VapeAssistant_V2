using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for BoosterCost_ChildView.xaml
    /// </summary>
    public partial class BoosterCost_ChildView : UserControl
    {
        public string title,message;
        public int autotimeout = 5000;

        public BoosterCost_ChildView()
        {
            InitializeComponent();
            gb_base.Visibility = Visibility.Collapsed;
            gb_nicml.Visibility = Visibility.Collapsed;
            gb_nicbtl.Visibility = Visibility.Collapsed;
            gb_totcost.Visibility = Visibility.Collapsed;
        }
        private static string fixdec(string value)
        {
            string input = value;
            string pattern = ",";
            string replacement = ".";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(input, replacement);
            return result;
        }

        private void calc_nic_reset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            cost_Target_ml.Text = "0";
            cost_nic_level.Text = "0";
            nic_booster_level.Text = "0";
            cost_booster_bottle.Text = "0";
            gb_base.Visibility = Visibility.Collapsed;
            gb_nicml.Visibility = Visibility.Collapsed;
            gb_nicbtl.Visibility = Visibility.Collapsed;
            gb_totcost.Visibility = Visibility.Collapsed;
            tbl_Baseml.Text = null;
            tbl_NicotineMl.Text = null;
            tbl_NicotineBottles.Text = null;
            tbl_BoosterCost.Text = null;
        }

        private void calc_cost_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            #region 10c
            cost_nic_level.Text = cost_nic_level.Text.Replace(",", ".");
            cost_booster_bottle.Text = cost_booster_bottle.Text.Replace(",", ".");
            double Targetml, Targetlvl, boosterlvl, cost_bottle;
            try
            {
                Targetml = Convert.ToDouble(cost_Target_ml.Text);
            }
            catch (Exception ex)
            {
                title = "Error";
                AutoClosingMessageBox.Show(cost_Target_ml + "\n" + ex.Message, title, autotimeout);
                return;
            }
            try
            {
                Targetlvl = Convert.ToDouble(cost_nic_level.Text);
            }
            catch (Exception ex)
            {
                title = "Error";
                AutoClosingMessageBox.Show(cost_nic_level + "\n" + ex.Message, title, autotimeout);
                return;
            }
            try
            {
                boosterlvl = Convert.ToDouble(nic_booster_level.Text);
            }
            catch (Exception ex)
            {
                title = "Error ";
                AutoClosingMessageBox.Show(nic_booster_level.Text + "\n" + ex.Message, title, autotimeout);
                return;
            }
            try
            {
                cost_bottle = Convert.ToDouble(cost_booster_bottle.Text);
            }
            catch (Exception ex)
            {
                title = "Error";
                AutoClosingMessageBox.Show(cost_booster_bottle.Text + "\n"+ ex.Message, title, autotimeout);
                return;
            }
            if (Targetml == 0 || Targetlvl == 0 || boosterlvl == 0 || cost_bottle == 0) return;
            if (Targetlvl > boosterlvl) {
                title = "Error";
                message = $"{Targetlvl} < {boosterlvl}";
                AutoClosingMessageBox.Show(message, title, autotimeout);
                return;
            }
            gb_base.Visibility = Visibility.Visible;
            gb_nicml.Visibility = Visibility.Visible;
            gb_nicbtl.Visibility = Visibility.Visible;
            gb_totcost.Visibility = Visibility.Visible;
            double tbl_10b, tbl_10c, tbl_10e, tbl_10f;
                                 
            if ((cost_nic_level.Text != "") && (nic_booster_level.Text != ""))
            {
                double temp = Targetlvl / boosterlvl;
                double costperml = cost_bottle / 10;

                //--10ml--
                tbl_10c = temp * Targetml;
                tbl_10b = Targetml - tbl_10c;
                tbl_10e = Math.Ceiling((tbl_10c * 1) / 10);
                tbl_10f = tbl_10e * cost_bottle;
                tbl_Baseml.Text = Convert.ToString(Math.Round(tbl_10b, 2));
                tbl_NicotineMl.Text = Convert.ToString(Math.Round(tbl_10c, 2));
                tbl_NicotineBottles.Text = Convert.ToString(Math.Round(tbl_10e, 2));
                tbl_BoosterCost.Text = Convert.ToString(Math.Round(tbl_10f, 2));
                #endregion
            }
        }


        private void txtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (Convert.ToDouble(textBox.Text) == 0)
            {
                textBox.Text = "";
            }
        }

        private void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text)) { return; }
            int count = 0;
            string test = textBox.Text;
            if (test.Contains(".")) { 
                count = test.Split('.').Length - 1;
            }
            if (test.Contains(","))
            {
                count = test.Split(',').Length - 1;
            }
            if (count > 1)
            {
                AutoClosingMessageBox.Show("There are too many decimal points.", "Error", autotimeout);
                return;
            }
            int start = textBox.SelectionStart;
            int length = textBox.SelectionLength;
            string value = textBox.Text;
            if (textBox.SelectionLength >= 2)
            {
                textBox.Text = fixdec(value);
            }

            // restore cursor position and selection
            textBox.Select(start, length);
        }

        private static bool IsTextAllowed(string text)
        {
            int count = 0;
            int autotimeout = 5000;



            string CurrentCulture = Settings.Default.Culture;
            if (CurrentCulture == "en-US")
            {
                Regex regex = new Regex(@"[^0-9.$]+"); //regex that matches disallowed text
                if (text.Contains("."))
                {
                    count = text.Split('.').Length - 1;
                }
                if (count > 1)
                {
                    AutoClosingMessageBox.Show("There are too many decimal points.", "Error", autotimeout);
                    return regex.IsMatch(text);
                }
                return !regex.IsMatch(text);
            }
            else
            {
                Regex regex = new Regex("[^0-9,$]+"); //regex that matches disallowed text
                if (text.Contains(","))
                {
                    count = text.Split(',').Length - 1;
                }
                if (count > 1)
                {
                    AutoClosingMessageBox.Show("There are too many decimal points.", "Error", autotimeout);
                    return regex.IsMatch(text);
                }
                return !regex.IsMatch(text);
            }
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}