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
    /// 

    public enum Skin { Dark, Light }
    public partial class App : Application
    {
        public static App Instance;

        public static Skin Skin { get; set; } = Skin.Dark;

        public static bool SkinChanged { get; set; } = false;


        public App()
        {
            if (Sonic3AIR_ModManager.Properties.Settings.Default.UseDarkTheme == true) ChangeSkin(Skin.Dark);
            else ChangeSkin(Skin.Light);

            #if DEBUG
                System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            #endif

            Instance = this;
            this.InitializeComponent();
        }

        public void RunAutoBoot(bool isForced = false)
        {

            var auto = new AutoBootDialog();
            if (auto.ShowDialog() == true)
            {

                if (Program.AutoBootCanceled == false)
                {
                   this.Run(new ModManager(true));
                }
                else if (!isForced) this.Run(new ModManager(false));
            }

        }

        public void GBAPI(string Arguments)
        {

            this.Run(new ModManager(Arguments));
        }


        public void DefaultStart()
        {

            this.Run(new ModManager());
        }


        public static void ChangeSkin(Skin newSkin)
        {
            Skin = newSkin;

            foreach (ResourceDictionary dict in Sonic3AIR_ModManager.App.Current.Resources.MergedDictionaries)
            {

                if (dict is SkinResourceDictionary skinDict)
                    skinDict.UpdateSource();
                else
                    dict.Source = dict.Source;
            }
        }
    }
}
