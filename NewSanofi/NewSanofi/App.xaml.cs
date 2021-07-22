using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NewSanofi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex myMutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            bool aIsNewInstance = false;
            myMutex = new Mutex(true, "NewSanofi", out aIsNewInstance);
            if (!aIsNewInstance)
            {
                MessageBox.Show("Only one instance of APPLICATION is allowed...");
                Environment.Exit(0);
                App.Current.Resources.Clear();
                App.Current.Shutdown();
            }
            else
                base.OnStartup(e);
        }
        
        

    }
}
