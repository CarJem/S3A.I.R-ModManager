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
    public partial class KeybindingsListDialog : Form
    {
        public List<string> KeybindList;
        public KeybindingsListDialog(List<string> _keybindList)
        {
            InitializeComponent();
            Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
            KeybindList = _keybindList;
            keybindsList.DataSource = KeybindList;
        }

        private KeybindingsListDialog Instance;

        public KeybindingsListDialog()
        {
            InitializeComponent();
            Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            KeyBindingDialog kb = new KeyBindingDialog();
            string newKeybind = kb.ShowInputDialog("NONE");
            if (newKeybind != "NONE")
            {
                KeybindList.Add(newKeybind);
                RefreshDataSource();
            }

        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (keybindsList.SelectedItem != null)
            {
                DialogResult result = MessageBox.Show($"{Program.LanguageResource.GetString("DeleteKeybindVerification1")} [ {keybindsList.SelectedItem.ToString()} ] {Program.LanguageResource.GetString("DeleteKeybindVerification2")}", Program.LanguageResource.GetString("DeleteKeybindTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int index = keybindsList.SelectedIndex;
                    KeybindList.RemoveAt(index);
                    RefreshDataSource();
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (keybindsList.SelectedItem != null)
            {
                int index = KeybindList.IndexOf(keybindsList.SelectedItem as string);
                KeyBindingDialog kb = new KeyBindingDialog();
                KeybindList[index] = kb.ShowInputDialog(KeybindList[index]);
                RefreshDataSource();
            }
        }

        private void RefreshDataSource()
        {
            keybindsList.DataSource = null;
            keybindsList.DataSource = KeybindList;
        }

            private void KeybindsList_SelectedValueChanged(object sender, EventArgs e)
            {
                if (keybindsList.SelectedItem != null)
                {
                    removeButton.Enabled = true;
                    editButton.Enabled = true;
                }
                else
                {
                    removeButton.Enabled = false;
                    editButton.Enabled = false;
                }
            }
    }

}
