using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Net.Http;
using System.Net;
using System.Security.Permissions;
using Microsoft.VisualBasic;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SharpCompress.Readers;
using SharpCompress.Writers;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Compressors;
using SharpCompress.IO;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Threading;
using System.Resources;

namespace Sonic3AIR_ModLoader
{
    public static class ExtraDialog
    {
        public static DialogResult ShowInputDialog(ref string input, string caption, string message = "")
        {
            System.Drawing.Size size = new System.Drawing.Size(500, 75);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = caption;
            inputBox.StartPosition = FormStartPosition.CenterScreen;

            System.Windows.Forms.Label label = new Label();
            label.Size = new System.Drawing.Size(size.Width - 10, 13);
            label.Location = new System.Drawing.Point(5, 5);
            label.Text = message;
            inputBox.Controls.Add(label);

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 25);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = Program.LanguageResource.GetString("Ok_Button");
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 49);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = Program.LanguageResource.GetString("Cancel_Button");
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 49);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;


            inputBox.ShowDialog();
            input = textBox.Text;
            return inputBox.DialogResult;
        }

        public static DialogResult ShowCustomMessageBox(string Text, string Caption)
        {
            System.Drawing.Size size = new System.Drawing.Size(700, 75);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = Caption;
            inputBox.StartPosition = FormStartPosition.CenterScreen;

            System.Windows.Forms.Label label = new Label();
            label.Size = new System.Drawing.Size(size.Width, size.Height );
            label.Location = new System.Drawing.Point(5, 5);
            label.Text = Text;
            inputBox.Controls.Add(label);

            Button yesButton = new Button();
            yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            yesButton.Name = "okButton";
            yesButton.Size = new System.Drawing.Size(75, 23);
            yesButton.Text = Program.LanguageResource.GetString("Ok_Button");
            yesButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 49);
            inputBox.Controls.Add(yesButton);

            Button noButton = new Button();
            noButton.DialogResult = System.Windows.Forms.DialogResult.No;
            noButton.Name = "noButton";
            noButton.Size = new System.Drawing.Size(75, 23);
            noButton.Text = Program.LanguageResource.GetString("No_Button");
            noButton.Location = new System.Drawing.Point(size.Width - 80, 49);
            inputBox.Controls.Add(noButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = Program.LanguageResource.GetString("Cancel_Button");
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 49);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = yesButton;
            inputBox.CancelButton = cancelButton;


            inputBox.ShowDialog();
            return inputBox.DialogResult;
        }

    }
}
