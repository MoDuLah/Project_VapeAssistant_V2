using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for BasesMix_ChildView.xaml
    /// </summary>
    public partial class BasesMix_ChildView : UserControl
    {
        public int autotimeout = 5000;
        string CurrentCulture = Settings.Default.Culture;
        public BasesMix_ChildView()
        {
            InitializeComponent();
            mb_submit.IsEnabled = true;
            mb_reset.IsEnabled = false;
            EventManager.RegisterClassHandler(typeof(TextBox), GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus));
        }
        void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null && !textBox.IsReadOnly && e.KeyboardDevice.IsKeyDown(Key.Tab))
                textBox.SelectAll();
        }

        void ClearTextBoxes(DependencyObject obj)
        {
            if (obj is TextBox tb)
            {
                if (tb.IsReadOnly == false)
                {
                    tb.Text = "";
                }
                if (tb.IsReadOnly == true)
                {
                    tb.Visibility = Visibility.Hidden;
                }
            }
            if (obj is ComboBox cb)
            {
                cb.SelectedIndex = -1;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj as DependencyObject); i++)
                ClearTextBoxes(VisualTreeHelper.GetChild(obj, i));
        }

        private void BaseMixnic_style_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox Combobox = (ComboBox)sender;
            mixNic_amount.Text = "0";
            switch (Combobox.SelectedIndex)
            {
                case 0:
                    mixNic_PG.Text = "100";
                    mixNic_VG.Text = "0";
                    break;
                case 1:
                    mixNic_PG.Text = "70";
                    mixNic_VG.Text = "30";
                    break;
                case 2:
                    mixNic_PG.Text = "50";
                    mixNic_VG.Text = "50";
                    break;
                case 3:
                    mixNic_PG.Text = "30";
                    mixNic_VG.Text = "70";
                    break;
                case 4:
                    mixNic_PG.Text = "0";
                    mixNic_VG.Text = "100";
                    break;

                default:
                    string Message = "";
                    string Title = "";
                    if (CurrentCulture == "en-US")
                    {
                        Title = "Error";
                        Message = "Choose Nicotine Type";
                    }
                    if (CurrentCulture == "el-GR")
                    {
                        Title = "Σφάλμα";
                        Message = "Επιλέξτε Τύπο Νικοτίνης!";
                    }
                    AutoClosingMessageBox.Show(Message, Title, autotimeout);
                    break;
            }
        }

        private void mixNic_amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                var s = Convert.ToInt32(mixNic_amount.Text);
                if (s >= 0)
                {
                    var ss = Convert.ToString(s * 10);
                    mixNic_ml.Text = ss;
                }
            }
        }

         private void mb_submit_Click(object sender, RoutedEventArgs e)
        {
            if  (mb_reset.IsEnabled == false)
            {
                mb_reset.IsEnabled = true;
            }
            if (string.IsNullOrEmpty(mixNic_ml.Text) && string.IsNullOrEmpty(mixNic_PG.Text))
            {
                mixNic_amount.Text = "0";
                mixNic_ml.Text = "0";
                mixNic_PG.Text = "0";
                mixNic_VG.Text = "100";
            }
            if (string.IsNullOrEmpty(mixBase1.Text) && string.IsNullOrEmpty(mixBase1_PG.Text))
            {
                mixBase1.Text = "0";
                mixBase1_PG.Text = "0";
                mixBase1_Nic.Text = "0";
            }
            if (string.IsNullOrEmpty(mixBase2.Text) && string.IsNullOrEmpty(mixBase2_PG.Text))
            {
                mixBase2.Text = "0";
                mixBase2_PG.Text = "0";
                mixBase2_Nic.Text = "0";
            }
            if (string.IsNullOrEmpty(mixBase3.Text) && string.IsNullOrEmpty(mixBase3_PG.Text))
            {
                mixBase3.Text = "0";
                mixBase3_PG.Text = "0";
                mixBase3_Nic.Text = "0";
            }
            if (string.IsNullOrEmpty(mixBase4.Text) && string.IsNullOrEmpty(mixBase4_PG.Text))
            {
                mixBase4.Text = "0";
                mixBase4_PG.Text = "0";
                mixBase4_Nic.Text = "0";
            }
            double m0 = Convert.ToDouble(mixNic_ml.Text);
            double m1 = Convert.ToDouble(mixBase1.Text);
            double m2 = Convert.ToDouble(mixBase2.Text);
            double m3 = Convert.ToDouble(mixBase3.Text);
            double m4 = Convert.ToDouble(mixBase4.Text);
            double p0 = Convert.ToDouble(mixNic_PG.Text);
            double p1 = Convert.ToDouble(mixBase1_PG.Text);
            double p2 = Convert.ToDouble(mixBase2_PG.Text);
            double p3 = Convert.ToDouble(mixBase3_PG.Text);
            double p4 = Convert.ToDouble(mixBase4_PG.Text);
            double v0 = Convert.ToDouble(mixNic_VG.Text);
            double v1 = Convert.ToDouble(mixBase1_VG.Text);
            double v2 = Convert.ToDouble(mixBase2_VG.Text);
            double v3 = Convert.ToDouble(mixBase3_VG.Text);
            double v4 = Convert.ToDouble(mixBase4_VG.Text);
            double m0l = Convert.ToDouble(mixNicLevel.Text);
            double m1l = Convert.ToDouble(mixBase1_Nic.Text);
            double m2l = Convert.ToDouble(mixBase2_Nic.Text);
            double m3l = Convert.ToDouble(mixBase3_Nic.Text);
            double m4l = Convert.ToDouble(mixBase4_Nic.Text);

            double addm = m0 + m1 + m2 + m3 + m4;
            if (addm == 0)
            {
                AutoClosingMessageBox.Show(Properties.Resources.BaseMixErrorMsg, "Error", autotimeout);
                return;
            }
            //double addn = m0l + m1l + m2l + m3l;
            var mnp0 = m0 / addm; //Mix Nic Percent
            var mnpml = Math.Round((p0 * m0) / 100, 2); //Mix Nic PG ML
            var mnvml = Math.Round((v0 * m0) / 100, 2); //Mix Nic VG ML
            var mnml = Math.Round(mnp0 * m0l, 2); //Mix Nic mg/ml
            var mb1p = m1 / addm; //Mix Base 1 Percent
            var mb1pml = Math.Round((p1 * m1) / 100, 2); //Mix Base1 PG ML
            var mb1vml = Math.Round((v1 * m1) / 100, 2); //Mix Base1 VG ML
            var mb1nml = Math.Round(mb1p * m1l, 2); //Mix Base1 Nic mg/ml
            var mb2p = m2 / addm; //Mix Base 2 Percent
            var mb2pml = Math.Round((p2 * m2) / 100, 2); //Mix Base2 PG ML
            var mb2vml = Math.Round((v2 * m2) / 100, 2); //Mix Base2 VG ML
            var mb2nml = Math.Round(mb2p * m2l, 2); //Mix Base2 Nic mg/ml
            var mb3p = m3 / addm; //Mix Base 3 Percent
            var mb3pml = Math.Round((p3 * m3) / 100, 2); //Mix Base3 PG ML
            var mb3vml = Math.Round((v3 * m3) / 100, 2); //Mix Base3 VG ML
            var mb3nml = Math.Round(mb3p * m3l, 2); //Mix Base3 Nic mg/ml
            var mb4p = m4 / addm; //Mix Base 4 Percent
            var mb4pml = Math.Round((p4 * m4) / 100, 2); //Mix Base4 PG ML
            var mb4vml = Math.Round((v4 * m4) / 100, 2); //Mix Base4 VG ML
            var mb4nml = Math.Round(mb4p * m4l, 2); //Mix Base4 Nic mg/ml


            var mtpml = mnpml + mb1pml + mb2pml + mb3pml + mb4pml; //Mix Total PG ML
            var mtpp = mtpml / addm; //Mix Total PG percent
            var mtvml = mnvml + mb1vml + mb2vml + mb3vml + mb4vml; //Mix Total VG ML
            var mtvp = mtvml / addm; //Mix Total VG percent
            var mtnmg = mnml + mb1nml + mb2nml + mb3nml + mb4nml;// Mix Total Nic MG

            mix_totalnic.Text = Math.Round(mtnmg, 2) + "mg/ml";
            mix_pgvgratio.Text = Math.Round((mtpp * 100), 0) + "/" + Math.Round((mtvp * 100), 0);
            mix_pgvgvolume.Text = mtpml + "/" + mtvml;
            mix_totalvolume.Text = addm + "ml";

            mixBase1_PG.IsTabStop = true;
            mixBase1_VG.IsTabStop = true;
            mixBase2_PG.IsTabStop = true;
            mixBase2_VG.IsTabStop = true;
            mixBase3_PG.IsTabStop = true;
            mixBase3_VG.IsTabStop = true;
            mixBase4_PG.IsTabStop = true;
            mixBase4_VG.IsTabStop = true;
        }

        private void mb_reset_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBoxes(this);
            mb_submit.IsEnabled = true;
            mb_reset.IsEnabled = false;
        }
        private void MixBase1_PG_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase1_VG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }

        private void MixBase1_VG_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);

            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase1_PG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }
        private void MixBase2_PG_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase2_VG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }

        private void MixBase2_VG_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase2_PG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }
        private void MixBase3_PG_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase3_VG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }

        private void MixBase3_VG_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase3_PG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }
        private void MixBase4_PG_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase4_VG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }

        private void MixBase4_VG_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase4_PG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }
        private static bool IsDecAllowed(string text)
        {
            string CurrentCulture = Settings.Default.Culture;
            Regex regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void MixBase1_PG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                mixBase1_VG.IsTabStop = true;
                return;
            }
            else
            {

                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;
                //fixdec(value);
                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase1_VG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                mixBase1_VG.IsTabStop = false;
            }
        }

        private void MixBase1_VG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                mixBase1_PG.IsTabStop = true;
                return;
            }
            else
            {

                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;
                //fixdec(value);
                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase1_PG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                mixBase1_PG.IsTabStop = false;
            }
        }
        private void MixBase2_PG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                mixBase2_VG.IsTabStop = true;
                return;
            }
            else
            {

                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;
                //fixdec(value);
                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase2_VG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                mixBase2_VG.IsTabStop = false;
            }
        }

        private void MixBase2_VG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                mixBase2_PG.IsTabStop = true;
                return;
            }
            else
            {

                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;
                //fixdec(value);
                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase2_PG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                mixBase2_PG.IsTabStop = false;
            }
        }
        private void MixBase3_PG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                mixBase3_VG.IsTabStop = true;
                return;
            }
            else
            {

                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;
                //fixdec(value);
                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase3_VG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                mixBase3_VG.IsTabStop = false;
            }
        }

        private void MixBase3_VG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                mixBase3_PG.IsTabStop = true;
                return;
            }
            else
            {

                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;
                //fixdec(value);
                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase3_PG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                mixBase3_PG.IsTabStop = false;
            }
        }
        private void MixBase4_PG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                mixBase4_VG.IsTabStop = true;
                return;
            }
            else
            {

                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;
                //fixdec(value);
                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase4_VG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                mixBase4_VG.IsTabStop = false;
            }
        }

        private void MixBase4_VG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                mixBase4_PG.IsTabStop = true;
                return;
            }
            else
            {

                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;
                //fixdec(value);
                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                mixBase4_PG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                mixBase4_PG.IsTabStop = false;
            }
        }
    }
}
