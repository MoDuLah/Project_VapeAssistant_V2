using System;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for OneShots_ChildView.xaml
    /// </summary>
    public partial class OneShots_ChildView : UserControl
    {
        public string connectionString = Settings.Default.VaConnect;
        public string CurrentCulture = Settings.Default.Culture;
        public string message, errmsg;
        public string caption;
        public string title;
        public string query;
        public int autotimeout = 5000;
        public bool oneshot_calced = false;
        public bool isEditing;
        double nic = 1.00925;
        double pg = 1.038;
        double vg = 1.2613;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;

        public OneShots_ChildView()
        {
            InitializeComponent();
            TotalGrams_label.Visibility = Visibility.Collapsed;
            pgBase_label.Visibility = Visibility.Collapsed;
            vgBase_label.Visibility = Visibility.Collapsed;
            snvFlavor_Label.Visibility = Visibility.Collapsed;
            snvNicotine_label.Visibility = Visibility.Collapsed;
            OS_Name_label.Visibility = Visibility.Collapsed;
            nicBooster_label.Visibility = Visibility.Hidden;
        }

        void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && !textBox.IsReadOnly && e.KeyboardDevice.IsKeyDown(Key.Tab))
                textBox.SelectAll();
        }

        private void snvFinalMix_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            double snvFlP = 0;
            double snvMixMl = 0;
            double percent = 0;
            double snvFlvMl = 0;

            if (string.IsNullOrEmpty(snvMlFlv.Text) && (string.IsNullOrEmpty(snvFinalFlv.Text)))
            {
                return;
            }

            if (!string.IsNullOrEmpty(snvMlFlv.Text))
            {
                snvFlvMl = Convert.ToDouble(snvMlFlv.Text);
                if (!string.IsNullOrEmpty(snvFinalMix.Text))
                {
                    snvMixMl = Convert.ToDouble(snvFinalMix.Text);
                }
                if (!string.IsNullOrEmpty(snvFinalFlv.Text))
                {
                    snvFlP = Convert.ToDouble(snvFinalFlv.Text);
                }
                if (snvMixMl > snvFlvMl)
                {
                    percent = Math.Round(((snvFlvMl / snvMixMl) * 100), 2);
                    snvFinalFlv.Text = percent.ToString();
                }

            }
        }

        private void snvFinal_PG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                snvFinal_VG.IsTabStop = true;
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
                snvFinal_VG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                snvFinal_VG.IsTabStop = false;
            }
        }
        private void snvFinal_VG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                snvFinal_PG.TabIndex = 1;
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
                snvFinal_PG.Text = Convert.ToString(100 - Convert.ToDouble(textBox.Text));
                snvFinal_PG.TabIndex = snvFinal_PG.TabIndex + 100;
            }
        }

        private void snvFinal_Nic_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void nic_style_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox Combobox = (ComboBox)sender;
            switch (Combobox.SelectedIndex)
            {
                case -1:
                    if (string.IsNullOrEmpty(snvFinal_Nic.Text))
                    {
                        snvNic_PGpercent.Text = "0";
                        snvNic_VGpercent.Text = "0";
                        break;
                    }
                    if (Convert.ToDouble(snvFinal_Nic.Text) > 0)
                    {
                        message = "";
                        title = "";
                        if (CurrentCulture == "en-US")
                        {
                            title = "Error";
                            message = "Choose Nicotine Type";
                        }
                        if (CurrentCulture == "el-GR")
                        {
                            title = "Σφάλμα";
                            message = "Επιλέξτε Τύπο Νικοτίνης!";
                        }
                        AutoClosingMessageBox.Show(message, title, autotimeout);
                    }
                    else
                    {
                        snvNic_PGpercent.Text = "0";
                        snvNic_VGpercent.Text = "0";
                    }
                    break;
                case 0:
                    snvNic_PGpercent.Text = "100";
                    snvNic_VGpercent.Text = "0";
                    break;
                case 1:
                    snvNic_PGpercent.Text = "70";
                    snvNic_VGpercent.Text = "30";
                    break;
                case 2:
                    snvNic_PGpercent.Text = "50";
                    snvNic_VGpercent.Text = "50";
                    break;
                case 3:
                    snvNic_PGpercent.Text = "30";
                    snvNic_VGpercent.Text = "70";
                    break;
                case 4:
                    snvNic_PGpercent.Text = "0";
                    snvNic_VGpercent.Text = "100";
                    break;

                default:
                    if (Convert.ToDouble(snvFinal_Nic.Text) > 0)
                    {
                        message = "";
                        title = "";
                        if (CurrentCulture == "en-US")
                        {
                            title = "Error";
                            message = "Choose Nicotine Type";
                        }
                        if (CurrentCulture == "el-GR")
                        {
                            title = "Σφάλμα";
                            message = "Επιλέξτε Τύπο Νικοτίνης!";
                        }
                        AutoClosingMessageBox.Show(message, title, autotimeout);
                    }
                    else
                    {

                    }
                    break;
            }
        }

        private void snvMlFlv_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isEditing == true) { return; }
            isEditing = true;
            if (string.IsNullOrEmpty(snvFinalMix.Text))
            {
                snvFinalFlv.Text = "";
                isEditing = false;
                return;
            }
            if (string.IsNullOrEmpty(snvMlFlv.Text))
            {
                snvFinalFlv.Text = "";
                isEditing = false;
                return;
            }
            if (Convert.ToDouble(snvMlFlv.Text) < 0 || Convert.ToDouble(snvMlFlv.Text) > Convert.ToDouble(snvFinalMix.Text))
            {
                snvMlFlv.Text = "";
                snvFinalFlv.Text = "";
                isEditing = false;
                return;
            }

            double snvFlvMl = Convert.ToDouble(snvMlFlv.Text);
            double snvMixMl = Convert.ToDouble(snvFinalMix.Text);
            if (snvMixMl > snvFlvMl)
            {
                double percent = Math.Round(((snvFlvMl / snvMixMl) * 100), 2);
                snvFinalFlv.Text = percent.ToString();
                snvFinalFlv.TabIndex = snvFinalFlv.TabIndex + 100;
            }
            else
            {
                snvFinalFlv.Text = "";
                snvFinalFlv.TabIndex = 4;
            }
            isEditing = false;
        }

        private void snvFinalFlv_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isEditing == true) { return; }
            isEditing = true;
            if (string.IsNullOrEmpty(snvFinalMix.Text))
            {
                snvMlFlv.Text = "";
                isEditing = false;
                return;
            }
            if (string.IsNullOrEmpty(snvFinalFlv.Text))
            {
                snvMlFlv.Text = "";
                isEditing = false;
                return;
            }
            double snvFlvP = Convert.ToDouble(snvFinalFlv.Text);
            double snvMixMl = Convert.ToDouble(snvFinalMix.Text);
            if (snvMixMl > snvFlvP)
            {
                double percent = Math.Round(((snvFlvP / 100) * snvMixMl), 2);
                snvMlFlv.Text = percent.ToString();
            }
            else
            {
                snvMlFlv.Text = "";
            }
            isEditing = false;
        }

        private void calc_ShakeNVape_reset_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBoxes(this);
            pgBase_label.Visibility = Visibility.Collapsed;
            vgBase_label.Visibility = Visibility.Collapsed;
            snvBasPG_ml.Text = "0";
            snvBasVG_ml.Text = "0";
            snvNic_ml.Text = "0";
            snvFlv_ml.Text = "0";
            snvFinal_Grams.Text = "0";
            BasPG_Grams.Text = "0";
            BasVG_Grams.Text = "0";
            snvNic_Grams.Text = "0";
            snvFlv_Grams.Text = "0";
            snvNicLevel.Text = "20";
            snvFinal_VG.Text = "";
            errorcatch.Text = "";
            oneshot_calced = false;
        }
        void ClearTextBoxes(DependencyObject obj)
        {
            if (obj is TextBox tb)
            {
                if (!string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = "";
                }
                if (tb.IsReadOnly == true)
                {
                    if (tb.Name != "snvFinal_VG") { 
                    tb.Visibility = Visibility.Collapsed;
                    }
                }
            }
            if (obj is ComboBox cb)
            {
                cb.SelectedIndex = -1;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj as DependencyObject); i++)
                ClearTextBoxes(VisualTreeHelper.GetChild(obj, i));
        }

        private void calc_ShakeNVape_Click(object sender, RoutedEventArgs e)
        {
            string err1, err2, err3, err4, err5, err6;
            errorcatch.Text = "";
            bool zeronic = false;
            if (snvFinal_Nic.Text == "0" || string.IsNullOrEmpty(snvFinal_Nic.Text))
            {
                zeronic = true;
                snvFinal_Nic.Text = "0";
            }
            if (
                string.IsNullOrEmpty(snvFinalMix.Text) ||
                string.IsNullOrEmpty(snvFinal_PG.Text) ||
                string.IsNullOrEmpty(snvFinal_Nic.Text) ||
                string.IsNullOrEmpty(snvFinalFlv.Text) ||
                (zeronic == false) || (zeronic == true)
               )
            {
                if (string.IsNullOrEmpty(snvFinalMix.Text))
                {
                    errorcatch.Text = errorcatch.Text + "1";
                    err1 = TotalMix_label.Header.ToString();
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                    err1 = "";
                }
                if (string.IsNullOrEmpty(snvFinal_PG.Text))
                {
                    errorcatch.Text = errorcatch.Text + "2";
                    err2 = TotalPG_label.Header.ToString();
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                    err2 = "";
                }
                if (string.IsNullOrEmpty(snvFinal_Nic.Text))
                {
                    errorcatch.Text = errorcatch.Text + "3";
                    err3 = TotalNic_label.Header.ToString();
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                    err3 = "";
                }
                if (nic_style.SelectedIndex < 0 && Convert.ToDouble(snvFinal_Nic.Text) > 0)
                {
                    errorcatch.Text = errorcatch.Text + "4";
                    err4 = nicBooster_label.Header.ToString();
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                    err4 = "";
                }
                if (string.IsNullOrEmpty(snvFinalFlv.Text))
                {
                    errorcatch.Text = errorcatch.Text + "5";
                    err5 = snvFinalFlv_label.Header.ToString();
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                    err5 = "";
                }
                if (string.IsNullOrEmpty(snvFinal_Nic.Text))
                {
                    errorcatch.Text = errorcatch.Text + "6";
                    err6 = TotalNic_label.Header.ToString();
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                    err6 = "";
                }
                string messageSingular = "";
                string messagePlural = "";
                //string caption = "";
                string sep = "\n- ";

                if (CurrentCulture == "en-US")
                {
                    messageSingular = "The following field is empty:\n- ";
                    messagePlural = "The following fields are empty:\n- ";
                    caption = "Error: " + errorcatch.Text;
                }
                if (CurrentCulture == "el-GR")
                {
                    messageSingular = "Το πάρακάτω πεδίο είναι κενό:\n- ";
                    messagePlural = "Τα παρακάτω πεδία είναι κενά:\n- ";
                    caption = "Σφάλμα: " + errorcatch.Text;
                }
                //string endmessagebox = caption + parameters;
                switch (Convert.ToInt32(errorcatch.Text))
                {
                    case 000000:
                        oneshot_calced = true;
                        break;
                    case 000006:
                        MessageBox.Show(messageSingular + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 000050:
                        MessageBox.Show(messageSingular + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 000400:
                        MessageBox.Show(messageSingular + err4, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 003000:
                        MessageBox.Show(messageSingular + err3, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 020000:
                        MessageBox.Show(messageSingular + err2, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 100000:
                        MessageBox.Show(messageSingular + err1, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 000056:
                        MessageBox.Show(messagePlural + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 000406:
                        MessageBox.Show(messagePlural + err4 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 000450:
                        MessageBox.Show(messagePlural + err4 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 000456:
                        MessageBox.Show(messagePlural + err4 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 003006:
                        MessageBox.Show(messagePlural + err3 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 003050:
                        MessageBox.Show(messagePlural + err3 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 003056:
                        MessageBox.Show(messagePlural + err3 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 003400:
                        MessageBox.Show(messagePlural + err3 + sep + err4, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 003406:
                        MessageBox.Show(messagePlural + err3 + sep + err4 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 003450:
                        MessageBox.Show(messagePlural + err3 + sep + err4 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 003456:
                        MessageBox.Show(messagePlural + err3 + sep + err4 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 020006:
                        MessageBox.Show(messagePlural + err2 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 020050:
                        MessageBox.Show(messagePlural + err2 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 020056:
                        MessageBox.Show(messagePlural + err2 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 020400:
                        MessageBox.Show(messagePlural + err2 + sep + err4, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 020406:
                        MessageBox.Show(messagePlural + err2 + sep + err4 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 020450:
                        MessageBox.Show(messagePlural + err2 + sep + err4 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 020456:
                        MessageBox.Show(messagePlural + err2 + sep + err4 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 023000:
                        MessageBox.Show(messagePlural + err2 + sep + err3, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 023006:
                        MessageBox.Show(messagePlural + err2 + sep + err3 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 023050:
                        MessageBox.Show(messagePlural + err2 + sep + err3 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 023056:
                        MessageBox.Show(messagePlural + err2 + sep + err3 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 023400:
                        MessageBox.Show(messagePlural + err2 + sep + err3 + sep + err4, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 023406:
                        MessageBox.Show(messagePlural + err2 + sep + err3 + sep + err4 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 023450:
                        MessageBox.Show(messagePlural + err2 + sep + err3 + sep + err4 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 023456:
                        MessageBox.Show(messagePlural + err2 + sep + err3 + sep + err4 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 100006:
                        MessageBox.Show(messagePlural + err1 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 100050:
                        MessageBox.Show(messagePlural + err1 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 100056:
                        MessageBox.Show(messagePlural + err1 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 100400:
                        MessageBox.Show(messagePlural + err1 + sep + err4, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 100406:
                        MessageBox.Show(messagePlural + err1 + sep + err4 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 100450:
                        MessageBox.Show(messagePlural + err1 + sep + err4 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 100456:
                        MessageBox.Show(messagePlural + err1 + sep + err4 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 103000:
                        MessageBox.Show(messagePlural + err1 + sep + err3, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 103006:
                        MessageBox.Show(messagePlural + err1 + sep + err3 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 103050:
                        MessageBox.Show(messagePlural + err1 + sep + err3 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 103056:
                        MessageBox.Show(messagePlural + err1 + sep + err3 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 103400:
                        MessageBox.Show(messagePlural + err1 + sep + err3 + sep + err4, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 103406:
                        MessageBox.Show(messagePlural + err1 + sep + err3 + sep + err4 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 103450:
                        MessageBox.Show(messagePlural + err1 + sep + err3 + sep + err4 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 103456:
                        MessageBox.Show(messagePlural + err1 + sep + err3 + sep + err4 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 120000:
                        MessageBox.Show(messagePlural + err1 + sep + err2, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 120006:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 120050:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 120056:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 120400:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err4, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 120406:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err4 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 120450:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err4 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 120456:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err4 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 123000:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err3, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 123006:
                        oneshot_calced = false;
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err3 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 123050:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err3 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 123056:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err3 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 123400:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err3 + sep + err4, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 123406:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err3 + sep + err4 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 123450:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err3 + sep + err4 + sep + err5, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                    case 123456:
                        MessageBox.Show(messagePlural + err1 + sep + err2 + sep + err3 + sep + err4 + sep + err5 + sep + err6, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        oneshot_calced = false;
                        return;
                }

                //get total mix
                double snv_FinalMix = Convert.ToDouble(snvFinalMix.Text);
                double snv_NicLevel = Convert.ToDouble(snvNicLevel.Text); //turn nic strength to decimal value
                double snv_Final_Nic = Convert.ToDouble(snvFinal_Nic.Text); //turn Target nic level value
                double snv_FinalFlv = Convert.ToDouble(snvFinalFlv.Text);
                double snv_Nic_ml = Math.Round((snv_Final_Nic / snv_NicLevel) * snv_FinalMix, 1); //done
                double snv_Flv_ml = Math.Round((snv_FinalMix * snv_FinalFlv) / 100); //done
                double snv_NicPGpercent = Convert.ToDouble(snvNic_PGpercent.Text);
                double snv_NicVGpercent = Convert.ToDouble(snvNic_VGpercent.Text);
                double NicPG_ml = (snv_Nic_ml * snv_NicPGpercent) / 100;
                double NicVG_ml = (snv_Nic_ml * snv_NicVGpercent) / 100; //turn PG Percentage into decimal
                double snv_Final_PG = Convert.ToDouble(snvFinal_PG.Text);
                double snv_Final_VG = Convert.ToDouble(snvFinal_VG.Text);
                double snvΒasPG_PGpercent = Convert.ToDouble(100);
                double snvΒasVG_VGpercent = Convert.ToDouble(100);
                double FinalPG_ml = (snv_FinalMix * snv_Final_PG) / 100;
                double FinalVG_ml = (snv_FinalMix * snv_Final_VG) / 100;
                double snv_Flv_PG = Convert.ToDouble(100);
                double snv_Flv_VG = Convert.ToDouble(0);
                double FlvPG_ml = (snv_Flv_ml * snv_Flv_PG) / 100;
                double FlvVG_ml = (snv_Flv_ml * snv_Flv_VG) / 100;
                double snvBasPGml = Math.Round(FinalPG_ml - NicPG_ml - FlvPG_ml, 2);
                double snv_BasPG_ml = Math.Round(FinalPG_ml - NicPG_ml - FlvPG_ml, 2);
                double snv_BasVG_ml = Math.Round(FinalVG_ml - NicVG_ml - FlvVG_ml, 2);

                //turn nic pg/vg percent to decimal value

                // calculate required ml
                this.snvBasPG_ml.Text = Convert.ToString(Math.Round(snv_BasPG_ml, 2));
                this.snvBasVG_ml.Text = Convert.ToString(Math.Round(snv_BasVG_ml, 2));
                this.snvNic_ml.Text = Convert.ToString(Math.Round(snv_Nic_ml, 2));
                this.snvFlv_ml.Text = Convert.ToString(Math.Round(snv_Flv_ml, 2));
                this.BasPG_Grams.Text = Convert.ToString(Math.Round(snv_BasPG_ml * pg, 2));
                this.BasVG_Grams.Text = Convert.ToString(Math.Round(snv_BasVG_ml * vg, 2));
                double niclevelpercentdecimal = (snv_NicLevel / 1000);
                double secondlevelpercentdecimal = 1 - niclevelpercentdecimal;
                double pggrav = (niclevelpercentdecimal * nic) + (secondlevelpercentdecimal * pg);
                double vggrav = (niclevelpercentdecimal * nic) + (secondlevelpercentdecimal * vg);
                double npgg = NicPG_ml * pggrav;
                double nvgg = NicVG_ml * vggrav;

                this.snvNic_Grams.Text = Convert.ToString(Math.Round(npgg + nvgg, 2));
                double fpgg = FlvPG_ml * pg;
                double fvgg = FlvVG_ml * vg;
                this.snvFlv_Grams.Text = Convert.ToString(Math.Round(fpgg + fvgg, 2));
                double TotalPG = (snv_BasPG_ml * pg) + npgg + (FlvPG_ml * pg);
                double TotalVG = (snv_BasVG_ml * vg) + nvgg + (FlvVG_ml * vg);
                this.snvFinal_Grams.Text = Convert.ToString(Math.Round(TotalPG + TotalVG, 2));

                // turn visible the invisible
                TotalGrams_label.Visibility = Visibility.Visible;
                pgBase_label.Visibility = Visibility.Visible;
                vgBase_label.Visibility = Visibility.Visible;
                snvFlavor_Label.Visibility = Visibility.Visible;
                snvNicotine_label.Visibility = Visibility.Visible;
                OS_Name_label.Visibility = Visibility.Visible;
                if (Convert.ToDouble(snvFinal_Nic.Text) > 0) {
                    snvNicotine_label.Visibility = Visibility.Visible;
                }

                if ((snv_BasPG_ml < 0) || (snv_BasVG_ml < 0))
                {
                    if (snv_BasPG_ml < 0)
                    {
                        Color bgcolor = (Color)ColorConverter.ConvertFromString(Settings.Default.ErrorBG);
                        var bgbrush = new SolidColorBrush(bgcolor);
                        Color fgcolor = (Color)ColorConverter.ConvertFromString("#FFAA0000");
                        var fgbrush = new SolidColorBrush(fgcolor);
                        snvBasPG_ml.Background = bgbrush;
                        this.snvBasPG_ml.Foreground = fgbrush;
                    }

                    if (snv_BasVG_ml < 0)
                    {
                        Color bgcolor = (Color)ColorConverter.ConvertFromString(Settings.Default.ErrorBG);
                        var bgbrush = new SolidColorBrush(bgcolor);
                        Color fgcolor = (Color)ColorConverter.ConvertFromString("#FFAA0000");
                        var fgbrush = new SolidColorBrush(fgcolor);
                        this.snvBasVG_ml.Background = bgbrush;
                        this.snvBasVG_ml.Foreground = fgbrush;
                    }
                    AutoClosingMessageBox.Show(Properties.Resources.OneShotErrorMsg1 + "\n" +
                    Properties.Resources.OneShotErrorMsg2 + "\n\nAutoclosing in: " + (autotimeout/1000)+ " seconds.", Properties.Resources.OneShotErrorTitle, autotimeout);
                    return;
                }
                else
                {
                    Color bgcolor = (Color)ColorConverter.ConvertFromString(Settings.Default.BGColor);
                    var bgbrush = new SolidColorBrush(bgcolor);
                    Color fgcolor = (Color)ColorConverter.ConvertFromString("#FFD3D3D3");
                    var fgbrush = new SolidColorBrush(fgcolor);
                    this.snvBasPG_ml.Background = bgbrush;
                    this.snvBasVG_ml.Background = bgbrush;
                    this.snvBasPG_ml.Foreground = fgbrush;
                    this.snvBasVG_ml.Foreground = fgbrush;
                    oneshot_calced = true;
                    insert_ShakeNVape.IsEnabled = true;
                }
            }
        }

        private void insert_ShakeNVape_Click(object sender, RoutedEventArgs e)
        {
            double pg = Convert.ToDouble(snvBasPG_ml.Text);
            if (!string.IsNullOrEmpty(OS_Name.Text) && (oneshot_calced == true) && (pg >= 0))
            {
                //Do Something Here
                try
                {
                    using (dbConn = new SQLiteConnection(connectionString))
                    {
                        dbConn.Open();
                        string CraftName = OS_Name.Text.Replace("'", "''");
                        string CraftAmount = snvFinalMix.Text;
                        string CraftRatio = snvFinal_PG.Text + "/" + snvFinal_VG.Text;
                        string CraftNicotine = snvFinal_Nic.Text;
                        string CraftDate = DateTime.Now.ToString("yyyy/MM/dd");
                        string query = $"Insert INTO [RecipeLog] ([Name], [Amount], [Ratio], [Nic], [Date], [Notes]) ";
                        query += $"VALUES('{CraftName}', '{CraftAmount}', '{CraftRatio}', '{CraftNicotine}', '{CraftDate}', ' - '); ";
                        query += $"UPDATE [RecipeLog] SET [Amount] = REPLACE([Amount], ',', '.'), [Nic] = REPLACE([Nic], ',', '.') WHERE Amount LIKE '%,%'; ";
                        dbCmd = new SQLiteCommand(query, dbConn);
                        dbCmd.ExecuteNonQuery();
                        dbConn.Close();
                        string title = "", message = "";
                        if (CurrentCulture == "en-US")
                        {
                            title = "Success";
                            message = "The Oneshot has saved successfully at Juice Log!";
                        }
                        if (CurrentCulture == "el-GR")
                        {
                            title = "Επιτυχία";
                            message = "Το Oneshot αποθηκεύτηκε επιτυχώς στο Αρχείο Υγρών!";
                        }
                        AutoClosingMessageBox.Show(message, title, autotimeout);
                        oneshot_calced = false;
                        insert_ShakeNVape.IsEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    AutoClosingMessageBox.Show(ex.Message, "Error", autotimeout);
                }
            }
            else
            {
                string message = "", title = "";
                if (CurrentCulture == "en-US")
                {
                    message = "The entry failed to save.";
                    title = "Error";
                }
                if (CurrentCulture == "el-GR")
                {
                    message = "Δεν αποθηκεύτηκε η εγγραφή";
                    title = "Σφάλμα";
                }
                AutoClosingMessageBox.Show(message, title, autotimeout);
            }
            //ENd Here
        }

        private void snvFinal_Nic_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text) || textBox.Text == "0") {
                nicBooster_label.Visibility = Visibility.Hidden;
                nic_style.SelectedIndex = -1;
                snvNic_ml.Text = "0";
                snvNic_PGpercent.Text = "0";
                snvNic_VGpercent.Text = "0";
                snvNic_Grams.Text = "0";
            }
            else
            {
                nicBooster_label.Visibility = Visibility.Visible;
                if (oneshot_calced == true) {
                snvNicotine_label.Visibility = Visibility.Visible;
                }
                if (Convert.ToDouble(textBox.Text) >= Convert.ToDouble(snvNicLevel.Text))
                {
                    string message = "", title = "";
                    if (CurrentCulture == "en-US")
                    {
                        message = "Target Nicotine level is impossible.";
                        title = "Error";
                    }
                    if (CurrentCulture == "el-GR")
                    {
                        message = "Το επιθυμητό επίπεδο νικοτίνης δεν είναι εφικτό.";
                        title = "Σφάλμα";
                    }
                    AutoClosingMessageBox.Show(message, title, autotimeout);
                }
            }
        }

        private static bool IsDecAllowed(string text)
        {
            string CurrentCulture = Settings.Default.Culture;
            if (CurrentCulture == "en-US")
            {
                Regex regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
                return !regex.IsMatch(text);
            }
            else
            {
                Regex regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
                return !regex.IsMatch(text);
            }
        }
    }
}
