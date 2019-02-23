using System;
using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Vape_Assistant.Properties;

namespace Vape_Assistant
{
    public partial class Login : Window
    {
        string connectionString;
        string pword = Settings.Default.Password;
        string CurrentCulture = Settings.Default.Culture;


        public Login()
        {
            InitializeComponent();


            connectionString = Settings.Default.VaConnect;
            if (!(string.IsNullOrEmpty(pword)))
            {
                PassWord.Password = pword;
                Remember.IsChecked = true;
            }
            
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection con = new SQLiteConnection(connectionString))
            {
                try
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {

                        //SELECT[Id],[userName],[passWord] FROM [Accounts];
                        string Query = $"SELECT * FROM [Accounts] WHERE [passWord]='{Hash(PassWord.Password)}' ; ";
                        SQLiteCommand createCommand = new SQLiteCommand(Query, con);

                        createCommand.ExecuteNonQuery();
                        SQLiteDataReader dr = createCommand.ExecuteReader();

                        int count = 0;
                        while (dr.Read())
                        {
                            count++;
                        }
                        if (count == 1)
                        {
                            //MessageBox.Show("Username And Password is correct");
                            Settings.Default.LoginSuccess = true;
                            Settings.Default.Save();
                            if (Remember.IsChecked == true)
                            {
                                Settings.Default.Password = PassWord.Password;
                                //MessageBox.Show(Settings.Default.Password);
                            }
                            dr.Dispose();
                            createCommand.Dispose();
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                            Close();
                            GC.Collect();
                        }
                        else
                        {
                            if (CurrentCulture == "en-US") { 
                            MessageBox.Show("The Pin is incorrect.","Error",MessageBoxButton.OK,MessageBoxImage.Warning);
                            }
                            if (CurrentCulture == "el-GR")
                            {
                            MessageBox.Show("Το Pin είναι λάθος.", "Σφάλμα", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            PassWord.Password = null;
                            Settings.Default.LoginSuccess = false;
                            Settings.Default.Password = "";
                            Settings.Default.Save();
                            dr.Dispose();
                            createCommand.Dispose();
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }

                        }

                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error" + ex.ToString());
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Settings.Default.LoginSuccess = false;
            Settings.Default.Save();
            Close(); //closing this splash screen
            GC.Collect();
        }
        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "1";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "2";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "3";
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "4";
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "5";
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "6";
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "7";
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "8";
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "9";
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            PassWord.Password = PassWord.Password + "0";
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            PassWord.Password = "";
        }

        private void pw_lbl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(Hash(PassWord.Password));
        }

    }
}
