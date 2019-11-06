using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sonic3AIR_ModLoader
{
    public partial class JoystickInputSelectorDialog : Form
    {
        public int Result = -1;
        public string ResultString = "";
        private bool NoDevicesFound = false;
        public JoystickInputSelectorDialog()
        {
            InitializeComponent();
            var instance = this;
            UserLanguage.ApplyLanguage(ref instance);
            RefreshInputsList();
        }


        private void RefreshInputsList()
        {
            listBox1.Enabled = true;
            NoDevicesFound = false;

            listBox1.DataSource = null;
            listBox1.Items.Clear();
            listBox1.DataSource = JoystickReader.GetJoysticks();
            if (JoystickReader.GetJoystickCount() == 0)
            {
                NoDevicesFound = true;
                string no_item_found = Program.LanguageResource.GetString("NoControllerFound");
                if (no_item_found == null) no_item_found = "";
                listBox1.Enabled = false;
                listBox1.DataSource = null;
                listBox1.Items.Add(no_item_found);
                selectButton.Enabled = false;
                listBox1.SelectedIndex = -1;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && !NoDevicesFound) selectButton.Enabled = true;
            else selectButton.Enabled = false;
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            this.Result = listBox1.SelectedIndex;
            this.ResultString = listBox1.SelectedItem.ToString();
            this.DialogResult = DialogResult.OK;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshInputsList();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
