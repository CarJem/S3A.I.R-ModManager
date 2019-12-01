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
                //TODO: Add Language Translations
                MessageBox.Show("Unable to Start Sonic 3 A.I.R. " + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.Message);
            }

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
