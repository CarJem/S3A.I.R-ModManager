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

namespace Sonic3AIR_ModManager
{
    public partial class KeybindingsListDialogV2 : Window
    {
        public List<string> KeybindList;
        public KeybindingsListDialogV2(List<string> _keybindList)
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
            KeybindList = _keybindList;
            keybindsList.ItemsSource = KeybindList;
        }

        private KeybindingsListDialogV2 Instance;

        public KeybindingsListDialogV2()
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            KeyBindingDialogV2 kb = new KeyBindingDialogV2();
            string newKeybind = kb.ShowInputDialog("NONE");
            if (newKeybind != "NONE")
            {
                KeybindList.Add(newKeybind);
                RefreshDataSource();
            }

        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (keybindsList.SelectedItem != null)
            {
                DialogResult result = System.Windows.Forms.MessageBox.Show($"{Program.LanguageResource.GetString("DeleteKeybindVerification1")} [ {keybindsList.SelectedItem.ToString()} ] {Program.LanguageResource.GetString("DeleteKeybindVerification2")}", Program.LanguageResource.GetString("DeleteKeybindTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    int index = keybindsList.SelectedIndex;
                    KeybindList.RemoveAt(index);
                    RefreshDataSource();
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (keybindsList.SelectedItem != null)
            {
                int index = KeybindList.IndexOf(keybindsList.SelectedItem as string);
                KeyBindingDialogV2 kb = new KeyBindingDialogV2();
                KeybindList[index] = kb.ShowInputDialog(KeybindList[index]);
                RefreshDataSource();
            }
        }

        private void RefreshDataSource()
        {
            keybindsList.ItemsSource = null;
            keybindsList.ItemsSource = KeybindList;
        }

        private void KeybindsList_SelectedValueChanged(object sender, SelectionChangedEventArgs e)
        {
            if (keybindsList.SelectedItem != null)
            {
                removeButton.IsEnabled = true;
                editButton.IsEnabled = true;
            }
            else
            {
                removeButton.IsEnabled = false;
                editButton.IsEnabled = false;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
