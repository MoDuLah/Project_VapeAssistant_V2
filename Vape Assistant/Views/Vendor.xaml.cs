using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vape_Assistant.Properties;

namespace Vape_Assistant
{
    /// <summary>
    /// Interaction logic for Vendor.xaml
    /// </summary>
    public partial class Vendor : Window
    {
        string connectionString = Settings.Default.VaConnect;
        string pword = Settings.Default.Password;
        string CurrentCulture = Settings.Default.Culture;
        RegionInfo country;
        public int EntryId;
        public int affectedrows;
        public string caption,title,message;
        public int autotimeout = 5000;
        SQLiteDataAdapter dbAdapter;
        SQLiteDataReader reader;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        DataSet ds;
        string dbName = "";


        public Vendor()
        {
            InitializeComponent();
            PopulateCountryComboBox();
            FillCombobox(purchase_vendor);
        }

        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }


            public override string ToString()
            {
                return Text;
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }


        private void PopulateCountryComboBox()
        {
            if (CurrentCulture == "en-US")
            {
                country = new RegionInfo(new CultureInfo("en-US", false).LCID);
            }

            if (CurrentCulture == "el-GR")
            {
                country = new RegionInfo(new CultureInfo("el-GR", false).LCID);
            }

            List<string> countryNames = new List<string>();
            foreach (CultureInfo cul in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                country = new RegionInfo(new CultureInfo(cul.Name, false).LCID);
                countryNames.Add(country.DisplayName.ToString());
            }

            IEnumerable nameAdded = countryNames.OrderBy(names => names).Distinct();

            foreach (string item in nameAdded)
            {
                add_Country.Items.Add(item);
                edit_Country.Items.Add(item);
            }
        }

        private void Vendor_add_Click(object sender, RoutedEventArgs e)
        {

            string nameEN = "Field 'Name' can not be empty/blank.";
            string nameEL = "Το πεδίο 'Όνομα' δεν μπορεί να είναι κενό.";
            string countryEN = "Field 'Country' can not be empty/blank.";
            string countryEL = "Το πεδίο 'Χώρα' δεν μπορεί να είναι κενό.";
            if (string.IsNullOrEmpty(add_vendorName.Text))
            {
                if (CurrentCulture == "en-US")
                {
                    MessageBox.Show(nameEN);
                    return;
                }
                else if (CurrentCulture == "el-GR")
                {
                    MessageBox.Show(nameEL);
                    return;
                }
            }
            if (string.IsNullOrEmpty(add_Country.Text))
            {
                if (CurrentCulture == "en-US")
                {
                    MessageBox.Show(countryEN);
                    return;
                }
                else if (CurrentCulture == "el-GR")
                {
                    MessageBox.Show(countryEL);
                    return;
                }
            }
            int check;
            if (add_Active.IsChecked == true)
            {
                check = 1;
            }
            else
            {
                check = 0;
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            try
            {
                string name, address, postcode, city, county, country, telephone, website, orders;
                int active;
                name = add_vendorName.Text;
                address = add_Address.Text;
                postcode = add_PostCode.Text;
                city = add_City.Text;
                county = add_County.Text;
                country = add_Country.Text;
                telephone = add_Telephone.Text;
                website = add_Website.Text;
                active = check;
                orders = "0";

                if (CurrentCulture == "el-GR")
                {
                    dbName = "Vendors_GR";
                }
                else
                {
                    dbName = "Vendors";
                }

                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                string query = $"INSERT INTO [{dbName}] ";
                if (CurrentCulture == "en-US")
                { 
                    query += "([Name], [Address], [PostCode], [City], [County], [Country], [Telephone], [Website], [Active], [Orders]) ";
                }
                if (CurrentCulture == "el-GR")
                {
                    query += "([Επωνυμία], [Διεύθυνση], [ΤΚ], [Πόλη], [Νομός], [Χώρα], [Τηλέφωνο], [Ιστοσελίδα], [Ενεργός], [Παραγγελίες]) ";
                }
                query += $"VALUES('{name}', '{address}', '{postcode}', '{city}', '{county}', '{country}', '{telephone}', '{website}', '{check}', '{orders}') ; ";
                dbCmd = new SQLiteCommand(query, dbConn);
                dbCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.ToString());
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
                dbCmd.Dispose();
            }
            finally
            {
                dbCmd.Dispose();
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
                FillCombobox(purchase_vendor);
                purchase_vendor.SelectedIndex = -1;
                add_vendorName.Text = "";
                add_Address.Text = "";
                add_PostCode.Text = "";
                add_Website.Text = "";
                add_Country.Text = "";
                add_City.Text = "";
                add_Telephone.Text = "";
                add_County.Text = "";
                add_Active.IsChecked = false;
            }
        }

        private void Load_table(object sender, RoutedEventArgs e)
        {
            string dbName = "";


            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            try
            {
                DataTable dt = null;
                dbConn = new SQLiteConnection(connectionString);
                string query = "";
                if (CurrentCulture == "el-GR")
                {
                    dbName = "Vendors_GR";
                    query = $"SELECT Επωνυμία,Ιστοσελίδα,Τηλέφωνο,Διεύθυνση,ΤΚ,Πόλη,Χώρα,Ενεργός FROM [{dbName}] WHERE Ενεργός = '1'";
                }

                if (CurrentCulture == "en-US")
                {
                    dbName = "Vendors";
                    query = $"SELECT Name,Website,Address,Telephone,PostCode,City,Country,Active FROM [{dbName}] WHERE Active = '1'";
                }

                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                dbCmd.ExecuteNonQuery();
                dbAdapter = new SQLiteDataAdapter(dbCmd);

                if (CurrentCulture == "el-GR")
                {
                    dt = new DataTable("Vendors_GR");
                }

                if (CurrentCulture == "en-US")
                {
                    dt = new DataTable("Vendors");
                }

                dbAdapter.Fill(dt);
                ViewEntries.ItemsSource = dt.DefaultView;
                dbAdapter.Update(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.ToString());
                dbAdapter.Dispose();
                dbCmd.Dispose();
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
            finally
            {
                dbAdapter.Dispose();
                dbCmd.Dispose();
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
        }
        public void FillCombobox(ComboBox cmbName)
        {
            string dbName = "";
            string companyName = "";

            if (CurrentCulture == "el-GR")
            {
                dbName = "Vendors_GR";
                companyName = "Επωνυμία";
            }
            else
            {
                dbName = "Vendors";
                companyName = "Name";
            }
            try
            {
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                dbCmd = new SQLiteCommand();
                dbAdapter = new SQLiteDataAdapter();
                ds = new DataSet();
                dbCmd.Connection = dbConn;
                dbCmd.CommandType = CommandType.Text;
                dbCmd.CommandText = $"Select * from {dbName} Order by {companyName} ASC ";
                dbAdapter.SelectCommand = dbCmd;
                dbAdapter.Fill(ds, dbName);
                //DataRow nRow = ds.Tables["Vendors"].NewRow();
                //ds.Tables["Vendors"].Rows.InsertAt(nRow, 1);
                cmbName.ItemsSource = ds.Tables[0].DefaultView;
                cmbName.DisplayMemberPath = ds.Tables[0].Columns[companyName].ToString();
                cmbName.SelectedValuePath = ds.Tables[0].Columns["Id"].ToString();

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
                dbAdapter.Dispose();
                dbCmd.Dispose();
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
        }

        private void purchase_vendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string dbName = "";
            string companyName = "";

            if (CurrentCulture == "el-GR")
            {
                dbName = "Vendors_GR";
                companyName = "Επωνυμία";
            }
            else
            {
                dbName = "Vendors";
                companyName = "Name";
            }
            if ((sender as ComboBox).SelectedIndex < 0 )
            {
                return;
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            try
            {
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                string query = $"SELECT * FROM {dbName} where Id = '{purchase_vendor.SelectedValue.ToString()}' Order by {companyName} ASC ";

                dbCmd = new SQLiteCommand(query, dbConn);
                dbCmd.ExecuteNonQuery();
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                reader = dbCmd.ExecuteReader();

                while (reader.Read())
                {
                    string sId = reader[0].ToString();
                    string sName = reader[1].ToString();
                    string sAddress = reader[2].ToString();
                    string sPostCode = reader[3].ToString();
                    string sTelephone = reader[7].ToString();
                    string sWebsite = reader[8].ToString();
                    string sCity = reader[4].ToString();
                    string sCountry = reader[6].ToString();
                    int sActive = Convert.ToInt32(reader[9]);
                    edit_txt_Id.Text = sId;
                    edit_vendorName.Text = sName;
                    edit_Address.Text = sAddress;
                    edit_PostCode.Text = sPostCode;
                    edit_Telephone.Text = sTelephone;
                    edit_Website.Text = sWebsite;
                    edit_City.Text = sCity;
                    edit_Country.Text = sCountry;
                    if (sActive == 1)
                    {
                        edit_Active.IsChecked = true;
                    }
                    else
                    {
                        edit_Active.IsChecked = false;
                    }
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

        private void edit_Submit_Click(object sender, RoutedEventArgs e)
        {
            string dbName = "";
            string dbCompanyName = "";
            string dbAddress = "";
            string dbPostCode = "";
            string dbWebsite = "";
            string dbCountry = "";
            string dbCity = "" ;
            string dbTelephone = "";
            string dbActive = "";

            if (CurrentCulture == "el-GR")
            {
                dbName = "Vendors_GR";
                dbCompanyName = "Επωνυμία";
                dbAddress = "Διεύθυνση";
                dbPostCode = "ΤΚ";
                dbWebsite = "Ιστοσελίδα";
                dbCountry = "Χώρα";
                dbCity = "Πόλη";
                dbTelephone = "Τηλέφωνο";
                dbActive = "Ενεργός";
            }
            else
            {
                dbName = "Vendors";
                dbCompanyName = "Name";
                dbAddress = "Address";
                dbPostCode = "PostCode";
                dbWebsite = "Website";
                dbCountry = "Country";
                dbCity = "City";
                dbTelephone = "Telephone";
                dbActive = "Active";
            }
            string nameEN = "Field 'Name' can not be empty/blank.";
            string nameEL = "Το πεδίο 'Όνομα' δεν μπορεί να είναι κενό.";
            string countryEN = "Field 'Country' can not be empty/blank.";
            string countryEL = "Το πεδίο 'Χώρα' δεν μπορεί να είναι κενό.";
            if (string.IsNullOrEmpty(edit_vendorName.Text)) {
                if (CurrentCulture == "en-US") {
                    MessageBox.Show(nameEN);
                    return;
                }
                else if (CurrentCulture == "el-GR")
                {
                    MessageBox.Show(nameEL);
                    return;
                }
            }
            if (string.IsNullOrEmpty(edit_Country.Text))
            {
                if (CurrentCulture == "en-US")
                {
                    MessageBox.Show(countryEN);
                    return;
                }
                else if (CurrentCulture == "el-GR")
                {
                    MessageBox.Show(countryEL);
                    return;
                }
            }
            int check;
            if (edit_Active.IsChecked == true)
            {
                check = 1;
            }
            else
            {
                check = 0;
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            try
            {
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                string query = $"Update {dbName} ";
                query += "Set ";
                query += $" {dbCompanyName} = '{edit_vendorName.Text}',";
                query += $" {dbAddress} = '{edit_Address.Text}', ";
                query += $" {dbPostCode} = '{edit_PostCode.Text}', ";
                query += $" {dbWebsite} = '{edit_Website.Text}', ";
                query += $" {dbCountry} = '{edit_Country.Text}', ";
                query += $" {dbCity} = '{edit_City.Text}', ";
                query += $" {dbTelephone} = '{edit_Telephone.Text}',";
                query += $" {dbActive} = '{check}' Where Id = '{edit_txt_Id.Text}' ";

                dbCmd = new SQLiteCommand(query, dbConn);
                //dbConn.Open();
                int resultAffectedRows = dbCmd.ExecuteNonQuery();
                if (CurrentCulture == "en-US")
                {
                    MessageBox.Show("Updated successfully " + resultAffectedRows + " entries!");
                }
                else
                {
                    MessageBox.Show("Ενημερώθηκε/αν επιτυχώς " + resultAffectedRows + " εγγραφή/ές!");
                }
                dbConn.Close();
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
                    FillCombobox(purchase_vendor);
                    purchase_vendor.SelectedIndex = -1;
                    edit_vendorName.Text = "";
                    edit_Address.Text = "";
                    edit_PostCode.Text = "";
                    edit_Website.Text = "";
                    edit_Country.Text = "";
                    edit_City.Text = "";
                    edit_Telephone.Text = "";
                    edit_Active.IsChecked = false;
            }

        }

        private void edit_delete_Click(object sender, RoutedEventArgs e)
        {
            string dbName = "";

            if (CurrentCulture == "el-GR")
            {
                dbName = "Vendors_GR";
            }
            else
            {
                dbName = "Vendors";
            }
            string nameEN = "Field 'Name' can not be empty/blank.";
            string nameEL = "Το πεδίο 'Όνομα' δεν μπορεί να είναι κενό.";
            string countryEN = "Field 'Country' can not be empty/blank.";
            string countryEL = "Το πεδίο 'Χώρα' δεν μπορεί να είναι κενό.";
            if (string.IsNullOrEmpty(edit_vendorName.Text))
            {
                if (CurrentCulture == "en-US")
                {
                    MessageBox.Show(nameEN);
                    return;
                }
                else if (CurrentCulture == "el-GR")
                {
                    MessageBox.Show(nameEL);
                    return;
                }
            }
            if (string.IsNullOrEmpty(edit_Country.Text))
            {
                if (CurrentCulture == "en-US")
                {
                    MessageBox.Show(countryEN);
                    return;
                }
                else if (CurrentCulture == "el-GR")
                {
                    MessageBox.Show(countryEL);
                    return;
                }
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            try
            {
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                string query = $"Delete From {dbName} Where Id = '{edit_txt_Id.Text}' ";

                dbCmd = new SQLiteCommand(query, dbConn);
                //dbConn.Open();
                int resultAffectedRows = dbCmd.ExecuteNonQuery();
                if (CurrentCulture == "en-US")
                {
                    MessageBox.Show("Deleted successfully " + resultAffectedRows + " entries!");
                }
                else
                {
                    MessageBox.Show("Διαγράφηκε/αν επιτυχώς " + resultAffectedRows + " εγγραφή/ές!");
                }
                dbConn.Close();
            }
            catch (Exception ex)
            {
                caption = "Error";
                AutoClosingMessageBox.Show(ex.ToString(),caption,autotimeout);
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
                    FillCombobox(purchase_vendor);
                    purchase_vendor.SelectedIndex = -1;
                    edit_vendorName.Text = "";
                    edit_Address.Text = "";
                    edit_PostCode.Text = "";
                    edit_Website.Text = "";
                    edit_Country.Text = "";
                    edit_City.Text = "";
                    edit_Telephone.Text = "";
                    edit_Active.IsChecked = false;
            }

        }

    }
}
