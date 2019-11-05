namespace Sonic3AIR_ModLoader
{
    partial class JoystickReaderDialog
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.manualButton = new System.Windows.Forms.Button();
            this.testingForInputLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(12, 97);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // manualButton
            // 
            this.manualButton.Location = new System.Drawing.Point(208, 97);
            this.manualButton.Name = "manualButton";
            this.manualButton.Size = new System.Drawing.Size(99, 23);
            this.manualButton.TabIndex = 1;
            this.manualButton.Text = "Reselect Input";
            this.manualButton.UseVisualStyleBackColor = true;
            this.manualButton.Click += new System.EventHandler(this.manualButton_Click);
            // 
            // testingForInputLabel
            // 
            this.testingForInputLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.testingForInputLabel.Location = new System.Drawing.Point(0, 0);
            this.testingForInputLabel.Name = "testingForInputLabel";
            this.testingForInputLabel.Size = new System.Drawing.Size(319, 94);
            this.testingForInputLabel.TabIndex = 2;
            this.testingForInputLabel.Tag = "Selected Input:";
            this.testingForInputLabel.Text = "Trigger the Desired Input to Map to this Control.....";
            this.testingForInputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(93, 97);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // DirectInputReaderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 132);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.testingForInputLabel);
            this.Controls.Add(this.manualButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DirectInputReaderDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.DirectInputReaderDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button manualButton;
        private System.Windows.Forms.Label testingForInputLabel;
        private System.Windows.Forms.Button okButton;
    }
}