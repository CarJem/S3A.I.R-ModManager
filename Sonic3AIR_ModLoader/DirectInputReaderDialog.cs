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
    public partial class DirectInputReaderDialog : Form
    {
        public JoystickUpdate? Result;
        public Joystick Input = null;
        private System.Timers.Timer timer1;
        private bool PollingInput = false;
        public DirectInputReaderDialog()
        {
            InitializeComponent();
            timer1 = new System.Timers.Timer();
            timer1.Elapsed += timer1_Tick;

        }


        public DialogResult ShowInputDialog()
        {
            timer1.Start();
            return this.ShowDialog();
        }

        private void DirectInputReaderDialog_Load(object sender, EventArgs e)
        {
            Input = DirectInputReader.GetDevice();

        }


        public void EndChecks(JoystickUpdate state)
        {
            var objInfo = Input.GetObjectInfoByOffset(state.RawOffset);
            var objProp = Input.GetObjectPropertiesById(objInfo.ObjectId);
            //var effectInfo = Input.GetEffectInfo(objInfo.ObjectType);
            Debug.Print(state.Value.ToString());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Input != null && !PollingInput)
            {
                PollingInput = true;
                Result = DirectInputReader.GetDeviceInput(Input);
                if (Result != null) EndChecks(Result.Value);
                PollingInput = false;
            }

        }
    }
}
