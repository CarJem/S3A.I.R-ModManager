using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for AutoBootDialogV2.xaml
    /// </summary>
    public partial class AutoBootDialogV2 : Window
    {
        private System.Windows.Forms.Timer CountDown = new System.Windows.Forms.Timer();
        private int TimeLeft = (int)(Properties.Settings.Default.AutoLaunchDelay - 1);


        public AutoBootDialogV2()
        {
            InitializeComponent();

            AutoBootDialogV2 Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);

            buildDetails.Text = $"{Program.LanguageResource.GetString("ModManagerVersion")}: {Program.Version}" + Environment.NewLine + $"{Program.LanguageResource.GetString("AIRVersion")}: {GetAIRVersion()}";
            Random rnd = new Random();
            int knuckMode = (rnd.Next(1, 25));
            if (knuckMode == 3)
            {
                logo.Visibility = Visibility.Collapsed;
                logoAlt.Visibility = Visibility.Visible;
            }

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
            this.Dispatcher.BeginInvoke((Action)(() =>
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
                    this.DialogResult = true;
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

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            CountDown.Enabled = false;
            Program.AutoBootCanceled = true;
            this.DialogResult = true;
        }

        private void ForceStartButton_Click(object sender, RoutedEventArgs e)
        {
            TimeLeft = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
