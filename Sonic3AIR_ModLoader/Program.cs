using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using System.Resources;
using System.Reflection;

namespace Sonic3AIR_ModLoader
{
    static class Program
    {
        public static bool AutoBootCanceled = false;

        public static string Version = "v.1.3";

        public static bool CheckedForUpdateOnStartup = false;
        public static Updater.UpdateState UpdaterState { get; set; } = Updater.UpdateState.NeverStarted;
        public static Updater.UpdateResult UpdateResult { get; set; } = Updater.UpdateResult.Null;
        public static Updater.UpdateResult LastUpdateResult { get; set; } = Updater.UpdateResult.Null;

        public static ResourceManager LanguageResource { get { return UserLanguage.CurrentResource; } set { UserLanguage.CurrentResource = value; } }

        private static bool DebugMode = false;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UserLanguage.ApplyLanguageResourcePath(UserLanguage.CurrentLanguage);

            if (DebugMode) Debug();
            else ModManager(args);

            void Debug()
            {
                try
                {
                    ModManager(args);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw ex;
                }
            }
        }

        static void ModManager(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => {
                if (o.gamebanana_api != null)
                {
                    Application.Run(new ModManager(o.gamebanana_api));
                }
                else MainProgram();

            });
        }

        public static void ShowWarning()
        {
            MessageBox.Show("NOTICE: This is Program is in RELEASE CANIDATE Stage, meaning while it's just about ready, it's unfinished and still may need some further tweaks. Any Bugs you may find are a sideffect of this early release and will hopefully be fixed on release. Please let me know via GameBanna if you encounter any problems!" + Environment.NewLine + Environment.NewLine + "-CarJem Generations", "A Message from CarJem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public class Options
        {
            [Option('g', "gamebanana_api", Required = false, HelpText = "Used with Gamebanna's 1 Click Install API")]
            public string gamebanana_api { get; set; }
        }

        public static void MainProgram()
        {
            if (Properties.Settings.Default.AutoLaunch)
            {
                Application.Run(new AutoBootDialog());
                if (AutoBootCanceled == false) Application.Run(new ModManager(true));
                else Application.Run(new ModManager(false));
            }
            else
            {
                Application.Run(new ModManager());
            }
        }

    }
}
