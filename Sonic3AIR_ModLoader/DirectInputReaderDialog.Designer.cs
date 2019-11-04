namespace Sonic3AIR_ModLoader
{
    partial class DirectInputReaderDialog
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
            this.manualButton.Text = "Set Manually...";
            this.manualButton.UseVisualStyleBackColor = true;
            // 
            // testingForInputLabel
            // 
            this.testingForInputLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.testingForInputLabel.Location = new System.Drawing.Point(0, 0);
            this.testingForInputLabel.Name = "testingForInputLabel";
            this.testingForInputLabel.Size = new System.Drawing.Size(319, 94);
            this.testingForInputLabel.TabIndex = 2;
            this.testingForInputLabel.Text = "Trigger the Desired Input to Map to this Control.....";
            this.testingForInputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DirectInputReaderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 132);
            this.Controls.Add(this.testingForInputLabel);
            this.Controls.Add(this.manualButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DirectInputReaderDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Awaiting Input...";
            this.Load += new System.EventHandler(this.DirectInputReaderDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button manualButton;
        private System.Windows.Forms.Label testingForInputLabel;
    }
}