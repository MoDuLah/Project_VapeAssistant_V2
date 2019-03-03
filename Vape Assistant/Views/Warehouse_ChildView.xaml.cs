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
    /// Interaction logic for Warehouse_ChildView.xaml
    /// </summary>
    public partial class Warehouse_ChildView : UserControl
    {
        string connectionString = Settings.Default.VaConnect;
        SQLiteDataAdapter dbAdapter;
        SQLiteDataReader reader;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        DataTable dbTable;
        object item;
        public string CurrentCulture = Settings.Default.Culture;
        public string query;
        public int autotimeout = 5000;
        string edititemid, edititemAmount;


        public Warehouse_ChildView()
        {
            InitializeComponent();
            BindComboBox(add_Brand);
            FillDataGrid();
            Warehousecount();
            EventManager.RegisterClassHandler(typeof(TextBox), GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus));
        }

        void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsReadOnly && e.KeyboardDevice.IsKeyDown(Key.Tab))
                textBox.SelectAll();
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
        public void BindComboBox(ComboBox comboBoxName)
        {
            dbConn = new SQLiteConnection(connectionString);
            dbConn.Open();
            SQLiteCommand sqlcmd = new SQLiteCommand();
            dbAdapter = new SQLiteDataAdapter();
            DataSet ds = new DataSet();
            sqlcmd.Connection = dbConn;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = "Select * from FlavorBrands Where Id > '0' ORDER BY Name ASC ";
            dbAdapter.SelectCommand = sqlcmd;
            dbAdapter.Fill(ds, "FlavorBrands");
            comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
            comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["Name"].ToString(); // + " (" + ds.Tables[0].Columns["ShortName"].ToString() + ")"
            comboBoxName.SelectedValuePath = ds.Tables[0].Columns["Id"].ToString();
            if (dbConn.State == ConnectionState.Open)
            {

                dbConn.Close();
            }
        }
        private void FillDataGrid()
        {
            string hdr0 = "", hdr1 = "", hdr2 = "", hdr3 = "";
            string dbTables = "Flavors";
            if (CurrentCulture == "en-US")
            {
                hdr0 = "Id";
                hdr1 = "Brand";
                hdr2 = "Flavor";
                hdr3 = "Amount (ml)";
            }
            if (CurrentCulture == "el-GR")
            {
                hdr0 = "Id";
                hdr1 = "Εταιρία";
                hdr2 = "Άρωμα";
                hdr3 = "Ποσότητα (ml)";
            }
            try
            {
                using (dbConn = new SQLiteConnection(connectionString))
                {
                    dbConn.Open();
                    //Select Command
                    query = $"SELECT Id, Brand, Flavor, Amount FROM {dbTables} where Amount NOT LIKE '0.0' order by Brand and Flavor ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    dbTable = new DataTable(dbTables);

                    dbAdapter.Fill(dbTable);
                    dbConn.Close();
                    warehouse.ItemsSource = dbTable.DefaultView;
                    dbAdapter.Update(dbTable);

                    warehouse.Columns[0].Header = hdr0;
                    warehouse.Columns[1].Header = hdr1;
                    warehouse.Columns[2].Header = hdr2;
                    warehouse.Columns[3].Header = hdr3;

                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show("Message: " + ex, "", autotimeout);
            }
        }

        private void Warehousecount()
        {
            string query;

            try
            {
                dbConn = new SQLiteConnection(connectionString);
                query = "Select Count(*) from Flavors Where Owned > 0 ;";
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    WarehouseCount.Text = reader[0].ToString();
                }

            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
            finally
            {
                if (dbConn.State == ConnectionState.Open)
                {

                    dbConn.Close();
                }
            }
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

        public void Fill_FlavorComboBox(ComboBox comboBoxName)
        {
            dbConn = new SQLiteConnection(connectionString);
            dbConn.Open();
            SQLiteCommand sqlcmd = new SQLiteCommand();
            dbAdapter = new SQLiteDataAdapter();
            DataSet ds = new DataSet();
            sqlcmd.Connection = dbConn;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = "Select * from Flavors WHERE Brand ='" + add_Brand.Text.Replace("'", "''") + "' ORDER BY Flavor ASC ";
            dbAdapter.SelectCommand = sqlcmd;
            dbAdapter.Fill(ds, "Flavors");
            comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
            string rep = ds.Tables[0].Columns["Flavor"].ToString(); // + " (" + ds.Tables[0].Columns["ShortName"].ToString() + ")"
            comboBoxName.DisplayMemberPath = rep.Replace("''", "'");
            comboBoxName.SelectedValuePath = ds.Tables[0].Columns["Id"].ToString();
            if (dbConn.State == ConnectionState.Open)
            {

                dbConn.Close();
            }
        }


        private void warehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (warehouse.SelectedItem != null)
                {
                    item = warehouse.SelectedItem;
                    edititemid = (warehouse.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                    edit_Id.Text = edititemid;
                    edititemAmount = (warehouse.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text;
                    edit_Amount.Text = edititemAmount;
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show("Message: " + ex,"",autotimeout);
            }
        }

        private void add_Item_Click(object sender, RoutedEventArgs e)
        {
            add_Popup.Visibility = Visibility.Visible;
            add_Amount.Select(0, add_Amount.Text.Length);
        }

        private void edit_Item_Click(object sender, RoutedEventArgs e)
        {
            if (warehouse.SelectedItem == null) { return; }
            edit_Id.Text = edititemid;
            edit_Amount.Text = edititemAmount;
            edit_Popup.Visibility = Visibility.Visible;
            edit_Amount.Select(0, edit_Amount.Text.Length);
        }

        private void Delete_Item_Click(object sender, RoutedEventArgs e)
        {
            //Save Changes
            try
            {
                if (warehouse.SelectedItem != null)
                {
                    using (dbConn = new SQLiteConnection(connectionString))
                    {
                        string message = "", caption = "";

                        if (CurrentCulture == "en-US")
                        {
                            message = "Are you sure that you want to delete this flavor?";
                            caption = "Question";
                        }
                        else if (CurrentCulture == "el-GR")
                        {
                            message = "Είστε σίγουροι για τη διαγραφή του αρώματος;";
                            caption = "Ερώτηση";
                        }
                        if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            dbConn.Open();
                            //Insert Command
                            dbCmd = new SQLiteCommand("Update Flavors SET Owned = '0', Amount = '0.0' Where Id = " + Int32.Parse(edititemid), dbConn);
                            dbCmd.ExecuteNonQuery();
                            dbCmd = null;
                            //Select Command
                            FillDataGrid();
                            Warehousecount();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }
        }

        private void add_Brand_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (add_Brand.SelectedIndex >= 0)
                {
                    Fill_FlavorComboBox(add_Flavor);
                }
                else
                {
                    add_Flavor.ItemsSource = "";
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }
        }

        private void add_Brand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            add_Flavor.SelectedIndex = -1;
        }

        private void add_Flavor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (add_Flavor.SelectedIndex >= 0)
            {
                SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                try
                {
                    string query = $"SELECT Id,Amount FROM [Flavors] WHERE Brand='" + add_Brand.Text.Replace("'", "''") +
                        "' and Flavor='" + add_Flavor.Text.Replace("'", "''") + "' ";
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int Id_Flv = int.Parse(reader[0].ToString());
                        double amount = double.Parse(reader[1].ToString());
                        add_Id.Text = Id_Flv.ToString();
                        add_Amount.Text = amount.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error" + ex.ToString());
                    if (dbConn.State == ConnectionState.Open)
                    {
                        dbConn.Close();
                    }
                }
                finally
                {
                    add_Amount.SelectAll();
                    if (dbConn.State == ConnectionState.Open)
                    {

                        dbConn.Close();
                    }
                }
            }
        }

        private void Amount_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "0")
            {
                textBox.Text = "";
            }
        }

        private void Amount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private void Amount_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "0")
            {
                textBox.Text = "";
            }
        }

        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
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

        private void add_Cancel_Click(object sender, RoutedEventArgs e)
        {
            string title = "";
            string message = "";
            if (
                !string.IsNullOrEmpty(add_Id.Text) ||
                !string.IsNullOrEmpty(add_Brand.Text) ||
                !string.IsNullOrEmpty(add_Flavor.Text) ||
                !string.IsNullOrEmpty(add_Amount.Text)
            )
            {
                if (CurrentCulture == "en-US")
                {
                    title = "Question";
                    message = "Are you sure that you want to cancel the entry?";
                }
                if (CurrentCulture == "el-GR")
                {
                    title = "Ερώτηση";
                    message = "Είστε σίγουρος για την ακύρωση της εγγραφής;";
                }
                if (MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }

            }
            add_Popup.Visibility = Visibility.Collapsed;
            add_Id.Text = String.Empty;
            add_Brand.SelectedIndex = -1;
            add_Flavor.SelectedIndex = -1;
            add_Amount.Text = String.Empty;
        }

        private void add_Submit_Click(object sender, RoutedEventArgs e)
        {
            if ((string.IsNullOrEmpty(add_Brand.Text)) ||
                (string.IsNullOrEmpty(add_Flavor.Text)) ||
                (string.IsNullOrEmpty(add_Amount.Text)) ||
                (add_Amount.Text == "0"))
            {
                return;
            }
            //Do Something Here
            try
            {
                using (dbConn = new SQLiteConnection(connectionString))
                {
                    dbConn.Open();
                    //Insert Command
                    dbCmd = new SQLiteCommand("UPDATE Flavors SET Amount ='" + double.Parse(add_Amount.Text.ToString()) + "', Owned = '1' where Id =" + Int32.Parse(add_Id.Text.ToString()), dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbCmd = null;
                    //Select Command
                    FillDataGrid();
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }

            //ENd Here
            add_Popup.Visibility = Visibility.Collapsed;
            ClearTextBoxes(this);
            //add_Id.Text = String.Empty;
            //add_Brand.SelectedIndex = -1;
            //add_Flavor.SelectedIndex = -1;
            //add_Amount.Text = String.Empty;
            Warehousecount();
            //checkIfWarehouseIsNotEmpty();
        }

        private void edit_Cancel_Click(object sender, RoutedEventArgs e)
        {
            string title = "";
            string message = "";
            if (
                !string.IsNullOrEmpty(edit_Id.Text) ||
                !string.IsNullOrEmpty(edit_Amount.Text)
            )
            {
                if (CurrentCulture == "en-US")
                {
                    title = "Question";
                    message = "Are you sure that you want to cancel the entry?";
                }
                if (CurrentCulture == "el-GR")
                {
                    title = "Ερώτηση";
                    message = "Είστε σίγουρος για την ακύρωση της εγγραφής;";
                }
                if (MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }

            }
            edit_Popup.Visibility = Visibility.Collapsed;
            edit_Id.Text = String.Empty;
            edit_Amount.Text = String.Empty;
        }

        private void edit_Submit_Click(object sender, RoutedEventArgs e)
        {
            //Do Something 
            if (!edit_Id.Text.Equals(edititemid) || !edit_Amount.Text.Equals(edititemAmount))
            {

                //Do Something Here
                try
                {
                    using (dbConn = new SQLiteConnection(connectionString))
                    {
                        dbConn.Open();
                        //Insert Command
                        query = $"UPDATE Flavors SET Amount = " +
                            $"'" + Double.Parse(edit_Amount.Text.ToString()) + "' " +
                            "WHERE Id = '" + Int32.Parse(edit_Id.Text.ToString()) + "' ; ";
                        dbCmd = new SQLiteCommand(query, dbConn);
                        dbCmd.ExecuteNonQuery();
                        dbCmd = null;
                        //Select Command
                        FillDataGrid();
                    }
                }
                catch (Exception ex)
                {
                    AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
                }
            }
            else
            {
                AutoClosingMessageBox.Show("No Entry Changed.", "", autotimeout);
            }
            //ENd Here
            edit_Popup.Visibility = Visibility.Collapsed;
            ClearTextBoxes(this);
            //edit_Id.Text = String.Empty;
            //edit_Amount.Text = String.Empty;
            //edititemid = String.Empty;
            //edititemAmount = String.Empty;
            Warehousecount();
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
