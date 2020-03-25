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
    public partial class App : Application
    {
        public static App Instance;

        public static GenerationsLib.WPF.Themes.Skin Skin
        {
            get
            {
                if (Sonic3AIR_ModManager.Management.MainDataModel.Settings.CurrentTheme != GenerationsLib.WPF.Themes.SkinResourceDictionary.CurrentTheme)
                {
                    GenerationsLib.WPF.Themes.SkinResourceDictionary.CurrentTheme = Sonic3AIR_ModManager.Management.MainDataModel.Settings.CurrentTheme;
                }
                return Sonic3AIR_ModManager.Management.MainDataModel.Settings.CurrentTheme;
            }
            set
            {
                GenerationsLib.WPF.Themes.SkinResourceDictionary.CurrentTheme = value;
                Sonic3AIR_ModManager.Management.MainDataModel.Settings.CurrentTheme = value;
            }
        }

        public static bool SkinChanged { get; set; } = false;


        public App()
        {
            GenerationsLib.WPF.Themes.SkinResourceDictionary.ChangeSkin(Sonic3AIR_ModManager.Management.MainDataModel.Settings.CurrentTheme, Sonic3AIR_ModManager.App.Current.Resources.MergedDictionaries);

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

        #endregion
    }
}
