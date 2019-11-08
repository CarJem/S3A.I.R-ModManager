using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance;
        public App()
        {
            Instance = this;
        }

        public void RunAutoBoot()
        {
            this.InitializeComponent();
            var auto = new AutoBootDialogV2();
            if (auto.ShowDialog() == true)
            {
                if (Program.AutoBootCanceled == false) this.Run(new ModManager(true));
                else this.Run(new ModManager(false));
            }

        }

        public void GBAPI(string Arguments)
        {
            this.InitializeComponent();
            this.Run(new ModManager(Arguments));
        }


        public void DefaultStart()
        {
            this.InitializeComponent();
            this.Run(new ModManager());
        }
    }
}
