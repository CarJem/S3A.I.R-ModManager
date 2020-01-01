using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.IO;

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
            if (Sonic3AIR_ModManager.MainDataModel.Settings.UseDarkTheme == true) ChangeSkin(Skin.Dark);
            else ChangeSkin(Skin.Light);

            DebugDisable();

            Instance = this;
            this.InitializeComponent();

        }

        #region Launch
        public void RunAutoBoot(bool isForced = false)
        {
            int? value = null;
            if (isForced)
            {
                Program.Log.InfoFormat("Starting in forced auto-boot mode...");
                value = 7;
            }
            else Program.Log.InfoFormat("Starting Auto-Boot Mode...");


            var auto = new AutoBootDialog(value);
            if (auto.ShowDialog() == true)
            {

                if (Program.AutoBootCanceled == false)
                {
                    this.Run(new ModManager(true, isForced));
                }
                else if (!isForced) this.Run(new ModManager(false));
            }

        }

        public void GBAPI(string Arguments)
        {
            Program.Log.InfoFormat("Starting Mod Manager with GB API Arguments...");
            this.Run(new ModManager(Arguments));
        }

        public void DefaultStart()
        {
            Program.Log.InfoFormat("Starting Mod Manager...");
            this.Run(new ModManager());
        }
        #endregion

        #region Misc

        private void DebugDisable()
        {
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
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

        #endregion
    }
}
