using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for Synthesize_ChildView.xaml
    /// </summary>
    public partial class Synthesize_ChildView : UserControl
    {
        string connectionString = Settings.Default.VaConnect;
        SQLiteDataAdapter dbAdapter;
        SQLiteDataReader reader;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        public string query;
        double nic = 1.00925;
        double pg = 1.038;
        double vg = 1.2613;
        public string message, errmsg;
        public string caption;
        public string title;
        public int autotimeout = 5000;
        string CurrentCulture = Settings.Default.Culture;

        public Synthesize_ChildView()
        {
            InitializeComponent();
            RecipesComboBox(synth_RecName);
            getItems();
            getRecipes();
            HideFlavs();
            HideNic();
        }
        private void HideFlavs()
        {
            RecScroll.Visibility = Visibility.Hidden;
        }
        private void HideNic()
        {
            synth_NicBooster_Label.Visibility = Visibility.Collapsed;
            mixnic_style.Visibility = Visibility.Collapsed;
            mixnic_style.SelectedIndex = -1;
            synth_mg_Label.Visibility = Visibility.Collapsed;
            nicboosterlevel.Visibility = Visibility.Collapsed;
            nicboosterlevel.Text = "20";
            nicbooster_pg.Visibility = Visibility.Collapsed;
            nicbooster_vg.Visibility = Visibility.Collapsed;
            left_nicotine_label.Visibility = Visibility.Collapsed;
            synth_Nic_ml.Visibility = Visibility.Collapsed;
            synth_Nic_ml.Text = "";
            synthNic_Grams.Visibility = Visibility.Collapsed;
            synthNic_Grams.Text = "";
            synth_nicperc.Visibility = Visibility.Collapsed;
            synth_nicperc.Text = "";
            synth_nicwarehouse.Visibility = Visibility.Collapsed;
            synth_nicwarehouse.Text = "";
        }
        public ObservableCollection<item> getItems()
        {
            ObservableCollection<item> setItem = new ObservableCollection<item>();
            dbConn = new SQLiteConnection(connectionString);
            string query = "SELECT Brand, BrandShort, Flavor, Amount from Flavors Where Amount NOT LIKE '0.0'; ";
            dbCmd = new SQLiteCommand(query, dbConn);
            if (dbConn.State == ConnectionState.Closed)
            {
                dbConn.Open();
            }
            reader = dbCmd.ExecuteReader();

            while (reader.Read())
            {
                setItem.Add(new item()
                {
                    Brand = reader[0].ToString(),
                    BrandShort = reader[1].ToString(),
                    Flavor = reader[2].ToString(),
                    Amount = Convert.ToDouble(reader[3].ToString())
                });
            }
            dbConn.Close();
            return setItem;
        }
        public ObservableCollection<Craft> getRecipes()
        {
            ObservableCollection<Craft> setCraft = new ObservableCollection<Craft>();
            dbConn = new SQLiteConnection(connectionString);
            string query = "SELECT * from [RecipeLog] Where id > 0;";
            dbCmd = new SQLiteCommand(query, dbConn);
            if (dbConn.State == ConnectionState.Closed)
            {
                dbConn.Open();
            }
            reader = dbCmd.ExecuteReader();

            while (reader.Read())
            {
                setCraft.Add(new Craft()
                {
                    Name = reader[0].ToString(),
                    Amount = reader[1].ToString(),
                    Ratio = reader[2].ToString(),
                    Nic = reader[3].ToString(),
                    Date = reader[4].ToString(),
                    Notes = reader[5].ToString(),
                });
            }
            dbConn.Close();
            return setCraft;
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
        private void synth_RecName_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBlock[] flv = { flav0, flav1, flav2, flav3, flav4, flav5, flav6, flav7, flav8, flav9, flav10, flav11, flav12, flav13, flav14, flav15, flav16, flav17, flav18, flav19, flav20 };
            TextBlock[] percentage = { perc0, perc1, perc2, perc3, perc4, perc5, perc6, perc7, perc8, perc9, perc10, perc11, perc12, perc13, perc14, perc15, perc16, perc17, perc18, perc19, perc20 };
            TextBlock[] flav_ml = { flav_ml0, flav_ml1, flav_ml2, flav_ml3, flav_ml4, flav_ml5, flav_ml6, flav_ml7, flav_ml8, flav_ml9, flav_ml10, flav_ml11, flav_ml12, flav_ml13, flav_ml14, flav_ml15, flav_ml16, flav_ml17, flav_ml18, flav_ml19, flav_ml20 };
            TextBlock[] flav_gr = { flav_gr0, flav_gr1, flav_gr2, flav_gr3, flav_gr4, flav_gr5, flav_gr6, flav_gr7, flav_gr8, flav_gr9, flav_gr10, flav_gr11, flav_gr12, flav_gr13, flav_gr14, flav_gr15, flav_gr16, flav_gr17, flav_gr18, flav_gr19, flav_gr20 };
            TextBlock[] fl_id = { fl_id0, fl_id1, fl_id2, fl_id3, fl_id4, fl_id5, fl_id6, fl_id7, fl_id8, fl_id9, fl_id10, fl_id11, fl_id12, fl_id13, fl_id14, fl_id15, fl_id16, fl_id17, fl_id18, fl_id19, fl_id20 };
            TextBlock[] mlwarehouse = { mlwarehouse0, mlwarehouse1, mlwarehouse2, mlwarehouse3, mlwarehouse4, mlwarehouse5, mlwarehouse6, mlwarehouse7, mlwarehouse8, mlwarehouse9, mlwarehouse10, mlwarehouse11, mlwarehouse12, mlwarehouse13, mlwarehouse14, mlwarehouse15, mlwarehouse16, mlwarehouse17, mlwarehouse18, mlwarehouse19, mlwarehouse20 };
            TextBlock[] flv_weight = { flv_weight0, flv_weight1, flv_weight2, flv_weight3, flv_weight4, flv_weight5, flv_weight6, flv_weight7, flv_weight8, flv_weight9, flv_weight10, flv_weight11, flv_weight12, flv_weight13, flv_weight14, flv_weight15, flv_weight16, flv_weight17, flv_weight18, flv_weight19, flv_weight20 };
            Rectangle[] Recta = { rect0, rect1, rect2, rect3, rect4, rect5, rect6, rect7, rect8, rect9, rect10, rect11, rect12, rect13, rect14, rect15, rect16, rect17, rect18, rect19, rect20 };

            for (int i = 0; i < flv.Length; i++)
            {
                Color bgcolor = (Color)ColorConverter.ConvertFromString("Transparent");
                var bgbrush = new SolidColorBrush(bgcolor);
                Color fgcolor = (Color)ColorConverter.ConvertFromString("#000000");
                var fgbrush = new SolidColorBrush(fgcolor);
                Recta[i].Fill = bgbrush;
                flv[i].Foreground = fgbrush;
                flav_ml[i].Foreground = fgbrush;
                flav_gr[i].Foreground = fgbrush;
                percentage[i].Foreground = fgbrush;
                mlwarehouse[i].Foreground = fgbrush;
            }
            flvpercent.Text = "";
            synth_RecName_Deduct.IsEnabled = false;
            if (RecScroll.Visibility != Visibility.Visible)
            {
                RecScroll.Visibility = Visibility.Visible;
            }

            if (synth_RecName.SelectedIndex >= 0)
            {
                SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
                dbConn = new SQLiteConnection(connectionString);
                dbConn.Open();
                try
                {
                    string query = $"SELECT * FROM [RecipeBook] WHERE Recipename='" + synth_RecName.Text.Replace("'", "''") + "' ";
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int Id_Brand = reader.GetInt32(0);
                        synth_recipeid.Text = Id_Brand.ToString();
                    }
                }
                catch (Exception ex)
                {
                    if (CurrentCulture == "en-US")
                    {
                        message = $"\n\nThis window will close automaticaly in {autotimeout / 1000} seconds.";
                    }
                    else
                    {
                        message = $"\n\nΤο παράθυρο θα κλείσει αυτόματα σε {autotimeout / 1000} δευτερόλεπτα.";
                    }
                    AutoClosingMessageBox.Show(ex.ToString() + message, "Error", autotimeout);
                    //MessageBox.Show("Error" + ex.ToString());
                    if (dbConn.State == ConnectionState.Open)
                    {
                        dbConn.Close();
                    }
                }
                finally
                {
                    string query = $"SELECT Count(*) from [hash] where RECIPE_ID = '{ synth_recipeid.Text }';";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        synth_flavors_shown.Text = reader.GetInt32(0).ToString();
                    }
                    query = $"Select * from [hash] where RECIPE_ID = '{ synth_recipeid.Text }';";
                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    int i = 0;
                    while (reader.Read())
                    {
                        double flvPerz;
                        i = i + 1;
                        string fid = reader.GetInt32(2).ToString();
                        string per = reader.GetDouble(3).ToString();
                        flv[i].Visibility = Visibility.Visible;
                        percentage[i].Visibility = Visibility.Visible;
                        flav_ml[i].Visibility = Visibility.Visible;
                        flav_gr[i].Visibility = Visibility.Visible;
                        mlwarehouse[i].Visibility = Visibility.Visible;
                        flv[i].Height = 24;
                        fl_id[i].Text = fid;
                        percentage[i].Text = per;
                        if (!(string.IsNullOrEmpty(flvpercent.Text)))
                        {
                            flvPerz = Convert.ToDouble(flvpercent.Text);
                        }
                        else
                        {
                            flvPerz = 0;
                        }
                        double perZ = Convert.ToDouble(per);
                        flvpercent.Text = Convert.ToString(perZ + flvPerz);
                        string fuery = $"Select * from [Flavors] Where Id = '{ fl_id[i].Text }'"; //string fuery = $"Select BrandShort, Flavor, Amount, M_Spec_Grav from [Flavors] Where Id = '{ fl_id[i].Text }'";
                        dbCmd = new SQLiteCommand(fuery, dbConn);
                        SQLiteDataReader freader = dbCmd.ExecuteReader();
                        while (freader.Read())
                        {
                            double w8 = Convert.ToDouble(freader[4].ToString());
                            if (w8 == 1)
                            {
                                w8 = pg;
                            }
                            flv[i].Text = "(" + freader[2].ToString() + ") " + freader[3].ToString();
                            flv[i].ToolTip = "(" + freader[1].ToString() + ") " + freader[3].ToString(); ;
                            mlwarehouse[i].Text = freader[7].ToString();
                            flv_weight[i].Text = w8.ToString();
                            flav_gr[i].ToolTip = w8 + " gram/ml";
                            //MessageBox.Show(Id_Brand.ToString());
                            //brand[i].Text = brandShort;
                        }
                    }
                    //MessageBox.Show(temp.Text);
                    Color bgcolor = (Color)ColorConverter.ConvertFromString("Transparent");
                    var bgbrush = new SolidColorBrush(bgcolor);
                    Color fgcolor = (Color)ColorConverter.ConvertFromString("#000000");
                    var fgbrush = new SolidColorBrush(fgcolor);
                    if (Convert.ToInt32(synth_flavors_shown.Text) < 20)
                    {
                        for (int y = Convert.ToInt32(synth_flavors_shown.Text) + 1; y < flv.Length; y++)
                        {
                            Recta[y].Fill = bgbrush;
                            flv[y].Foreground = fgbrush;
                            flav_ml[y].Foreground = fgbrush;
                            flav_gr[y].Foreground = fgbrush;
                            percentage[y].Foreground = fgbrush;
                            mlwarehouse[y].Foreground = fgbrush;
                            fl_id[y].Visibility = Visibility.Collapsed;
                            flv[y].Visibility = Visibility.Collapsed;
                            flav_ml[y].Visibility = Visibility.Collapsed;
                            flav_gr[y].Visibility = Visibility.Collapsed;
                            percentage[y].Visibility = Visibility.Collapsed;
                            mlwarehouse[y].Visibility = Visibility.Collapsed;
                            fl_id[y].Text = "";
                            flv[y].Text = "";
                            flav_ml[y].Text = "";
                            flav_gr[y].Text = "";
                            percentage[y].Text = "";
                            mlwarehouse[y].Text = "";
                        }
                    }
                    synth_recipeid.Text = "";
                    if (dbConn.State == ConnectionState.Open)
                    {

                        dbConn.Close();
                    }
                }
            }
            if (string.IsNullOrEmpty(synth_RecName.Text)) { return; }
        }
        private void synth_finalPG_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                textBox.Text = "0";
                return;
                //snvFinalFlv.Background = System.Windows.SystemColors.MenuHighlightBrush;
            }
            else
            {
                if (Convert.ToInt32(textBox.Text) > 100)
                {
                    textBox.Text = "100";
                }
                if (Convert.ToInt32(textBox.Text) < 0)
                {
                    textBox.Text = "0";
                }
                int start = textBox.SelectionStart;
                int length = textBox.SelectionLength;
                string value = textBox.Text;

                // update text
                textBox.Text = value;

                // restore cursor position and selection
                textBox.Select(start, length);
                synth_FinalVG.Text = Convert.ToString(100 - Convert.ToInt32(textBox.Text));
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
        private void synth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (synth_RecName_Deduct.IsEnabled == true)
            {
                synth_RecName_Deduct.IsEnabled = false;
            }
            e.Handled = !IsDecAllowed(e.Text);
        }
        private void mixnic_style_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (mixnic_style.SelectedIndex)
            {
                case 0:
                    nicbooster_pg.Text = "100";
                    nicbooster_vg.Text = "0";
                    break;
                case 1:
                    nicbooster_pg.Text = "70";
                    nicbooster_vg.Text = "30";
                    break;
                case 2:
                    nicbooster_pg.Text = "50";
                    nicbooster_vg.Text = "50";
                    break;
                case 3:
                    nicbooster_pg.Text = "30";
                    nicbooster_vg.Text = "70";
                    break;
                case 4:
                    nicbooster_pg.Text = "0";
                    nicbooster_vg.Text = "100";
                    break;
            }
        }
        private void mixnic_style_DropDownClosed(object sender, EventArgs e)
        {
            switch (mixnic_style.SelectedIndex)
            {
                case 0:
                    nicbooster_pg.Text = "100";
                    nicbooster_vg.Text = "0";
                    break;
                case 1:
                    nicbooster_pg.Text = "70";
                    nicbooster_vg.Text = "30";
                    break;
                case 2:
                    nicbooster_pg.Text = "50";
                    nicbooster_vg.Text = "50";
                    break;
                case 3:
                    nicbooster_pg.Text = "30";
                    nicbooster_vg.Text = "70";
                    break;
                case 4:
                    nicbooster_pg.Text = "0";
                    nicbooster_vg.Text = "100";
                    break;
            }
        }
        private void synth_RecName_Calc_Click(object sender, RoutedEventArgs e)
        {
            TextBlock[] flv = { flav0, flav1, flav2, flav3, flav4, flav5, flav6, flav7, flav8, flav9, flav10, flav11, flav12, flav13, flav14, flav15, flav16, flav17, flav18, flav19, flav20 };
            TextBlock[] mlwarehouse = { mlwarehouse0, mlwarehouse1, mlwarehouse2, mlwarehouse3, mlwarehouse4, mlwarehouse5, mlwarehouse6, mlwarehouse7, mlwarehouse8, mlwarehouse9, mlwarehouse10, mlwarehouse11, mlwarehouse12, mlwarehouse13, mlwarehouse14, mlwarehouse15, mlwarehouse16, mlwarehouse17, mlwarehouse18, mlwarehouse19, mlwarehouse20 };
            TextBlock[] flav_ml = { flav_ml0, flav_ml1, flav_ml2, flav_ml3, flav_ml4, flav_ml5, flav_ml6, flav_ml7, flav_ml8, flav_ml9, flav_ml10, flav_ml11, flav_ml12, flav_ml13, flav_ml14, flav_ml15, flav_ml16, flav_ml17, flav_ml18, flav_ml19, flav_ml20 };
            TextBlock[] flav_gr = { flav_gr0, flav_gr1, flav_gr2, flav_gr3, flav_gr4, flav_gr5, flav_gr6, flav_gr7, flav_gr8, flav_gr9, flav_gr10, flav_gr11, flav_gr12, flav_gr13, flav_gr14, flav_gr15, flav_gr16, flav_gr17, flav_gr18, flav_gr19, flav_gr20 };
            TextBlock[] flv_weight = { flv_weight0, flv_weight1, flv_weight2, flv_weight3, flv_weight4, flv_weight5, flv_weight6, flv_weight7, flv_weight8, flv_weight9, flv_weight10, flv_weight11, flv_weight12, flv_weight13, flv_weight14, flv_weight15, flv_weight16, flv_weight17, flv_weight18, flv_weight19, flv_weight20 };
            TextBlock[] percentage = { perc0, perc1, perc2, perc3, perc4, perc5, perc6, perc7, perc8, perc9, perc10, perc11, perc12, perc13, perc14, perc15, perc16, perc17, perc18, perc19, perc20 };
            Rectangle[] Recta = { rect0, rect1, rect2, rect3, rect4, rect5, rect6, rect7, rect8, rect9, rect10, rect11, rect12, rect13, rect14, rect15, rect16, rect17, rect18, rect19, rect20 };
            bool zeronic = true;
            errorcatch.Text = "";
            if (synth_FinalNic.Text == "0" || string.IsNullOrEmpty(synth_FinalNic.Text))
            {
                zeronic = true;
                synth_FinalNic.Text = "0";
            }
            if (synth_RecName.SelectedIndex < 0 ||
                string.IsNullOrEmpty(synth_TargetML.Text) ||
                string.IsNullOrEmpty(synth_FinalNic.Text) || 
                string.IsNullOrEmpty(synth_FinalPG.Text) || 
                zeronic == false || 
                zeronic == true)
            {

                if (string.IsNullOrEmpty(synth_TargetML.Text))
                {
                    errorcatch.Text = errorcatch.Text + "1";
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                }
                if (string.IsNullOrEmpty(synth_FinalPG.Text))
                {
                    errorcatch.Text = errorcatch.Text + "2";
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                }
                if (string.IsNullOrEmpty(synth_FinalNic.Text))
                {
                    errorcatch.Text = errorcatch.Text + "3";
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                }
                if (mixnic_style.SelectedIndex < 0 && Convert.ToDouble(synth_FinalNic.Text) > 0)
                {
                    errorcatch.Text = errorcatch.Text + "4";
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                }
                if (synth_RecName.SelectedIndex < 0)
                {
                    errorcatch.Text = errorcatch.Text + "5";
                }
                else
                {
                    errorcatch.Text = errorcatch.Text + "0";
                }
                string singular = "";
                string plural = "";
                string title = "";
                string sep = "\n- ";
                if (CurrentCulture == "en-US")
                {
                    singular = "The following field is empty:\n- ";
                    plural = "The following fields are empty:\n- ";
                    title = "Error: ";
                }
                if (CurrentCulture == "el-GR")
                {
                    singular = "Το πάρακάτω πεδίο είναι κενό:\n- ";
                    plural = "Τα παρακάτω πεδία είναι κενά:\n- ";
                    title = "Σφάλμα: ";
                }
                switch (Convert.ToInt32(errorcatch.Text))
                {
                    case 00005:
                        MessageBox.Show(singular + synth_RecName_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 00040:
                        MessageBox.Show(singular + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 00045:
                        MessageBox.Show(singular + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 00300:
                        MessageBox.Show(singular + synth_FinalNic_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 00305:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synth_FinalNic_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 00340:
                        MessageBox.Show(plural + synth_FinalNic_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 00345:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synth_FinalNic_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 02000:
                        MessageBox.Show(singular + synth_FinalPG_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 02005:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synth_FinalPG_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 02040:
                        MessageBox.Show(plural + synth_FinalPG_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 02045:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synth_FinalPG_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 02300:
                        MessageBox.Show(plural + synth_FinalPG_Label.Header + sep + synth_FinalNic_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 02305:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synth_FinalPG_Label.Header + sep + synth_FinalNic_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 02340:
                        MessageBox.Show(plural + synth_FinalPG_Label.Header + sep + synth_FinalNic_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 02345:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synth_FinalPG_Label.Header + sep + synth_FinalNic_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 10000:
                        MessageBox.Show(singular + synthFinalMix.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 10005:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synthFinalMix.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 10040:
                        MessageBox.Show(plural + synthFinalMix.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 10045:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synthFinalMix.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 10300:
                        MessageBox.Show(plural + synthFinalMix.Header + sep + synth_FinalNic_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 10305:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synthFinalMix.Header + sep + synth_FinalNic_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 10340:
                        MessageBox.Show(plural + synthFinalMix.Header + sep + synth_FinalNic_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 10345:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synthFinalMix.Header + sep + synth_FinalNic_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 12000:
                        MessageBox.Show(plural + synthFinalMix.Header + sep + synth_FinalPG_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 12005:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synthFinalMix.Header + sep + synth_FinalPG_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 12040:
                        MessageBox.Show(plural + synthFinalMix.Header + sep + synth_FinalPG_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 12045:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synthFinalMix.Header + sep + synth_FinalPG_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 12300:
                        MessageBox.Show(plural + synthFinalMix.Header + sep + synth_FinalPG_Label.Header + sep + synth_FinalNic_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 12305:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synthFinalMix.Header + sep + synth_FinalPG_Label.Header + sep + synth_FinalNic_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 12340:
                        MessageBox.Show(plural + synthFinalMix.Header + sep + synth_FinalPG_Label.Header + sep + synth_FinalNic_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case 12345:
                        MessageBox.Show(plural + synth_RecName_Label.Header + sep + synthFinalMix.Header + sep + synth_FinalPG_Label.Header + sep + synth_FinalNic_Label.Header + sep + synth_NicBooster_Label.Header, title + errorcatch.Text, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                }
                //get total mix
                double snv_FinalMix = Convert.ToDouble(synth_TargetML.Text);
                double snv_NicLevel = Convert.ToDouble(nicboosterlevel.Text); //turn nic strength to decimal value
                double snv_Final_Nic = Convert.ToDouble(synth_FinalNic.Text); //turn Target nic level value
                double snv_FinalFlv = Convert.ToDouble(flvpercent.Text);
                double snv_Nic_ml = Math.Round((snv_Final_Nic / snv_NicLevel) * snv_FinalMix, 2); //done
                double snv_Flv_ml = Math.Round((snv_FinalMix * snv_FinalFlv) / 100, 2); //done
                double snvNic_PGpercent = Convert.ToDouble(nicbooster_pg.Text);
                double snvNic_VGpercent = Convert.ToDouble(nicbooster_vg.Text);
                double NicPG_ml = (snv_Nic_ml * snvNic_PGpercent) / 100;
                double NicVG_ml = (snv_Nic_ml * snvNic_VGpercent) / 100; //turn PG Percentage into decimal
                double snv_Final_PG = Convert.ToDouble(synth_FinalPG.Text);
                double snv_Final_VG = Convert.ToDouble(synth_FinalVG.Text);
                double snvΒasPG_PGpercent = Convert.ToDouble(100);
                double snvΒasVG_VGpercent = Convert.ToDouble(100);
                double FinalPG_ml = (snv_FinalMix * snv_Final_PG) / 100;
                double FinalVG_ml = (snv_FinalMix * snv_Final_VG) / 100;
                double snv_Flv_PG = Convert.ToDouble(100);
                double snv_Flv_VG = Convert.ToDouble(0);
                double FlvPG_ml = (snv_Flv_ml * snv_Flv_PG) / 100;
                double FlvVG_ml = (snv_Flv_ml * snv_Flv_VG) / 100;
                double snvBasPGml = FinalPG_ml - NicPG_ml - FlvPG_ml;
                double snv_BasPG_ml = FinalPG_ml - NicPG_ml - FlvPG_ml;
                double snv_BasVG_ml = FinalVG_ml - NicVG_ml - FlvVG_ml;



                //double snvFlv_PGpercent = Convert.ToDouble(snvFlv_PG.Text);



                //turn nic pg/vg percent to decimal value




                // calculate required ml
                synth_BasPG_ml.Text = Convert.ToString(snv_BasPG_ml);
                synth_BasVG_ml.Text = Convert.ToString(snv_BasVG_ml);
                synth_Nic_ml.Text = Convert.ToString(snv_Nic_ml);
                synth_Flv_ml.Text = Convert.ToString(snv_Flv_ml);
                synth_ΒasPG_Grams.Text = Convert.ToString(Math.Round(snv_BasPG_ml * pg, 2));
                synth_BasVG_Grams.Text = Convert.ToString(Math.Round(snv_BasVG_ml * vg, 2));
                synth_pgperc.Text = Convert.ToString(Math.Round((snv_BasPG_ml / snv_FinalMix) * 100, 2));
                synth_vgperc.Text = Convert.ToString(Math.Round((snv_BasVG_ml / snv_FinalMix) * 100, 2));
                synth_nicperc.Text = Convert.ToString(Math.Round((snv_Nic_ml / snv_FinalMix) * 100, 2));
                double niclevelpercentdecimal = (snv_NicLevel / 1000);
                double secondlevelpercentdecimal = 1 - niclevelpercentdecimal;
                double pggrav = (niclevelpercentdecimal * nic) + (secondlevelpercentdecimal * pg);
                double vggrav = (niclevelpercentdecimal * nic) + (secondlevelpercentdecimal * vg);
                double npgg = NicPG_ml * pggrav;
                double nvgg = NicVG_ml * vggrav;

                synthNic_Grams.Text = Convert.ToString(Math.Round(npgg + nvgg, 2));
                double fpgg = FlvPG_ml * pg;
                double fvgg = FlvVG_ml * vg;
                synthFlv_Grams.Text = Convert.ToString(Math.Round(fpgg + fvgg, 2));
                double TotalPG = (snv_BasPG_ml * pg) + npgg + (FlvPG_ml * pg);
                double TotalVG = (snv_BasVG_ml * vg) + nvgg + (FlvVG_ml * vg);

                totalML.Text = Convert.ToString(snv_BasPG_ml + snv_BasVG_ml + snv_Nic_ml + snv_Flv_ml);
                totalGr.Text = Convert.ToString(Math.Round(TotalPG + TotalVG,2));
                totalPercent.Text = Convert.ToString(Convert.ToDouble(synth_pgperc.Text) + Convert.ToDouble(synth_vgperc.Text) + 
                    Convert.ToDouble(synth_nicperc.Text) + Convert.ToDouble(flvpercent.Text));

                double pert = 0;
                double ownml = 0;
                double flavgrams = 0;
                double flavmls = 0;
                double weight = 0;
                string messageending = "";
                string messageless = "";
                int messagelessLength = 0;
                int messageLength = 0;
                autotimeout = 20000;
                bool errors = false;
                if (CurrentCulture == "en-US")
                {
                    caption = "Error";
                    message = $"You don't have the following flavor(s) in your warehouse.\n\n";
                    messageending = $"\nThis window will close automaticaly in {autotimeout / 1000} seconds.";
                    messageless = $"You have less than the required ml of:\n\n";
                    
                }
                if (CurrentCulture == "el-GR")
                {
                    caption = "Σφάλμα";
                    message = $"Δεν έχετε το(τα) παρακάτω άρωμα(τα) στο μπαούλο σας.\n\n";
                    messageending = $"\nΤο παράθυρο θα κλείσει αυτόματα σε {autotimeout / 1000} δευτερόλεπτα.";
                    messageless = $"Έχετε λιγότερα από τα απαιτούμενα ml στο:\n\n";
                }
                messageLength = message.Length;
                messagelessLength = messageless.Length;
                int fry = Convert.ToInt32(synth_flavors_shown.Text) + 1;
                for (int i = 1; i < fry; i++)
                {
                    weight = Convert.ToDouble(flv_weight[i].Text);
                    pert = Convert.ToDouble(percentage[i].Text);
                    flavmls = Math.Round(snv_FinalMix * (pert / 100), 2);
                    ownml = Convert.ToDouble(mlwarehouse[i].Text);
                    
                    if (weight == 1)
                    {
                        flavgrams = Math.Round(flavmls * pg, 2);
                    }
                    else
                    {
                        flavgrams = Math.Round(flavmls * weight, 2);
                    }
                    flav_ml[i].Text = Convert.ToString(flavmls);
                    flav_gr[i].Text = Convert.ToString(flavgrams);
                    if (mlwarehouse[i].Text == "0" || ownml < flavmls)
                    {
                        if (mlwarehouse[i].Text == "0")
                        {
                            message += $"{ flv[i].Text }\n";

                        }
                        if (ownml < flavmls && ownml > 0)
                        {
                                messageless += $"{flv[i].Text} ({ownml}/{flavmls}ml)\n";

                        }

                        Color bgcolor = (Color)ColorConverter.ConvertFromString(Settings.Default.ErrorBG);
                        var bgbrush = new SolidColorBrush(bgcolor);
                        Color fgcolor = (Color)ColorConverter.ConvertFromString("#FFFFFF");
                        var fgbrush = new SolidColorBrush(fgcolor);
                        Recta[i].Fill = bgbrush;
                        flv[i].Foreground = fgbrush;
                        flav_ml[i].Foreground = fgbrush;
                        flav_gr[i].Foreground = fgbrush;
                        percentage[i].Foreground = fgbrush;
                        mlwarehouse[i].Foreground = fgbrush;
                        errors = true;
                    }
                    else
                    {
                        Color bgcolor = (Color)ColorConverter.ConvertFromString("Transparent");
                        var bgbrush = new SolidColorBrush(bgcolor);
                        Color fgcolor = (Color)ColorConverter.ConvertFromString("#000000");
                        var fgbrush = new SolidColorBrush(fgcolor);
                        Recta[i].Fill = bgbrush;
                        flv[i].Foreground = fgbrush;
                        flav_ml[i].Foreground = fgbrush;
                        flav_gr[i].Foreground = fgbrush;
                        percentage[i].Foreground = fgbrush;
                        mlwarehouse[i].Foreground = fgbrush;
                    }
                }
                if (errors == true) {
                    string finalMessage = "";
                    if (message.Length > messageLength)
                    {
                        finalMessage += message + "\n";
                    }
                    if (messageless.Length > messagelessLength)
                    {
                        finalMessage += messageless + "\n";
                    }
                    synth_RecName_Deduct.IsEnabled = false;
                    AutoClosingMessageBox.Show(finalMessage + messageending, caption, autotimeout);
                }
                else
                {
                    synth_RecName_Deduct.IsEnabled = true;
                }

                if ((snv_BasPG_ml < 0) || (snv_BasVG_ml < 0))
                {
                    if (snv_BasPG_ml < 0)
                    {
                        Color bgcolor = (Color)ColorConverter.ConvertFromString(Settings.Default.ErrorBG);
                        var bgbrush = new SolidColorBrush(bgcolor);
                        Color fgcolor = (Color)ColorConverter.ConvertFromString("#FFFFFF");
                        var fgbrush = new SolidColorBrush(fgcolor);
                        synth_BasPG_ml.Background = bgbrush;
                        synth_BasPG_ml.Foreground = fgbrush;
                    }

                    if (snv_BasVG_ml < 0)
                    {
                        Color bgcolor = (Color)ColorConverter.ConvertFromString(Settings.Default.ErrorBG);
                        var bgbrush = new SolidColorBrush(bgcolor);
                        Color fgcolor = (Color)ColorConverter.ConvertFromString("#FFFFFF");
                        var fgbrush = new SolidColorBrush(fgcolor);
                        synth_BasVG_ml.Background = bgbrush;
                        synth_BasVG_ml.Foreground = fgbrush;
                    }
                    MessageBox.Show(Properties.Resources.OneShotErrorMsg1 + "\n" +
                    Properties.Resources.OneShotErrorMsg2, Properties.Resources.OneShotErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {
                    Color bgcolor = (Color)ColorConverter.ConvertFromString("Transparent");
                    var bgbrush = new SolidColorBrush(bgcolor);
                    Color fgcolor = (Color)ColorConverter.ConvertFromString("#000000");
                    var fgbrush = new SolidColorBrush(fgcolor);
                    synth_BasPG_ml.Background = bgbrush;
                    synth_BasVG_ml.Background = bgbrush;
                    synth_BasPG_ml.Foreground = fgbrush;
                    synth_BasVG_ml.Foreground = fgbrush;
                }
            }
        }
        private void Synth_FinalNic_TextChanged(object sender, TextChangedEventArgs e)
        {
            string messageending = "";
            if (CurrentCulture == "en-US")
            {
                messageending = $"\nThis window will close automaticaly in {autotimeout / 1000} seconds.";
            }
            if (CurrentCulture == "el-GR")
            {
                messageending = $"\nΤο παράθυρο θα κλείσει αυτόματα σε {autotimeout / 1000} δευτερόλεπτα.";
            }
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text) || textBox.Text == "0")
            {
                synth_NicBooster_Label.Visibility = Visibility.Collapsed;
                mixnic_style.Visibility = Visibility.Collapsed;
                mixnic_style.SelectedIndex = -1;
                synth_mg_Label.Visibility = Visibility.Collapsed;
                nicboosterlevel.Visibility = Visibility.Collapsed;
                nicboosterlevel.Text = "20";
                nicbooster_pg.Text = "0";
                nicbooster_vg.Text = "0";
                left_nicotine_label.Visibility = Visibility.Collapsed;
                synth_Nic_ml.Visibility = Visibility.Collapsed;
                synth_Nic_ml.Text = "";
                synthNic_Grams.Visibility = Visibility.Collapsed;
                synthNic_Grams.Text = "";
                synth_nicperc.Visibility = Visibility.Collapsed;
                synth_nicperc.Text = "";
                synth_nicwarehouse.Visibility = Visibility.Collapsed;
                synth_nicwarehouse.Text = "";
            }
            else
            {
                synth_NicBooster_Label.Visibility = Visibility.Visible;
                mixnic_style.Visibility = Visibility.Visible;
                synth_mg_Label.Visibility = Visibility.Visible;
                left_nicotine_label.Visibility = Visibility.Visible;
                synth_Nic_ml.Visibility = Visibility.Visible;
                synthNic_Grams.Visibility = Visibility.Visible;
                synth_nicperc.Visibility = Visibility.Visible;
                synth_nicwarehouse.Visibility = Visibility.Visible;
                nicboosterlevel.Visibility = Visibility.Visible;
                if (Convert.ToDouble(textBox.Text) >= Convert.ToDouble(nicboosterlevel.Text))
                {
                    string message = "", title = "";
                    if (CurrentCulture == "en-US")
                    {
                        message = "Target Nicotine level is impossible.";
                        title = "Error";
                        textBox.Text = "0";
                    }
                    if (CurrentCulture == "el-GR")
                    {
                        message = "Το επιθυμητό επίπεδο νικοτίνης δεν είναι εφικτό.";
                        title = "Σφάλμα";
                        textBox.Text = "0";
                    }
                    AutoClosingMessageBox.Show(message + messageending, title, autotimeout);
                }
            }
        }
        private void synth_RecName_Deduct_Click(object sender, RoutedEventArgs e)
        {
            TextBlock[] flv = { flav0, flav1, flav2, flav3, flav4, flav5, flav6, flav7, flav8, flav9, flav10, flav11, flav12, flav13, flav14, flav15, flav16, flav17, flav18, flav19, flav20 };
            TextBlock[] percentage = { perc0, perc1, perc2, perc3, perc4, perc5, perc6, perc7, perc8, perc9, perc10, perc11, perc12, perc13, perc14, perc15, perc16, perc17, perc18, perc19, perc20 };
            TextBlock[] flav_ml = { flav_ml0, flav_ml1, flav_ml2, flav_ml3, flav_ml4, flav_ml5, flav_ml6, flav_ml7, flav_ml8, flav_ml9, flav_ml10, flav_ml11, flav_ml12, flav_ml13, flav_ml14, flav_ml15, flav_ml16, flav_ml17, flav_ml18, flav_ml19, flav_ml20 };
            TextBlock[] flav_gr = { flav_gr0, flav_gr1, flav_gr2, flav_gr3, flav_gr4, flav_gr5, flav_gr6, flav_gr7, flav_gr8, flav_gr9, flav_gr10, flav_gr11, flav_gr12, flav_gr13, flav_gr14, flav_gr15, flav_gr16, flav_gr17, flav_gr18, flav_gr19, flav_gr20 };
            TextBlock[] fl_id = { fl_id0, fl_id1, fl_id2, fl_id3, fl_id4, fl_id5, fl_id6, fl_id7, fl_id8, fl_id9, fl_id10, fl_id11, fl_id12, fl_id13, fl_id14, fl_id15, fl_id16, fl_id17, fl_id18, fl_id19, fl_id20 };
            TextBlock[] mlwarehouse = { mlwarehouse0, mlwarehouse1, mlwarehouse2, mlwarehouse3, mlwarehouse4, mlwarehouse5, mlwarehouse6, mlwarehouse7, mlwarehouse8, mlwarehouse9, mlwarehouse10, mlwarehouse11, mlwarehouse12, mlwarehouse13, mlwarehouse14, mlwarehouse15, mlwarehouse16, mlwarehouse17, mlwarehouse18, mlwarehouse19, mlwarehouse20 };

            SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
            dbConn = new SQLiteConnection(connectionString);
            dbConn.Open();
            bool error = false;
            try
            {
                int fly = Convert.ToInt32(synth_flavors_shown.Text) + 1;
                for (int i = 1; i < fly; i++)
                {
                    double flvml = Convert.ToDouble(flav_ml[i].Text);
                    double mlsta = Convert.ToDouble(mlwarehouse[i].Text);
                    if ((mlwarehouse[i].Text == "0") || (flvml > mlsta))
                    {

                        if (CurrentCulture == "en-US")
                        {
                            message = "This mix is impossible";
                            message += $"\n\nThis window will close automaticaly in {autotimeout / 1000} seconds.";
                            caption = "Error";
                        }
                        if (CurrentCulture == "el-GR")
                        {
                            message = "Αυτή η μίξη είναι αδύνατη.";
                            message += $"\n\nΤο παράθυρο θα κλείσει αυτόματα σε {autotimeout / 1000} δευτερόλεπτα.";
                            caption = "Σφάλμα";
                        }
                        AutoClosingMessageBox.Show(message, caption, autotimeout);
                        error = true;
                        return;
                    }
                    string query = $"SELECT Amount FROM [Flavors] WHERE Id='" + fl_id[i].Text + "' ";
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }

                    dbCmd = new SQLiteCommand(query, dbConn);
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        double dbAmount = reader.GetDouble(0);
                        double rcAmount = Convert.ToDouble(flav_ml[i].Text);
                        double nwAmount = Math.Round(dbAmount - rcAmount, 2);
                        int id = Convert.ToInt32(fl_id[i].Text);
                        string fuery = $"update Flavors set Amount = '{nwAmount}' WHERE Id = '{id}'";
                        dbCmd = new SQLiteCommand(fuery, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }
                }
                string addmake = $"UPDATE [RecipeBook] SET TimesMade = TimesMade +1 WHERE [RecipeName] = '{synth_RecName.Text.Replace("'", "''")}' ";
                dbCmd = new SQLiteCommand(addmake, dbConn);
                dbCmd.ExecuteNonQuery();
                string CraftName = synth_RecName.Text.Replace("'", "''");
                double CraftAmount = Math.Round(Convert.ToDouble(synth_TargetML.Text), 2);
                string CraftRatio = synth_FinalPG.Text + "/" + synth_FinalVG.Text;
                string CraftNicotine = synth_FinalNic.Text;
                string CraftDate = DateTime.Now.ToString("yyyy/MM/dd");
                string CraftNotes = synth_Notes.Text;
                string finisher = $"UPDATE [Flavors] SET [Amount] = REPLACE([Amount], ',', '.') WHERE Amount LIKE '%,%'; ";
                finisher += $"Insert INTO [RecipeLog] ([Name], [Amount], [Ratio], [Nic], [Date], [Notes]) ";
                finisher += $"VALUES('{CraftName}', '{CraftAmount}', '{CraftRatio}', '{CraftNicotine}', '{CraftDate}', '{CraftNotes}'); ";
                finisher += $"UPDATE [RecipeLog] SET [Amount] = REPLACE([Amount], ',', '.'), [Nic] = REPLACE([Nic], ',', '.') WHERE Amount LIKE '%,%'; ";
                dbCmd = new SQLiteCommand(finisher, dbConn);
                dbCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (CurrentCulture == "en-US") { 
                    message = $"\n\nThis window will close automaticaly in {autotimeout / 1000} seconds.";
                }
                else
                {
                    message = $"\n\nΤο παράθυρο θα κλείσει αυτόματα σε {autotimeout / 1000} δευτερόλεπτα.";
                }
                AutoClosingMessageBox.Show(ex.ToString() + message, "Error", autotimeout);
                if (dbConn.State == ConnectionState.Open)
                {
                    dbConn.Close();
                }
            }
            finally
            {
                if (error == false)
                {
                    getRecipes();
                    getItems();
                    synth_RecName.SelectedIndex = -1;
                    synth_TargetML.Text = "";
                    synth_FinalPG.Text = "0";
                    synth_FinalNic.Text = "";
                    nicbooster_pg.Text = "";
                    nicbooster_vg.Text = "";
                    synth_BasPG_ml.Text = "";
                    synth_ΒasPG_Grams.Text = "";
                    synth_pgperc.Text = "";
                    synth_BasVG_ml.Text = "";
                    synth_BasVG_Grams.Text = "";
                    synth_vgperc.Text = "";
                    mixnic_style.SelectedIndex = -1;
                    synth_Notes.Text = "";
                    synth_Nic_ml.Text = "";
                    synthNic_Grams.Text = "";
                    synth_nicperc.Text = "";
                    synth_Flv_ml.Text = "";
                    synthFlv_Grams.Text = "";

                    for (int i = 1; i < mlwarehouse.Length; i++)
                    {
                        fl_id[i].Text = "";
                        flv[i].Text = "";
                        percentage[i].Text = "";
                        flav_ml[i].Text = "";
                        flav_gr[i].Text = "";
                        mlwarehouse[i].Text = "";
                        fl_id[i].Visibility = Visibility.Collapsed;
                        flv[i].Visibility = Visibility.Collapsed;
                        percentage[i].Visibility = Visibility.Collapsed;
                        flav_ml[i].Visibility = Visibility.Collapsed;
                        flav_gr[i].Visibility = Visibility.Collapsed;
                        mlwarehouse[i].Visibility = Visibility.Collapsed;
                    }
                    RecScroll.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
    public class Craft
    {
        public string Name { set; get; }
        public string Amount { set; get; }
        public string Ratio { set; get; }
        public string Nic { set; get; }
        public string Date { set; get; }
        public string Notes { set; get; }
    }
    public class item
    {
        public string Brand { set; get; }
        public string BrandShort { set; get; }
        public string Flavor { set; get; }
        public double Amount { set; get; }
    }
}
