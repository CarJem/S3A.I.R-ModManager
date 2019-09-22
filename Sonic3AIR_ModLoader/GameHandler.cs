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

namespace Sonic3AIR_ModLoader
{
    public class GameHandler
    {
        public static bool isGameRunning = false;

        public GameHandler()
        {

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
                MessageBox.Show("Path not set. Cannot start Sonic 3 A.I.R!");
            }
        }

        public static void RunSonic3AIR()
        {
            string filename = ProgramPaths.Sonic3AIRPath;
            var start = new ProcessStartInfo() { FileName = filename, WorkingDirectory = Path.GetDirectoryName(filename) };
            var process = Process.Start(start);
            GameStartHandler();
            process.WaitForExit();
            GameEndHandler();
        }


        public static void GameEndHandler()
        {
            isGameRunning = false;
            if (!Properties.Settings.Default.KeepOpenOnQuit) Application.Exit();
            else if (!Properties.Settings.Default.KeepOpenOnLaunch)
            {
                ModManager.Instance.BeginInvoke((Action)(() =>
                {
                    ModManager.Instance.Show();
                }));
            }
            ModManager.Instance.BeginInvoke((Action)(() =>
            {
                ModManager.Instance.UpdateInGameButtons();
            }));

        }

        public static void GameStartHandler()
        {
            isGameRunning = true;
            if (!Properties.Settings.Default.KeepOpenOnLaunch)
            {
                ModManager.Instance.BeginInvoke((Action)(() =>
                {
                    ModManager.Instance.Hide();
                }));

            }
            else
            {
                ModManager.Instance.BeginInvoke((Action)(() =>
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
                DialogResult result = MessageBox.Show("Sonic 3 A.I.R.'s File Path is Not Set, Would you like to set it now?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
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
                    Filter = "Executable File (*.exe)|*.exe",
                    Title = "Select Sonic 3 A.I.R. Executable..."
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
