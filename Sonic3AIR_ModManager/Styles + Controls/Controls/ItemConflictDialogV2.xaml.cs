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

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for ItemConflictDialogV2.xaml
    /// </summary>
    public partial class ItemConflictDialogV2 : Window
    {
        System.Windows.Forms.DialogResult DialogResultForms { get; set; } = System.Windows.Forms.DialogResult.None;
        public ItemConflictDialogV2()
        {
            InitializeComponent();
        }

        public System.Windows.Forms.DialogResult ShowDialog(AIR_SDK.Mod NewMod, AIR_SDK.Mod ExistingMod)
        {
            string nL = Environment.NewLine;

            this.label1.Document.Blocks.Clear();
            this.label2.Document.Blocks.Clear();
            this.label3.Document.Blocks.Clear();



            Bold(ref label1);
            this.label1.AppendText("Existing Mod: " + nL);
            Bold(ref label1);
            this.label1.AppendText(nL + "Name: ");
            Normal(ref label1);
            this.label1.AppendText($"{(ExistingMod != null ? ExistingMod.Name : "N/A")}" + nL);
            Bold(ref label1);
            this.label1.AppendText("Author: ");
            Normal(ref label1);
            this.label1.AppendText($"{(ExistingMod != null ? ExistingMod.Author : "N/A")}" + nL);
            Bold(ref label1);
            this.label1.AppendText("Mod Version: ");
            Normal(ref label1);
            this.label1.AppendText($"{(ExistingMod != null ? ExistingMod.ModVersion : "N/A")}" + nL);
            Bold(ref label1);
            this.label1.AppendText("Game Version: ");
            Normal(ref label1);
            this.label1.AppendText($"{(ExistingMod != null ? ExistingMod.GameVersion : "N/A")}" + nL);

            Bold(ref label2);
            this.label2.AppendText("Conflicting Mod: " + nL);
            Bold(ref label2);
            this.label2.AppendText(nL + "Name: ");
            Normal(ref label2);
            this.label2.AppendText($"{(NewMod != null ? NewMod.Name : "N/A")}" + nL);
            Bold(ref label2);
            this.label2.AppendText("Author: ");
            Normal(ref label2);
            this.label2.AppendText($"{(NewMod != null ? NewMod.Author : "N/A")}" + nL);
            Bold(ref label2);
            this.label2.AppendText("Mod Version: ");
            Normal(ref label2);
            this.label2.AppendText($"{(NewMod != null ? NewMod.ModVersion : "N/A")}" + nL);
            Bold(ref label2);
            this.label2.AppendText("Game Version: ");
            Normal(ref label2);
            this.label2.AppendText($"{(NewMod != null ? NewMod.GameVersion : "N/A")}" + nL);


            string message = "How would you like to resolve this conflict?";

            this.label3.Document.Blocks.Add(new Paragraph(new Run(message)));
            base.ShowDialog();
            return this.DialogResultForms;


            void Bold(ref RichTextBox textBox)
            {
                textBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }

            void Normal(ref RichTextBox textBox)
            {
                textBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultForms = System.Windows.Forms.DialogResult.Cancel;
            this.DialogResult = true;
        }

        private void MakeCopyButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultForms = System.Windows.Forms.DialogResult.No;
            this.DialogResult = true;
        }

        private void OverwriteButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultForms = System.Windows.Forms.DialogResult.Yes;
            this.DialogResult = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResultForms = System.Windows.Forms.DialogResult.Abort;
        }
    }
}
