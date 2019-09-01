using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIR_Protocol_Handler
{
    public partial class HandlerManagementUI : Form
    {
        string LoaderPath = "";
        public HandlerManagementUI(bool isInstalled, bool isRightPath, string exePath)
        {
            InitializeComponent();
            LoaderPath = exePath;
            AppendInfoText(isInstalled, isRightPath);
        }

        public void AppendInfoText(bool isInstalled, bool isRightPath)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;

            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.AppendText("Installed: ");

            AppendYesOrNo(isInstalled);
            if (isInstalled)
            {
                button4.Enabled = false;
            }
            else
            {
                button3.Enabled = false;
                button2.Enabled = false;
            }

            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.AppendText(Environment.NewLine + "Path Matches Loader: ");

            AppendYesOrNo(isRightPath);

            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.AppendText(Environment.NewLine + "What would you like to do?");
        }


        public void AppendYesOrNo(bool isTrue)
        {
            if (isTrue)
            {
                richTextBox1.SelectionColor = Color.Green;
                richTextBox1.AppendText("YES");
            }
            else
            {
                richTextBox1.SelectionColor = Color.Red;
                richTextBox1.AppendText("NO");
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Mod Manager Path: " + LoaderPath);
        }
    }
}
