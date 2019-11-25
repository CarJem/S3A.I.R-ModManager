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
            var Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
        }

        public System.Windows.Forms.DialogResult ShowDialog(AIR_API.Mod NewMod, AIR_API.Mod ExistingMod)
        {
            string nL = Environment.NewLine;

            this.label1.Document.Blocks.Clear();
            this.label2.Document.Blocks.Clear();
            this.label3.Document.Blocks.Clear();


            string existingMod = Program.LanguageResource.GetString("ExistingModSection");
            string conflictingMod = Program.LanguageResource.GetString("ConflictingModSection");
            string name = Program.LanguageResource.GetString("NameSection");
            string author = Program.LanguageResource.GetString("AuthorSection");
            string modVersion = Program.LanguageResource.GetString("ModVersionSection");
            string gameVersion = Program.LanguageResource.GetString("GameVersionSection");

            Bold(ref label1);
            this.label1.AppendText($"{existingMod} " + nL);            
            Bold(ref label1);
            this.label1.AppendText(nL + $"{name} ");           
            Normal(ref label1);
            this.label1.AppendText($"{(ExistingMod != null ? ExistingMod.Name : "N/A")}" + nL);            
            Bold(ref label1);
            this.label1.AppendText($"{author} ");            
            Normal(ref label1);
            this.label1.AppendText($"{(ExistingMod != null ? ExistingMod.Author : "N/A")}" + nL);            
            Bold(ref label1);
            this.label1.AppendText($"{modVersion} ");            
            Normal(ref label1);
            this.label1.AppendText($"{(ExistingMod != null ? ExistingMod.ModVersion : "N/A")}" + nL);            
            Bold(ref label1);
            this.label1.AppendText($"{gameVersion} ");            
            Normal(ref label1);
            this.label1.AppendText($"{(ExistingMod != null ? ExistingMod.GameVersion : "N/A")}" + nL);


            Bold(ref label2);
            this.label2.AppendText($"{conflictingMod} " + nL);
            Bold(ref label2);
            this.label2.AppendText(nL + $"{name} ");
            Normal(ref label2);
            this.label2.AppendText($"{(NewMod != null ? NewMod.Name : "N/A")}" + nL);            
            Bold(ref label2);
            this.label2.AppendText($"{author} ");
            Normal(ref label2);
            this.label2.AppendText($"{(NewMod != null ? NewMod.Author : "N/A")}" + nL);          
            Bold(ref label2);
            this.label2.AppendText($"{modVersion} ");
            Normal(ref label2);
            this.label2.AppendText($"{(NewMod != null ? NewMod.ModVersion : "N/A")}" + nL);            
            Bold(ref label2);
            this.label2.AppendText($"{gameVersion} ");
            Normal(ref label2);
            this.label2.AppendText($"{(NewMod != null ? NewMod.GameVersion : "N/A")}" + nL);


            string message = Program.LanguageResource.GetString("ConflictDialogCaption");

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
            if (this.DialogResult != true)
            {
                DialogResultForms = System.Windows.Forms.DialogResult.Abort;
            }
        }
    }
}
