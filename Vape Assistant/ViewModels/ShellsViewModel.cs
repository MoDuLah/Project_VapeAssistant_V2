using Caliburn.Micro;
using System;
using System.Windows;
using Vape_Assistant.Properties;

namespace Vape_Assistant.ViewModels
{
    public class ShellsViewModel : Conductor<object>
    {
        public ShellsViewModel()
        {
            
        }

        public void Load_OhmsLaw()
        {
            ActivateItem(new OhmsLaw_ChildViewModel());
        }

        public void Load_Statistics()
        {
            ActivateItem(new Statistics_ChildViewModel());
        }

        public void Load_Settings()
        {
            ActivateItem(new Settings_ChildViewModel());
        }

        public void Load_About()
        {
            ActivateItem(new About_ChildViewModel());
        }

        public void Load_BoosterCost()
        {
            ActivateItem(new BoosterCost_ChildViewModel());
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
            }
        }
        public void Load_OneShots()
        {
            ActivateItem(new OneShots_ChildViewModel());
        }
        public void Load_Warehouse()
        {
            ActivateItem(new Warehouse_ChildViewModel());
        }
        public void Load_JuiceLog()
        {
            ActivateItem(new JuiceLog_ChildViewModel());
        }

        public void Load_Flavors()
        {
            ActivateItem(new Flavors_ChildViewModel());
        }
        public void Load_BasesMix()
        {
            ActivateItem(new BasesMix_ChildViewModel());
        }

        public void Load_Recipes()
        {
            ActivateItem(new Recipes_ChildViewModel());
        }
        public void Load_Synthesize()
        {
            ActivateItem(new Synthesize_ChildViewModel());
        }

        public void Load_Help()
        {
            ActivateItem(new Help_ChildViewModel());
        }

        private void ShowChildWindow()
        {
            Window childWindow = new Login
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
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
