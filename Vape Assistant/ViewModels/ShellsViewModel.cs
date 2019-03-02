using Caliburn.Micro;
using System;
using System.IO;
using System.Net;
using System.Windows;
using Vape_Assistant.Properties;

namespace Vape_Assistant.ViewModels
{
    public class ShellsViewModel : Conductor<object>
    {
        public ShellsViewModel()
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Shells;");
            }
        }

        public void Load_OhmsLaw()
        {
            ActivateItem(new OhmsLaw_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("OhmsLaw;");
            }
        }

        public void Load_Statistics()
        {
            ActivateItem(new Statistics_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Statistics;");
            }
        }

        public void Load_Settings()
        {
            ActivateItem(new Settings_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Settings;");
            }
        }

        public void Load_About()
        {
            ActivateItem(new About_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("About;");
            }
        }

        public void Load_BoosterCost()
        {
            ActivateItem(new BoosterCost_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("BoosterCost;");
            }
        }

        public void Load_Purchases()
        {
            if (Settings.Default.isPinEnabled == true)
            {
                ShowChildWindow();
            }
            else
            {
                ActivateItem(new Purchases_ChildViewModel());
                string Path = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = "Temp.txt";

                if (!File.Exists(fileName))
                {
                    FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                    fs.Close();
                }
                using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                using (StreamWriter swT = new StreamWriter(fsWHT))
                {
                    swT.WriteLine("Purchases;");
                }
            }
        }
        public void Load_OneShots()
        {
            ActivateItem(new OneShots_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("OneShots;");
            }
        }
        public void Load_Warehouse()
        {
            ActivateItem(new Warehouse_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Warehouse;");
            }
        }
        public void Load_JuiceLog()
        {
            ActivateItem(new JuiceLog_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("JuiceLog;");
            }
        }

        public void Load_Flavors()
        {
            ActivateItem(new Flavors_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Flavors;");
            }
        }
        public void Load_BasesMix()
        {
            ActivateItem(new BasesMix_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("BasesMix;");
            }
        }

        public void Load_Recipes()
        {
            ActivateItem(new Recipes_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Recipes;");
            }
        }
        public void Load_Synthesize()
        {
            ActivateItem(new Synthesize_ChildViewModel());
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Synthesize;");
            }
        }

        private void ShowChildWindow()
        {
            Window childWindow = new Login
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            string Path = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Temp.txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
            }
            using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter swT = new StreamWriter(fsWHT))
            {
                swT.WriteLine("Login;");
            }
            childWindow.Closed += ChildWindowClosed;
            childWindow.Show();
        }

        private void ChildWindowClosed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= ChildWindowClosed;
            if (Settings.Default.LoginSuccess == true)
            {
                ActivateItem(new Purchases_ChildViewModel());
            }
            else
            {
                //ActivateItem(new TenthChildViewModel());
            }
        }
    }
}
