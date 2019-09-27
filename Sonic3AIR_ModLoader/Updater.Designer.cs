namespace Sonic3AIR_ModLoader
{
    partial class Updater
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.yesButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.updateMessageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(12, 47);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(285, 362);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // yesButton
            // 
            this.yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yesButton.Location = new System.Drawing.Point(12, 415);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(184, 23);
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            // 
            // noButton
            // 
            this.noButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.noButton.Location = new System.Drawing.Point(202, 415);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(95, 23);
            this.noButton.TabIndex = 2;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            // 
            // updateMessageLabel
            // 
            this.updateMessageLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.updateMessageLabel.Location = new System.Drawing.Point(0, 0);
            this.updateMessageLabel.Margin = new System.Windows.Forms.Padding(3, 20, 3, 0);
            this.updateMessageLabel.Name = "updateMessageLabel";
            this.updateMessageLabel.Size = new System.Drawing.Size(309, 44);
            this.updateMessageLabel.TabIndex = 3;
            this.updateMessageLabel.Tag = "";
            this.updateMessageLabel.Text = "An Update is Avaliable! Would You Like to Update Now?";
            this.updateMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Updater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 450);
            this.Controls.Add(this.updateMessageLabel);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Updater";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updater";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.Button yesButton;
        public System.Windows.Forms.Button noButton;
        public System.Windows.Forms.Label updateMessageLabel;
    }
}