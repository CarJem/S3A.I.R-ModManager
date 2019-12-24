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
using Gma.System.MouseKeyHook.Implementation;
using System.Windows.Controls;
using Gma.System.MouseKeyHook;
using System.Runtime.InteropServices;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Sonic3AIR_ModManager
{
    public static class GameHandler
    {
        public static bool isGameRunning = false;
        public static Process CurrentGameProcess;
        private static ModManager Instance;
        public static void UpdateInstance(ref ModManager _Instance)
        {
            Instance = _Instance;
        }


        #region Sonic 3 A.I.R. Launcher

        public static bool TimeTravelSafetyNet()
        {
            VersionManagement.VersionReader.AIRVersionData fileData = VersionManagement.VersionReader.GetVersionData(Path.GetDirectoryName(ProgramPaths.Sonic3AIRPath), false);
            if (MainDataModel.S3AIRSettings != null && MainDataModel.S3AIRSettings.Version != null)
            {
                if (!(fileData.Version.CompareTo(MainDataModel.S3AIRSettings.Version) >= 0))
                {
                    string title = Program.LanguageResource.GetString("TimeTravelSafetyNet_Title");
                    string text = Program.LanguageResource.GetString("TimeTravelSafetyNet_OlderVersion");
                    DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes) return true;
                    else return false;
                }
                else return true;
            }
            else
            {
                string title = Program.LanguageResource.GetString("TimeTravelSafetyNet_Title");
                string text = Program.LanguageResource.GetString("TimeTravelSafetyNet_NullVersion");
                DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes) return true;
                else return false;
            }
        }

        public static void LaunchSonic3AIR()
        {
            bool IsGamePathSet = true;
            if (ProgramPaths.Sonic3AIRPath == null || ProgramPaths.Sonic3AIRPath == "")
            {
                IsGamePathSet = ProcessLauncher.UpdateSonic3AIRLocation();
            }
            if (IsGamePathSet)
            {
                if (TimeTravelSafetyNet() == true)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(GameHandler.RunSonic3AIR);
                    thread.Start();
                }
            }
            else
            {
                MessageBox.Show(Program.LanguageResource.GetString("AIRCanNotStartNoPath"));
            }
        }

        public static void RunSonic3AIR()
        {
            try
            {
                GameStartHandler();
                string filename = ProgramPaths.Sonic3AIRPath;
                var start = new ProcessStartInfo() { FileName = filename, WorkingDirectory = Path.GetDirectoryName(filename) };
                CurrentGameProcess = Process.Start(start);
                CurrentGameProcess.WaitForExit();
                GameEndHandler();
            }
            catch (Exception ex)
            {
                MessageBox.Show(UserLanguage.GetOutputString("UnableToStartS3AIR") + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.Message);
            }

        }

        public static void GameStartHandler()
        {
            isGameRunning = true;
            if (!MainDataModel.Settings.KeepOpenOnLaunch)
            {
                Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Instance.Hide();
                }));

            }
            else
            {
                Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    MainDataModel.UpdateInGameButtons(ref Instance);
                }));
            }

        }

        public static void GameEndHandler()
        {
            isGameRunning = false;
            if (!MainDataModel.Settings.KeepOpenOnQuit) Environment.Exit(0);
            else if (!MainDataModel.Settings.KeepOpenOnLaunch)
            {
                Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Instance.Show();
                }));
            }
            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                MainDataModel.UpdateInGameButtons(ref Instance);
                string filePath = MainDataModel.S3AIRSettings.FilePath;
                MainDataModel.S3AIRSettings = new AIR_API.Settings(new FileInfo(filePath));
                MainDataModel.UpdateAIRSettings(ref Instance);
            }));

        }

        #endregion

        #region Game Recording Player

        private static AIR_API.Settings GameRecordingSettings { get; set; }
        private static AIR_API.GameConfig GameRecordingCurrentGameConfig { get; set; }
        private static List<string> Temporary_Settings { get; set; }
        private static List<string> CurrentSettings { get; set; }

        public static void LaunchGameRecording(string file, string viewer_exe)
        {
            try
            {
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
                RecordingStartHandler(file, viewer_exe);
                var start = new ProcessStartInfo() { FileName = filename, WorkingDirectory = Path.GetDirectoryName(filename) };
                CurrentGameProcess = Process.Start(start);
                CurrentGameProcess.WaitForExit();
                RecordingEndHandler(viewer_exe);
            }
            catch (Exception ex)
            {
                MessageBox.Show(UserLanguage.GetOutputString("UnableToStartS3AIRRecordingViewer") + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.Message);
            }

        }

        public static void RecordingStartHandler(string file, string viewer_exe)
        {
            isGameRunning = true;
            BackupSettings(file, viewer_exe);

            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                MainDataModel.UpdateInGameButtons(ref Instance);
            }));
        }

        public static void RecordingEndHandler(string viewer_exe)
        {
            RestoreSettings(viewer_exe);
            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                MainDataModel.UpdateAIRSettings(ref Instance);
                MainDataModel.RetriveLaunchOptions(ref Instance);
            }));
            isGameRunning = false;
            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                MainDataModel.UpdateInGameButtons(ref Instance);
            }));
        }

        private static void BackupSettings(string file, string viewer_exe)
        {
            string config_file = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.json");
            string config_file_bak = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.bak.json");

            if (File.Exists(config_file_bak)) File.Delete(config_file_bak);
            File.Copy(config_file, config_file_bak);

            string setting_file = Path.Combine(ProgramPaths.Sonic3AIRAppDataFolder, "settings.json");
            string setting_file_bak = Path.Combine(ProgramPaths.Sonic3AIRAppDataFolder, "settings.bak.json");

            if (File.Exists(setting_file_bak)) File.Delete(setting_file_bak);
            File.Copy(setting_file, setting_file_bak);

            GameRecordingCurrentGameConfig = new AIR_API.GameConfig(new FileInfo(config_file));
            GameRecordingSettings = new AIR_API.Settings(new FileInfo(setting_file));

            RecordingManagement.CopyRecordingToDestination(file, Path.GetDirectoryName(viewer_exe));

            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                GameRecordingSettings.FullscreenMode = (int)AIR_API.Settings.FullscreenType.Windowed;
                GameRecordingCurrentGameConfig.StartPhase = 3;
                GameRecordingCurrentGameConfig.GameRecording = 2;

                GameRecordingSettings.Save();
                GameRecordingCurrentGameConfig.Save();
            }));
        }

        private static void RestoreSettings(string viewer_exe)
        {
            string config_file = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.json");
            string config_file_bak = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.bak.json");

            File.Delete(config_file);
            File.Copy(config_file_bak, config_file);
            File.Delete(config_file_bak);

            string setting_file = Path.Combine(ProgramPaths.Sonic3AIRAppDataFolder, "settings.json");
            string setting_file_bak = Path.Combine(ProgramPaths.Sonic3AIRAppDataFolder, "settings.bak.json");

            File.Delete(setting_file);
            File.Copy(setting_file_bak, setting_file);
            File.Delete(setting_file_bak);

            RecordingManagement.DeletePlaybackRecording(Path.GetDirectoryName(viewer_exe));
        }

        #endregion

        #region Other Methods

        public static void ForceQuitSonic3AIR()
        {
            if (CurrentGameProcess != null && !CurrentGameProcess.HasExited)
            {
                string title = Program.LanguageResource.GetString("ForceQuitGameDialog_Title");
                string text = Program.LanguageResource.GetString("ForceQuitGameDialog_Text");
                DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    CurrentGameProcess.Kill();
                }
            }


        }

        #endregion

        public static class InGameContextMenu
        {
            #region Window Detector
            // The GetForegroundWindow function returns a handle to the foreground window
            // (the window  with which the user is currently working).
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();

            // The GetWindowThreadProcessId function retrieves the identifier of the thread
            // that created the specified window and, optionally, the identifier of the
            // process that created the window.
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
            private static bool IsAIRFocused()
            {
                IntPtr hwnd = GetForegroundWindow();

                // The foreground window can be NULL in certain circumstances, 
                // such as when a window is losing activation.
                if (hwnd == null)
                    return false;

                uint pid;
                GetWindowThreadProcessId(hwnd, out pid);

                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
                {
                    if (GameHandler.CurrentGameProcess == null) return false;
                    if (p.Id == GameHandler.CurrentGameProcess.Id && pid == GameHandler.CurrentGameProcess.Id)
                        return true;
                }

                return false;
            }

            class NativeMethods
            {
                // http://msdn.microsoft.com/en-us/library/ms633519(VS.85).aspx
                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

                // http://msdn.microsoft.com/en-us/library/a5ch4fda(VS.80).aspx
                [StructLayout(LayoutKind.Sequential)]
                public struct RECT
                {
                    public int Left;
                    public int Top;
                    public int Right;
                    public int Bottom;
                }
            }

            private static System.Windows.Rect GetWindowRect(IntPtr hWnd)
            {
                NativeMethods.RECT rectangle = new NativeMethods.RECT();
                NativeMethods.GetWindowRect(hWnd, ref rectangle);
                //Console.WriteLine("GetRect : {0},{1},{2},{3}", rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
                int x = rectangle.Left;
                int y = rectangle.Top;
                int width = rectangle.Right - rectangle.Left;
                int height = rectangle.Bottom - rectangle.Top;
                return new System.Windows.Rect(x, y, width, height);
            }


            #endregion

            public static IKeyboardMouseEvents m_GlobalHook;

            private static bool CurrentlySubscribed = false;

            public static void Subscribe()
            {
                if (CurrentlySubscribed == false)
                {
                    // Note: for the application hook, use the Hook.AppEvents() instead
                    m_GlobalHook = Hook.GlobalEvents();
                    m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
                    CurrentlySubscribed = true;
                }

            }

            private static Controls.InGameContextMenu cm { get; set; }

            public static void CreateContextMenu()
            {
                if (cm == null)
                {
                    cm = new Controls.InGameContextMenu();
                    cm.StaysOpen = false;
                }
                else
                {
                    if (cm.IsOpen) cm.IsOpen = false;
                    cm.Reload();
                }

            }

            public static void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
            {
                if (GameHandler.CurrentGameProcess != null && GameHandler.CurrentGameProcess.HasExited == false)
                {
                    System.Windows.Rect rectangle = (GameHandler.CurrentGameProcess != null ? GetWindowRect(GameHandler.CurrentGameProcess.MainWindowHandle) : new System.Windows.Rect());
                    System.Windows.Point MousePos = new System.Windows.Point(e.Location.X, e.Location.Y);

                    bool isWithin = (GameHandler.CurrentGameProcess != null ? isWithinControl(rectangle, MousePos) : false);
                    //Console.WriteLine("IsWithin : {0}", isWithin.ToString());
                    //Console.WriteLine("Rect : {0}", rectangle.ToString());
                    //Console.WriteLine("MousePos : {0}", MousePos.ToString());
                    if (isWithin)
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Right && IsAIRFocused() && GameHandler.isGameRunning)
                        {
                            CreateContextMenu();
                            cm.IsOpen = true;
                            cm.Focus();
                        }
                        else
                        {
                            bool isOutsideOfRange = !(cm != null ? IsWithinContextMenu(new System.Windows.Point(e.Location.X, e.Location.Y)) : true);
                            if (cm != null && cm.IsOpen && isOutsideOfRange)
                            {
                                cm.IsOpen = false;

                            }
                        }
                    }
                    else
                    {
                        if (cm != null && cm.IsOpen)
                        {
                            cm.IsOpen = false;
                        }
                    }
                }
                else
                {
                    if (cm != null && cm.IsOpen)
                    {
                        cm.IsOpen = false;
                    }
                }

            }

            #region IsWithin Control Methods

            public static bool IsWithinContextMenu(System.Windows.Point mousePosition)
            {
                List<bool> IsWithin = new List<bool>();
                foreach (System.Windows.Controls.Control item in Extensions.FindVisualChildren<System.Windows.Controls.Control>(cm))
                {
                    try
                    {
                        if (item is MenuItem)
                        {
                            if (item.IsVisible)
                            {
                                var mItem = item as MenuItem;
                                bool isMainItem = (mItem.Equals(cm.airGuidesItem) || mItem.Equals(cm.airPlacesButton) || mItem.Equals(cm.airModManagerPlacesButton) || mItem.Equals(cm.airTipsItem));
                                IsWithin.Add(mItem.IsHighlighted && !(isMainItem));
                                if (isMainItem)
                                {
                                    foreach (System.Windows.Controls.Control subItem in mItem.Items)
                                    {
                                        if (subItem.IsVisible)
                                        {
                                            var contextMenuPosition = subItem.PointToScreen(new System.Windows.Point(0, 0));
                                            var contextMenuLW = new System.Windows.Point(subItem.ActualWidth, subItem.ActualHeight);
                                            IsWithin.Add(isWithinControl(contextMenuPosition, mousePosition, contextMenuLW.X, contextMenuLW.Y));
                                        }
                                        else
                                        {
                                            IsWithin.Add(false);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                IsWithin.Add(false);
                            }
                        }
                        else
                        {
                            if (item.IsVisible)
                            {
                                var contextMenuPosition = item.PointToScreen(new System.Windows.Point(0, 0));
                                var contextMenuLW = new System.Windows.Point(item.ActualWidth, item.ActualHeight);
                                IsWithin.Add(isWithinControl(contextMenuPosition, mousePosition, contextMenuLW.X, contextMenuLW.Y));
                            }
                            else
                            {
                                IsWithin.Add(false);
                            }
                        }
                    }
                    catch
                    {

                    }

                }

                if (IsWithin.Contains(true)) return true;
                else return false;
            }

            public static bool isWithinControl(System.Windows.Point point, System.Windows.Point mousePosition, double width, double height)
            {
                System.Windows.Rect rect = new System.Windows.Rect(point.X, point.Y, width, height);
                if (!rect.Contains(mousePosition))
                {
                    // Outside screen bounds.
                    return false;
                }
                else return true;
            }

            public static bool isWithinControl(System.Windows.Rect rect, System.Windows.Point mousePosition)
            {
                if (!rect.Contains(mousePosition))
                {
                    // Outside screen bounds.
                    return false;
                }
                else return true;
            }

            #endregion

            public static void Unsubscribe()
            {
                if (CurrentlySubscribed == true)
                {
                    m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
                    m_GlobalHook.Dispose();
                    CurrentlySubscribed = false;
                }

            }
        }
    }
}
