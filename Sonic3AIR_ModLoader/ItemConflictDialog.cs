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
    public partial class ItemConflictDialog : Form
    {
        public ItemConflictDialog()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(AIR_SDK.Mod NewMod, AIR_SDK.Mod ExistingMod)
        {
            string nL = Environment.NewLine;

            this.label1.Clear();
            this.label2.Clear();
            this.label3.Clear();



            Bold(ref label1);
            this.label1.AppendText("Existing Mod:" + nL);
            Bold(ref label1);
            this.label1.AppendText("Name: ");
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
            this.label2.AppendText("Conflicting Mod:" + nL);
            Bold(ref label2);
            this.label2.AppendText("Name: ");
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

            this.label3.Text = message;
            return base.ShowDialog();


            void Bold(ref RichTextBox textBox)
            {
                textBox.SelectionFont = new Font(label1.Font, FontStyle.Bold);
            }

            void Normal(ref RichTextBox textBox)
            {
                textBox.SelectionFont = new Font(label1.Font, FontStyle.Regular);
            }
        }
    }
}
