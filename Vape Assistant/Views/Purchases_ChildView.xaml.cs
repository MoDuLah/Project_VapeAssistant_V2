using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vape_Assistant.Properties;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Net;
using System.IO;

namespace Vape_Assistant.Views
{

    public partial class Purchases_ChildView : UserControl
    {

        string connectionstring = Settings.Default.VaConnect;
        public string Vendor;
        public int vendorId;
        public int EntryId;
        public int affectedrows;
        object Pitem;
        public string query;
        public int autotimeout = 5000;
        SQLiteDataAdapter dbAdapter;
        SQLiteDataReader reader;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        string connectionString = Settings.Default.VaConnect;
        string CurrentCulture = Settings.Default.Culture;
        public string message, errmsg;
        public string caption, title;
        DataTable mTable, dt;
        string editDate, editReference, editName, editDescription, editPID, editVID, editQuantity, editPrice, editDiscount, editShipping, editTotal;
        int editId;
        string hdr0 = "", hdr1 = "", hdr2 = "", hdr3 = "", hdr4 = "", hdr5 = "", hdr6 = "", hdr7 = "", hdr8 = "", hdr9 = "", hdr10 = "", hdr11 = "";

         public Purchases_ChildView()
        {
            if (CurrentCulture == "en-US")
            {
                hdr0 = " # ";
                hdr1 = " Order Date ";
                hdr2 = " Order Ref. ";
                hdr3 = " Company ";
                hdr4 = " Company ID ";
                hdr5 = " Product Desc. ";
                hdr6 = " Product ID (SKU) ";
                hdr7 = " Quantity ";
                hdr8 = " Price ($) ";
                hdr9 = " Discount ($) ";
                hdr10 = " Shipping ($) ";
                hdr11 = " Total Cost ($) ";
            }

            if (CurrentCulture == "el-GR")
            {
                hdr0 = " # ";
                hdr1 = " Ημ/νία Παραγγελίας ";
                hdr2 = " Αρ. Παραστατικού ";
                hdr3 = " Επων. Εταιρίας ";
                hdr4 = " Κωδ. Εταίριας ";
                hdr5 = " Περ. Προϊόντος ";
                hdr6 = " Κωδ. Προϊόντος (SKU) ";
                hdr7 = " Ποσότητα ";
                hdr8 = " Τιμή (€) ";
                hdr9 = " Έκπτωση (€) ";
                hdr10 = " Μεταφορικά (€) ";
                hdr11 = " Τελικό Κόστος (€) ";
            }

            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(TextBox), GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus));

            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != "." ||
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ",")
            {
                //Handler attach - will not be done if not needed
                PreviewKeyDown += new KeyEventHandler(ShellView_PreviewKeyDown);
            }
            BindComboBox(purchase_vendor_add);
            BindComboBox(purchase_vendor_view);
            BindComboBox(EditNameBox);
            purchase_date.Height = purchase_vendor_add.Height;
            NextID();
            FillDataGrid();
            RetrieveOrders();
            HideTheRest();
        }
        void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null && !textBox.IsReadOnly && e.KeyboardDevice.IsKeyDown(Key.Tab))
                textBox.SelectAll();
        }

        public void HideTheRest()
        {
            //hides and clears all the rows below recipe
            TextBlock[] sn = { sn1, sn2, sn3, sn4, sn5, sn6, sn7, sn8, sn9, sn10, sn11, sn12, sn13, sn14, sn15, sn16, sn17, sn18, sn19, sn20 };
            TextBox[] productArray = { product1, product2, product3, product4, product5, product6, product7, product8, product9, product10, product11, product12, product13, product14, product15, product16, product17, product18, product19, product20 };
            TextBox[] skuArray = { sku1, sku2, sku3, sku4, sku5, sku6, sku7, sku8, sku9, sku10, sku11, sku12, sku13, sku14, sku15, sku16, sku17, sku18, sku19, sku20 };
            TextBox[] quantityArray = { quantity1, quantity2, quantity3, quantity4, quantity5, quantity6, quantity7, quantity8, quantity9, quantity10, quantity11, quantity12, quantity13, quantity14, quantity15, quantity16, quantity17, quantity18, quantity19, quantity20 };
            TextBox[] priceArray = { price1, price2, price3, price4, price5, price6, price7, price8, price9, price10, price11, price12, price13, price14, price15, price16, price17, price18, price19, price20 };
            TextBox[] discountArray = { discount1, discount2, discount3, discount4, discount5, discount6, discount7, discount8, discount9, discount10, discount11, discount12, discount13, discount14, discount15, discount16, discount17, discount18, discount19, discount20 };
            TextBox[] shippingArray = { shipping1, shipping2, shipping3, shipping4, shipping5, shipping6, shipping7, shipping8, shipping9, shipping10, shipping11, shipping12, shipping13, shipping14, shipping15, shipping16, shipping17, shipping18, shipping19, shipping20 };
            TextBox[] totalArray = { total1, total2, total3, total4, total5, total6, total7, total8, total9, total10, total11, total12, total13, total14, total15, total16, total17, total18, total19, total20 };
            int y = Convert.ToInt32(additems_Count.Text);
            for (int i = y; i < productArray.Length; i++)
                {
                sn[i].Visibility = Visibility.Collapsed;
                productArray[i].Visibility = Visibility.Collapsed;
                skuArray[i].Visibility = Visibility.Collapsed;
                quantityArray[i].Visibility = Visibility.Collapsed;
                priceArray[i].Visibility = Visibility.Collapsed;
                discountArray[i].Visibility = Visibility.Collapsed;
                shippingArray[i].Visibility = Visibility.Collapsed;
                totalArray[i].Visibility = Visibility.Collapsed;
                sn[i].Text = Convert.ToString(i + 1) + ".";
                productArray[i].Text = "";
                skuArray[i].Text = "";
                quantityArray[i].Text = "";
                priceArray[i].Text = "";
                discountArray[i].Text = "";
                shippingArray[i].Text = "";
                totalArray[i].Text = "";
                }
            sn[0].Text = Convert.ToString(0 + 1) + ".";
            productArray[0].Text = "";
            skuArray[0].Text = "-";
            quantityArray[0].Text = "0";
            priceArray[0].Text = "0.0";
            discountArray[0].Text = "0.0";
            shippingArray[0].Text = "0.0";
            totalArray[0].Text = "0.0";
        }

        public static void ShellView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;

                if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Length > 0)
                {
                    Keyboard.FocusedElement.RaiseEvent(
                        new TextCompositionEventArgs(
                            InputManager.Current.PrimaryKeyboardDevice,
                            new TextComposition(InputManager.Current,
                                Keyboard.FocusedElement,
                                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                            )
                        { RoutedEvent = TextCompositionManager.TextInputEvent });
                }
            }
        }
        private void ShowChildWindow()
        {
            Window childWindow = new Vendor
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "temp.tmp";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Vendor;");
            }
            childWindow.Closed += ChildWindowClosed;
            childWindow.Show();
        }
        private void ChildWindowClosed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= ChildWindowClosed;
            BindComboBox(purchase_vendor_add); // Κάνει UPDATE το Combobox. :)
            BindComboBox(purchase_vendor_view);
        }
        public void BindComboBox(ComboBox comboBoxName)
        {
            try
            {
                dbConn = new SQLiteConnection(connectionstring);
                dbConn.Open();
                dbCmd = new SQLiteCommand();
                dbAdapter = new SQLiteDataAdapter();
                DataSet ds = new DataSet();
                dbCmd.Connection = dbConn;
                dbCmd.CommandType = CommandType.Text;
                dbCmd.CommandText = $"Select * FROM [Vendors] WHERE [Active] = '1' Order by Name ASC ";
                dbAdapter.SelectCommand = dbCmd;
                dbAdapter.Fill(ds, "Vendors");
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["Name"].ToString();
                comboBoxName.SelectedValuePath = ds.Tables[0].Columns["Id"].ToString();
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
                dbAdapter.Dispose();
                dbCmd.Dispose();
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
        }
        public void BlindComboBox(ComboBox comboBoxName)
        {
            try
            {
                dbConn = new SQLiteConnection(connectionstring);
                dbConn.Open();
                dbCmd = new SQLiteCommand();
                dbAdapter = new SQLiteDataAdapter();
                DataSet ds = new DataSet();
                dbCmd.Connection = dbConn;
                dbCmd.CommandType = CommandType.Text;
                dbCmd.CommandText = $"Select distinct [Reference] From [Purchases] WHERE [Name] ='" + purchase_vendor_view.Text + "' ORDER BY Reference ASC  ";
                dbAdapter.SelectCommand = dbCmd;
                dbAdapter.Fill(ds, "Reference");
                comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
                comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["Reference"].ToString();
                comboBoxName.IsEnabled = true;
                //comboBoxName.SelectedValuePath = ds.Tables[0].Columns["Id"].ToString();
            }
            catch (Exception ex)
            {

                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
                comboBoxName.IsEnabled = false;
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
        private void emptyTextBoxColoring(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text)) { 
                textBox.BorderBrush = Brushes.Red;
                textBox.BorderThickness = new Thickness(2, 2, 2, 2);
            }
            else
            {
                textBox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFABADB3"));
                textBox.BorderThickness = new Thickness(1, 1, 1, 1);
            }
        }
        private void purchase_submit_Click(object sender, RoutedEventArgs e)
        {
            string sMonth;
            string sDay;
            int itemsCount = Convert.ToInt32(additems_Count.Text);
            retrieveVendorId();

            List<string> myProductList = new List<string>();
            List<string> mySKUList = new List<string>();
            List<string> myQuantityList = new List<string>();
            List<string> myPriceList = new List<string>();
            List<string> myDiscountList = new List<string>();
            List<string> myShippingList = new List<string>();
            List<string> myTotalList = new List<string>();
            TextBox[] productArray = { product1, product2, product3, product4, product5, product6, product7, product8, product9, product10, product11, product12, product13, product14, product15, product16, product17, product18, product19, product20 };
            TextBox[] skuArray = { sku1, sku2, sku3, sku4, sku5, sku6, sku7, sku8, sku9, sku10, sku11, sku12, sku13, sku14, sku15, sku16, sku17, sku18, sku19, sku20 };
            TextBox[] quantityArray = { quantity1, quantity2, quantity3, quantity4, quantity5, quantity6, quantity7, quantity8, quantity9, quantity10, quantity11, quantity12, quantity13, quantity14, quantity15, quantity16, quantity17, quantity18, quantity19, quantity20 };
            TextBox[] priceArray = { price1, price2, price3, price4, price5, price6, price7, price8, price9, price10, price11, price12, price13, price14, price15, price16, price17, price18, price19, price20 };
            TextBox[] discountArray = { discount1, discount2, discount3, discount4, discount5, discount6, discount7, discount8, discount9, discount10, discount11, discount12, discount13, discount14, discount15, discount16, discount17, discount18, discount19, discount20 };
            TextBox[] shippingArray = { shipping1, shipping2, shipping3, shipping4, shipping5, shipping6, shipping7, shipping8, shipping9, shipping10, shipping11, shipping12, shipping13, shipping14, shipping15, shipping16, shipping17, shipping18, shipping19, shipping20 };
            TextBox[] totalArray = { total1, total2, total3, total4, total5, total6, total7, total8, total9, total10, total11, total12, total13, total14, total15, total16, total17, total18, total19, total20 };

            if ((purchase_date.cmbMonths.SelectedIndex < 9) && (purchase_date.cmbMonths.SelectedIndex >= 0)) {
                sMonth = "0" + Convert.ToString(purchase_date.cmbMonths.SelectedIndex + 1);
            }
            else
            {
                sMonth = Convert.ToString(purchase_date.cmbMonths.SelectedIndex + 1);
            }
            if ((Convert.ToInt32(purchase_date.cmbDays.Text) <= 9) && (Convert.ToInt32(purchase_date.cmbDays.Text) >= 0))
            {
                sDay = "0" + Convert.ToString(purchase_date.cmbDays.Text);
            }
            else
            {
                sDay = Convert.ToString(purchase_date.cmbDays.Text);
            }

            string sDate = purchase_date.cmbYear.Text + "-" + sMonth + "-" + sDay;
            purchase_error_catch.Text = "";
            if (string.IsNullOrEmpty(purchase_ord_num.Text))
            {
                dbConn = new SQLiteConnection(connectionstring);
                query = $"SELECT Count(Distinct Reference) FROM [Purchases] Where Reference Like 'MY%' ; ";
                dbCmd = new SQLiteCommand(query, dbConn);
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                reader = dbCmd.ExecuteReader();

                while (reader.Read())
                {
                    int temp = Convert.ToInt32(reader[0].ToString()) + 1;

                    if (temp < 10) { 
                        purchase_ord_num.Text = "MY0" + temp.ToString();
                    }
                    else
                    {
                        purchase_ord_num.Text = "MY" + temp.ToString();
                    }
                 }
                reader.Close();
                dbCmd.Dispose();
                dbConn.Close();
            }
            for (int i = 0; i < itemsCount; i++)
            {
                if (string.IsNullOrEmpty(productArray[i].Text))
                {
                    MessageBox.Show($"Missing description for item: {i + 1}" +
                        $"\n\nWith SKU: {skuArray[i].Text}" +
                        $"\nQuantity: {quantityArray[i].Text}" +
                        $"\nPrice: {priceArray[i].Text}" +
                        $"\nDiscount: {discountArray[i].Text}" +
                        $"\nShipping: {shippingArray[i].Text}" +
                        $"\nTotal Cost: {totalArray[i].Text}");
                    emptyTextBoxColoring(productArray[i], e);
                    return;
                }
                else
                {
                    emptyTextBoxColoring(productArray[i], e);
                }

            }
            purchase_error_catch.Text = "";

            if (string.IsNullOrEmpty(sDate) || purchase_vendor_add.SelectedIndex < 0)
            {
                if (string.IsNullOrEmpty(sDate))
                {
                    purchase_error_catch.Text = purchase_error_catch.Text + "1";
                }
                else
                {
                    purchase_error_catch.Text = purchase_error_catch.Text + "0";
                }
                if (purchase_vendor_add.SelectedIndex < 0)
                {
                    purchase_error_catch.Text = purchase_error_catch.Text + "1";
                }
                else
                {
                    purchase_error_catch.Text = purchase_error_catch.Text + "0";
                }
                if (CurrentCulture == "en-US")
                {
                    switch (Convert.ToInt32(purchase_error_catch.Text))
                    {
                        case 01:
                            MessageBox.Show("The following field is empty:\n- " + purchase_vendor_lbl.Header, "Error: " + purchase_error_catch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            break;
                        case 10:
                            MessageBox.Show("The following field is empty:\n- " + purchase_date_lbl.Header, "Error: " + purchase_error_catch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            break;
                        case 11:
                            MessageBox.Show("The following fields are empty:\n- " + purchase_date_lbl.Header + "\n- " + purchase_vendor_lbl.Header, "Error: " + purchase_error_catch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            break;
                        default:
                            MessageBox.Show("Some required field has not been filled up.", "Error");
                            break;
                    }
                }
                if (CurrentCulture == "el-GR")
                {
                    switch (Convert.ToInt32(purchase_error_catch.Text))
                    {
                        case 01:
                            MessageBox.Show("Το παρακάτω πεδίο είναι κενό:\n- " + purchase_vendor_lbl.Header, "Σφάλμα: " + purchase_error_catch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            break;
                        case 100:
                            MessageBox.Show("Το παρακάτω πεδίο είναι κενό:\n- " + purchase_date_lbl.Header, "Σφάλμα: " + purchase_error_catch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            break;
                        case 11:
                            MessageBox.Show("Τα παρακάτω πεδία είναι κενά:\n- " + purchase_date_lbl.Header + "\n- " + purchase_vendor_lbl.Header, "Σφάλμα: " + purchase_error_catch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            break;
                        default:
                            MessageBox.Show("Κάποια από τις απαραίτητες τιμές δεν έχει συμπληρωθεί", "Σφάλμα");
                            break;
                    }
                }
            }
            else
            {
                int endresult = Convert.ToInt32(additems_Count.Text);
                int y = 0;

                for (int i = 0; i < endresult; i++)
                {
                    if (productArray[i].Visibility == Visibility.Visible)
                    {
                        myProductList.Add(productArray[i].Text);
                        mySKUList.Add(skuArray[i].Text);
                        myQuantityList.Add(quantityArray[i].Text);
                        myPriceList.Add(priceArray[i].Text);
                        myDiscountList.Add(discountArray[i].Text);
                        myShippingList.Add(shippingArray[i].Text);
                        myTotalList.Add(totalArray[i].Text);
                        y++;
                    }
                }
                var myProductArray = myProductList.ToArray();
                var mySKUArray = mySKUList.ToArray();
                var myQuantityArray = myQuantityList.ToArray();
                var myPriceArray = myPriceList.ToArray();
                var myDiscountArray = myDiscountList.ToArray();
                var myShippingArray = myShippingList.ToArray();
                var myTotalArray = myTotalList.ToArray();
                try
                    {
                    for (int i = 0; i < myProductArray.Length; i++)
                    {
                        dbConn = new SQLiteConnection(connectionstring);
                        if (dbConn.State == ConnectionState.Closed)
                        {
                            dbConn.Open();
                        }

                        Vendor = purchase_vendor_add.Text;
                        string order_numb = purchase_ord_num.Text;
                        string query = "Insert INTO Purchases ";
                        query += "([Date], [Reference], [Name], [VID], [Description], [PID], [Quantity], [Price], [Discount], [Shipping], [Total]) ";
                        query += $"VALUES('{sDate}', '{order_numb}', '{Vendor}', '{vendorId}', '{myProductArray[i].Replace("'", "''")}', '";
                        query += $"{mySKUArray[i]}', '{myQuantityArray[i]}', '{myPriceArray[i]}', '{myDiscountArray[i]}', '{myShippingArray[i]}' , '{myTotalArray[i]}') ; ";
                        dbCmd = new SQLiteCommand(query, dbConn);
                        dbCmd.ExecuteNonQuery();

                        dbCmd.Dispose();
                        if (dbConn.State == ConnectionState.Open)
                        {
                            dbConn.Close();
                        }
                    }

                    }
                    catch (Exception ex)
                    {
                        string errormsg = "";
                        if (CurrentCulture == "en-US")
                        {
                            errormsg = "Failure";
                        }
                        if (CurrentCulture == "el-GR")
                        {
                            errormsg = "Αποτυχία";
                        }
                        AutoClosingMessageBox.Show(ex.ToString(), errormsg, autotimeout);
                        dbConn.Close();
                    }
                    finally
                    {
                    int temp = 0;
                    dbConn = new SQLiteConnection(connectionstring);
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }
                    query = $"Select Count(distinct Reference) from Purchases where VID = {vendorId} ;";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        temp = Convert.ToInt32(reader[0].ToString());
                    }
                    reader.Close();

                    query = $"Update Vendors Set Orders = {temp} Where Id = '{vendorId}' ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();

                    dbCmd.Dispose();
                    if (dbConn.State == ConnectionState.Open)
                        {
                            dbConn.Close();
                        }
                    }
                purchase_Reset_Click(this, e);
                FixDecimalPoint();
                FillDataGrid();
                ReturnSumOrders();
            }
        }

        protected void FixDecimalPoint()
        {
            try
            {
                dbConn = new SQLiteConnection(connectionstring);
                dbConn.Open();
                dbCmd = new SQLiteCommand();

                string fuery = "";
                fuery = $"UPDATE Purchases SET Quantity = replace(Quantity, ',', '.') WHERE Quantity LIKE '%,%'; ";
                fuery += $"UPDATE Purchases SET Price = replace(Price, ',', '.')  WHERE Price LIKE '%,%'; ";
                fuery += $"UPDATE Purchases SET Discount = replace(Discount, ',', '.') WHERE Discount LIKE '%,%'; ";
                fuery += $"UPDATE Purchases SET Shipping = replace(Shipping, ',', '.') WHERE Shipping LIKE '%,%'; ";
                fuery += $"UPDATE Purchases SET Total = replace(Total, ',', '.') WHERE Total LIKE '%,%'; ";

                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }

                dbCmd = new SQLiteCommand(fuery, dbConn);
                dbCmd.ExecuteNonQuery();

                dbCmd.Dispose();
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
            catch (Exception ex)
            {
                string errormsg = "";
                if (CurrentCulture == "en-US")
                {
                    errormsg = "Failure: ";
                }
                if (CurrentCulture == "el-GR")
                {
                    errormsg = "Αποτυχία: ";
                }
                AutoClosingMessageBox.Show(ex.ToString(), errormsg, autotimeout);
                dbConn.Close();
            }
            finally
            {

                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }

            }
        }

        protected void ReturnSumOrders()
        {
            string purName = "Purchases";

            dbConn = new SQLiteConnection(connectionstring);
            dbConn.Open();

            string query = "";

            query = $"SELECT count(*) FROM {purName} WHERE Id > 0 ; ";
            dbCmd = new SQLiteCommand(query, dbConn);
            reader = dbCmd.ExecuteReader();
            while (reader.Read())
            {
                Orders.Text = reader[0].ToString();
            }

            query = $"SELECT Sum(Total) FROM {purName} WHERE Id > 0 ; ";
            dbCmd = new SQLiteCommand(query, dbConn);
            SQLiteDataReader freader = dbCmd.ExecuteReader();
            while (freader.Read())
            {
                sumOrders.Text = freader[0].ToString();
            }
            dbConn.Close();
        }

        protected void NextID()
        {
            string dbName = "Purchases";
            SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
            dbConn = new SQLiteConnection(connectionstring);
            dbConn.Open(); // Open the connection. Now you can fire SQL-Queries
            SQLiteConnection con = dbConn;
            SQLiteCommand sqlcmd = new SQLiteCommand();
            try
            {
                string query = $"SELECT seq FROM [sqlite_sequence] WHERE name='{dbName}'";
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    int value = reader.GetInt32(0);
                    EntryId = value + 1;
                    retrievedID.Text = EntryId.ToString();
                }
            }
            catch (Exception)
            {
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
        protected void retrieveVendorId()
        {
            SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
            dbConn = new SQLiteConnection(connectionstring);
            dbConn.Open(); // Open the connection. Now you can fire SQL-Queries
            SQLiteConnection con = dbConn;
            SQLiteCommand sqlcmd = new SQLiteCommand();
            try
            {

                string query = $"SELECT ID FROM [Vendors] WHERE [Name] = '{purchase_vendor_add.Text}' ; ";
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    int value = int.Parse(reader[0].ToString());
                    vendorId = value;
                }
                dbCmd.Dispose();
            }
            catch (Exception)
            {
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

        protected void RetrieveOrders()
        {
            SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
            dbConn = new SQLiteConnection(connectionstring);

            string CaSe = "";
            string companyName = "Name";
            string purName = "Purchases";
            string orderReference = "Reference";
            string orderDate = "Date";
            string totalCost = "Total";

            try
            {
                if (purchase_vendor_add.SelectedIndex > 0)
                {
                    CaSe = "VendorSelected";
                    string query = $"SELECT DISTINCT([{orderReference}] || ' - ' || [{companyName}]) FROM {purName} WHERE {companyName} = '{purchase_vendor_add.Text}' Order by {orderDate} DESC ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                }
                else if (purchase_vendor_view.SelectedIndex > 0)
                {
                    CaSe = "VendorSelected";
                    string query = $"SELECT DISTINCT([{orderReference}] || ' - ' || [{companyName}]) FROM {purName} WHERE {companyName} = '{purchase_vendor_view.Text}' Order by {orderDate} DESC ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                }
                else
                {
                    CaSe = "OrdersSelected";
                    string query = $"SELECT DISTINCT([{orderReference}] || ' - ' || [{companyName}]) FROM {purName} Order by {orderDate} DESC  ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                }
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                int i = 0;
                string xxx = "0";
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    i++;
                    if (CaSe == "VendorSelected")
                    {
                        xxx = reader.GetInt32(0).ToString();
                        retrievedOrders.Text = xxx;
                        Orders.Text = xxx.ToString();
                    } else
                    {
                        int y = i - 1;
                        retrievedOrders.Text = y.ToString();
                        Orders.Text = y.ToString();
                    }
                }
                dbCmd.Dispose();
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }

            }
            catch (Exception)
            {
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
            finally
            {
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                string fuery = "";
                string name = "";
                if (purchase_vendor_add.Text != "")
                {
                    name = purchase_vendor_add.Text;
                }
                if (purchase_vendor_view.Text != "")
                {
                    name = purchase_vendor_view.Text;
                }
                if (CaSe == "VendorSelected") {
                    fuery = $"Select Sum({totalCost}) FROM {purName} WHERE {companyName} = '{name}' ";
                    dbCmd = new SQLiteCommand(fuery, dbConn);
                }
                if (CaSe == "OrdersSelected")
                {
                    fuery = $"Select Sum({totalCost}) FROM {purName}";
                    dbCmd = new SQLiteCommand(fuery, dbConn);
                }
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    SafeGetstring(reader, 0);
                    if (!reader.IsDBNull(0))
                    {
                        sumOrders.Text = reader.GetDouble(0).ToString();
                    }
                    else
                    {
                        sumOrders.Text = "0";
                    }

                }
                if (dbConn.State == ConnectionState.Open)
                {

                    dbConn.Close();
                }
            }
        }

        private void SafeGetstring(object sender, int e)
        {
            if (!reader.IsDBNull(e)) {
                reader.GetDouble(e);
            }
        }

        private void Load_table(object sender, RoutedEventArgs e)
        {
            try
            {
                dbConn = new SQLiteConnection(connectionstring);
                string s = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));
                string dSinceD;
                string dUntilD;
                string mSinceM;
                string mUntilM;
                string purName = "Purchases";
                string companyName = "Name";
                string orderDate = "Date";
                string query;
                int dSinceDays = Convert.ToInt32(datepSince.cmbDays.Text);
                int dUntilDays = Convert.ToInt32(datepUntil.cmbDays.Text);
                int mSinceMonths = datepSince.cmbMonths.SelectedIndex + 1;
                int mUntilMonths = datepUntil.cmbMonths.SelectedIndex + 1;

                if (dSinceDays < 10)
                {
                    dSinceD = "0" + dSinceDays.ToString();
                }
                else
                {
                    dSinceD = dSinceDays.ToString();
                }
                if (dUntilDays < 10)
                {
                    dUntilD = "0" + dUntilDays.ToString();
                }
                else
                {
                    dUntilD = dUntilDays.ToString();
                }
                if (mSinceMonths < 10)
                {
                    mSinceM = "0" + mSinceMonths.ToString();
                }
                else
                {
                    mSinceM = mSinceMonths.ToString();
                }
                if (mUntilMonths < 10)
                {
                    mUntilM = "0" + mUntilMonths.ToString();
                }
                else
                {
                    mUntilM = mUntilMonths.ToString();
                }
                string dSince = datepSince.cmbYear.Text + "-" + mSinceM + "-" + dSinceD;
                string dUntil = datepUntil.cmbYear.Text + "-" + mUntilM + "-" + dUntilD;

                bool result = s.Equals(dSince, StringComparison.Ordinal);
                bool result2 = s.Equals(dUntil, StringComparison.Ordinal);


                if (string.IsNullOrEmpty(purchase_vendor_view.Text) && result == true && result2 == true)
                {
                    dbConn.Open();
                    query = $"SELECT * FROM {purName} WHERE id >= '1' ORDER BY Date DESC ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();

                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    dt = new DataTable(purName);

                    dbAdapter.Fill(dt);
                    dbConn.Close();
                    ViewEntries.ItemsSource = dt.DefaultView;
                    dbAdapter.Update(dt);

                    query = $"SELECT Count(distinct Reference) FROM {purName} WHERE id >= '1' ; ";
                    dbConn.Open();
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Orders.Text = reader[0].ToString();
                    }
                    dbConn.Close();

                    query = $"SELECT Sum(Total) FROM {purName} WHERE id >= '1' ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbConn.Open();
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        if (string.IsNullOrEmpty(freader[0].ToString())) { break; }
                        double value = double.Parse(freader[0].ToString());
                        sumOrders.Text = value.ToString();
                    }
                }
                if (result == false && result2 == false && purchase_vendor_view.SelectedIndex < 0)
                {
                    dbConn.Open();
                    query = $"Select * FROM [{purName}] WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') ORDER BY Date DESC ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    dt = new DataTable(purName);

                    dbAdapter.Fill(dt);
                    ViewEntries.ItemsSource = dt.DefaultView;
                    dbAdapter.Update(dt);
                    dbConn.Close();

                    query = $"SELECT Count(distinct Reference) FROM {purName} WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') ; ";
                    dbConn.Open();
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Orders.Text = reader[0].ToString();
                    }
                    dbConn.Close();

                    query = "";
                    query = $"SELECT Sum(Total) FROM {purName} WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') ;";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbConn.Open();
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        double value = 0.0;
                        if (string.IsNullOrEmpty(freader[0].ToString()))
                        {
                            value = 0.0;
                        }
                        else
                        {
                            value = double.Parse(freader[0].ToString());
                        }
                        sumOrders.Text = value.ToString();
                    }
                    dbConn.Close();

                }

                else if (result == true && result2 == true && purchase_vendor_view.SelectedIndex >= 0)
                {
                    if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                    ViewEntries.ItemsSource = "";
                    if (purchase_vendor_view.SelectedIndex == 0)
                    {
                        query = $"Select * FROM [{purName}] WHERE id >= 1 ORDER BY Date DESC ; ";
                    }
                    else
                    { 
                        query = $"Select * FROM [{purName}] WHERE [{companyName}] = '{purchase_vendor_view.Text}' AND id >= '1' ORDER BY Date DESC ; ";
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    DataTable dt = new DataTable(purName);
                    dbAdapter.Fill(dt);
                    ViewEntries.ItemsSource = dt.DefaultView;
                    dbAdapter.Update(dt);

                    if (purchase_vendor_view.SelectedIndex == 0)
                    {
                        query = $"Select Count(distinct Reference) FROM [{purName}] WHERE id >= '1' ORDER BY Date DESC ; ";
                    }
                    else
                    {
                        query = $"SELECT Count(distinct Reference) FROM {purName} WHERE [{companyName}] = '{purchase_vendor_view.Text}' AND id >= '1' ORDER BY Date DESC ; ";
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Orders.Text = reader[0].ToString();
                    }

                    if (purchase_vendor_view.SelectedIndex == 0)
                    {
                        query = $"Select Sum(Total) FROM [{purName}] WHERE id >= '1' ; ";
                    }
                    else
                    {
                        query = $"SELECT Sum(Total) FROM {purName} WHERE [{companyName}] = '{purchase_vendor_view.Text}' AND id >= '1' ; ";
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        double value = 0.0;
                        if (string.IsNullOrEmpty(freader[0].ToString()))
                        {
                            value = 0.0;
                        }
                        else
                        {
                            value = double.Parse(freader[0].ToString());
                        }                        
                        sumOrders.Text = value.ToString();
                    }
                    if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }

                }
                else
                {
                    if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                    ViewEntries.ItemsSource = "";
                    if (purchase_vendor_view.SelectedIndex == 0)
                    {
                        query = $"Select * FROM [{purName}] WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') AND id >= '1' ORDER BY Date DESC; ";
                    }
                    else
                    {
                        query = $"Select * FROM [{purName}] WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') AND [{companyName}] = '{purchase_vendor_view.Text}' AND id >= '1' ORDER BY Date DESC; ";
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    DataTable dt = new DataTable(purName);
                    dbAdapter.Fill(dt);
                    ViewEntries.ItemsSource = dt.DefaultView;
                    dbAdapter.Update(dt);

                    query = $"SELECT Count(distinct Reference) FROM {purName} WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') AND [{companyName}] = '{purchase_vendor_view.Text}' ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Orders.Text = reader[0].ToString();
                    }

                    query = "";
                    query = $"SELECT Sum(Total) FROM {purName} WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') AND [{companyName}] = '{purchase_vendor_view.Text}' ;  ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        if (string.IsNullOrEmpty(freader[0].ToString())) { break; }
                        double value = double.Parse(freader[0].ToString());
                        sumOrders.Text = value.ToString();
                    }
                    if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }
                }

            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error",autotimeout);
            }
            finally
            {
                ViewEntries.Columns[0].Header = hdr0;
                ViewEntries.Columns[1].Header = hdr1;
                ViewEntries.Columns[2].Header = hdr2;
                ViewEntries.Columns[3].Header = hdr3;
                ViewEntries.Columns[4].Header = hdr4;
                ViewEntries.Columns[5].Header = hdr5;
                ViewEntries.Columns[6].Header = hdr6;
                ViewEntries.Columns[7].Header = hdr7;
                ViewEntries.Columns[8].Header = hdr8;
                ViewEntries.Columns[9].Header = hdr9;
                ViewEntries.Columns[10].Header = hdr10;
                ViewEntries.Columns[11].Header = hdr11;
                if (dbConn.State == ConnectionState.Open)
                {

                    dbConn.Close();
                }
            }
        }

        private void Load_tabled(object sender, RoutedEventArgs e)
        {
            try
            {
                dbConn = new SQLiteConnection(connectionstring);
                string s = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));
                string dSinceD;
                string dUntilD;
                string mSinceM;
                string mUntilM;
                string purName = "Purchases";
                string companyName = "Name";
                string orderDate = "Date";
                string query;
                int dSinceDays = Convert.ToInt32(datepSince.cmbDays.Text);
                int dUntilDays = Convert.ToInt32(datepUntil.cmbDays.Text);
                int mSinceMonths = datepSince.cmbMonths.SelectedIndex + 1;
                int mUntilMonths = datepUntil.cmbMonths.SelectedIndex + 1;

                if (dSinceDays < 10)
                {
                    dSinceD = "0" + dSinceDays.ToString();
                }
                else
                {
                    dSinceD = dSinceDays.ToString();
                }
                if (dUntilDays < 10)
                {
                    dUntilD = "0" + dUntilDays.ToString();
                }
                else
                {
                    dUntilD = dUntilDays.ToString();
                }
                if (mSinceMonths < 10)
                {
                    mSinceM = "0" + mSinceMonths.ToString();
                }
                else
                {
                    mSinceM = mSinceMonths.ToString();
                }
                if (mUntilMonths < 10)
                {
                    mUntilM = "0" + mUntilMonths.ToString();
                }
                else
                {
                    mUntilM = mUntilMonths.ToString();
                }
                string dSince = datepSince.cmbYear.Text + "-" + mSinceM + "-" + dSinceD;
                string dUntil = datepUntil.cmbYear.Text + "-" + mUntilM + "-" + dUntilD;

                bool result = s.Equals(dSince, StringComparison.Ordinal);
                bool result2 = s.Equals(dUntil, StringComparison.Ordinal);



                if (string.IsNullOrEmpty(purchase_reference.Text) && result == true && result2 == true)
                {
                    dbConn.Open();
                    query = $"SELECT * FROM {purName} WHERE id >= '1' AND Name = '{purchase_vendor_view.Text}' ORDER BY Date DESC ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();

                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    dt = new DataTable(purName);

                    dbAdapter.Fill(dt);
                    dbConn.Close();
                    ViewEntries.ItemsSource = dt.DefaultView;
                    dbAdapter.Update(dt);

                    query = $"SELECT Count(distinct Reference) FROM {purName} WHERE id >= 1 AND Name = '{purchase_vendor_view.Text}' ; ";
                    dbConn.Open();
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Orders.Text = reader[0].ToString();
                    }
                    dbConn.Close();

                    query = $"SELECT Sum(Total) FROM {purName} WHERE id >= 1 AND Name = '{purchase_vendor_view.Text}'; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbConn.Open();
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        if (string.IsNullOrEmpty(freader[0].ToString())) { break; }
                        double value = double.Parse(freader[0].ToString());
                        sumOrders.Text = value.ToString();
                    }
                }
                if (result == false && result2 == false && purchase_vendor_view.SelectedIndex < 0)
                {
                    dbConn.Open();
                    query = $"Select * FROM [{purName}] WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}')  ORDER BY Date DESC ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    dt = new DataTable(purName);

                    dbAdapter.Fill(dt);
                    ViewEntries.ItemsSource = dt.DefaultView;
                    dbAdapter.Update(dt);
                    dbConn.Close();

                    query = $"SELECT Count(distinct Reference) FROM {purName} WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') ; ";
                    dbConn.Open();
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Orders.Text = reader[0].ToString();
                    }
                    dbConn.Close();

                    query = "";
                    query = $"SELECT Sum(Total) FROM {purName} WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') ;";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbConn.Open();
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        double value = 0.0;
                        if (string.IsNullOrEmpty(freader[0].ToString()))
                        {
                            value = 0.0;
                        }
                        else
                        {
                            value = double.Parse(freader[0].ToString());
                        }
                        sumOrders.Text = value.ToString();
                    }
                    dbConn.Close();

                }

                else if (result == true && result2 == true && purchase_reference.SelectedIndex >= 0)
                {
                    if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                    ViewEntries.ItemsSource = "";
                    query = $"Select * FROM [{purName}] WHERE [{companyName}] = '{purchase_vendor_view.Text}' AND id >= '1' AND Reference = '{purchase_reference.Text}' ORDER BY Description ASC ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    DataTable dt = new DataTable(purName);
                    dbAdapter.Fill(dt);
                    ViewEntries.ItemsSource = dt.DefaultView;
                    dbAdapter.Update(dt);

                    query = $"SELECT Count(Reference) FROM {purName} WHERE [{companyName}] = '{purchase_vendor_view.Text}' AND id >= '1' AND Reference = '{purchase_reference.Text}'; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Orders.Text = reader[0].ToString();
                    }

                    query = $"SELECT Sum(Total) FROM {purName} WHERE [{companyName}] = '{purchase_vendor_view.Text}' AND id >= '1' AND Reference = '{purchase_reference.Text}' ; ";

                    dbCmd = new SQLiteCommand(query, dbConn);
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        double value = 0.0;
                        if (string.IsNullOrEmpty(freader[0].ToString()))
                        {
                            value = 0.0;
                        }
                        else
                        {
                            value = double.Parse(freader[0].ToString());
                        }
                        sumOrders.Text = value.ToString();
                    }
                    if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }

                }
                else
                {
                    if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                    ViewEntries.ItemsSource = "";
                    query = $"Select * FROM [{purName}] WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') AND [{companyName}] = '{purchase_vendor_view.Text}' AND id >= '1' AND Reference = '{purchase_reference.Text}' ORDER BY Description ASC; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    DataTable dt = new DataTable(purName);
                    dbAdapter.Fill(dt);
                    ViewEntries.ItemsSource = dt.DefaultView;
                    dbAdapter.Update(dt);

                    query = $"SELECT Count(distinct Reference) FROM {purName} WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') AND [{companyName}] = '{purchase_vendor_view.Text}' AND Reference = '{purchase_reference.Text}'; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Orders.Text = reader[0].ToString();
                    }

                    query = "";
                    query = $"SELECT Sum(Total) FROM {purName} WHERE [{orderDate}] >= date('{dSince}') AND {orderDate} <= date('{dUntil}') AND [{companyName}] = '{purchase_vendor_view.Text}' AND Reference = '{purchase_reference.Text}' ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        if (string.IsNullOrEmpty(freader[0].ToString())) { break; }
                        double value = double.Parse(freader[0].ToString());
                        sumOrders.Text = value.ToString();
                    }
                    if (dbConn.State == ConnectionState.Open) { dbConn.Close(); }
                }

            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }
            finally
            {
                ViewEntries.Columns[0].Header = hdr0;
                ViewEntries.Columns[1].Header = hdr1;
                ViewEntries.Columns[2].Header = hdr2;
                ViewEntries.Columns[3].Header = hdr3;
                ViewEntries.Columns[4].Header = hdr4;
                ViewEntries.Columns[5].Header = hdr5;
                ViewEntries.Columns[6].Header = hdr6;
                ViewEntries.Columns[7].Header = hdr7;
                ViewEntries.Columns[8].Header = hdr8;
                ViewEntries.Columns[9].Header = hdr9;
                ViewEntries.Columns[10].Header = hdr10;
                ViewEntries.Columns[11].Header = hdr11;
                if (dbConn.State == ConnectionState.Open)
                {

                    dbConn.Close();
                }
            }
        }

        private void EditBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private static bool IsDecAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void Change_Total(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (string.IsNullOrEmpty(textBox.Text)) { return; }
            //if (Convert.ToDouble(textBox.Text) < 0.01) { return; }
            TextBox[] quantityArray = { quantity1, quantity2, quantity3, quantity4, quantity5, quantity6, quantity7, quantity8, quantity9, quantity10, quantity11, quantity12, quantity13, quantity14, quantity15, quantity16, quantity17, quantity18, quantity19, quantity20 };
            TextBox[] priceArray = { price1, price2, price3, price4, price5, price6, price7, price8, price9, price10, price11, price12, price13, price14, price15, price16, price17, price18, price19, price20 };
            TextBox[] discountArray = { discount1, discount2, discount3, discount4, discount5, discount6, discount7, discount8, discount9, discount10, discount11, discount12, discount13, discount14, discount15, discount16, discount17, discount18, discount19, discount20 };
            TextBox[] shippingArray = { shipping1, shipping2, shipping3, shipping4, shipping5, shipping6, shipping7, shipping8, shipping9, shipping10, shipping11, shipping12, shipping13, shipping14, shipping15, shipping16, shipping17, shipping18, shipping19, shipping20 };
            TextBox[] totalArray = { total1, total2, total3, total4, total5, total6, total7, total8, total9, total10, total11, total12, total13, total14, total15, total16, total17, total18, total19, total20 };
            int i = textBox.Name.Length;
            int RemoveCount = 0;
            if (i == 9 || i == 6)
            {
                RemoveCount = i - 1;
            }
            else if (i == 10 || i == 7)
            {
                RemoveCount = i - 2;
            }
            else
            {
                AutoClosingMessageBox.Show(i.ToString(), "Error", autotimeout);
            }
            int y = Convert.ToInt32(textBox.Name.Remove(0, RemoveCount)) - 1;
            string name = textBox.Name;
            decimal QuantityValue = 0.0M;
            decimal PriceValue = 0.0M;
            decimal DiscountValue = 0.0M;
            decimal ShippingValue = 0.0M;
            if (quantityArray[y].Visibility == Visibility.Visible)
            {
                try
                {
                    QuantityValue = Convert.ToDecimal(quantityArray[y].Text.Replace(",", "."));
                }
                catch
                {
                    return;
                }
                try
                {
                    PriceValue = Convert.ToDecimal(priceArray[y].Text.Replace(",", "."));
                }
                catch
                {
                    return;
                }
                try
                {
                    DiscountValue = Convert.ToDecimal(discountArray[y].Text.Replace(",", "."));
                }
                catch
                {
                    return;
                }
                try
                {
                    ShippingValue = Convert.ToDecimal(shippingArray[y].Text.Replace(",", "."));
                }
                catch
                {
                    return;
                }

                totalArray[y].Text = Convert.ToString(Math.Round((QuantityValue * PriceValue) - DiscountValue + ShippingValue, 2, MidpointRounding.AwayFromZero));
            }
        }

        private void EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text)) { return; }
            if (!string.IsNullOrEmpty(EditPriceBox.Text)  && 
                !string.IsNullOrEmpty(EditQuantityBox.Text) &&
                !string.IsNullOrEmpty(EditDiscountBox.Text) &&
                !string.IsNullOrEmpty(EditShippingBox.Text))
            {
                double Quan, Pric, Disc, Ship, Tot;
                EditQuantityBox.Text = EditQuantityBox.Text.Replace(",", ".");
                EditPriceBox.Text = EditPriceBox.Text.Replace(",", ".");
                EditDiscountBox.Text = EditDiscountBox.Text.Replace(",", ".");
                EditShippingBox.Text = EditShippingBox.Text.Replace(",", ".");
                Quan = Convert.ToDouble(EditQuantityBox.Text);
                Pric = Convert.ToDouble(EditPriceBox.Text);
                Disc = Convert.ToDouble(EditDiscountBox.Text);
                Ship = Convert.ToDouble(EditShippingBox.Text);
                Tot = (((Quan * Pric) - Disc) + Ship);
                EditTotalBox.Text = Tot.ToString();
                EditTotalBox.Text = EditTotalBox.Text.Replace(",", ".");
            }
        }

        private void EditNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
            dbConn = new SQLiteConnection(connectionstring);
            dbConn.Open(); // Open the connection. Now you can fire SQL-Queries
            SQLiteConnection con = dbConn;
            SQLiteCommand sqlcmd = new SQLiteCommand();
            try
            {

                string query = $"SELECT ID FROM [Vendors] WHERE [Name] = '{comboBox.Text}' ; ";
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    if (string.IsNullOrEmpty(reader[0].ToString())) { break; }
                    int value = int.Parse(reader[0].ToString());
                    EditVIDBox.Text = value.ToString();
                }
                dbCmd.Dispose();
            }
            catch
            { 
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

        private void ButtonEdit_Update(object sender, RoutedEventArgs e)
        {
            string dbTable = "Purchases";

            //Do Something 
            if (!EditIdBox.Text.Equals(editId.ToString()) ||
                !EditReferenceBox.Text.Equals(editReference) ||
                !EditNameBox.Text.Equals(editName) ||
                !EditDateBox.Text.Equals(editDate) ||
                !EditDescriptionBox.Text.Equals(editDescription) ||
                !EditPIDBox.Text.Equals(editPID) ||
                !EditPriceBox.Text.Equals(editPrice.ToString()) ||
                !EditDiscountBox.Text.Equals(editDiscount.ToString()) ||
                !EditShippingBox.Text.Equals(editShipping.ToString()) ||
                !EditTotalBox.Text.Equals(editTotal.ToString()) ) 
            {
                EditElementInputBox.Visibility = Visibility.Collapsed;
                try
                {
                    using (dbConn = new SQLiteConnection(connectionstring))
                    {
                        dbConn.Open();
                        //Insert Command
                        dbCmd = new SQLiteCommand($"UPDATE {dbTable} SET" +
                            $" Date ='{EditDateBox.Text.ToString()}'," +
                            $" Reference ='{EditReferenceBox.Text.ToString()}',"+
                            $" Name = '{EditNameBox.Text.ToString().Replace("'","''")}'," +
                            $" VID = '{EditVIDBox.Text.ToString()}'," +
                            $" Description = '{EditDescriptionBox.Text.ToString().Replace("'", "''")}'," +
                            $" PID = '{EditPIDBox.Text.ToString()}'," +
                            $" Quantity = '{EditQuantityBox.Text.ToString()}'," +
                            $" Price = '{EditPriceBox.Text.ToString()}'," +
                            $" Discount = '{EditDiscountBox.Text.ToString()}'," +
                            $" Shipping = '{EditShippingBox.Text.ToString()}'," +
                            $" Total = '{EditTotalBox.Text.ToString()}'" +
                            $" WHERE id = " + int.Parse(EditIdBox.Text.ToString()) + " ; ", dbConn);
                        dbCmd.ExecuteNonQuery();
                        dbCmd = null;
                        //Select Command
                        string VendorId = "";
                        if (purchase_vendor_view.SelectedIndex > 0) {
                            VendorId = $"AND VID ='{EditVIDBox.Text}'";
                        }
                        query = $"SELECT * FROM {dbTable} WHERE id >= '1' {VendorId} order by id DESC ; ";
                        dbCmd = new SQLiteCommand(query, dbConn);
                        dbCmd.ExecuteNonQuery();
                        dbAdapter = new SQLiteDataAdapter(dbCmd);
                        mTable = new DataTable(dbTable);

                        dbAdapter.Fill(mTable);
                        ViewEntries.ItemsSource = mTable.DefaultView;
                        dbAdapter.Update(mTable);

                        string fuery = $"UPDATE {dbTable} SET Quantity = replace(Quantity, ',', '.') WHERE Quantity like '%,%' ; ";
                        fuery += $"UPDATE {dbTable} SET Price = replace(Price, ',', '.') WHERE Price LIKE '%,%' ; ";
                        fuery += $"UPDATE {dbTable} SET Discount = replace(Discount, ',', '.') WHERE Discount LIKE '%,%' ; ";
                        fuery += $"UPDATE {dbTable} SET Shipping = replace(Shipping, ',', '.') WHERE Shipping LIKE '%,%' ; ";
                        fuery += $"UPDATE {dbTable} SET Total = replace(Total, ',', '.') WHERE Total LIKE '%,%' ; ";
                        dbCmd = new SQLiteCommand(fuery, dbConn);
                        dbCmd.ExecuteNonQuery();
                        dbCmd = null;

                        dbConn.Close();
                        ViewEntries.Columns[0].Header = hdr0;
                        ViewEntries.Columns[1].Header = hdr1;
                        ViewEntries.Columns[2].Header = hdr2;
                        ViewEntries.Columns[3].Header = hdr3;
                        ViewEntries.Columns[4].Header = hdr4;
                        ViewEntries.Columns[5].Header = hdr5;
                        ViewEntries.Columns[6].Header = hdr6;
                        ViewEntries.Columns[7].Header = hdr7;
                        ViewEntries.Columns[8].Header = hdr8;
                        ViewEntries.Columns[9].Header = hdr9;
                        ViewEntries.Columns[10].Header = hdr10;
                        ViewEntries.Columns[11].Header = hdr11;
                        RetrieveOrders();
                        //dbConn.Close();
                    }
                }
                catch (Exception ex)
                {
                    AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
                }

                EditIdBox.Text = string.Empty;
                EditDateBox.Text = string.Empty;
                EditReferenceBox.Text = string.Empty;
                EditNameBox.Text = string.Empty;
                EditVIDBox.Text = string.Empty;
                EditDescriptionBox.Text = string.Empty;
                EditPIDBox.Text = string.Empty;
                EditQuantityBox.Text = string.Empty;
                EditPriceBox.Text = string.Empty;
                EditDiscountBox.Text = string.Empty;
                EditShippingBox.Text = string.Empty;
                EditTotalBox.Text = string.Empty;
                editId = 0;
                editDate = string.Empty;
                editReference = string.Empty;
                editName = string.Empty;
                editVID = string.Empty;
                editDescription = string.Empty;
                editPID = string.Empty;
                editQuantity = string.Empty;
                editPrice = string.Empty;
                editDiscount = string.Empty;
                editShipping = string.Empty;
                editTotal = string.Empty;

            }
            else
            {
                AutoClosingMessageBox.Show("No Entry Changed.","No Change",autotimeout);
            }


        }

        private void Quantity_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "0")
            {
                textBox.Text = "";
            }
        }

        private void Quantity_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private void DoubleDigit_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "0.0")
            {
                textBox.Text = "";
            }
        }

        private void DoubleDigit_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0.0";
            }
        }

        private void Dash_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "-")
            {
                textBox.Text = "";
            }
        }

        private void Dash_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "-";
            }
        }
        private void Change_Total(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (string.IsNullOrEmpty(textBox.Text)) { return; }
            //if (Convert.ToDouble(textBox.Text) < 0.01) { return; }
            TextBox[] quantityArray = { quantity1, quantity2, quantity3, quantity4, quantity5, quantity6, quantity7, quantity8, quantity9, quantity10, quantity11, quantity12, quantity13, quantity14, quantity15, quantity16, quantity17, quantity18, quantity19, quantity20 };
            TextBox[] priceArray = { price1, price2, price3, price4, price5, price6, price7, price8, price9, price10, price11, price12, price13, price14, price15, price16, price17, price18, price19, price20 };
            TextBox[] discountArray = { discount1, discount2, discount3, discount4, discount5, discount6, discount7, discount8, discount9, discount10, discount11, discount12, discount13, discount14, discount15, discount16, discount17, discount18, discount19, discount20 };
            TextBox[] shippingArray = { shipping1, shipping2, shipping3, shipping4, shipping5, shipping6, shipping7, shipping8, shipping9, shipping10, shipping11, shipping12, shipping13, shipping14, shipping15, shipping16, shipping17, shipping18, shipping19, shipping20 };
            TextBox[] totalArray = { total1, total2, total3, total4, total5, total6, total7, total8, total9, total10, total11, total12, total13, total14, total15, total16, total17, total18, total19, total20 };
            int i = textBox.Name.Length;
            int RemoveCount = 0;
            if (i == 9 || i == 6)
            {
                RemoveCount = i - 1;
            }
            else if (i == 10 || i == 7)
            {
                RemoveCount = i - 2;
            }
            else
            {
                AutoClosingMessageBox.Show(i.ToString(),"Error",autotimeout);
            }
            int y = Convert.ToInt32(textBox.Name.Remove(0, RemoveCount))-1;
            string name = textBox.Name;
            decimal QuantityValue = 0.0M;
            decimal PriceValue = 0.0M;
            decimal DiscountValue = 0.0M;
            decimal ShippingValue = 0.0M;
            if (quantityArray[y].Visibility == Visibility.Visible)
            {
                try
                {
                QuantityValue = Convert.ToDecimal(quantityArray[y].Text.Replace(",","."));
                }
                catch
                {
                    return;
                }
                try
                {
                PriceValue = Convert.ToDecimal(priceArray[y].Text.Replace(",", "."));
                }
                catch
                {
                    return;
                }
                try
                { 
                DiscountValue = Convert.ToDecimal(discountArray[y].Text.Replace(",", "."));
                }
                catch
                {
                    return;
                }
                try { 
                ShippingValue = Convert.ToDecimal(shippingArray[y].Text.Replace(",", "."));
                }
                catch
                {
                    return;
                }

                totalArray[y].Text = Convert.ToString(Math.Round((QuantityValue * PriceValue) - DiscountValue + ShippingValue,2,MidpointRounding.AwayFromZero));
            }
        }

        private void Change_Total_Cost(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text)) { return; }
            if (Convert.ToDouble(textBox.Text) < 0.01) { return; }
            double sum = 0.0;
            TextBox[] totalArray = { total1, total2, total3, total4, total5, total6, total7, total8, total9, total10, total11, total12, total13, total14, total15, total16, total17, total18, total19, total20 };
            List<string> myTotalList = new List<string>();

            for (int i = 0; i < totalArray.Length; i++)
            {
                if (totalArray[i].Visibility == Visibility.Visible)
                {
                    myTotalList.Add(totalArray[i].Text);

                }
            }
            var myTotalArray = myTotalList.ToArray();
            foreach (var item in myTotalArray)
            {
                sum = Math.Round(sum + Convert.ToDouble(item),2,MidpointRounding.AwayFromZero);
                order_sum.Text = sum.ToString();
            }

        }

        private void purchase_vendor_view_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(purchase_vendor_view.Text))
            {
                purchase_reference.IsEnabled = false;
                return;
            }
            BlindComboBox(purchase_reference);
        }


        private void purchase_Reset_Click(object sender, RoutedEventArgs e)
        {
            TextBox[] productArray = { product1, product2, product3, product4, product5, product6, product7, product8, product9, product10, product11, product12, product13, product14, product15, product16, product17, product18, product19, product20 };
            TextBox[] skuArray = { sku1, sku2, sku3, sku4, sku5, sku6, sku7, sku8, sku9, sku10, sku11, sku12, sku13, sku14, sku15, sku16, sku17, sku18, sku19, sku20 };
            TextBox[] quantityArray = { quantity1, quantity2, quantity3, quantity4, quantity5, quantity6, quantity7, quantity8, quantity9, quantity10, quantity11, quantity12, quantity13, quantity14, quantity15, quantity16, quantity17, quantity18, quantity19, quantity20 };
            TextBox[] priceArray = { price1, price2, price3, price4, price5, price6, price7, price8, price9, price10, price11, price12, price13, price14, price15, price16, price17, price18, price19, price20 };
            TextBox[] discountArray = { discount1, discount2, discount3, discount4, discount5, discount6, discount7, discount8, discount9, discount10, discount11, discount12, discount13, discount14, discount15, discount16, discount17, discount18, discount19, discount20 };
            TextBox[] shippingArray = { shipping1, shipping2, shipping3, shipping4, shipping5, shipping6, shipping7, shipping8, shipping9, shipping10, shipping11, shipping12, shipping13, shipping14, shipping15, shipping16, shipping17, shipping18, shipping19, shipping20 };
            TextBox[] totalArray = { total1, total2, total3, total4, total5, total6, total7, total8, total9, total10, total11, total12, total13, total14, total15, total16, total17, total18, total19, total20 };

            string s = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));

            string dSinceD;
            string mSinceM;

            int dSinceDays = Convert.ToInt32(purchase_date.cmbDays.Text);
            int mSinceMonths = purchase_date.cmbMonths.SelectedIndex + 1;
            if (dSinceDays < 10)
            {
                dSinceD = "0" + dSinceDays.ToString();
            }
            else
            {
                dSinceD = dSinceDays.ToString();
            }
            if (mSinceMonths < 10)
            {
                mSinceM = "0" + mSinceMonths.ToString();
            }
            else
            {
                mSinceM = mSinceMonths.ToString();
            }

            string dSince = purchase_date.cmbYear.Text + "-" + mSinceM + "-" + dSinceD;

            bool result = s.Equals(dSince, StringComparison.Ordinal);

            if (result == false)
            {
                purchase_date.cmbYear.Text = DateTime.Now.ToString("yyyy");
                purchase_date.cmbMonths.SelectedIndex = Convert.ToInt32(DateTime.Now.ToString("MM")) - 1;
                purchase_date.cmbDays.SelectedIndex = Convert.ToInt32(DateTime.Now.ToString("dd")) - 1;
            }
            purchase_vendor_add.SelectedIndex = -1;
            purchase_ord_num.Text = "";
            for (int y = 0; y < productArray.Length; y++)
            {
                productArray[y].Text = "";
                skuArray[y].Text = "-";
                quantityArray[y].Text = "0";
                priceArray[y].Text = "0.0";
                discountArray[y].Text = "0.0";
                shippingArray[y].Text = "0.0";
                totalArray[y].Text = "0.0";
            }
            //ScrollViewer.SetVerticalScrollBarVisibility(ProductScroller, ScrollBarVisibility.Hidden);
            order_sum.Text = "0";
            additems_Count.Text = "1";
            purchase_additem.IsEnabled = true;  
            HideTheRest();

        }

        private void purchase_additem_Click(object sender, RoutedEventArgs e)
        {
            TextBlock[] sn = { sn1, sn2, sn3, sn4, sn5, sn6, sn7, sn8, sn9, sn10, sn11, sn12, sn13, sn14, sn15, sn16, sn17, sn18, sn19, sn20 };
            TextBox[] productArray = { product1, product2, product3, product4, product5, product6, product7, product8, product9, product10, product11, product12, product13, product14, product15, product16, product17, product18, product19, product20 };
            TextBox[] skuArray = { sku1, sku2, sku3, sku4, sku5, sku6, sku7, sku8, sku9, sku10, sku11, sku12, sku13, sku14, sku15, sku16, sku17, sku18, sku19, sku20 };
            TextBox[] quantityArray = { quantity1, quantity2, quantity3, quantity4, quantity5, quantity6, quantity7, quantity8, quantity9, quantity10, quantity11, quantity12, quantity13, quantity14, quantity15, quantity16, quantity17, quantity18, quantity19, quantity20 };
            TextBox[] priceArray = { price1, price2, price3, price4, price5, price6, price7, price8, price9, price10, price11, price12, price13, price14, price15, price16, price17, price18, price19, price20 };
            TextBox[] discountArray = { discount1, discount2, discount3, discount4, discount5, discount6, discount7, discount8, discount9, discount10, discount11, discount12, discount13, discount14, discount15, discount16, discount17, discount18, discount19, discount20 };
            TextBox[] shippingArray = { shipping1, shipping2, shipping3, shipping4, shipping5, shipping6, shipping7, shipping8, shipping9, shipping10, shipping11, shipping12, shipping13, shipping14, shipping15, shipping16, shipping17, shipping18, shipping19, shipping20 };
            TextBox[] totalArray = { total1, total2, total3, total4, total5, total6, total7, total8, total9, total10, total11, total12, total13, total14, total15, total16, total17, total18, total19, total20 };

            int y = 0;
            int i = Convert.ToInt32(additems_Count.Text);
            for (y = i; y < i + 1; y++)
            {
                sn[i].Visibility = Visibility.Visible;
                productArray[y].Visibility = Visibility.Visible;
                skuArray[y].Visibility = Visibility.Visible;
                skuArray[y].Text = "-";
                quantityArray[y].Visibility = Visibility.Visible;
                quantityArray[y].Text = "0";
                priceArray[y].Visibility = Visibility.Visible;
                priceArray[y].Text = "0.0";
                discountArray[y].Visibility = Visibility.Visible;
                discountArray[y].Text = "0.0";
                shippingArray[y].Visibility = Visibility.Visible;
                shippingArray[y].Text = "0.0";
                totalArray[y].Visibility = Visibility.Visible;
                totalArray[y].Text = "0.0";
            }
            if (i <= 19)
            {
                i++;
                //if (i <= 6)
                //{
                //    //Spacer.Width = new GridLength(37, GridUnitType.Pixel);
                //    ScrollViewer.SetVerticalScrollBarVisibility(ProductScroller, ScrollBarVisibility.Hidden);
                //}
                //else
                //{
                //    Spacer.Width = new GridLength(37, GridUnitType.Pixel);
                //    ScrollViewer.SetVerticalScrollBarVisibility(ProductScroller, ScrollBarVisibility.Visible);
                //}
            }
            else
            {
                purchase_additem.IsEnabled = false;
                return;
            }

            additems_Count.Text = y.ToString();
            if (y == 20)
            {
                purchase_additem.IsEnabled = false;
            }
            else
            {
                purchase_additem.IsEnabled = true;
            }
        }

        private void Add_V_Click(object sender, RoutedEventArgs e)
        {
            ShowChildWindow();
        }

        private void FillDataGrid()
        {
            string dbTable = "Purchases";

            try
            {
                using (dbConn = new SQLiteConnection(connectionstring))
                {
                    dbConn.Open();
                    //Select Command
                    dbCmd = new SQLiteCommand($"SELECT * FROM {dbTable} WHERE Id > '0' order by Date DESC ", dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    mTable = new DataTable(dbTable);

                    dbAdapter.Fill(mTable);
                    dbConn.Close();
                    ViewEntries.ItemsSource = mTable.DefaultView;
                    dbAdapter.Update(mTable);

                    ViewEntries.Columns[0].Header = hdr0;
                    ViewEntries.Columns[1].Header = hdr1;
                    ViewEntries.Columns[2].Header = hdr2;
                    ViewEntries.Columns[3].Header = hdr3;
                    ViewEntries.Columns[4].Header = hdr4;
                    ViewEntries.Columns[5].Header = hdr5;
                    ViewEntries.Columns[6].Header = hdr6;
                    ViewEntries.Columns[7].Header = hdr7;
                    ViewEntries.Columns[8].Header = hdr8;
                    ViewEntries.Columns[9].Header = hdr9;
                    ViewEntries.Columns[10].Header = hdr10;
                    ViewEntries.Columns[11].Header = hdr11;

                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(),"Error",autotimeout);
            }
        }
  
        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var Pitem in itemsSource)
            {
                if (grid.ItemContainerGenerator.ContainerFromItem(Pitem) is DataGridRow row)
                {
                    yield return row;
                }
            }
        }

        private void ViewEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewEntries.SelectedIndex < 0) { return; }
            try
            {
                if (ViewEntries.SelectedItem != null)
                {
                    Pitem = this.ViewEntries.SelectedItem;
                    string a = (ViewEntries.SelectedCells[0].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editId = Int32.Parse(a);
                    editDate = (ViewEntries.SelectedCells[1].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editReference = (ViewEntries.SelectedCells[2].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editName = (ViewEntries.SelectedCells[3].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editVID = (ViewEntries.SelectedCells[4].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editDescription = (ViewEntries.SelectedCells[5].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editPID = (ViewEntries.SelectedCells[6].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editQuantity = (ViewEntries.SelectedCells[7].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editPrice = (ViewEntries.SelectedCells[8].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editDiscount = (ViewEntries.SelectedCells[9].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editShipping = (ViewEntries.SelectedCells[10].Column.GetCellContent(Pitem) as TextBlock).Text;
                    editTotal = (ViewEntries.SelectedCells[11].Column.GetCellContent(Pitem) as TextBlock).Text;
                    EditIdBox.Text = editId.ToString();
                    EditDateBox.Text = editDate;
                    EditReferenceBox.Text = editReference;
                    EditNameBox.Text = editName;
                    EditVIDBox.Text = editVID;
                    EditDescriptionBox.Text = editDescription;
                    EditPIDBox.Text = editPID;
                    EditQuantityBox.Text = editQuantity;
                    EditPriceBox.Text = editPrice;
                    EditDiscountBox.Text = editDiscount;
                    EditShippingBox.Text = editShipping;
                    EditTotalBox.Text = editTotal;
                }
            }
            catch (Exception exp)
            {
                AutoClosingMessageBox.Show(exp.ToString(), "Error", autotimeout);
            }
        }
        #region CategoryTable Functions
        private void EntryEdit_Click(object sender, RoutedEventArgs e)
        {
            EditElementInputBox.Visibility = Visibility.Visible;
        }

        private void EntryDelete_Click(object sender, RoutedEventArgs e)
        {
            string dbTable = "Purchases";

            //Save Changes
            try
            {
                if (this.ViewEntries.SelectedItem != null)
                {
                    using (dbConn = new SQLiteConnection(connectionstring))
                    {
                        string message = "",title = "";
                        if (CurrentCulture == "en-US")
                        {
                            message = "Are you sure that you want to delete this item?";
                            title = "Question";

                        }
                        if (CurrentCulture == "el-GR")
                        {
                            message = "Είστε σίγουροι για τη διαγραφή του αντικειμένου;";
                            title = "Ερώτηση";
                        }
                        if (MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            dbConn.Open();
                            //Delete Command
                            query = $"Update Vendors SET Orders = Orders - 1 Where Id = {editVID} ; ";
                            query += $"Delete FROM [{dbTable}] WHERE Id = '{editId.ToString()}' ; ";
                            dbCmd = new SQLiteCommand(query, dbConn);
                            dbCmd.ExecuteNonQuery();
                            dbCmd = null;


                            //Select Command
                            FillDataGrid();
                            //query = $"SELECT * FROM [{dbTable}] WHERE Id > 0 order by id DESC ; ";
                            //dbCmd = new SQLiteCommand(query, dbConn);
                            //dbCmd.ExecuteNonQuery();
                            //dbAdapter = new SQLiteDataAdapter(dbCmd);
                            //mTable = new DataTable(dbTable);

                            //dbAdapter.Fill(mTable);
                            //dbConn.Close();
                            //ViewEntries.ItemsSource = mTable.DefaultView;
                            //dbAdapter.Update(mTable);

                            //ViewEntries.Columns[0].Header = hdr0;
                            //ViewEntries.Columns[1].Header = hdr1;
                            //ViewEntries.Columns[2].Header = hdr2;
                            //ViewEntries.Columns[3].Header = hdr3;
                            //ViewEntries.Columns[4].Header = hdr4;
                            //ViewEntries.Columns[5].Header = hdr5;
                            //ViewEntries.Columns[6].Header = hdr6;
                            //ViewEntries.Columns[7].Header = hdr7;
                            //ViewEntries.Columns[8].Header = hdr8;
                            //ViewEntries.Columns[9].Header = hdr9;
                            //ViewEntries.Columns[10].Header = hdr10;
                            //ViewEntries.Columns[11].Header = hdr11;
                            Ordercount();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }
        }

        private void Ordercount()
        {
            string query;

            try
            {
                using (dbConn = new SQLiteConnection(connectionstring)) { 
                query = "Select Count(Distinct Reference) FROM Purchases Where Id > 0 ;";
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    Orders.Text = reader[0].ToString();
                }
                reader.Close();
                query = "";

                query = "Select Sum(Total) FROM Purchases";
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    sumOrders.Text = reader[0].ToString();
                }
                reader.Close();

                dbConn.Close();
                dbCmd = null;
                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }

        }
        private void Button_Delete_Element_Click(object sender, RoutedEventArgs e)
        {

            //Save Changes
            try
            {
                using (dbConn = new SQLiteConnection(connectionstring))
                {
                    dbConn.Open();
                    //Insert Command
                    dbCmd = new SQLiteCommand($"DELETE FROM Purchases WHERE id = '{editId.ToString()}' ; ", dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbCmd.Dispose();
                    //Select Command
                    dbCmd = new SQLiteCommand($"SELECT * FROM Purchases ORDER BY id DESC", dbConn);
                    dbCmd.ExecuteNonQuery();
                    dbAdapter = new SQLiteDataAdapter(dbCmd);
                    dbCmd.Dispose();
                    mTable = new DataTable("Purchases");

                    dbAdapter.Fill(mTable);
                    dbConn.Close();
                    ViewEntries.ItemsSource = mTable.DefaultView;
                    dbAdapter.Update(mTable);

                    ViewEntries.Columns[0].Header = hdr0;
                    ViewEntries.Columns[1].Header = hdr1;
                    ViewEntries.Columns[2].Header = hdr2;
                    ViewEntries.Columns[3].Header = hdr3;
                    ViewEntries.Columns[4].Header = hdr4;
                    ViewEntries.Columns[5].Header = hdr5;
                    ViewEntries.Columns[6].Header = hdr6;
                    ViewEntries.Columns[7].Header = hdr7;
                    ViewEntries.Columns[8].Header = hdr8;
                    ViewEntries.Columns[9].Header = hdr9;
                    ViewEntries.Columns[10].Header = hdr10;
                    ViewEntries.Columns[11].Header = hdr11;

                }
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
            }
        }

        private void Button_Edit_Element_Click(object sender, RoutedEventArgs e)
        {
            if (ViewEntries.SelectedItem == null) { return; }
            BindComboBox(EditNameBox);
            EditIdBox.Text = editId.ToString();
            EditDateBox.Text = editDate;
            EditReferenceBox.Text = editReference;
            EditNameBox.Text = editName;
            EditVIDBox.Text = editVID;
            EditDescriptionBox.Text = editDescription;
            EditPIDBox.Text = editPID;
            EditQuantityBox.Text = editQuantity;
            EditPriceBox.Text = editPrice;
            EditDiscountBox.Text = editDiscount;
            EditShippingBox.Text = editShipping;
            EditTotalBox.Text = editTotal;
            EditElementInputBox.Visibility = Visibility.Visible;

        }

        private void ButtonEdit_Cancel(object sender, RoutedEventArgs e)
        {
            EditElementInputBox.Visibility = Visibility.Collapsed;
            EditNameBox.ItemsSource = "";
            EditIdBox.Text = string.Empty;
            EditDateBox.Text = string.Empty;
            EditReferenceBox.Text = string.Empty;
            EditNameBox.Text = string.Empty;
            EditVIDBox.Text = string.Empty;
            EditDescriptionBox.Text = string.Empty;
            EditPIDBox.Text = string.Empty;
            EditQuantityBox.Text = string.Empty;
            EditPriceBox.Text = string.Empty;
            EditDiscountBox.Text = string.Empty;
            EditShippingBox.Text = string.Empty;
            EditTotalBox.Text = string.Empty;
        }
        #endregion

    }
}