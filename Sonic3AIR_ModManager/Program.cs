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
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public static ResourceManager LanguageResource { get { return UserLanguage.CurrentResource; } set { UserLanguage.CurrentResource = value; } }

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
            NeverStarted,
        }

        public static UpdateState AIRUpdaterState { get; set; } = UpdateState.NeverStarted;
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
            StartLogging();
            RealMain(args);
            EndLogging();
        }

        static void RealMain(string[] args)
        {
            Log.InfoFormat("Starting Sonic 3 A.I.R. Mod Manager...");
            MMSettingsManagement.LoadModManagerSettings();
            StartDiscord();
            try
            {
                ProgramPaths.CreateMissingModManagerFolders();
                isDebugging();
                Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => { Arguments = o; });
                isDeveloper = Arguments.dev_mode;
                var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
                if (exists)
                {
                    Log.InfoFormat("Application Already Open! Adding Potential URL to Gamebanana 1-Click Install Handler...");
                    GamebannaAPIHandler(args);
                }
                else StartApplication(args);

            }
            catch
            {
                //TODO : Add Proper Catch Statement
            }
            EndDiscord();
            Log.InfoFormat("Shuting Down!");
        }
        #endregion

        #region Discord Threads

        static void StartDiscord()
        {
            Thread thread = new Thread(() => DiscordRP.InitDiscord());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        static void EndDiscord()
        {
            Thread thread = new Thread(() => DiscordRP.DisposeDiscord());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
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
                    string path = Path.Combine(ProgramPaths.Sonic3AIR_MM_GBRequestsFolder, $"gb_api{currentFileIndex}.txt");
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

        static void StartApplication(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            UserLanguage.ApplyLanguageResourcePath(UserLanguage.CurrentLanguage);
            FileManagement.CleanUpAPIRequests();

            if (Arguments?.gamebanana_api != null)
            {
                Log.InfoFormat("1-Click Install Detected! (URL: {0})", Arguments?.gamebanana_api);
                GamebanannaAPIHandler_Startup();
            }
            else
            {
                if (Arguments?.auto_boot == true) ForcedAutoBootStartup();
                else StockStartup();

            }

        }

        static void ForcedAutoBootStartup()
        {
            // Save Original Values
            var autoLaunchOld = MainDataModel.Settings.AutoLaunch;
            var preStartOld = MainDataModel.Settings.KeepOpenOnLaunch;
            var postCloseOld = MainDataModel.Settings.KeepOpenOnQuit;
            var autoLaunchDelayOld = MainDataModel.Settings.AutoLaunchDelay;

            // Start Auto-Boot
            AutoBootLoader(true);

        }

        static void StockStartup()
        {
            if (MainDataModel.Settings.AutoLaunch) AutoBootLoader();
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

        public static void PrintOutput(string output, int type = 0)
        {
            switch (type)
            {
                case 0:
                    Log.InfoFormat(output);
                    break;
                case 1:
                    Log.ErrorFormat(output);
                    break;
                case 2:
                    Log.WarnFormat(output);
                    break;
                case 3:
                    Log.DebugFormat(output);
                    break;
            }
        }

        static void StartLogging()
        {
            //ConsoleManager.Show();
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                if ((!isDebug && !allowDebugOutput))
                {
                    if (e.Exception.TargetSite != null && e.Exception.TargetSite.DeclaringType.Assembly == Assembly.GetExecutingAssembly())
                    {
                        Log.ErrorFormat("[Exception Thrown] {0} {1}", RemoveNewLineChars(e.Exception.Message), RemoveNewLineChars(e.Exception.StackTrace));
                    }
                    else if (MainDataModel.Settings.ShowFullDebugOutput)
                    {
                        Log.ErrorFormat("[FULL] [Exception Thrown] {0} {1}", RemoveNewLineChars(e.Exception), RemoveNewLineChars(e.Exception.StackTrace));
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
                            Log.ErrorFormat("[Unhandled Exception Thrown] {0} {1}", RemoveNewLineChars(ex.Message), RemoveNewLineChars(ex.StackTrace));
                        }
                        else if (MainDataModel.Settings.ShowFullDebugOutput)
                        {
                            Log.ErrorFormat("[FULL] [Unhandled Exception Thrown] {0} {1}", RemoveNewLineChars(ex), RemoveNewLineChars(ex.StackTrace));
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
            if (Directory.Exists(ProgramPaths.Sonic3AIR_MM_LogsFolder))
            {
                string app_log_filepath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "app.log");

                string log_filename = string.Format("S3AIR_MM_{0}_{1}.log", string.Format("[{0}]", GetVersionString()), DateTime.Now.ToString("[M-dd-yyyy]_[hh-mm-ss]"));
                string log_filepath = Path.Combine(ProgramPaths.Sonic3AIR_MM_LogsFolder, log_filename);

                if (File.Exists(app_log_filepath)) File.Copy(app_log_filepath, log_filepath);
                DirectoryInfo logsFolder = new DirectoryInfo(ProgramPaths.Sonic3AIR_MM_LogsFolder);
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
