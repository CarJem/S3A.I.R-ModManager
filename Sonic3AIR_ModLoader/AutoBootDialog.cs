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
            Random rnd = new Random();
            int knuckMode = (rnd.Next(1, 25));
            if (knuckMode == 3)
            {
                pictureBox1.Image = Sonic3AIR_ModLoader.Properties.Resources.Sonic3KAIRLogoV4;
            }

            button1.Parent = pictureBox1;
            label1.Parent = pictureBox1;
            button1.BackColor = Color.FromArgb(64, 0, 0, 0);
            label1.BackColor = Color.FromArgb(64, 0, 0, 0);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(128, 0, 0, 0);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(85, 0, 0, 0);
            label1.Text = "  Initializing...";
            CountDown.Interval = 1000;
            CountDown.Tick += CountDown_Tick;
            CountDown.Enabled = true;

            }



        private void StartUpdater()
        {
            this.BeginInvoke((Action)(() =>
            {
                Program.UpdaterState = Updater.UpdateState.Running;
                label1.Text = "  Checking for Updates...";
                Updater updaterTask = new Updater();

            }));

        }

        private void UpdateTimeLeftLabel(bool startUp = false)
        {
            int time = TimeLeft;
            if (startUp) time = time + 1;
            TimeSpan result = TimeSpan.FromSeconds(time);
            string fromTimeString = result.ToString("mm':'ss");
            label1.Text = $"  Launching in: {fromTimeString}";
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
    }
}
