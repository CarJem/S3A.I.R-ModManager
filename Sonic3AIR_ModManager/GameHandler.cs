using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace Sonic3AIR_ModManager
{
    public class GameHandler
    {
        public static bool isGameRunning = false;
        public static Process CurrentGameProcess;

        private static AIR_API.GameConfig GameRecordingCurrentGameConfig { get; set; }
        private static AIR_API.Settings GameRecordingCurrentSettings { get; set; }
        private static AIR_API.Settings GameRecordingLastSettings { get; set; }

        public GameHandler()
        {

        }

        public static void ForceQuitSonic3AIR()
        {
            // TODO: Add Warning Dialog
            if (CurrentGameProcess != null && !CurrentGameProcess.HasExited) CurrentGameProcess.Kill();
        }

        public static void LaunchSonic3AIR()
        {
            bool IsGamePathSet = true;
            if (ProgramPaths.Sonic3AIRPath == null || ProgramPaths.Sonic3AIRPath == "")
            {
                IsGamePathSet = UpdateSonic3AIRLocation();
            }
            if (IsGamePathSet)
            {
                System.Threading.Thread thread = new System.Threading.Thread(GameHandler.RunSonic3AIR);
                thread.Start();
            }
            else
            {
                MessageBox.Show(Program.LanguageResource.GetString("AIRCanNotStartNoPath"));
            }
        }


        public static void LaunchGameRecording(string file, string viewer_exe)
        {
            try
            {
                string config_file = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.json");
                GameRecordingLastSettings = ModManager.S3AIRSettings;
                GameRecordingCurrentGameConfig = new AIR_API.GameConfig(new FileInfo(config_file));
                GameRecordingCurrentSettings = new AIR_API.Settings(new FileInfo(ProgramPaths.Sonic3AIRSettingsFile));
                System.Threading.Thread thread = new System.Threading.Thread(() => GameHandler.RunGameRecordingViewer(file, viewer_exe));
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void RunGameRecordingViewer(string file, string viewer_exe)
        {
            try
            {
                string filename = viewer_exe;
                var start = new ProcessStartInfo() { FileName = filename, WorkingDirectory = Path.GetDirectoryName(filename) };
                if (RecordingPreStartHandler(file, Path.GetDirectoryName(viewer_exe)) == true)
                {
                    CurrentGameProcess = Process.Start(start);
                    RecordingStartHandler();
                    CurrentGameProcess.WaitForExit();
                    RecordingEndHandler(viewer_exe);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(UserLanguage.GetOutputString("UnableToStartS3AIRRecordingViewer") + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.Message);
            }

        }

        public static void RunSonic3AIR()
        {
            try
            {
                string filename = ProgramPaths.Sonic3AIRPath;
                var start = new ProcessStartInfo() { FileName = filename, WorkingDirectory = Path.GetDirectoryName(filename) };
                CurrentGameProcess = Process.Start(start);
                GameStartHandler();
                CurrentGameProcess.WaitForExit();
                GameEndHandler();
            }
            catch (Exception ex)
            {
                MessageBox.Show(UserLanguage.GetOutputString("UnableToStartS3AIR") + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.Message);
            }

        }

        private static int LastFullscreen = (int)AIR_API.Settings.FullscreenType.ExclusiveFS;
        private static int LastStartPhase = 0;
        private static int LastGameRecording = -1;



        public static bool RecordingPreStartHandler(string file, string exe_directory)
        {
            isGameRunning = true;
            FileManagement.CopyRecordingToDestination(file, exe_directory);
            if (FileManagement.BackupEntireGame() == true)
            {
                ModManager.Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    LastFullscreen = GameRecordingCurrentSettings.Fullscreen;
                    LastStartPhase = GameRecordingCurrentGameConfig.StartPhase ?? 0;
                    LastGameRecording = GameRecordingCurrentGameConfig.GameRecording ?? -1;
                    GameRecordingCurrentSettings.Fullscreen = (int)AIR_API.Settings.FullscreenType.Windowed;
                    GameRecordingCurrentGameConfig.StartPhase = 3;
                    GameRecordingCurrentGameConfig.GameRecording = 2;

                    GameRecordingCurrentSettings.Save();
                    GameRecordingCurrentGameConfig.Save();
                }));
                return true;
            }
            else return false;

        }

        public static void RecordingPostEndHandler(string viewer_exe)
        {
            FileManagement.DeletePlaybackRecording(Path.GetDirectoryName(viewer_exe));
            ModManager.Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                
                GameRecordingCurrentGameConfig.StartPhase = LastStartPhase;
                GameRecordingCurrentGameConfig.GameRecording = LastGameRecording;

                GameRecordingCurrentGameConfig.Save();

                FileManagement.RestoreEntireGame();

                ModManager.Instance.UpdateAIRSettings();
                ModManager.Instance.RetriveLaunchOptions();
            }));
        }

        public static void RecordingEndHandler(string viewer_exe)
        {
            RecordingPostEndHandler(viewer_exe);
            isGameRunning = false;
            ModManager.Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                ModManager.Instance.UpdateInGameButtons();
            }));


        }

        public static void RecordingStartHandler()
        {
            ModManager.Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                ModManager.Instance.UpdateInGameButtons();
            }));
        }


        public static void GameEndHandler()
        {
            isGameRunning = false;
            if (!Properties.Settings.Default.KeepOpenOnQuit) Environment.Exit(0);
            else if (!Properties.Settings.Default.KeepOpenOnLaunch)
            {
                ModManager.Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ModManager.Instance.Show();
                }));
            }
            ModManager.Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                ModManager.Instance.UpdateInGameButtons();
            }));

        }

        public static void GameStartHandler()
        {
            isGameRunning = true;
            if (!Properties.Settings.Default.KeepOpenOnLaunch)
            {
                ModManager.Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ModManager.Instance.Hide();
                }));

            }
            else
            {
                ModManager.Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ModManager.Instance.UpdateInGameButtons();
                }));
            }
        }

        public static bool UpdateSonic3AIRLocation(bool intended = false)
        {
            if (intended)
            {
                return LocationDialog();
            }
            else
            {
                DialogResult result = MessageBox.Show(Program.LanguageResource.GetString("MissingAIRSetNowAlert"), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    return LocationDialog();
                }
                else return false;
            }



            bool LocationDialog()
            {
                OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Filter = $"{Program.LanguageResource.GetString("EXEFileDialogFilter")} (*.exe)|*.exe",
                    Title = Program.LanguageResource.GetString("EXEFileDialogTitle")
                };
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    ProgramPaths.Sonic3AIRPath = fileDialog.FileName;
                    return true;
                }
                else return false;
            }

        }
    }
}
