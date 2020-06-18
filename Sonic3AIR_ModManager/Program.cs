using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Sonic3AIR_ModManager
{
    static class Program
    {
        #region Variables
        public static log4net.ILog Log;

        public static bool AutoBootCanceled = false;

        public static Options Arguments;

        public static bool isDebug;

        public static bool allowDebugOutput { get; set; } = true;

        public static bool isDeveloper { get; set; } = false;

        [ConditionalAttribute("DEBUG")]
        public static void isDebugging()
        {
            isDebug = true;
        }

        public static ResourceManager LanguageResource { get { return Management.UserLanguage.CurrentResource; } set { Management.UserLanguage.CurrentResource = value; } }

        #endregion

        #region Version Variables

        private static string VersionString
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public static string Version { get => GetVersionString(); }

        private static string GetVersionString()
        {
            return (isDebug ? "DEV" : "v." + VersionString);
        }

        public static Version InternalVersion { get; } = new Version(VersionString);

        #endregion

        #region Update Variables

        public static bool CheckedForUpdateOnStartup = false;

        public enum UpdateResult : int
        {
            OutOfDate,
            UpToDate,
            Offline,
            FileNotFound,
            ValueNull,
            Null,
            Error
        }

        public enum UpdateState : int
        {
            Running,
            Finished,
            Checking,
            NeverStarted,
        }

        public static UpdateState AIRUpdaterState { get ; set; } = UpdateState.NeverStarted;
        public static UpdateState MMUpdaterState { get; set; } = UpdateState.NeverStarted;

        public static UpdateResult AIRUpdateResults { get; set; } = UpdateResult.Null;
        public static UpdateResult MMUpdateResults { get; set; } = UpdateResult.Null;

        public static UpdateResult AIRLastUpdateResult { get; set; } = UpdateResult.Null;
        public static UpdateResult MMLastUpdateResult { get; set; } = UpdateResult.Null;

        #endregion

        #region Main Region
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                StartLogging();
                RealMain(args);
                EndLogging();
            }
            catch (Exception ex)
            {
                //TODO: Add Language Translations
                string button_abort = "Yes - Open Logs Folder & Close Application";
                string button_ignore = "No - Close Application";
                string button_retry = "Cancel - Try to Ignore Error";
                string note = string.Format("{1}{0}{2}{0}{3}", Environment.NewLine, button_abort, button_ignore, button_retry);
                var result = System.Windows.Forms.MessageBox.Show(string.Format("{0}{1}{2}{1}{3}", ex.Message, Environment.NewLine + "|" + Environment.NewLine, ex.StackTrace, note), "ERROR", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(Sonic3AIR_ModManager.Management.ProgramPaths.Sonic3AIR_MM_LogsFolder);
                    throw ex;
                }
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                {

                }
                else
                {
                    throw ex;
                }

            }

        }

        static void RealMain(string[] args)
        {
            Log.InfoFormat("Starting Sonic 3 A.I.R. Mod Manager...");
            Management.ProgramPaths.CreateMissingModManagerFolders();
            Management.MMSettingsManagement.LoadModManagerSettings();
            isDebugging();
            PraseArguments(args);
            isDeveloper = Arguments.dev_mode;
            var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
            if (exists)
            {
                Log.InfoFormat("Application Already Open! Adding Potential URL to Gamebanana 1-Click Install Handler...");
                GamebannaAPIHandler(args);
            }
            else StartApplication();
            Log.InfoFormat("Shuting Down!");
        }

        static void PraseArguments(string[] args)
        {
            Program.Log.InfoFormat("Prasing Launch Arguments...");
            if (args != null && args.Length > 0)
            {
                var prasedArgs = Parser.Default.ParseArguments<Options>(args);
                if (prasedArgs != null)
                {
                    prasedArgs.WithParsed<Options>(o => { Arguments = o; });
                }
                else Arguments = new Options();
            }
            else Arguments = new Options();
        }
        #endregion

        #region Startup Region

        static void GamebannaAPIHandler(string[] args)
        {
            if (Arguments.gamebanana_api != null)
            {
                Log.InfoFormat("1-Click Install Detected! (URL: {0})", Arguments?.gamebanana_api);
                int currentFileIndex = 0;
                bool fileCreated = false;
                while (!fileCreated)
                {
                    string path = Path.Combine(Management.ProgramPaths.Sonic3AIR_MM_GBRequestsFolder, $"gb_api{currentFileIndex}.txt");
                    Log.InfoFormat("Attempting to creating file at \"{0}\"...", path);
                    if (!File.Exists(path))
                    {
                        CreateFile(path, Arguments.gamebanana_api);
                        fileCreated = true;
                        Log.InfoFormat("File created at \"{0}\"!", path);
                    }
                    else
                    {
                        Log.InfoFormat("Unable to create file at \"{0}\", it already exists, trying again...", path);
                        currentFileIndex++;
                    }
                }

            }
            else Log.InfoFormat("No 1-Click Install Detected!");
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

        static void StartApplication()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Management.UserLanguage.ApplyLanguageResourcePath(Management.UserLanguage.CurrentLanguage);
            Management.ModManagement.CleanUpAPIRequests();

            if (Arguments?.gamebanana_api != null)
            {
                Log.InfoFormat("1-Click Install Detected! (URL: {0})", Arguments?.gamebanana_api);
                GamebanannaAPIHandler_Startup();
            }
            else
            {
                bool forcedAutoBoot = (Arguments?.auto_boot ?? false);
                if (forcedAutoBoot == true) ForcedAutoBootStartup();
                else StockStartup();

            }

        }

        static void ForcedAutoBootStartup()
        {
            // Start Auto-Boot
            AutoBootLoader(true);

        }

        static void StockStartup()
        {
            if (Management.MainDataModel.Settings.AutoLaunch) AutoBootLoader();
            else
            {
                var app = new App();
                app.DefaultStart();
            }
        }



        static void AutoBootLoader(bool isForced = false)
        {
            var app = new App();
            app.RunAutoBoot(isForced);
        }

        #endregion

        public class Options
        {
            [Option('g', "gamebanana_api", Required = false, HelpText = "Used with Gamebanna's 1 Click Install API")]
            public string gamebanana_api { get; set; }

            [Option('a', "auto_boot", Required = false, HelpText = "Launch's the Application in Auto Boot Mode (Ideal for Steam Big Picture)")]
            public bool auto_boot { get; set; } = false;

            [Option('d', "dev_mode", Required = false, Hidden = true, Default = false)]
            public bool dev_mode { get; set; } = false;
        }

        #region Logging

        static void StartLogging()
        {
            log4net.GlobalContext.Properties["MMVersion"] = Version;
            log4net.Config.XmlConfigurator.Configure();
            Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //ConsoleManager.Show();
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                if ((!isDebug && !allowDebugOutput))
                {
                    if (e.Exception.TargetSite != null && e.Exception.TargetSite.DeclaringType.Assembly == Assembly.GetExecutingAssembly())
                    {
                        if (Log != null) Log.ErrorFormat("[Exception Thrown] {0} {1}", RemoveNewLineChars(e.Exception.Message), RemoveNewLineChars(e.Exception.StackTrace));
                    }
                    else if (Management.MainDataModel.Settings.ShowFullDebugOutput)
                    {
                        if (Log != null) Log.ErrorFormat("[FULL] [Exception Thrown] {0} {1}", RemoveNewLineChars(e.Exception), RemoveNewLineChars(e.Exception.StackTrace));
                    }
                }

            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if ((!isDebug && !allowDebugOutput))
                {
                    if (e.ExceptionObject != null && e.ExceptionObject is Exception)
                    {
                        Exception ex = e.ExceptionObject as Exception;
                        if (ex.TargetSite != null && ex.TargetSite.DeclaringType.Assembly == Assembly.GetExecutingAssembly())
                        {
                            if (Log != null) Log.ErrorFormat("[Unhandled Exception Thrown] {0} {1}", RemoveNewLineChars(ex.Message), RemoveNewLineChars(ex.StackTrace));
                        }
                        else if (Management.MainDataModel.Settings.ShowFullDebugOutput)
                        {
                            if (Log != null) Log.ErrorFormat("[FULL] [Unhandled Exception Thrown] {0} {1}", RemoveNewLineChars(ex), RemoveNewLineChars(ex.StackTrace));
                        }
                    }
                }

            };
        }

        static string RemoveNewLineChars(Exception exception_to_search, string replacement_string = " ")
        {
            if (exception_to_search.Message != null) return System.Text.RegularExpressions.Regex.Replace(exception_to_search.Message, @"\t|\n|\r", replacement_string);
            return exception_to_search.ToString();
        }

        static string RemoveNewLineChars(string string_to_search, string replacement_string = " ")
        {
            if (string_to_search != null) return System.Text.RegularExpressions.Regex.Replace(string_to_search, @"\t|\n|\r", replacement_string);
            return "";
        }

        static void CleanUpLogsFolder()
        {
            if (Directory.Exists(Management.ProgramPaths.Sonic3AIR_MM_LogsFolder))
            {
                DirectoryInfo logsFolder = new DirectoryInfo(Management.ProgramPaths.Sonic3AIR_MM_LogsFolder);
                var fileList = logsFolder.GetFiles("*.log", SearchOption.AllDirectories).ToList();
                if (fileList.Count > 10)
                {
                    foreach (var file in fileList.OrderByDescending(file => file.CreationTime).Skip(10))
                    {
                        file.Delete();
                    }
                }

            }
        }

        static void EndLogging()
        {
            CleanUpLogsFolder();
            //ConsoleManager.Hide();
        }

        #endregion


    }
}
