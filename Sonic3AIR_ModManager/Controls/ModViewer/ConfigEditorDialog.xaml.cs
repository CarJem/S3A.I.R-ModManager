using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using DialogResult = System.Windows.Forms.DialogResult;
using Binding = System.Windows.Data.Binding;

namespace Sonic3AIR_ModManager
{
    public partial class ConfigEditorDialog : Window
    {

        public ConfigEditorDialog()
        {
            InitializeComponent();
        }

        public ConfigEditorDialog(ref Window window)
        {
            InitializeComponent();
            this.Owner = window;
        }

        public bool? ShowConfigEditDialog(AIR_API.Mod mod)
        {
            EditorNameField.Text = mod.Name;
            EditorAuthorField.Text = mod.Author;
            EditorDescriptionField.Text = mod.Description;
            EditorURLField.Text = mod.URL;
            EditorGameVersionField.Text = mod.GameVersion;
            EditorModVersionField.Text = mod.ModVersion;
            return this.ShowDialog();
        }



        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}