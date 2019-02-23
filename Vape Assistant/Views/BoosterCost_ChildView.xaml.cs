using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for BoosterCost_ChildView.xaml
    /// </summary>
    public partial class BoosterCost_ChildView : UserControl
    {
        public BoosterCost_ChildView()
        {
            InitializeComponent();
            lbl_base.Visibility = Visibility.Collapsed;
            lbl_nicml.Visibility = Visibility.Collapsed;
            lbl_nicbtl.Visibility = Visibility.Collapsed;
            lbl_totcost.Visibility = Visibility.Collapsed;
            tbl_Baseml.Visibility = Visibility.Collapsed;
            tbl_NicotineMl.Visibility = Visibility.Collapsed;
            tbl_NicotineBottles.Visibility = Visibility.Collapsed;
            tbl_BoosterCost.Visibility = Visibility.Collapsed;
        }
        private static string fixdec(string value)
        {
            string input = value;
            string pattern = ".";
            string replacement = ",";
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
            lbl_base.Visibility = Visibility.Collapsed;
            lbl_nicml.Visibility = Visibility.Collapsed;
            lbl_nicbtl.Visibility = Visibility.Collapsed;
            lbl_totcost.Visibility = Visibility.Collapsed;
            tbl_Baseml.Visibility = Visibility.Collapsed;
            tbl_NicotineMl.Visibility = Visibility.Collapsed;
            tbl_NicotineBottles.Visibility = Visibility.Collapsed;
            tbl_BoosterCost.Visibility = Visibility.Collapsed;
            tbl_Baseml.Text = null;
            tbl_NicotineMl.Text = null;
            tbl_NicotineBottles.Text = null;
            tbl_BoosterCost.Text = null;
        }

        private void calc_cost_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            #region 10c
            double Targetml, Targetlvl, boosterlvl, cost_bottle;
            Targetml = Convert.ToDouble(cost_Target_ml.Text);
            Targetlvl = Convert.ToDouble(cost_nic_level.Text);
            boosterlvl = Convert.ToDouble(nic_booster_level.Text);
            cost_bottle = Convert.ToDouble(cost_booster_bottle.Text);
            if (Targetml == 0 || Targetlvl == 0 || boosterlvl == 0 || cost_bottle == 0) return;
            if (Targetlvl > boosterlvl) { return; }
            lbl_base.Visibility = Visibility.Visible;
            lbl_nicml.Visibility = Visibility.Visible;
            lbl_nicbtl.Visibility = Visibility.Visible;
            lbl_totcost.Visibility = Visibility.Visible;
            tbl_Baseml.Visibility = Visibility.Visible;
            tbl_NicotineMl.Visibility = Visibility.Visible;
            tbl_NicotineBottles.Visibility = Visibility.Visible;
            tbl_BoosterCost.Visibility = Visibility.Visible;
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
            int start = textBox.SelectionStart;
            int length = textBox.SelectionLength;
            string value = textBox.Text;
            if (textBox.SelectionLength > 3)
            {
                textBox.Text = fixdec(value);
            }

            // restore cursor position and selection
            textBox.Select(start, length);
        }

    }
}

