using System;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for Flavors_ChildView.xaml
    /// </summary>
    public partial class Flavors_ChildView : UserControl
    {
        string connectionString = Settings.Default.VaConnect;
        SQLiteDataAdapter dbAdapter;
        SQLiteDataReader reader;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        public string CurrentCulture = Settings.Default.Culture;
        public string query;
        public int EntryId;
        public int autotimeout = 5000;

        public Flavors_ChildView()
        {
            InitializeComponent();
            BindComboBox(cmb_brandname);
            NextID();
        }

        private void flv_add_Click(object sender, RoutedEventArgs e)
        {
            int owned;
            if (brandName.Text == "") { return; }
            if (brandShort.Text == "") { return; }
            if (flv_Name.Text == "") { return; }
            if (Owned_y_n.IsChecked == true) { owned = 1; } else { owned = 0; }
            if (MsG.Text == "") { MsG.Text = "1.0"; }
            if (AmP.Text == "") { AmP.Text = "0.0"; }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            try
            {
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                string query;
                int Count = 0;
                if (cmb_brandname.SelectedIndex < 0)
                {
                    string puery = "SELECT Count(*) FROM FlavorBrands WHERE Name = '" + brandName.Text.Replace("'", "''") + "'";
                    dbCmd = new SQLiteCommand(puery, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Count = int.Parse(reader[0].ToString());
                    }
                    if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }

                    if (Count == 0)
                    {
                        if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                        string fuery = "INSERT INTO FlavorBrands ";
                        fuery += "([Name], [ShortName], [Flavor_Count]) ";
                        fuery += "VALUES('" + brandName.Text.Replace("'", "''") + "', '" + brandShort.Text + "', '0')";
                        dbCmd = new SQLiteCommand(fuery, dbConn);
                        dbCmd.ExecuteNonQuery();
                        if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }
                        dbCmd.Dispose();
                    }
                    else
                    {
                        if (CurrentCulture == "en-US")
                        {
                            MessageBox.Show("The Flavor Brand already exists.");
                        }
                        else if (CurrentCulture == "el-GR")
                        {
                            MessageBox.Show("Η εταιρία αρωμάτων υπάρχει ήδη.");
                        }
                    }
                }
                query = "SELECT Count(*) FROM Flavors WHERE Brand = '" + brandName.Text.Replace("'", "''") + "' and Flavor = '" + flv_Name.Text + "' ;";

                if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }

                dbCmd = new SQLiteCommand(query, dbConn);

                reader = dbCmd.ExecuteReader();

                while (reader.Read())
                {
                    Count = int.Parse(reader[0].ToString());
                }

                if (!reader.IsClosed) { reader.Close(); }
                dbCmd.Dispose();
                if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }

                if (Count == 0)
                {
                    if (cmb_flavname.SelectedIndex < 0)
                    {
                        query = "INSERT INTO Flavors ";
                        query += "([Brand], [BrandShort], [Flavor], [M_Spec_Grav], [Notes], [Average_Mixing], [Amount], [Owned]) ";
                        query += "VALUES('" + brandName.Text.Replace("'", "''") + "', '" + brandShort.Text + "', '" + flv_Name.Text.Replace("'", "''") + "', '"
                            + MsG.Text.Replace(",", ".") + "', '" + Notes.Text + "', '"
                            + AmP.Text.Replace(",", ".") + "', '0.0', " + owned + ");";

                        if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }

                        dbCmd = new SQLiteCommand(query, dbConn);

                        int resultAffectedRows = dbCmd.ExecuteNonQuery();

                        if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }

                        if (CurrentCulture == "en-US")
                        {
                            AutoClosingMessageBox.Show("Added successfully " + resultAffectedRows + " flavor!", "Success", autotimeout);
                            //                            MessageBox.Show("Added successfully " + resultAffectedRows + " entry!");
                        }
                        if (CurrentCulture == "el-GR")
                        {
                            AutoClosingMessageBox.Show("Προστέθηκε επιτυχώς " + resultAffectedRows + " άρωμα!", "Επιτυχία", autotimeout);
                            //MessageBox.Show("Προστέθηκε επιτυχώς " + resultAffectedRows + " εγγραφή!");
                        }
                    }
                    else
                    {
                        string qquery = "UPDATE FlavorBrands SET Flavor_count = Flavor_count + 1 WHERE ShortName = '" + brandShort.Text + "'; ";

                        if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }

                        dbCmd = new SQLiteCommand(qquery, dbConn);

                        dbCmd.ExecuteNonQuery();

                        dbConn.Close();
                    }
                }
                else
                {
                    query = "Update Flavors SET ";
                    query += $"Brand ='{ brandName.Text.Replace("'", "''") }', ";
                    query += $"BrandShort ='{ brandShort.Text }', ";
                    query += $"Flavor ='{ flv_Name.Text.Replace("'", "''") }', ";
                    query += $"M_Spec_Grav ='{ MsG.Text.Replace(",", ".") }', ";
                    query += $"Notes ='{ Notes.Text.Replace("'", "''") }', ";
                    query += $"Average_Mixing ='{ AmP.Text.Replace(",", ".")  }', ";
                    query += $"Owned ='{ owned }' ";
                    query += $"Where Id ='{ add_flv_id.Text }'; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                    int resultAffectedRows = dbCmd.ExecuteNonQuery();
                    dbConn.Close();
                    if (CurrentCulture == "en-US")
                    {
                        AutoClosingMessageBox.Show("Updated successfully " + resultAffectedRows + " entry!", "Success", 1500);
                    }
                    if (CurrentCulture == "el-GR")
                    {
                        AutoClosingMessageBox.Show("Ενημερώθηκε επιτυχώς " + resultAffectedRows + " εγγραφή!", "Επιτυχία", 1500);
                    }
                }
                query = "";
                query = "update Flavors SET M_Spec_Grav = replace(M_Spec_Grav, ',', '.') WHERE M_Spec_Grav LIKE '%,%'; ";
                query += "update Flavors SET Average_Mixing = replace(Average_Mixing, ',', '.') WHERE Average_Mixing LIKE '%,%'; ";
                dbCmd = new SQLiteCommand(query, dbConn);
                if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                dbCmd.ExecuteNonQuery();
                dbConn.Close();
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show("Error" + ex.ToString(), "Error", autotimeout);
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
                if (brandKeeper.IsChecked == false)
                {
                    BindComboBox(cmb_brandname);
                    brandName.Text = "";
                    brandShort.Text = "";
                }
                cmb_flavname.SelectedIndex = -1;
                flv_Name.Text = "";
                MsG.Text = "";
                Notes.Text = "";
                AmP.Text = "";
                Owned_y_n.IsChecked = false;
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

        private void cmb_brandname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (brandKeeper.IsChecked == false)
            {
                brandName.Text = "";
                brandShort.Text = "";
            }
            flv_Name.Text = "";
            cmb_flavname.SelectedIndex = -1;
            MsG.Text = "";
            Notes.Text = "";
            AmP.Text = "";
        }

        private void cmb_brandname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmb_brandname.SelectedIndex >= 0)
            {
                SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                SQLiteConnection con = dbConn;
                SQLiteCommand sqlcmd = new SQLiteCommand();
                try
                {
                    var t = cmb_brandname.Text.Replace("'", "''");
                    string query = $"SELECT * FROM [FlavorBrands] WHERE NAME='" + t + "'";
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int Id_Brand = int.Parse(reader[0].ToString());
                        add_flv_id.Text = EntryId.ToString();
                        string Brand = reader[1].ToString();
                        brandName.Text = Brand;
                        string shortname = reader[2].ToString();
                        brandShort.Text = shortname;
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
                    BlindComboBox(cmb_flavname);
                    if (dbConn.State == ConnectionState.Open)
                    {

                        dbConn.Close();
                    }
                }
            }
        }

        private void cmb_flavname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmb_brandname.SelectedIndex >= 0)
            {
                SQLiteConnection dbConn;
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                SQLiteConnection con = dbConn;
                SQLiteCommand sqlcmd = new SQLiteCommand();
                try
                {
                    string query = $"SELECT * FROM [Flavors] WHERE BrandShort='" + brandShort.Text + "' And Flavor ='" + cmb_flavname.Text.Replace("'", "''") + "' ";
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int Id_Flv = int.Parse(reader[0].ToString());
                        flv_Name.Text = cmb_flavname.Text;
                        add_flv_id.Text = Id_Flv.ToString();
                        string MsGz = reader[4].ToString();
                        MsG.Text = MsGz;
                        string Notez = reader[5].ToString();
                        Notes.Text = Notez;
                        string AMP = reader[6].ToString();
                        AmP.Text = AMP;
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
        }

        private void AmP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void MsG_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        public void BlindComboBox(ComboBox comboBoxName)
        {

            dbConn = new SQLiteConnection(connectionString);
            dbConn.Open();
            SQLiteCommand sqlcmd = new SQLiteCommand();
            dbAdapter = new SQLiteDataAdapter();
            DataSet ds = new DataSet();
            sqlcmd.Connection = dbConn;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = $"Select * from Flavors WHERE BrandShort ='{brandShort.Text}' ORDER BY Flavor ASC ";
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
        protected void NextID()
        {
            SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
            dbConn = new SQLiteConnection(connectionString);
            dbConn.Open();
            SQLiteConnection con = dbConn;
            SQLiteCommand sqlcmd = new SQLiteCommand();
            try
            {
                string query = "SELECT seq FROM [sqlite_sequence] WHERE NAME='Flavors'";
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    int value = int.Parse(reader[0].ToString()) + 1;
                    add_flv_id.Text = value.ToString();
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
                if (dbConn.State == ConnectionState.Open)
                {

                    dbConn.Close();
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
                Regex regex = new Regex("[^0-9,]+"); //regex that matches disallowed text
                return !regex.IsMatch(text);
            }
        }
    }
}
