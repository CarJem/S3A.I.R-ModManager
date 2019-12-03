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
    /// Interaction logic for AutoBootDialog.xaml
    /// </summary>
    public partial class AutoBootDialog : Window
    {
        private System.Windows.Forms.Timer CountDown = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer AnimationLoop = new System.Windows.Forms.Timer();
        private int TimeLeft = (int)(Properties.Settings.Default.AutoLaunchDelay - 1);


        private static Color TransparentSpecial = Color.FromArgb(Colors.Transparent.A, Colors.White.R, Colors.White.G, Colors.White.B);
        private double AnimationPostion = -2.0;
        private GradientStop WhitePoint = new GradientStop(Colors.White, 0);
        private GradientStop TransparentPoint = new GradientStop(TransparentSpecial, 0.5);
        private GradientStop WhitePoint2 = new GradientStop(Colors.White, 1);


        private GradientStop TransparentPoint2A = new GradientStop(TransparentSpecial, 0);
        private GradientStop WhitePoint3 = new GradientStop(Colors.White, 0.5);
        private GradientStop TransparentPoint2B = new GradientStop(TransparentSpecial, 1);
        private LinearGradientBrush BrushTest = new LinearGradientBrush();
        private LinearGradientBrush BrushTest2 = new LinearGradientBrush();

        public AutoBootDialog()
        {
            InitializeComponent();

            AutoBootDialog Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);

            buildDetails.Text = $"{Program.LanguageResource.GetString("ModManagerVersion")}: {Program.Version}" + Environment.NewLine + $"{Program.LanguageResource.GetString("AIRVersion")}: {GetAIRVersion()}";
            Random rnd = new Random();
            int knuckMode = (rnd.Next(1, 25));
            if (knuckMode == 3)
            {
                S3Logo.Visibility = Visibility.Collapsed;
                S3KLogo.Visibility = Visibility.Visible;
            }

            S3Logo.Opacity = 0;
            S3KLogo.Opacity = 0;

            label1.Text = $"  {Program.LanguageResource.GetString("AutoBoot_Initalizing")}";
            CountDown.Interval = 1000;
            AnimationLoop.Tick += AnimationLoop_Tick;
            AnimationLoop.Enabled = true;
            CountDown.Tick += CountDown_Tick;
            CountDown.Enabled = true;

        }

        bool AllowFadeIn = false;
        bool HasPlayedSFX = false;
        private Color CurrentTransparency = TransparentSpecial;

        private void AnimationLoop_Tick(object sender, EventArgs e)
        {   
            bool allowedToTick = (Properties.Settings.Default.AutoUpdates ? Program.CheckedForUpdateOnStartup && Program.AIRUpdaterState == Program.UpdateState.Finished && Program.MMUpdaterState == Program.UpdateState.Finished : true);
            if (allowedToTick)
            {

                if (!(AnimationPostion >= 1.0) && CurrentTransparency.A != (byte)255)
                {
                    if (AnimationPostion <= 0.0 && AllowFadeIn)
                    {
                        S3Logo.Opacity += 0.1;
                        S3KLogo.Opacity += 0.1;

                        if (!HasPlayedSFX)
                        {
                            System.IO.Stream str = Properties.Resources.EntrySFX;
                            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                            snd.Play();
                            HasPlayedSFX = true;
                        }
                    }

                    if (logoAltShadow.Opacity < 1.0) logoAltShadow.Opacity += 0.06;
                    if (logoShadow.Opacity < 1.0) logoShadow.Opacity += 0.06;
                    AnimationPostion += 0.1;
                    CurrentTransparency = TransparentSpecial;
                }


                if (AnimationPostion >= 1.0)
                {
                    if (CurrentTransparency.A != (byte)255 && logoAltShadow.Opacity != 0 && logoShadow.Opacity != 0)
                    {
                        var color = CurrentTransparency;
                        color.A = (byte)(color.A + 0x0F);
                        CurrentTransparency = color;
                        if (logoAltShadow.Opacity > 0.0) logoAltShadow.Opacity -= 0.05;
                        if (logoShadow.Opacity > 0.0) logoShadow.Opacity -= 0.05;
                    }
                    else
                    {
                        if (logoAltShadow.Opacity > 0.0) logoAltShadow.Opacity -= 0.05;
                        if (logoShadow.Opacity > 0.0) logoShadow.Opacity -= 0.05;
                        //AnimationPostion = -2.0;
                    }

                }
                else if (CurrentTransparency.A != (byte)0)
                {
                    var color = CurrentTransparency;
                    color.A = (byte)(color.A - 0x0F);
                    CurrentTransparency = color;
                }
                else
                {
                    AllowFadeIn = true;
                    CancelButton.IsEnabled = true;
                    ForceStartButton.IsEnabled = true;
                }

                TransparentPoint.Color = CurrentTransparency;
                TransparentPoint2A.Color = CurrentTransparency;
                TransparentPoint2B.Color = CurrentTransparency;

                BrushTest.StartPoint = new Point(-1, 0);
                BrushTest.EndPoint = new Point(1, 1);

                BrushTest2.StartPoint = new Point(-1, 0);
                BrushTest2.EndPoint = new Point(1, 1);

                WhitePoint.Offset = 0;
                TransparentPoint.Offset = AnimationPostion;
                WhitePoint2.Offset = 1;

                TransparentPoint2A.Offset = 0;
                WhitePoint3.Offset = AnimationPostion;
                TransparentPoint2B.Offset = 1;

                TransparentPoint.Offset = AnimationPostion;
                BrushTest.GradientStops = new GradientStopCollection(new List<GradientStop>() { WhitePoint, TransparentPoint, WhitePoint2 });
                BrushTest2.GradientStops = new GradientStopCollection(new List<GradientStop>() { TransparentPoint2A, WhitePoint3, TransparentPoint2B });

                logoAltShadow.OpacityMask = BrushTest2;
                logoAltShadow.InvalidateVisual();

                logoAlt.OpacityMask = BrushTest;
                logoAlt.InvalidateVisual();

                logoShadow.OpacityMask = BrushTest2;
                logoShadow.InvalidateVisual();

                logo.OpacityMask = BrushTest;
                logo.InvalidateVisual();




            }

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
                        var CurrentAIRVersion = new AIR_API.VersionMetadata(new FileInfo(metaDataFile));
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
                Program.AIRUpdaterState = Program.UpdateState.Running;
                Program.MMUpdaterState = Program.UpdateState.Running;
                label1.Text = $"  {Program.LanguageResource.GetString("AutoBoot_CheckingForUpdates")}";
                Updater updaterTask = new Updater();
                ModManagerUpdater modManagerUpdater = new ModManagerUpdater();
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
         
        private bool HasUpdatesFinalized()
        {
            if (Program.CheckedForUpdateOnStartup)
            {
                return Program.AIRUpdaterState == Program.UpdateState.Finished && Program.MMUpdaterState == Program.UpdateState.Finished;
            }
            else
            {
                if (Program.MMUpdateResults == Program.UpdateResult.Offline || Program.AIRUpdateResults == Program.UpdateResult.Offline)
                {
                    Program.CheckedForUpdateOnStartup = true;
                    return true;
                }
                else return false;
            }
        }

        private void CountDown_Tick(object sender, EventArgs evt)
        {
            bool allowedToProcced = (Properties.Settings.Default.AutoUpdates ? HasUpdatesFinalized() : true);
            if (allowedToProcced)
            {
                if (!CancelButton.IsEnabled) CancelButton.IsEnabled = true;
                if (!ForceStartButton.IsEnabled) ForceStartButton.IsEnabled = true;

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
                if (Program.AIRUpdaterState == Program.UpdateState.NeverStarted && Program.MMUpdaterState == Program.UpdateState.NeverStarted) StartUpdater();
                else if (Program.AIRUpdateResults != Program.UpdateResult.Null && Program.MMUpdateResults != Program.UpdateResult.Null && Program.CheckedForUpdateOnStartup && Program.AIRUpdaterState == Program.UpdateState.Finished && Program.MMUpdaterState == Program.UpdateState.Finished)
                {
                    Program.AIRLastUpdateResult = Program.AIRUpdateResults;
                    Program.MMLastUpdateResult = Program.MMUpdateResults;

                    Program.AIRUpdateResults = Program.UpdateResult.Null;
                    Program.MMUpdateResults = Program.UpdateResult.Null;
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
