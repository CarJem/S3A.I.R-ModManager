using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.DirectInput;
using System.Diagnostics;

namespace Sonic3AIR_ModLoader
{
    public partial class JoystickReaderDialog : Form
    {
        private System.Timers.Timer timer1;
        private bool PollingInput = false;
        public string Result = null;
        private IntPtr Joystick;
        private bool Allowed = true;
        public JoystickReaderDialog()
        {
            InitializeComponent();
            var instance = this;
            UserLanguage.ApplyLanguage(ref instance);
            timer1 = new System.Timers.Timer();
            timer1.Elapsed += timer1_Tick;
            this.reselectInputButton.Enabled = false;




        }


        public DialogResult ShowInputDialog()
        {
            JoystickInputSelectorDialog dlg = new JoystickInputSelectorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Joystick = JoystickReader.GetJoystick();
                timer1.Start();
                return this.ShowDialog();
            }
            else
            {
                return DialogResult.Cancel;
            }

        }

        private void DirectInputReaderDialog_Load(object sender, EventArgs e)
        {

        }


        public void EndChecks(string value)
        {
            this.testingForInputLabel.BeginInvoke((MethodInvoker)delegate ()
            {
                testingForInputLabel.Text = testingForInputLabel.Tag + Environment.NewLine + value;
            });
            this.okButton.BeginInvoke((MethodInvoker)delegate ()
            {
                okButton.Enabled = true;
            });
            this.reselectInputButton.BeginInvoke((MethodInvoker)delegate ()
            {
                reselectInputButton.Enabled = true;
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

        private void manualButton_Click(object sender, EventArgs e)
        {
            Allowed = true;
            this.reselectInputButton.Enabled = false;
            this.okButton.Enabled = false;
            testingForInputLabel.Text = Program.LanguageResource.GetString("WaitingForInputDialogLabel");
        }
    }
}
