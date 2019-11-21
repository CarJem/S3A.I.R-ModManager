using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace Sonic3AIR_ModManager
{
    static class Program
    {
        public static bool AutoBootCanceled = false;

        public static Options Arguments;

        public static string Version { get; } = "v.1.4.1";

        public static bool CheckedForUpdateOnStartup = false;
        public static Updater.UpdateState UpdaterState { get; set; } = Updater.UpdateState.NeverStarted;
        public static Updater.UpdateResult UpdateResult { get; set; } = Updater.UpdateResult.Null;
        public static Updater.UpdateResult LastUpdateResult { get; set; } = Updater.UpdateResult.Null;

        public static ResourceManager LanguageResource { get { return UserLanguage.CurrentResource; } set { UserLanguage.CurrentResource = value; } }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {         
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>( o => { Arguments = o; });
            ProgramPaths.CreateMissingModManagerFolders();
            var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
            if (exists) GamebannaAPIHandler(args);
            else StartApplication(args);

        }

        static void GamebannaAPIHandler(string[] args)
        {
            if (Arguments.gamebanana_api != null)
            {

                int currentFileIndex = 0;
                bool fileCreated = false;
                while (!fileCreated)
                {
                    string path = Path.Combine(ProgramPaths.Sonic3AIR_MM_GBRequestsFolder, $"gb_api{currentFileIndex}.txt");
                    if (!File.Exists(path))
                    {
                        CreateFile(path, Arguments.gamebanana_api);
                        fileCreated = true;
                    }
                    else currentFileIndex++;
                }

            }
            Environment.Exit(Environment.ExitCode);


            void CreateFile(string path, string contents)
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(contents);
                }
            }

        }

        static void GamebanannaAPIHandler_Startup()
        {
            var app = new App();
            app.GBAPI(Arguments.gamebanana_api);
        }

        static void StartApplication(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false); 
            UserLanguage.ApplyLanguageResourcePath(UserLanguage.CurrentLanguage);
            ModFileManagement.CleanUpAPIRequests();


            if (Arguments.gamebanana_api != null)
            {
                GamebanannaAPIHandler_Startup();
            }
            else
            {
                if (Properties.Settings.Default.AutoLaunch) AutoBootLoader();
                else
                {
                    var app = new App();
                    app.DefaultStart();
                }
            }
            
        }

        static void AutoBootLoader()
        {
            var app = new App();
            app.RunAutoBoot();
        }

        public class Options
        {
            [Option('g', "gamebanana_api", Required = false, HelpText = "Used with Gamebanna's 1 Click Install API")]
            public string gamebanana_api { get; set; }
        }


    }
}
