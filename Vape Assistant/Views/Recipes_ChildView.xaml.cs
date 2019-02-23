using System;
using System.Collections.Generic;
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
    /// Interaction logic for Recipes_ChildView.xaml
    /// </summary>
    public partial class Recipes_ChildView : UserControl
    {
        string connectionString = Settings.Default.VaConnect;
        string CurrentCulture = Settings.Default.Culture;
        SQLiteDataAdapter dbAdapter;
        SQLiteDataReader reader;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        public string query;
        public string message, errmsg;
        public string caption, title;
        public int autotimeout = 5000;

        public Recipes_ChildView()
        {
            InitializeComponent();
            RecipesComboBox(RecipeName);
            ComboBox[] brand = { brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            for (int i = 0; i < brand.Length; i++)
            {
                RecipeBrandComboBox(brand[i]);
            }
            dbConn = new SQLiteConnection(connectionString);
            HideTheRest();
        }
        public void RecipeBrandComboBox(ComboBox comboBoxName)
        {
            dbConn = new SQLiteConnection(connectionString);
            dbConn.Open();
            SQLiteCommand sqlcmd = new SQLiteCommand();
            dbAdapter = new SQLiteDataAdapter();
            DataSet ds = new DataSet();
            sqlcmd.Connection = dbConn;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = "Select * from FlavorBrands Where Id > '0' ORDER BY ShortName ASC ";
            dbAdapter.SelectCommand = sqlcmd;
            dbAdapter.Fill(ds, "FlavorBrands");
            comboBoxName.ItemsSource = ds.Tables[0].DefaultView;
            comboBoxName.DisplayMemberPath = ds.Tables[0].Columns["ShortName"].ToString(); // + " (" + ds.Tables[0].Columns["ShortName"].ToString() + ")"
            comboBoxName.SelectedValuePath = ds.Tables[0].Columns["Id"].ToString();
            if (dbConn.State == ConnectionState.Open)
            {

                dbConn.Close();
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
        private void RecipeName_DropDownClosed(object sender, EventArgs e)
        {
            StackPanel[] flv = { flv0, flv1, flv2, flv3, flv4, flv5, flv6, flv7, flv8, flv9, flv10, flv11, flv12, flv13, flv14, flv15, flv16, flv17, flv18, flv19, flv20 };
            ComboBox[] brand = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flavor = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            TextBox[] percentage = { percentage0, percentage1, percentage2, percentage3, percentage4, percentage5, percentage6, percentage7, percentage8, percentage9, percentage10, percentage11, percentage12, percentage13, percentage14, percentage15, percentage16, percentage17, percentage18, percentage19, percentage20 };
            TextBox[] flavor_id = { flavor_id0, flavor_id1, flavor_id2, flavor_id3, flavor_id4, flavor_id5, flavor_id6, flavor_id7, flavor_id8, flavor_id9, flavor_id10, flavor_id11, flavor_id12, flavor_id13, flavor_id14, flavor_id15, flavor_id16, flavor_id17, flavor_id18, flavor_id19, flavor_id20 };
            if (RecipeName.SelectedIndex < 0 ) { return; }

            //Reset the controls
            for (int y = 0; y < brand.Length; y++)
            {
                brand[y].SelectedIndex = -1;
                flavor[y].SelectedIndex = -1;
                percentage[y].Text = "";
                flavor_id[y].Text = "";
            }

            dbConn.Open();
            try
            {
                string query = $"SELECT * FROM [RecipeBook] WHERE Recipename='" + RecipeName.Text.Replace("'", "''") + "' ";
                if (dbConn.State == ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    int Id_Brand = reader.GetInt32(0);
                    int TimeMade = reader.GetInt32(2);
                    string authname = reader.GetString(3);
                    recipeid.Text = Id_Brand.ToString();
                    TimesMade.Text = TimeMade.ToString();
                    Author.Text = authname;
                }
                reader.Close();
                dbCmd.Dispose();
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
                //MessageBox.Show("Error" + ex.ToString());
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
            finally
            {
                temp.Text = "";
                string query = $"select Count(*) from [hash] where RECIPE_ID = '{ recipeid.Text }';";
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                while (reader.Read())
                {
                    flavors_shown.Text = reader.GetInt32(0).ToString();
                }
                reader.Close();
                query = $"Select * from [hash] where RECIPE_ID = '{ recipeid.Text }';";
                dbCmd = new SQLiteCommand(query, dbConn);
                reader = dbCmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    i = i + 1;
                    string fid = reader.GetInt32(2).ToString();
                    string per = reader.GetDouble(3).ToString();
                    flv[i].Visibility = Visibility.Visible;
                    flv[i].Height = 24;
                    flavor_id[i].Text = fid;
                    percentage[i].Text = per;
                    string fuery = $"Select BrandShort, Flavor from [Flavors] Where Id = '{ flavor_id[i].Text }'";
                    dbCmd = new SQLiteCommand(fuery, dbConn);
                    SQLiteDataReader freader = dbCmd.ExecuteReader();
                    while (freader.Read())
                    {
                        brand[i].Text = freader[0].ToString();
                    }
                    freader.Close();
                    dbCmd.Dispose();
                }
                reader.Close();
                dbCmd.Dispose();
                int y = Convert.ToInt32(flavors_shown.Text);
                if (y < 11)
                {
                    FlavorScroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                } else {
                    FlavorScroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                for (i = 1; i <= y; i++)
                {
                    TakeBrandGiveFlavor(brand[i]);
                    if (dbConn.State == ConnectionState.Closed)
                    {

                        dbConn.Open();
                    }
                    query = $"Select Flavor,Amount from [Flavors] where Id = '{ flavor_id[i].Text }'";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        flavor[i].Text = reader[0].ToString();
                    }
                }
                reader.Close();
                dbCmd.Dispose();
                HideTheRest();
                if (dbConn.State == ConnectionState.Open)
                {

                    dbConn.Close();
                }
            }
            
        }


        private void RecipeName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(RecipeName.Text)) { return; }
            if (RecipeName.Text.Length >= 0)
            {
                SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                try
                {
                    string query = $"SELECT RecipeName FROM [RecipeBook]";
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string Name = reader.GetString(0);
                        if (RecipeName.Text == Name)
                        {
                            caption = "";
                            message = "";
                            if (CurrentCulture == "en-US")
                            {
                                message = "The name of your recipe " + RecipeName.Text + "\n\nAlready Exists. Please choose a different one!";
                                caption = "Error";
                            }
                            if (CurrentCulture == "el-GR")
                            {
                                message = "Το όνομα της συνταγής " + RecipeName.Text + "\n\nΥπάρχει ήδη.";
                                caption = "Σφάλμα";
                            }
                            AutoClosingMessageBox.Show(message, caption, autotimeout);
                            return;
                        }
                    }
                    reader.Close();
                    dbCmd.Dispose();
                    if (RecipeName.Text.Length > 0)
                    {
                        string fuery = $"SELECT seq FROM [sqlite_sequence] WHERE name='RecipeBook'";
                        if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                        dbCmd = new SQLiteCommand(fuery, dbConn);
                        if (dbConn.State == ConnectionState.Closed) { dbConn.Open(); }
                        SQLiteDataReader freader = dbCmd.ExecuteReader();
                        while (freader.Read())
                        {
                            int currentId = int.Parse(freader[0].ToString());
                            int nextId = currentId + 1;
                            recipeid.Text = nextId.ToString();
                        }
                        freader.Close();
                        dbCmd.Dispose();
                    }
                }
                catch
                {
                    AutoClosingMessageBox.Show(e.ToString(), "Error", autotimeout);
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

        private void PreviewTextBoxInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDecAllowed(e.Text);
        }

        private void delRecipe_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dbConn.Open();

            if (string.IsNullOrEmpty(RecipeName.Text)) { return; }
            if (string.IsNullOrEmpty(recipeid.Text)) { return; }

            if (CurrentCulture == "en-US")
            {
                message = "Are you sure?";
                caption = "Question";
            }
            if (CurrentCulture == "el-GR")
            {
                message = "Είστε σίγουρος/η;";
                caption = "Ερώτηση";
            }
            if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                query = $"Delete From [RecipeBook] Where Id = '{ recipeid.Text }' ;";
                query += $"Delete From [hash] Where RECIPE_ID ='{ recipeid.Text }' ;";
                dbCmd = new SQLiteCommand(query, dbConn);
                dbCmd.ExecuteNonQuery();
            }

            RecipeName.SelectedIndex = -1;
            RecipeName.Text = "";
            recipeid.Text = "";
            Author.Text = "";
            flavors_shown.Text = "1";
            flv1.Visibility = Visibility.Visible;
            brand1.SelectedIndex = -1;
            flavor1.SelectedIndex = -1;
            percentage1.Text = "";
            if (dbConn.State == ConnectionState.Open)
            {

                dbConn.Close();
            }
            RecipesComboBox(RecipeName);
            HideTheRest();
        }


        private void add_recipe_new_Click(object sender, RoutedEventArgs e)
        {
            StackPanel[] flv = { flv0, flv1, flv2, flv3, flv4, flv5, flv6, flv7, flv8, flv9, flv10, flv11, flv12, flv13, flv14, flv15, flv16, flv17, flv18, flv19, flv20 };
            ComboBox[] brand = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flavor = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            TextBox[] percentage = { percentage0, percentage1, percentage2, percentage3, percentage4, percentage5, percentage6, percentage7, percentage8, percentage9, percentage10, percentage11, percentage12, percentage13, percentage14, percentage15, percentage16, percentage17, percentage18, percentage19, percentage20 };
            TextBox[] flavor_id = { flavor_id0, flavor_id1, flavor_id2, flavor_id3, flavor_id4, flavor_id5, flavor_id6, flavor_id7, flavor_id8, flavor_id9, flavor_id10, flavor_id11, flavor_id12, flavor_id13, flavor_id14, flavor_id15, flavor_id16, flavor_id17, flavor_id18, flavor_id19, flavor_id20 };

                for (int y = 0; y <= 20; y++)
                {
                    brand[y].SelectedIndex = -1;
                    flavor[y].SelectedIndex = -1;
                    percentage[y].Text = "";
                    flavor_id[y].Text = "";
                }
            Adder_flv.IsEnabled = true;
            RecipeName.Text = "";
            RecipeName.SelectedIndex = -1;
            recipeid.Text = "";
            TimesMade.Text = "";
            flavors_shown.Text = "1";
            Author.Text = "";
            HideTheRest();
        }

        public void TakeBrandGiveFlavor(ComboBox senderBox)
        {
            ComboBox[] flavor = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            if (senderBox.SelectedIndex >= 0)
            {
                try
                {
                    var t = senderBox.Text.Replace("'", "''");
                    string query = $"SELECT * FROM [Flavors] WHERE BrandShort='" + t + "'";
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int Id_Brand = int.Parse(reader[0].ToString());
                        vendoras.Text = reader[1].ToString();
                        break;
                    }
                    reader.Close();
                    dbCmd.Dispose();
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
                    int i = 0;
                    if (senderBox.Name.Length == 6)
                    {
                        i = Convert.ToInt32(senderBox.Name.Substring(senderBox.Name.Length - 1));
                    }
                    else if (senderBox.Name.Length == 7)
                    {
                        i = Convert.ToInt32(senderBox.Name.Substring(senderBox.Name.Length - 2));
                    }


                    if (dbConn.State == ConnectionState.Open)
                    {

                        dbConn.Close();
                    }

                    FlavorComboBox(flavor[i]);
                }
            }
            else
            {
                // senderBox.ItemsSource = "";
            }
        }
        public void FlavorComboBox(ComboBox comboBoxName)
        {
            if (dbConn.State == ConnectionState.Closed)
            {
                dbConn.Open();
            }
            query = "Select * from Flavors WHERE Brand ='" + vendoras.Text.Replace("'", "''") + "' ORDER BY Flavor ASC ";
            dbCmd = new SQLiteCommand(query, dbConn);
            dbAdapter = new SQLiteDataAdapter();
            DataSet ds = new DataSet();
            dbAdapter.SelectCommand = dbCmd;
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
        private void HideTheRest()
        {
            //hides and clears all the rows below recipe
            StackPanel[] flv = { flv0, flv1, flv2, flv3, flv4, flv5, flv6, flv7, flv8, flv9, flv10, flv11, flv12, flv13, flv14, flv15, flv16, flv17, flv18, flv19, flv20 };
            Image[] delflv = { delflv0, delflv1, delflv2, delflv3, delflv4, delflv5, delflv6, delflv7, delflv8, delflv9, delflv10, delflv11, delflv12, delflv13, delflv14, delflv15, delflv16, delflv17, delflv18, delflv19, delflv20 };
            ComboBox[] brnd = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flname = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            TextBox[] prc = { percentage0, percentage1, percentage2, percentage3, percentage4, percentage5, percentage6, percentage7, percentage8, percentage9, percentage10, percentage11, percentage12, percentage13, percentage14, percentage15, percentage16, percentage17, percentage18, percentage19, percentage20 };
            TextBox[] fid = { flavor_id0, flavor_id1, flavor_id2, flavor_id3, flavor_id4, flavor_id5, flavor_id6, flavor_id7, flavor_id8, flavor_id9, flavor_id10, flavor_id11, flavor_id12, flavor_id13, flavor_id14, flavor_id15, flavor_id16, flavor_id17, flavor_id18, flavor_id19, flavor_id20 };

            for (int i = Convert.ToInt32(flavors_shown.Text) + 1; i < flv.Length; i++)
            {
                flv[i].Visibility = Visibility.Collapsed;
                delflv[i].Visibility = Visibility.Visible;
                brnd[i].SelectedIndex = -1;
                brnd[i].Text = "";
                flname[i].SelectedIndex = -1;
                flname[i].Text = "";
                prc[i].Text = "";
                prc[i].BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFABADB3"));
                fid[i].Text = "";
            }
        }

        private void brand_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox[] fid = { flavor_id0, flavor_id1, flavor_id2, flavor_id3, flavor_id4, flavor_id5, flavor_id6, flavor_id7, flavor_id8, flavor_id9, flavor_id10, flavor_id11, flavor_id12, flavor_id13, flavor_id14, flavor_id15, flavor_id16, flavor_id17, flavor_id18, flavor_id19, flavor_id20 };
            ComboBox comboBox = (ComboBox)sender;
            int i = Convert.ToInt32(comboBox.Name.Remove(0, 5));
            if (comboBox.SelectedIndex >= 0)
            {
                TakeBrandGiveFlavor(sender as ComboBox);
                fid[i].Text = "";
            }
            else
            {
                return;
            }
        }
        #region Recipes DelFlavor
        private void delflv_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            StackPanel[] flv = { flv0, flv1, flv2, flv3, flv4, flv5, flv6, flv7, flv8, flv9, flv10, flv11, flv12, flv13, flv14, flv15, flv16, flv17, flv18, flv19, flv20 };
            Image[] delflv = { delflv0, delflv1, delflv2, delflv3, delflv4, delflv5, delflv6, delflv7, delflv8, delflv9, delflv10, delflv11, delflv12, delflv13, delflv14, delflv15, delflv16, delflv17, delflv18, delflv19, delflv20 };
            ComboBox[] brnd = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flname = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            TextBox[] prc = { percentage0, percentage1, percentage2, percentage3, percentage4, percentage5, percentage6, percentage7, percentage8, percentage9, percentage10, percentage11, percentage12, percentage13, percentage14, percentage15, percentage16, percentage17, percentage18, percentage19, percentage20 };
            TextBox[] fid = { flavor_id0, flavor_id1, flavor_id2, flavor_id3, flavor_id4, flavor_id5, flavor_id6, flavor_id7, flavor_id8, flavor_id9, flavor_id10, flavor_id11, flavor_id12, flavor_id13, flavor_id14, flavor_id15, flavor_id16, flavor_id17, flavor_id18, flavor_id19, flavor_id20 };
            int i = Convert.ToInt32(img.Name.Remove(0, 6));
            flv[i].Visibility = Visibility.Collapsed;
            brnd[i].SelectedIndex = -1;
            flname[i].SelectedIndex = -1;
            prc[i].Text = null;
            fid[i].Text = null;
            int z = flv.Length;
            int xyz = 0;
            for (int y = 0; y < z; y++)
            {
                if (flv[y].Visibility == Visibility.Visible)
                {
                    xyz = xyz + 1;
                }
            }
            flavors_shown.Text = xyz.ToString();
            if (Adder_flv.IsEnabled == false)
            {
                Adder_flv.IsEnabled = true;
            }
            if (xyz <= 8)
            {
                ScrollViewer.SetVerticalScrollBarVisibility(FlavorScroller, ScrollBarVisibility.Hidden);
            }
            else
            {
                ScrollViewer.SetVerticalScrollBarVisibility(FlavorScroller, ScrollBarVisibility.Visible);
            }
        }

        #endregion


        private void recipeSave_Click(object sender, RoutedEventArgs e)
        {
            #region Error Check
            string errorcode = "";
            string error_1_en = "Recipe Name can not be empty.";
            string error_1_el = "Το όνομα της συνταγής δεν μπορεί να είναι κενό.";
            string error_2_en = "Author name can not be empty.";
            string error_2_el = "Το όνομα του εμπνευστή δεν μπορεί να είναι κενό.";
            string error_3_en = "Flavor Percentage can not be less than 0.01%";
            string error_3_gr = "Το ποσοστό του αρώματος δεν μπορεί να είναι λιγότερο από 0.01%";
            string error_4_en = "Flavor Percentage can not be more than 100%";
            string error_4_gr = "Το ποσοστό του αρώματος δεν μπορεί να είναι μεγαλύτερο από 100%";
            string brnd_us = "Brand can not be empty!";
            string brnd_gr = "Το όνομα της εταιρίας δεν μπορεί να είναι κενό!";
            string flv_us = "Flavor can not be empty!";
            string flv_gr = "Το όνομα του αρώματος δεν μπορεί να είναι κενό!";
            string perc_us = "Percentage can not be empty!";
            string perc_gr = "Το ποσοστό του αρώματος δεν μπορεί να είναι κενό!";
            string brndmessage = "";
            string flvmessage = "";
            string percmessage = "";
            string lowpercent = "";
            string highpercent = "";


            if ((string.IsNullOrEmpty(RecipeName.Text)) || (string.IsNullOrEmpty(Author.Text)))
            {
                if (string.IsNullOrEmpty(RecipeName.Text))
                {
                    errorcode = errorcode + "1";
                }
                else
                {
                    errorcode = errorcode + "0";
                }
                if (string.IsNullOrEmpty(Author.Text))
                {
                    errorcode = errorcode + "2";
                }
                else
                {
                    errorcode = errorcode + "0";
                }
                switch (Convert.ToInt32(errorcode))
                {
                    case 02:
                        if (CurrentCulture == "en-US")
                        {
                            AutoClosingMessageBox.Show(error_2_en, "Error", autotimeout);
                            return;
                        }
                        else if (CurrentCulture == "el-GR")
                        {
                            AutoClosingMessageBox.Show(error_2_el, "Error", autotimeout);
                            return;
                        }
                        break;
                    case 10:
                        if (CurrentCulture == "en-US")
                        {
                            AutoClosingMessageBox.Show(error_1_en, "Error", autotimeout);
                            return;
                        }
                        else if (CurrentCulture == "el-GR")
                        {
                            AutoClosingMessageBox.Show(error_1_el, "Error", autotimeout);
                            return;
                        }
                        break;
                    case 12:
                        if (CurrentCulture == "en-US")
                        {
                            AutoClosingMessageBox.Show(error_1_en + "\n" + error_2_en, "Error", autotimeout);
                            return;
                        }
                        else if (CurrentCulture == "el-GR")
                        {
                            AutoClosingMessageBox.Show(error_1_el + "\n" + error_2_el, "Error", autotimeout);
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion

            flavors_shown.Text = "0";
            List<string> myBrandList = new List<string>();
            List<string> myFlavorList = new List<string>();
            List<string> myPercentList = new List<string>();
            List<string> myIdList = new List<string>();
            int y = 0;
            StackPanel[] flv = { flv0, flv1, flv2, flv3, flv4, flv5, flv6, flv7, flv8, flv9, flv10, flv11, flv12, flv13, flv14, flv15, flv16, flv17, flv18, flv19, flv20 };
            ComboBox[] brnd = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flname = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            TextBox[] percentage = { percentage0, percentage1, percentage2, percentage3, percentage4, percentage5, percentage6, percentage7, percentage8, percentage9, percentage10, percentage11, percentage12, percentage13, percentage14, percentage15, percentage16, percentage17, percentage18, percentage19, percentage20 };
            TextBox[] flv_id = { flavor_id0, flavor_id1, flavor_id2, flavor_id3, flavor_id4, flavor_id5, flavor_id6, flavor_id7, flavor_id8, flavor_id9, flavor_id10, flavor_id11, flavor_id12, flavor_id13, flavor_id14, flavor_id15, flavor_id16, flavor_id17, flavor_id18, flavor_id19, flavor_id20 };
            for (int i = 0; i < flv.Length; i++)
            {
                if (flv[i].Visibility == Visibility.Visible)
                {
                    myBrandList.Add(brnd[i].Text);
                    myFlavorList.Add(flname[i].Text);
                    myPercentList.Add(percentage[i].Text);
                    myIdList.Add(flv_id[i].Text);
                    y++;
                }
            }
            var myBrandArray = myBrandList.ToArray();
            var myFlavorArray = myFlavorList.ToArray();
            var myPercentArray = myPercentList.ToArray();
            var myIdArray = myIdList.ToArray();
            flavors_shown.Text = Convert.ToString(myBrandArray.Length);



            for (int i = 0; i < y; i++)
            {
                //MessageBox.Show(myBrandArray[i].ToString()+"\n"+ myFlavorArray[i].ToString() + "\n"+ myPercentArray[i].ToString() + "\n" + myIdArray[i].ToString());
                if (string.IsNullOrEmpty(myBrandArray[i]))
                {
                    if (CurrentCulture == "en-US")
                    {
                        brndmessage = brnd_us;
                    }
                    else
                    {
                        brndmessage = brnd_gr;
                    }
                    AutoClosingMessageBox.Show(brndmessage, "Error", autotimeout);
                    return;
                }
                if (string.IsNullOrEmpty(myFlavorArray[i]))
                {
                    if (CurrentCulture == "en-US")
                    {
                        flvmessage = flv_us;
                    }
                    else
                    {
                        flvmessage = flv_gr;
                    }
                    AutoClosingMessageBox.Show(flvmessage, "Error", autotimeout);
                    return;
                }

                if (string.IsNullOrEmpty(myPercentArray[i]))
                {
                    if (CurrentCulture == "en-US")
                    {
                        percmessage = perc_us;
                    }
                    else
                    {
                        percmessage = perc_gr;
                    }
                    AutoClosingMessageBox.Show(percmessage, "Error", autotimeout);
                    return;
                }

                double perc = Convert.ToDouble(myPercentArray[i]);
                if (perc < 0.01)
                {
                    if (CurrentCulture == "en-US")
                    {
                        lowpercent = error_3_en;
                    }
                    else
                    {
                        lowpercent = error_3_gr;
                    }
                    AutoClosingMessageBox.Show(lowpercent, "Error", autotimeout);
                    return;
                }
                if (perc > 100)
                {
                    if (CurrentCulture == "en-US")
                    {
                        highpercent = error_4_en;
                    }
                    else
                    {
                        highpercent = error_4_gr;
                    }
                    AutoClosingMessageBox.Show(highpercent, "Error", autotimeout);
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
                string query;
                if ((RecipeName.SelectedIndex == -1) && (!(string.IsNullOrEmpty(recipeid.Text))))
                {
                    query = $"INSERT Into RecipeBook ";
                    query += "([RecipeName], [TimesMade], [Author]) ";
                    query += $"VALUES('{RecipeName.Text.Replace("'", "''")}', '0', '{ Author.Text.Replace("'", "''") }');";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    int resultAffectedRows = dbCmd.ExecuteNonQuery();
                    if (CurrentCulture == "en-US")
                    {
                        message = "The recipe: " + RecipeName.Text + "\nAdded successfully!";
                        caption = "Success";
                    }
                    if (CurrentCulture == "el-GR")
                    {
                        message = "Η συνταγή: " + RecipeName.Text + "\nΠροστέθηκε επιτυχώς!";
                        caption = "Επιτυχία";
                    }
                    AutoClosingMessageBox.Show(message, caption, autotimeout);
                    dbConn.Close();
                }
                else
                {
                    if ((RecipeName.SelectedIndex >= 0) || (!(string.IsNullOrEmpty(recipeid.Text))))
                    {

                        query = $"UPDATE RecipeBook Set RecipeName='{ RecipeName.Text.Replace("'", "''") }', Author='{ Author.Text.Replace("'", "''") }' Where Id = '{ recipeid.Text }'; ";
                        dbCmd = new SQLiteCommand(query, dbConn);
                        int resultAffectedRows = dbCmd.ExecuteNonQuery();
                        if (CurrentCulture == "en-US")
                        {
                            message = "Updated successfully " + resultAffectedRows + " entry!";
                            caption = "Success";
                        }
                        if (CurrentCulture == "el-GR")
                        {
                            message = "Ενημερώθηκε επιτυχώς " + resultAffectedRows + " εγγραφή!";
                            caption = "Επιτυχία!";
                        }
                        AutoClosingMessageBox.Show(message, caption, autotimeout);
                        query = $"Delete from [hash] where RECIPE_ID = '{ recipeid.Text }'; ";
                        dbCmd = new SQLiteCommand(query, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }
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
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                string query = "";
                int resultAffectedRows;
                for (int i = 0; i < y; i++)
                {
                        query += $"INSERT Into hash ([RECIPE_ID], [FLAVOUR_ID], [PERCENTAGE]) VALUES('{ recipeid.Text }', '{ myIdArray[i] }' ,'{ myPercentArray[i] }'); ";
                }
                dbCmd = new SQLiteCommand(query, dbConn);
                resultAffectedRows = dbCmd.ExecuteNonQuery();

                string fuery = "update hash SET PERCENTAGE = replace(PERCENTAGE, ',', '.') WHERE PERCENTAGE  LIKE '%,%'; ";
                dbCmd = new SQLiteCommand(fuery, dbConn);
                resultAffectedRows = dbCmd.ExecuteNonQuery();

                if (dbConn.State == ConnectionState.Open)
                {

                    dbConn.Close();
                }
                RecipesComboBox(RecipeName);
                flavors_shown.Text = "1";
                delflv1.Visibility = Visibility.Visible;
                brand1.SelectedIndex = -1;
                flavor1.SelectedIndex = -1;
                percentage1.Text = String.Empty;
                flavor_id1.Text = String.Empty;
                RecipeName.SelectedIndex = -1;
                Adder_flv.IsEnabled = true;
                RecipeName.Text = "";
                recipeid.Text = "";
                Author.Text = "";
                HideTheRest();
                //checkIfWarehouseIsNotEmpty();
            }
        }

        private void flavor_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1) { return; }
            ComboBox[] brnd = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flname = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            int i = Convert.ToInt32(comboBox.Name.Remove(0, 6));
            if (comboBox.SelectedIndex >= 0)
            {
                getFlavorId(brnd[i], flname[i]);
            }
            else
            {
                AutoClosingMessageBox.Show("error", "Error", autotimeout);
            }
        }

        private void flavor_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1) { return; }
            ComboBox[] brnd = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flname = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            int i = Convert.ToInt32(comboBox.Name.Remove(0, 6));
            if (comboBox.SelectedIndex >= 0)
            {
                getFlavorId(brnd[i], flname[i]);
            }
            else
            {
                AutoClosingMessageBox.Show("error", "Error", autotimeout);
            }
        }
        private void flavor_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1) { return; }
            ComboBox[] brnd = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flname = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            int i = Convert.ToInt32(comboBox.Name.Remove(0, 6));
            if (comboBox.SelectedIndex >= 0)
                {
                getFlavorId(brnd[i], flname[i]);
            }
            else
            {
                AutoClosingMessageBox.Show("error", "Error", autotimeout);
            }
        }
        private void flavor_id_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ComboBox[] brnd = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flname = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            TextBox[] percentage = { percentage0, percentage1, percentage2, percentage3, percentage4, percentage5, percentage6, percentage7, percentage8, percentage9, percentage10, percentage11, percentage12, percentage13, percentage14, percentage15, percentage16, percentage17, percentage18, percentage19, percentage20 };

            int i = textBox.Name.Length;
            int RemoveCount = 0;
            if (i == 10)
            {
                RemoveCount = i - 1;
            }
            else
            {
                RemoveCount = i - 2;
            }
            int y = Convert.ToInt32(textBox.Name.Remove(0, RemoveCount));

            if (!string.IsNullOrEmpty(brnd[y].Text) || (!string.IsNullOrEmpty(flname[y].Text)))
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    percentage[y].BorderBrush = Brushes.Green;
                    percentage[y].BorderThickness = new Thickness(2, 2, 2, 2);
                }
                else
                {
                    percentage[y].BorderBrush = Brushes.Red;
                    percentage[y].BorderThickness = new Thickness(2, 2, 2, 2);
                }
            }
            else
            {
                percentage[y].BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFABADB3"));
                percentage[y].BorderThickness = new Thickness(1, 1, 1, 1);
            }
        }
        public void getFlavorId(ComboBox comboBox, ComboBox tombo)
        {
            ComboBox[] brnd = { brand0, brand1, brand2, brand3, brand4, brand5, brand6, brand7, brand8, brand9, brand10, brand11, brand12, brand13, brand14, brand15, brand16, brand17, brand18, brand19, brand20 };
            ComboBox[] flname = { flavor0, flavor1, flavor2, flavor3, flavor4, flavor5, flavor6, flavor7, flavor8, flavor9, flavor10, flavor11, flavor12, flavor13, flavor14, flavor15, flavor16, flavor17, flavor18, flavor19, flavor20 };
            TextBox[] percentage = { percentage0, percentage1, percentage2, percentage3, percentage4, percentage5, percentage6, percentage7, percentage8, percentage9, percentage10, percentage11, percentage12, percentage13, percentage14, percentage15, percentage16, percentage17, percentage18, percentage19, percentage20 };
            TextBox[] flv_id = { flavor_id0, flavor_id1, flavor_id2, flavor_id3, flavor_id4, flavor_id5, flavor_id6, flavor_id7, flavor_id8, flavor_id9, flavor_id10, flavor_id11, flavor_id12, flavor_id13, flavor_id14, flavor_id15, flavor_id16, flavor_id17, flavor_id18, flavor_id19, flavor_id20 };

            if (comboBox.SelectedIndex >= 0)
            {
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                try
                {
                    string query = $"SELECT Id FROM [Flavors] WHERE BrandShort='{ comboBox.Text }' AND Flavor='{ tombo.Text }'";
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int Id_Brand = int.Parse(reader[0].ToString());
                        //MessageBox.Show(Id_Brand.ToString());
                        int i = 0;
                        if (tombo.Name.Length == 7)
                        {
                            i = Convert.ToInt32(tombo.Name.Substring(tombo.Name.Length - 1));
                        }
                        else if (tombo.Name.Length == 8)
                        {
                            i = Convert.ToInt32(tombo.Name.Substring(tombo.Name.Length - 2));
                        }
                        flv_id[i].Text = Id_Brand.ToString();
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

        private void Adder_flv_Click(object sender, RoutedEventArgs e)
        {
            int i = Convert.ToInt32(flavors_shown.Text);
            if (i <= 19)
            {
                i++;
                flavors_shown.Text = Convert.ToString(i);
                if (i <= 8)
                {
                    ScrollViewer.SetVerticalScrollBarVisibility(FlavorScroller, ScrollBarVisibility.Hidden);
                }
                else
                {
                    ScrollViewer.SetVerticalScrollBarVisibility(FlavorScroller, ScrollBarVisibility.Visible);
                }
            }
            else
            {
                Adder_flv.IsEnabled = false;
                flavors_shown.IsEnabled = false;
                return;
            }
            #region Switch
            switch (i)
            {
                case 0:
                    flv1.Visibility = Visibility.Visible;
                    flv1.Height = 24;
                    break;
                case 1:
                    flv2.Visibility = Visibility.Visible;
                    flv2.Height = 24;
                    break;
                case 2:
                    flv3.Visibility = Visibility.Visible;
                    flv3.Height = 24;
                    break;
                case 3:
                    flv4.Visibility = Visibility.Visible;
                    flv4.Height = 24;
                    break;
                case 4:
                    flv5.Visibility = Visibility.Visible;
                    flv5.Height = 24;
                    break;
                case 5:
                    flv6.Visibility = Visibility.Visible;
                    flv6.Height = 24;
                    break;
                case 6:
                    flv7.Visibility = Visibility.Visible;
                    flv7.Height = 24;
                    break;
                case 7:
                    flv8.Visibility = Visibility.Visible;
                    flv8.Height = 24;
                    break;
                case 8:
                    flv9.Visibility = Visibility.Visible;
                    flv9.Height = 24;
                    break;
                case 9:
                    flv10.Visibility = Visibility.Visible;
                    flv10.Height = 24;
                    break;
                case 10:
                    flv11.Visibility = Visibility.Visible;
                    flv11.Height = 24;
                    break;
                case 11:
                    flv12.Visibility = Visibility.Visible;
                    flv12.Height = 24;
                    break;
                case 12:
                    flv13.Visibility = Visibility.Visible;
                    flv13.Height = 24;
                    break;
                case 13:
                    flv14.Visibility = Visibility.Visible;
                    flv14.Height = 24;
                    break;
                case 14:
                    flv15.Visibility = Visibility.Visible;
                    flv15.Height = 24;
                    break;
                case 15:
                    flv16.Visibility = Visibility.Visible;
                    flv16.Height = 24;
                    break;
                case 16:
                    flv17.Visibility = Visibility.Visible;
                    flv17.Height = 24;
                    break;
                case 17:
                    flv18.Visibility = Visibility.Visible;
                    flv18.Height = 24;
                    break;
                case 18:
                    flv19.Visibility = Visibility.Visible;
                    flv19.Height = 24;
                    break;
                case 19:
                    flv20.Visibility = Visibility.Visible;
                    flv20.Height = 24;
                    break;
                default:
                    //                    MessageBox.Show(Properties.Resources.BaseMixNicBoosterError);
                    break;
            }
            #endregion
        }


        public void RecipesComboBox(ComboBox comboBox)
        {
            dbConn = new SQLiteConnection(connectionString);
            dbConn.Open();
            SQLiteCommand sqlcmd = new SQLiteCommand();
            dbAdapter = new SQLiteDataAdapter();
            DataSet ds = new DataSet();
            sqlcmd.Connection = dbConn;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = "Select * from RecipeBook Where Id > '0' ORDER BY RecipeName ASC ";
            dbAdapter.SelectCommand = sqlcmd;
            dbAdapter.Fill(ds, "RecipeBook");
            comboBox.ItemsSource = ds.Tables[0].DefaultView;
            comboBox.DisplayMemberPath = ds.Tables[0].Columns["RecipeName"].ToString(); // + " (" + ds.Tables[0].Columns["ShortName"].ToString() + ")"
            comboBox.SelectedValuePath = ds.Tables[0].Columns["Id"].ToString();
            if (dbConn.State == ConnectionState.Open)
            {

                dbConn.Close();
            }
        }
    }
}
