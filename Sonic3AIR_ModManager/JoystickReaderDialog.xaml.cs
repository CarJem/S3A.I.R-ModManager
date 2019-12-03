using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Sonic3AIR_ModManager
{
    public partial class JoystickReaderDialog : Window
    {
        private System.Timers.Timer timer1;
        private bool PollingInput = false;
        public string Result = null;
        private IntPtr Joystick;
        private bool Allowed = true;
        public JoystickReaderDialog()
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            var instance = this;
            UserLanguage.ApplyLanguage(ref instance);
            timer1 = new System.Timers.Timer();
            timer1.Elapsed += timer1_Tick;
            this.reselectInputButton.IsEnabled = false;




        }


        public bool ShowInputDialog()
        {
            JoystickInputSelectorDialog dlg = new JoystickInputSelectorDialog();
            if (dlg.ShowDialog() == true)
            {
                Joystick = JoystickReader.GetJoystick();
                timer1.Start();
                return this.ShowDialog().Value;
            }
            else
            {
                return false;
            }

        }

        private void DirectInputReaderDialog_Load(object sender, EventArgs e)
        {

        }


        public void EndChecks(string value)
        {
            this.Dispatcher.BeginInvoke((MethodInvoker)delegate ()
            {
                testingForInputLabel.Text = testingForInputLabel.Tag + Environment.NewLine + value;
            });
            this.Dispatcher.BeginInvoke((MethodInvoker)delegate ()
            {
                okButton.IsEnabled = true;
            });
            this.Dispatcher.BeginInvoke((MethodInvoker)delegate ()
            {
                reselectInputButton.IsEnabled = true;
            });
            Allowed = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!PollingInput && Joystick != null && Allowed)
            {
                PollingInput = true;
                Result = JoystickReader.GetJoystickInput(Joystick);
                if (Result != null) EndChecks(Result);
                PollingInput = false;
            }

        }

        private void manualButton_Click(object sender, RoutedEventArgs e)
        {
            Allowed = true;
            this.reselectInputButton.IsEnabled = false;
            this.okButton.IsEnabled = false;
            testingForInputLabel.Text = Program.LanguageResource.GetString("WaitingForInputDialogLabel");
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
