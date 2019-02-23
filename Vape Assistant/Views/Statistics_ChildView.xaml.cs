using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for Statistics_ChildView.xaml
    /// </summary>
    public partial class Statistics_ChildView : UserControl, INotifyPropertyChanged
    {
        string connectionString = Settings.Default.VaConnect;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        public string CurrentCulture = Settings.Default.Culture;
        public string query;
        string ced;
        string vsd;
        DateTime ceddt;
        DateTime vsddt;
        public Statistics_ChildView()
        {

            InitializeComponent();
            ReturnSumOrders();
            string CurrentCulture = Settings.Default.Culture;
            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                //Handler attach - will not be done if not needed
                PreviewKeyDown += new KeyEventHandler(ShellView.ShellView_PreviewKeyDown);

            }
            cig_end_date.cmbYear.Text = Settings.Default.cig_end_year.ToString();
            cig_end_date.cmbMonths.SelectedIndex = Settings.Default.cig_end_month - 1;
            cig_end_date.cmbDays.Text = Settings.Default.cig_end_day.ToString();
            vap_start_date.cmbYear.Text = Settings.Default.vap_start_year.ToString();
            vap_start_date.cmbMonths.SelectedIndex = Settings.Default.vap_start_month-1;
            vap_start_date.cmbDays.Text = Settings.Default.vap_start_day.ToString();
            cig_quantity_pack.Text = Settings.Default.cig_quantity_pack.ToString();
            cig_price.Text = Settings.Default.cig_price.ToString();
            cig_per_day.Text = Settings.Default.cig_per_day.ToString();
            Liq_cost.Text = Settings.Default.vap_liq_cost.ToString();
            Liq_ml.Text = Settings.Default.vap_liq_ml.ToString();
            Nic_cost.Text = Settings.Default.vap_nic_cost.ToString();
            //vap_hardware_cost.Text = Settings.Default.vap_hardware_cost.ToString();
            vap_ejuiceml_cost.Text = Settings.Default.vap_ejuiceml_cost.ToString();
            vap_juice_consumption.Text = Settings.Default.vap_juice_consumption.ToString();
            ced = cig_end_date.cmbYear.Text + "-" + (cig_end_date.cmbMonths.SelectedIndex + 1) + "-" + cig_end_date.cmbDays.Text;
            ceddt = Convert.ToDateTime(cig_end_date.cmbDays.Text + "-" + (cig_end_date.cmbMonths.SelectedIndex +1) + "-" + cig_end_date.cmbYear.Text);
            vsd = vap_start_date.cmbYear.Text + "-" + (vap_start_date.cmbMonths.SelectedIndex + 1) + "-" + vap_start_date.cmbDays.Text;
            vsddt = Convert.ToDateTime(vap_start_date.cmbDays.Text + "-" + (vap_start_date.cmbMonths.SelectedIndex + 1) + "-" + vap_start_date.cmbYear.Text);
            if (ceddt != Convert.ToDateTime("1-1-2004"))
            {
                DateTime DisplayDateStart = ceddt;
                DateTime dt = DisplayDateStart;
                DateTime pt = DateTime.Today;
                var StartDate = dt;
                var EndDate = DateTime.Today;
                var qt = (EndDate - StartDate).TotalDays;
                if (qt < 0)
                {
                    //MessageBox.Show("Error");
                    //ced = DateTime.Today.AddDays(-1).ToString();
                    return;
                }
                var stringVal = Convert.ToString(qt);
                cig_days_off.Text = stringVal;
            }
            if (vsddt != Convert.ToDateTime("1-1-2004"))
            {
                DateTime DisplayDateStart = vsddt;
                DateTime dt = DisplayDateStart;
                DateTime pt = DateTime.Today;
                var StartDate = dt;
                var EndDate = DateTime.Today;
                var qt = (EndDate - StartDate).TotalDays;
                if (qt < 0)
                {
                    //MessageBox.Show("Error");
                    //vsd = DateTime.Today.AddDays(-1).ToString();
                    return;
                }
                var stringVal = Convert.ToString(qt);
                vap_days_on.Text = stringVal;
            }
            if (!(string.IsNullOrEmpty(vap_hardware_cost.Text)))
            {
                double vap_hw_cost = Convert.ToDouble(vap_hardware_cost.Text);
                int days_on = Convert.ToInt32(vap_days_on.Text);
                double vap_hw_daily_cost = Math.Round((vap_hw_cost / days_on),2);
                vap_hardware_daily_cost.Text = Convert.ToString(vap_hw_daily_cost);

                double vap_j_cost = Convert.ToDouble(vap_daily_cost.Text);
                vap_yearly_cost.Text = Convert.ToString((vap_j_cost + vap_hw_daily_cost) * 365);
            }
        }

        protected void ReturnSumOrders()
        {
            string purName = "Purchases";

            dbConn = new SQLiteConnection(connectionString);
            dbConn.Open();

            string query = "";

            query = $"select Sum(Total) from {purName} where Id > 0 ; ";
            dbCmd = new SQLiteCommand(query, dbConn);
            SQLiteDataReader freader = dbCmd.ExecuteReader();
            while (freader.Read())
            {
                vap_hardware_cost.Text = freader[0].ToString();
            }
            dbConn.Close();
            if (string.IsNullOrEmpty(vap_hardware_cost.Text))
            {
                vap_hardware_cost.Text = "0";
            }
        }

        private void stats_reset_Click(object sender, RoutedEventArgs e)
        {
            string CurrentCulture = Settings.Default.Culture;
            string message = "", caption = "";
            if (CurrentCulture == "en-US")
            {
                message = "Reset will clear your statistics\n\nDo you wish to proceed?";
                caption = "Are you sure?";
            }
            if (CurrentCulture == "el-GR")
            {
                message = "Τα δεδομένα σας θα χαθούν.\n\nΕπιθυμείτε να συνεχίσετε;";
                caption = "Είστε σίγουρος;";
            }
            if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            cig_end_date.cmbYear.Text = DateTime.Today.Year.ToString();
            cig_end_date.cmbMonths.SelectedIndex = Convert.ToInt32(DateTime.Today.Month) -1;
            cig_end_date.cmbDays.Text = DateTime.Today.Day.ToString();
            vap_start_date.cmbYear.Text = DateTime.Today.Year.ToString();
            vap_start_date.cmbMonths.SelectedIndex = Convert.ToInt32(DateTime.Today.Month) - 1;
            vap_start_date.cmbDays.Text = DateTime.Today.Day.ToString();
            Liq_ml.Text = "0";
            Liq_cost.Text = "0";
            Nic_cost.Text = "0";
            cig_quantity_pack.Text = "0";
            cig_price.Text = "0";
            cig_per_day.Text = "0";
            cig_daily_cost.Text = "0";
            cig_yearly_cost.Text = "0";
            vap_ejuiceml_cost.Text = "0";
            vap_juice_consumption.Text = "0";
            vap_hardware_daily_cost.Text = "0";
            vap_daily_cost.Text = "0";
            vap_yearly_cost.Text = "0";
            Settings.Default.cig_end_year = Convert.ToInt32(DateTime.Today.Year);
            Settings.Default.cig_end_month = Convert.ToInt32(DateTime.Today.Month);
            Settings.Default.cig_end_day = Convert.ToInt32(DateTime.Today.Day);
            Settings.Default.vap_start_year = Convert.ToInt32(DateTime.Today.Year);
            Settings.Default.vap_start_month = Convert.ToInt32(DateTime.Today.Month);
            Settings.Default.vap_start_day = Convert.ToInt32(DateTime.Today.Day);
            Settings.Default.cig_quantity_pack = 0;
            Settings.Default.cig_price = 0;
            Settings.Default.cig_per_day = 0;
            Settings.Default.vap_liq_ml = 0;
            Settings.Default.vap_liq_cost = 0;
            Settings.Default.vap_nic_cost = 0;
            Settings.Default.vap_ejuiceml_cost = 0;
            Settings.Default.vap_juice_consumption = 0;
            Settings.Default.Save();

            string success;

            if (CurrentCulture == "en-US")
            {
                success = "Your data erased successfully!";
            }
            else
            {
                success = "Τα δεδομένα σας διαγράφηκαν επιτυχώς!";
            }
            AutoClosingMessageBox.Show(success, "", 3000);

        }
        private void stats_submit_Click(object sender, RoutedEventArgs e)
        {
            string CurrentCulture = Settings.Default.Culture;
            string success;
            string fail;
            int i = 0;
            if (CurrentCulture == "en-US")
            {
                success = "Your data saved successfully!";
                fail = "Some fields are not filled up, please review your data.";
            }
            else
            {
                success = "Τα δεδομένα σας αποθηκεύτηκαν επιτυχώς!";
                fail = "Κάποιo(α) πεδίo(α) είναι κενό(ά), παρακαλώ επιβεβαιώστε τα δεδομένα σας.";
            }

            if (string.IsNullOrEmpty(cig_days_off.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(vap_days_on.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(cig_quantity_pack.Text) == true) { i = i + 1; }
            //if (string.IsNullOrEmpty(vap_hardware_cost.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(cig_price.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(Liq_ml.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(Liq_cost.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(Nic_cost.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(vap_ejuiceml_cost.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(cig_per_day.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(vap_juice_consumption.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(cig_per_price.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(vap_hardware_daily_cost.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(cig_daily_cost.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(vap_daily_cost.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(cig_yearly_cost.Text) == true) { i = i + 1; }
            if (string.IsNullOrEmpty(vap_yearly_cost.Text) == true) { i = i + 1; }

            if (i == 0)
            {
                Settings.Default.cig_end_year = Convert.ToInt32(cig_end_date.cmbYear.Text);
                Settings.Default.cig_end_month = cig_end_date.cmbMonths.SelectedIndex + 1;
                Settings.Default.cig_end_day = Convert.ToInt32(cig_end_date.cmbDays.Text);
                Settings.Default.vap_start_year = Convert.ToInt32(vap_start_date.cmbYear.Text);
                Settings.Default.vap_start_month = vap_start_date.cmbMonths.SelectedIndex + 1;
                Settings.Default.vap_start_day = Convert.ToInt32(vap_start_date.cmbDays.Text);
                Settings.Default.cig_quantity_pack = Convert.ToInt32(cig_quantity_pack.Text);
                Settings.Default.cig_price = Convert.ToDecimal(cig_price.Text);
                Settings.Default.cig_per_day = Convert.ToInt32(cig_per_day.Text);
                //Settings.Default.vap_hardware_cost = Convert.ToDouble(vap_hardware_cost.Text);
                Settings.Default.vap_liq_ml = Convert.ToInt32(Liq_ml.Text);
                Settings.Default.vap_liq_cost = Convert.ToDouble(Liq_cost.Text);
                Settings.Default.vap_nic_cost = Convert.ToDouble(Nic_cost.Text);
                Settings.Default.vap_ejuiceml_cost = Convert.ToDouble(vap_ejuiceml_cost.Text);
                Settings.Default.vap_juice_consumption = Convert.ToDouble(vap_juice_consumption.Text);
                Settings.Default.Save();
                AutoClosingMessageBox.Show(success,"Success",3000);
                return;
            }
            else
            {
                AutoClosingMessageBox.Show(fail,"Fail",3000);
                return;
            }
        }



        private void cig_price_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int start = textBox.SelectionStart;
            int length = textBox.SelectionLength;
            string value = textBox.Text;
;
            // update text
            textBox.Text = value;

            // restore cursor position and selection
            textBox.Select(start, length);
            if (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrEmpty(cig_quantity_pack.Text))
            {
                return;
            }
            else
            {
                if ((textBox.Text == "0") || (cig_quantity_pack.Text == "0"))
                {
                    cig_per_price.Text = "0";
                    return;
                }
                else
                {
                    var packprice = Convert.ToDouble(cig_price.Text);
                    var cigquant = Convert.ToDouble(cig_quantity_pack.Text);
                    cig_per_price.Text = Convert.ToString(Math.Round(packprice / cigquant, 2));
                }
            }
        }
        private void cig_per_day_TextChanged(object sender, TextChangedEventArgs e)
        {
            //cig_per_price
            if (string.IsNullOrEmpty(cig_per_day.Text))
            {
                return;
            }
            if (cig_per_day.Text != "0")
            {
                int cigperday = Convert.ToInt32(cig_per_day.Text);
                double cigprice = Convert.ToDouble(cig_per_price.Text);
                cig_daily_cost.Text = Convert.ToString(Math.Round(cigperday * cigprice, 2));
            }
            else
            {
                cig_per_price.Text = "0";
                cig_daily_cost.Text = "0";
            }
        }
        private void cig_daily_cost_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal cig_daily_cost = Convert.ToDecimal(this.cig_daily_cost.Text);
            cig_yearly_cost.Text = Convert.ToString(Math.Round(cig_daily_cost * 365, 2));
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
                Regex regex = new Regex("[^0-9,]+"); //regex that matches disallowed text
                return !regex.IsMatch(text);
            }
        }
        private static bool IsIntAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void vap_hardware_cost_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int start = textBox.SelectionStart;
            int length = textBox.SelectionLength;
            string value = textBox.Text;

            // update text
            textBox.Text = value;

            // restore cursor position and selection
            textBox.Select(start, length);
            if (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrEmpty(vap_days_on.Text))
            {
                vap_hardware_daily_cost.Text = "0";
            }
            else
            {
                int vap_days = Convert.ToInt32(this.vap_days_on.Text);
                double vap_hw_cost = Convert.ToDouble(textBox.Text);
                vap_hardware_daily_cost.Text = Convert.ToString(Math.Round(vap_hw_cost / vap_days, 2));
            }
        }

        private void vap_ejuiceml_cost_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int start = textBox.SelectionStart;
            int length = textBox.SelectionLength;

            // restore cursor position and selection
            textBox.Select(start, length);

            if (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrEmpty(vap_juice_consumption.Text))
            {
                return;
            }
            else
            {
                double vap_j_cons = Convert.ToInt32(vap_juice_consumption.Text);
                double vap_ml_cost = Convert.ToDouble(textBox.Text);
                vap_daily_cost.Text = Convert.ToString(Math.Round(vap_j_cons * vap_ml_cost, 2));
            }

            if (string.IsNullOrEmpty(vap_hardware_daily_cost.Text) && (string.IsNullOrEmpty(vap_juice_consumption.Text)))
            {
                return;
            }
            else
            {
                double vap_hw_cost = Convert.ToDouble(vap_hardware_daily_cost.Text);
                double vap_j_cost = Convert.ToDouble(vap_daily_cost.Text);
                vap_yearly_cost.Text = Convert.ToString((vap_j_cost + vap_hw_cost) * 365);
            }
        }
        private void vap_juice_consumption_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int start = textBox.SelectionStart;
            int length = textBox.SelectionLength;
            // restore cursor position and selection
            textBox.Select(start, length);

            if (string.IsNullOrEmpty(vap_juice_consumption.Text) || string.IsNullOrEmpty(vap_ejuiceml_cost.Text))
            {
                return;
            }
            else
            {
                double vap_JC = Convert.ToDouble(textBox.Text);
                double vap_ml_cost = Convert.ToDouble(vap_ejuiceml_cost.Text);

                vap_daily_cost.Text = Convert.ToString(Math.Round(vap_ml_cost * vap_JC, 2));
            }

            if (string.IsNullOrEmpty(vap_hardware_daily_cost.Text))
            {
                return;
            }
            else
            {
                double vap_hw_cost = Convert.ToDouble(vap_hardware_daily_cost.Text);
                double vap_j_cost = Convert.ToDouble(vap_daily_cost.Text);
                vap_yearly_cost.Text = Convert.ToString((vap_j_cost + vap_hw_cost) * 365);
            }

        }

        private void cig_quantity_pack_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsIntAllowed(e.Text);
        }
        private void cig_price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void cig_per_day_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }
        private void vap_hardware_cost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void vap_ejuiceml_cost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }
        private void vap_juice_consumption_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsDecAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Your code

        }
        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = !(string.IsNullOrEmpty(cig_days_off.Text) && string.IsNullOrEmpty(vap_days_on.Text)
                && string.IsNullOrEmpty(cig_quantity_pack.Text) && string.IsNullOrEmpty(vap_hardware_cost.Text)
                && string.IsNullOrEmpty(cig_price.Text) && string.IsNullOrEmpty(vap_ejuiceml_cost.Text)
                && string.IsNullOrEmpty(cig_per_day.Text) && string.IsNullOrEmpty(vap_juice_consumption.Text)
                && string.IsNullOrEmpty(cig_per_price.Text) && string.IsNullOrEmpty(vap_hardware_daily_cost.Text)
                && string.IsNullOrEmpty(cig_daily_cost.Text) && string.IsNullOrEmpty(vap_daily_cost.Text)
                && string.IsNullOrEmpty(cig_yearly_cost.Text) && string.IsNullOrEmpty(vap_yearly_cost.Text));
        }

        private void cig_end_date_LostFocus(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrEmpty(cig_end_date.cmbDays.Text) || string.IsNullOrEmpty(cig_end_date.cmbMonths.Text) || string.IsNullOrEmpty(cig_end_date.cmbYear.Text))
            {
                cig_end_date.cmbDays.SelectedIndex = 0;
                return;
            }
            string temp = cig_end_date.cmbDays.Text + "/" + (cig_end_date.cmbMonths.SelectedIndex + 1) + "/" + cig_end_date.cmbYear.Text;
            //MessageBox.Show(temp + " " + DateTime.Today.ToString());
            ceddt = Convert.ToDateTime(temp);
            DateTime DisplayDateStart = ceddt;
            var StartDate = DisplayDateStart;
            var EndDate = DateTime.Today;
            var qt = (EndDate - StartDate).TotalDays;
            if (qt < 0)
            {
                AutoClosingMessageBox.Show("Error","",3000);
                ced = DateTime.Today.AddDays(-1).ToString();
                return;
            }
            var stringVal = Convert.ToString(qt);
            cig_days_off.Text = stringVal;
        }

        private void vap_start_date_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(vap_start_date.cmbDays.Text) || string.IsNullOrEmpty(vap_start_date.cmbMonths.Text) || string.IsNullOrEmpty(vap_start_date.cmbYear.Text))
            {
                vap_start_date.cmbDays.SelectedIndex = 0;
                return;
            }
            string temp = vap_start_date.cmbDays.Text + "-" + (vap_start_date.cmbMonths.SelectedIndex + 1) + "-" + vap_start_date.cmbYear.Text;
            //MessageBox.Show(temp);
            vsddt = Convert.ToDateTime(temp);
            DateTime DisplayDateStart = vsddt;
            DateTime dt = DisplayDateStart;
            DateTime pt = DateTime.Today;
            var StartDate = dt;
            var EndDate = DateTime.Today;
            var qt = (EndDate - StartDate).TotalDays;
            if (qt < 0)
            {
                AutoClosingMessageBox.Show("End Date Can't Be before Start Date", "Error", 3000);
                vsd = DateTime.Today.AddDays(-1).ToString();
                return;
            }
            var stringVal = Convert.ToString(qt);
            vap_days_on.Text = stringVal;
            if (!string.IsNullOrEmpty(vap_days_on.Text) && !string.IsNullOrEmpty(vap_hardware_cost.Text))
            {
                int vap_days = Convert.ToInt32(this.vap_days_on.Text);
                double vap_hw_cost = Convert.ToDouble(vap_hardware_cost.Text);
                vap_hardware_daily_cost.Text = Convert.ToString(Math.Round(vap_hw_cost / vap_days, 2));
            }
        }

        private void Liq_ml_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text)) { return; }
            if (!string.IsNullOrEmpty(Liq_cost.Text) && !string.IsNullOrEmpty(Nic_cost.Text))
            {
                double liq_ml = Convert.ToDouble(Liq_ml.Text);
                double liq_cost = Convert.ToDouble(Liq_cost.Text);
                double nic_cost = Convert.ToDouble(Nic_cost.Text);
                vap_ejuiceml_cost.Text = Convert.ToString(Math.Round(((liq_cost + nic_cost) / liq_ml),2));
            }
        }

        private void Liq_cost_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text)) { return; }
            if (!string.IsNullOrEmpty(Liq_ml.Text) && !string.IsNullOrEmpty(Nic_cost.Text))
            {
                double liq_ml = Convert.ToDouble(Liq_ml.Text);
                double liq_cost = Convert.ToDouble(Liq_cost.Text);
                double nic_cost = Convert.ToDouble(Nic_cost.Text);
                vap_ejuiceml_cost.Text = Convert.ToString(Math.Round(((liq_cost + nic_cost) / liq_ml), 2));
            }
        }

        private void Nic_cost_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text)) { return; }
            if (!string.IsNullOrEmpty(Liq_ml.Text) && !string.IsNullOrEmpty(Liq_cost.Text))
            {
                double liq_ml = Convert.ToDouble(Liq_ml.Text);
                double liq_cost = Convert.ToDouble(Liq_cost.Text);
                double nic_cost = Convert.ToDouble(Nic_cost.Text);
                vap_ejuiceml_cost.Text = Convert.ToString(Math.Round(((liq_cost + nic_cost) / liq_ml), 2));
            }
        }

        private void TxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "0.0" || textBox.Text == "0")
            {
                textBox.Text = "";
            }
        }

        private void TxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }
    }
}
