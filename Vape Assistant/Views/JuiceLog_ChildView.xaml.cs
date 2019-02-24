using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for Warehouse_ChildView.xaml
    /// </summary>
    public partial class JuiceLog_ChildView : UserControl
    {
        string connectionString = Settings.Default.VaConnect;
        SQLiteDataAdapter dbAdapter;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        DataTable dbTable;
        public string query;
        int editId;
        string CurrentCulture = Settings.Default.Culture;
        public int autotimeout = 5000;

        public JuiceLog_ChildView()
        {
            InitializeComponent();
            FillDataGrid();
        }

        private void Log_Add_Click(object sender, RoutedEventArgs e)
        {
            AddLogElementInputBox.Visibility = Visibility.Visible;
            Add_LogRecipeName.Select(0, Add_LogRecipeName.Text.Length);
        }

        private void Log_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (JuiceLogList.SelectedItem == null) { return; }
            EditLogElementInputBox.Visibility = Visibility.Visible;
            Edit_LogRecipeName.Select(0, Edit_LogRecipeName.Text.Length);
        }

        private void Add_LogAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Add_LogPGRatio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Add_LogVGRatio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Add_LogNic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Add_LogPGRatio_TextChanged(object sender, TextChangedEventArgs e)
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
                string value = textBox.Text;
                int chck = Convert.ToInt32(value);
                if (chck <= 0)
                {
                    chck = 0;
                }
                else if (chck >= 100)
                {
                    chck = 100;
                }
                //fixdec(value);
                // update text
                textBox.Text = chck.ToString();

                // restore cursor position and selection
                textBox.Select(start, length);
                Add_LogVGRatio.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }

        private void Add_LogVGRatio_TextChanged(object sender, TextChangedEventArgs e)
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
                string value = textBox.Text;
                int chck = Convert.ToInt32(value);
                if (chck < 0)
                {
                    chck = 0;
                }
                else if (chck > 100)
                {
                    chck = 100;
                }
                //fixdec(value);
                // update text
                textBox.Text = chck.ToString();

                // restore cursor position and selection
                textBox.Select(start, length);
                Add_LogPGRatio.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }

        private void Add_LogNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                Add_LogNotesLimit.Text = "(128)";
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                textBox.Select(start, length);
                Add_LogNotesLimit.Text = "(" + Convert.ToString(Add_LogNotes.MaxLength - textBox.SelectionStart) + ")";
            }
        }

        private void LogCancelButton_Click(object sender, RoutedEventArgs e)
        {
            string title = "";
            string message = "";
            if (
                !string.IsNullOrEmpty(Add_LogRecipeName.Text) ||
                !string.IsNullOrEmpty(Add_LogAmount.Text) ||
                !string.IsNullOrEmpty(Add_LogPGRatio.Text) ||
                !string.IsNullOrEmpty(Add_LogVGRatio.Text) ||
                !string.IsNullOrEmpty(log_date.cmbYear.Text) ||
                !string.IsNullOrEmpty(log_date.cmbMonths.Text) ||
                !string.IsNullOrEmpty(log_date.cmbDays.Text)
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
            AddLogElementInputBox.Visibility = Visibility.Collapsed;
            Add_LogRecipeName.Text = String.Empty;
            Add_LogAmount.Text = String.Empty;
            Add_LogPGRatio.Text = String.Empty;
            Add_LogVGRatio.Text = String.Empty;
            Add_LogNotes.Text = String.Empty;
        }
        private void LogSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (
                string.IsNullOrEmpty(Add_LogRecipeName.Text) ||
                string.IsNullOrEmpty(Add_LogAmount.Text) ||
                string.IsNullOrEmpty(Add_LogPGRatio.Text) ||
                string.IsNullOrEmpty(Add_LogVGRatio.Text) ||
                string.IsNullOrEmpty(log_date.cmbYear.Text) ||
                string.IsNullOrEmpty(log_date.cmbMonths.Text) ||
                string.IsNullOrEmpty(log_date.cmbDays.Text)
                )
            {
                return;
            }
            //Do Something Here

            try
            {
                using (dbConn = new SQLiteConnection(connectionString))
                {
                    string months, days;
                    int month = (log_date.cmbMonths.SelectedIndex + 1);
                    int day = (log_date.cmbDays.SelectedIndex + 1);
                    if (month <= 9)
                    {
                        months = "0" + month.ToString();
                    }
                    else
                    {
                        months = month.ToString();
                    }
                    if (day <= 9)
                    {
                        days = "0" + day.ToString();
                    }
                    else
                    {
                        days = day.ToString();
                    }
                    string logPGVG = Add_LogPGRatio.Text + "/" + Add_LogVGRatio.Text;
                    string logDate = log_date.cmbYear.Text + "/" + months + "/" + days;
                    dbConn.Open();
                    //Insert Command
                    string query;
                    query = $"Insert INTO RecipeLog ";
                    query += $"([Name], [Amount], [Ratio], [Nic], [Date], [Notes]) ";
                    query += $"VALUES('{Add_LogRecipeName.Text.Replace("'", "''")}','{Add_LogAmount.Text}','{logPGVG}','{Add_LogNic.Text}','{logDate}','{Add_LogNotes.Text.Replace("'", "''")}') ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbCmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Message : " + ex);
                return;
            }
            //ENd Here
            AddLogElementInputBox.Visibility = Visibility.Collapsed;
            Add_LogRecipeName.Text = String.Empty;
            Add_LogAmount.Text = String.Empty;
            Add_LogPGRatio.Text = String.Empty;
            Add_LogVGRatio.Text = String.Empty;
            Add_LogNotes.Text = String.Empty;
            FillDataGrid();
        }

        private void Log_Delete_Click(object sender, RoutedEventArgs e)
        {
            //Save Changes
            try
            {
                if (this.JuiceLogList.SelectedItem != null)
                {
                    using (dbConn = new SQLiteConnection(connectionString))
                    {
                        string title, message;

                        if (CurrentCulture == "en-US")
                        {
                            title = "Question";
                            message = "Are you sure that you want to delete this log?";
                        }
                        else
                        {
                            title = "Ερώτηση";
                            message = "Είστε σίγουροι για την διαγραφή αυτού του στοιχείου;";
                        }
                        if (MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            dbConn.Open();
                            //Insert Command
                            string query = $"Delete FROM [RecipeLog] where id = '{editId}'";
                            dbCmd = new SQLiteCommand(query, dbConn);
                            dbCmd.ExecuteNonQuery();
                            dbCmd = null;
                            dbConn.Close();
                            //Select Command
                            JuiceLogList.SelectedIndex = -1;
                            FillDataGrid();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }
        }

        private void JuiceLogList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JuiceLogList.SelectedIndex < 0) { return; }
            object Pitem;
            string editName = "", editAmount = "", editRatio = "", editPGRatio = "", editVGRatio = "", editNic = "", editDate = "", editNotes = "";
            try
            {
                if (JuiceLogList.SelectedItem != null)
                {
                    Pitem = this.JuiceLogList.SelectedItem;
                    string a = (JuiceLogList.SelectedCells[0].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editId = Int32.Parse(a);
                    editName = (JuiceLogList.SelectedCells[1].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editAmount = (JuiceLogList.SelectedCells[2].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editRatio = (JuiceLogList.SelectedCells[3].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editNic = (JuiceLogList.SelectedCells[4].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editDate = (JuiceLogList.SelectedCells[5].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editNotes = (JuiceLogList.SelectedCells[6].Column.GetCellContent(Pitem) as TextBlock).Text;
                    TextBox[] x = { x0, x1 };
                    string[] tagArray = editRatio.Split('/');
                    for (int o = 0; o < tagArray.Length; o++)
                    {
                        x[o].Text = tagArray[o].ToString();
                    }
                    ComboBox[] y = { y0, y1, y2 };
                    string[] bagArray = editDate.Split('/');
                    for (int o = 0; o < bagArray.Length; o++)
                    {
                        y[o].Text = bagArray[o].ToString();
                    }
                    editPGRatio = x0.Text;
                    editVGRatio = x1.Text;
                    editlog_date.cmbYear.Text = y0.Text;
                    editlog_date.cmbMonths.SelectedIndex = Convert.ToInt32(y1.Text) - 1;
                    int y2end = Convert.ToInt32(y2.Text);
                    editlog_date.cmbDays.Text = y2end.ToString();
                    EditItemIdBox.Text = editId.ToString();
                    Edit_LogRecipeName.Text = editName;
                    Edit_LogAmount.Text = editAmount;
                    Edit_LogPGRatio.Text = editPGRatio;
                    Edit_LogVGRatio.Text = editVGRatio;
                    Edit_LogNic.Text = editNic;
                    Edit_LogNotes.Text = editNotes;
                    Edit_LogNotesLimit.Text = "(" + Convert.ToString(Edit_LogNotes.MaxLength - Edit_LogNotes.SelectionStart) + ")";
                }
            }
            catch (Exception exp)
            {
                AutoClosingMessageBox.Show(exp.ToString(), "Error", autotimeout);
            }
        }


        private void EditLogCancelButton_Click(object sender, RoutedEventArgs e)
        {
            string title, message;

            if (CurrentCulture == "en-US")
            {
                title = "Question";
                message = "Are you sure that you want to cancel the changes you made?";
            }
            else
            {
                title = "Ερώτηση";
                message = "Είστε σίγουροι για την ακύρωση των αλλαγών σας;";
            }
            if (MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            EditLogElementInputBox.Visibility = Visibility.Collapsed;
            JuiceLogList.SelectedIndex = -1;
            Edit_LogRecipeName.Text = String.Empty;
            Edit_LogAmount.Text = String.Empty;
            Edit_LogPGRatio.Text = String.Empty;
            Edit_LogVGRatio.Text = String.Empty;
            Edit_LogNic.Text = String.Empty;
            Edit_LogNotes.Text = String.Empty;
            editId = 0;
        }

        private void EditLogUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (
               string.IsNullOrEmpty(Edit_LogRecipeName.Text) ||
               string.IsNullOrEmpty(Edit_LogAmount.Text) ||
               string.IsNullOrEmpty(Edit_LogPGRatio.Text) ||
               string.IsNullOrEmpty(Edit_LogVGRatio.Text) ||
               string.IsNullOrEmpty(editlog_date.cmbYear.Text) ||
               string.IsNullOrEmpty(editlog_date.cmbMonths.Text) ||
               string.IsNullOrEmpty(editlog_date.cmbDays.Text)
               )
            {
                return;
            }
            //Do Something Here

            try
            {
                using (dbConn = new SQLiteConnection(connectionString))
                {
                    string months, days;
                    int month = (editlog_date.cmbMonths.SelectedIndex + 1);
                    int day = (editlog_date.cmbDays.SelectedIndex + 1);
                    if (month <= 9)
                    {
                        months = "0" + month.ToString();
                    }
                    else
                    {
                        months = month.ToString();
                    }
                    if (day <= 9)
                    {
                        days = "0" + day.ToString();
                    }
                    else
                    {
                        days = day.ToString();
                    }
                    string logPGVG = Edit_LogPGRatio.Text + "/" + Edit_LogVGRatio.Text;
                    string logDate = editlog_date.cmbYear.Text + "/" + months + "/" + days;
                    dbConn.Open();
                    //Insert Command
                    string query;
                    query = $"UPDATE [RecipeLog] SET" +
                        $" Name = '{Edit_LogRecipeName.Text.Replace("'", "''")}'," +
                        $" Amount = '{Edit_LogAmount.Text}'," +
                        $" Ratio = '{logPGVG}'," +
                        $" Date = '{logDate}'," +
                        $" Notes = '{Edit_LogNotes.Text.Replace("'", "''")}'" +
                        $" where id = " + Int32.Parse(this.EditItemIdBox.Text.ToString()) + "; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbCmd.Dispose();
                    dbConn.Close();
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(),"Error",autotimeout);
                return;
            }
            //ENd Here
            EditLogElementInputBox.Visibility = Visibility.Collapsed;
            Edit_LogRecipeName.Text = String.Empty;
            Edit_LogAmount.Text = String.Empty;
            Edit_LogPGRatio.Text = String.Empty;
            Edit_LogVGRatio.Text = String.Empty;
            Edit_LogNotes.Text = String.Empty;
            FillDataGrid();
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
        private void Edit_LogAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Edit_LogPGRatio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Edit_LogVGRatio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Edit_LogNic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void Edit_LogPGRatio_TextChanged(object sender, TextChangedEventArgs e)
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
                string value = textBox.Text;
                int chck = Convert.ToInt32(value);
                if (chck <= 0)
                {
                    chck = 0;
                }
                else if (chck >= 100)
                {
                    chck = 100;
                }
                //fixdec(value);
                // update text
                textBox.Text = chck.ToString();

                // restore cursor position and selection
                textBox.Select(start, length);
                Edit_LogVGRatio.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }

        private void Edit_LogVGRatio_TextChanged(object sender, TextChangedEventArgs e)
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
                string value = textBox.Text;
                int chck = Convert.ToInt32(value);
                if (chck < 0)
                {
                    chck = 0;
                }
                else if (chck > 100)
                {
                    chck = 100;
                }
                //fixdec(value);
                // update text
                textBox.Text = chck.ToString();

                // restore cursor position and selection
                textBox.Select(start, length);
                Edit_LogPGRatio.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
            }
        }

        private void Edit_LogNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                Edit_LogNotesLimit.Text = "(128)";
                return;
            }
            else
            {
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                textBox.Select(start, length);
                Edit_LogNotesLimit.Text = "(" + Convert.ToString(Edit_LogNotes.MaxLength - textBox.SelectionStart) + ")";
            }
        }

        private void FillDataGrid()
        {
            string hdr0 = "", hdr1 = "", hdr2 = "", hdr3 = "", hdr4 = "", hdr5 = "", hdr6 = "";
            string dbTables = "[RecipeLog]";
            if (CurrentCulture == "en-US")
            {
                hdr0 = "#";
                hdr1 = "Name";
                hdr2 = "Amount (ml)";
                hdr3 = "Ratio (PG/VG)";
                hdr4 = "Nicotine (mg/ml)";
                hdr5 = "Date";
                hdr6 = "Notes";

            }
            if (CurrentCulture == "el-GR")
            {
                hdr0 = "#";
                hdr1 = "Ονομασία";
                hdr2 = "Ποσότητα (ml)";
                hdr3 = "Αναλογία (PG/VG)";
                hdr4 = "Νικοτίνη (mg/ml)";
                hdr5 = "Ημερομηνία";
                hdr6 = "Σημειώσεις";
            }
            try
            {
                using (dbConn = new SQLiteConnection(connectionString))
                {
                    dbConn.Open();
                    //Select Command
                    query = $"Select * from {dbTables} WHERE id > 0 ORDER BY [id] DESC; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    dbTable = new DataTable(dbTables);

                    dbAdapter.Fill(dbTable);
                    dbConn.Close();
                    JuiceLogList.ItemsSource = dbTable.DefaultView;
                    dbAdapter.Update(dbTable);

                    JuiceLogList.Columns[0].Header = hdr0;
                    JuiceLogList.Columns[1].Header = hdr1;
                    JuiceLogList.Columns[2].Header = hdr2;
                    JuiceLogList.Columns[3].Header = hdr3;
                    JuiceLogList.Columns[4].Header = hdr4;
                    JuiceLogList.Columns[5].Header = hdr5;
                    JuiceLogList.Columns[6].Header = hdr6;
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}