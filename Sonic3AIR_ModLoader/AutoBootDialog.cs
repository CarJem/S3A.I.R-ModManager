using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Sonic3AIR_ModLoader
{
    public partial class AutoBootDialog : Form
    {
        private Timer CountDown = new Timer();
        private int TimeLeft = (int)(Properties.Settings.Default.AutoLaunchDelay - 1);


        public AutoBootDialog()
        {
            InitializeComponent();

            AutoBootDialog Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);

            buildDetails.Parent = pictureBox1;

            buildDetails.Text = $"{Program.LanguageResource.GetString("ModManagerVersion")}: {Program.Version}" + Environment.NewLine + $"{Program.LanguageResource.GetString("AIRVersion")}: {GetAIRVersion()}";
            Random rnd = new Random();
            int knuckMode = (rnd.Next(1, 25));
            if (knuckMode == 3)
            {
                pictureBox1.Image = Sonic3AIR_ModLoader.Properties.Resources.Sonic3KAIRLogoV4;
            }
            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
            cancelButton.Parent = pictureBox1;
            forceStartButton.Parent = pictureBox1;
            buildDetails.BackColor = Color.FromArgb(64, 0, 0, 0);
            cancelButton.BackColor = Color.FromArgb(64, 0, 0, 0);
            forceStartButton.BackColor = Color.FromArgb(64, 0, 0, 0);

            label1.BackColor = Color.FromArgb(64, 0, 0, 0);
            label2.BackColor = Color.FromArgb(64, 0, 0, 0);
            cancelButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(128, 0, 0, 0);
            cancelButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(85, 0, 0, 0);
            forceStartButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(128, 0, 0, 0);
            forceStartButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(85, 0, 0, 0);
            label1.Text = $"  {Program.LanguageResource.GetString("AutoBoot_Initalizing")}";
            CountDown.Interval = 1000;
            CountDown.Tick += CountDown_Tick;
            CountDown.Enabled = true;

        }

        private string GetAIRVersion()
        {
            if (File.Exists(ProgramPaths.Sonic3AIRPath))
            {
                string metaDataFile = Directory.GetFiles(Path.GetDirectoryName(ProgramPaths.Sonic3AIRPath), "metadata.json", SearchOption.AllDirectories).FirstOrDefault();
                if (metaDataFile != null)
                {
                    try
                    {
                        var CurrentAIRVersion = new AIR_SDK.VersionMetadata(new FileInfo(metaDataFile));
                        return CurrentAIRVersion.VersionString;
                    }
                    catch
                    {
                        return "N/A";

                    }

                }
                else
                {
                    return "N/A";
                }
            }
            else
            {
                return "N/A";
            }
        }



        private void StartUpdater()
        {
            this.BeginInvoke((Action)(() =>
            {
                Program.UpdaterState = Updater.UpdateState.Running;
                label1.Text = $"  {Program.LanguageResource.GetString("AutoBoot_CheckingForUpdates")}";
                Updater updaterTask = new Updater();

            }));

        }

        private void UpdateTimeLeftLabel(bool startUp = false)
        {
            int time = TimeLeft;
            if (startUp) time = time + 1;
            TimeSpan result = TimeSpan.FromSeconds(time);
            string fromTimeString = result.ToString("mm':'ss");
            label1.Text = $" {Program.LanguageResource.GetString("AutoBoot_LaunchingIn")}: {fromTimeString}";
        }

        private void CountDown_Tick(object sender, EventArgs evt)
        {
            bool allowedToProcced = (Properties.Settings.Default.AutoUpdates ? Program.CheckedForUpdateOnStartup && Program.UpdaterState == Updater.UpdateState.Finished : true);
            if (allowedToProcced)
            {
                if (TimeLeft >= 1)
                {
                    UpdateTimeLeftLabel();
                    TimeLeft -= 1;
                }
                else
                {
                    CountDown.Enabled = false;
                    Close();
                }
            }
            else
            {
                if (Program.UpdaterState == Updater.UpdateState.NeverStarted) StartUpdater();
                else if (Program.UpdateResult != Updater.UpdateResult.Null && Program.CheckedForUpdateOnStartup && Program.UpdaterState == Updater.UpdateState.Finished)
                {
                    Program.LastUpdateResult = Program.UpdateResult;
                    Program.UpdateResult = Updater.UpdateResult.Null;
                }
            }
            

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            CountDown.Enabled = false;
            Program.AutoBootCanceled = true;
            Close();
        }

        private void ForceStartButton_Click(object sender, EventArgs e)
        {
            TimeLeft = 0;
        }
    }
}
