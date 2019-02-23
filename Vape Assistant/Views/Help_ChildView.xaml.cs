using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Controls;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for Help_ChildView.xaml
    /// </summary>
    public partial class Help_ChildView : UserControl
    {
        string connectionString = Settings.Default.VaConnect;
        SQLiteDataAdapter dbAdapter;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        //SQLiteDataReader reader;
        DataTable dbTable;
        public string CurrentCulture = Settings.Default.Culture;
        public int autotimeout = 5000;
        public string query;
        string caption;

        public string Caption { get => caption; set => caption = value; }

        public Help_ChildView()
        {
            InitializeComponent();
            FillDataGrid();
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void FillDataGrid()
        {
            string dbTables = "";
            if (CurrentCulture == "en-US")
            {
                dbTables = "Help_US";
                Caption = "Error";
            }
            if (CurrentCulture == "el-GR")
            {
                dbTables = "Help_GR";
                Caption = "Σφάλμα";
            }
            try
            {
                using (dbConn = new SQLiteConnection(connectionString))
                {
                    dbConn.Open();
                    //Select Command
                    query = $"SELECT id, question, answer FROM {dbTables} order by id ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    dbTable = new DataTable(dbTables);

                    dbAdapter.Fill(dbTable);
                    dbConn.Close();
                    help.ItemsSource = dbTable.DefaultView;
                    dbAdapter.Update(dbTable);
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), Caption, autotimeout);
            }
        }

    }

}
