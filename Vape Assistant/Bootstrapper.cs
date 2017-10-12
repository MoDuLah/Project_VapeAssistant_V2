using Caliburn.Micro;
using System.Windows;
using Vape_Assistant.ViewModels;

namespace Vape_Assistant
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
