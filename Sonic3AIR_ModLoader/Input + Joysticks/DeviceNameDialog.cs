using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sonic3AIR_ModManager
{
    public partial class DeviceNameDialog : Form
    {
        public DeviceNameDialog()
        {
            InitializeComponent();
            var instance = this;
            UserLanguage.ApplyLanguage(ref instance);
        }

        public DialogResult ShowDeviceNameDialog(ref string input, string caption, string message = "")
        {
            this.Text = caption;
            label1.Text = message;
            textBox1.Text = input;


            this.ShowDialog();
            input = textBox1.Text;
            return this.DialogResult;

        }

        private void detectControllerButton_Click(object sender, EventArgs e)
        {
            JoystickInputSelectorDialog dlg = new JoystickInputSelectorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dlg.ResultString;
            }
        }
    }
}
