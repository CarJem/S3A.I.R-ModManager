namespace Sonic3AIR_ModLoader
{
    partial class KeyBindingDialog
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
            this.inputDeviceRadioButton1 = new System.Windows.Forms.RadioButton();
            this.keyBox = new System.Windows.Forms.ComboBox();
            this.keyLabel = new System.Windows.Forms.Label();
            this.inputDeviceRadioButton3 = new System.Windows.Forms.RadioButton();
            this.resultText = new System.Windows.Forms.TextBox();
            this.resultLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.getInputButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputDeviceRadioButton1
            // 
            this.inputDeviceRadioButton1.AutoSize = true;
            this.inputDeviceRadioButton1.Checked = true;
            this.inputDeviceRadioButton1.Location = new System.Drawing.Point(12, 12);
            this.inputDeviceRadioButton1.Name = "inputDeviceRadioButton1";
            this.inputDeviceRadioButton1.Size = new System.Drawing.Size(70, 17);
            this.inputDeviceRadioButton1.TabIndex = 0;
            this.inputDeviceRadioButton1.TabStop = true;
            this.inputDeviceRadioButton1.Text = "Keyboard";
            this.inputDeviceRadioButton1.UseVisualStyleBackColor = true;
            this.inputDeviceRadioButton1.CheckedChanged += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // keyBox
            // 
            this.keyBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.keyBox.FormattingEnabled = true;
            this.keyBox.Location = new System.Drawing.Point(58, 35);
            this.keyBox.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.keyBox.Name = "keyBox";
            this.keyBox.Size = new System.Drawing.Size(270, 21);
            this.keyBox.TabIndex = 2;
            this.keyBox.SelectedIndexChanged += new System.EventHandler(this.KeyBox_SelectedIndexChanged);
            // 
            // keyLabel
            // 
            this.keyLabel.AutoSize = true;
            this.keyLabel.Location = new System.Drawing.Point(9, 38);
            this.keyLabel.Name = "keyLabel";
            this.keyLabel.Size = new System.Drawing.Size(28, 13);
            this.keyLabel.TabIndex = 3;
            this.keyLabel.Text = "Key:";
            // 
            // inputDeviceRadioButton3
            // 
            this.inputDeviceRadioButton3.AutoSize = true;
            this.inputDeviceRadioButton3.Location = new System.Drawing.Point(12, 62);
            this.inputDeviceRadioButton3.Name = "inputDeviceRadioButton3";
            this.inputDeviceRadioButton3.Size = new System.Drawing.Size(54, 17);
            this.inputDeviceRadioButton3.TabIndex = 17;
            this.inputDeviceRadioButton3.Text = "Other:";
            this.inputDeviceRadioButton3.UseVisualStyleBackColor = true;
            this.inputDeviceRadioButton3.CheckedChanged += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // resultText
            // 
            this.resultText.Location = new System.Drawing.Point(58, 82);
            this.resultText.Name = "resultText";
            this.resultText.Size = new System.Drawing.Size(270, 20);
            this.resultText.TabIndex = 18;
            this.resultText.TextChanged += new System.EventHandler(this.resultText_TextChanged);
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.Location = new System.Drawing.Point(9, 85);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(40, 13);
            this.resultLabel.TabIndex = 16;
            this.resultLabel.Text = "Result:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(253, 126);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 19;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.cancelButton.Location = new System.Drawing.Point(172, 126);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 20;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // getInputButton
            // 
            this.getInputButton.Location = new System.Drawing.Point(12, 126);
            this.getInputButton.Name = "getInputButton";
            this.getInputButton.Size = new System.Drawing.Size(151, 23);
            this.getInputButton.TabIndex = 21;
            this.getInputButton.Text = "Detect Gamepad Input...\r\n";
            this.getInputButton.UseVisualStyleBackColor = true;
            this.getInputButton.Click += new System.EventHandler(this.getInputButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.getInputButton);
            this.groupBox1.Controls.Add(this.cancelButton);
            this.groupBox1.Controls.Add(this.okButton);
            this.groupBox1.Controls.Add(this.resultLabel);
            this.groupBox1.Controls.Add(this.resultText);
            this.groupBox1.Controls.Add(this.inputDeviceRadioButton3);
            this.groupBox1.Controls.Add(this.keyLabel);
            this.groupBox1.Controls.Add(this.keyBox);
            this.groupBox1.Controls.Add(this.inputDeviceRadioButton1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(337, 162);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // KeyBindingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 162);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyBindingDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Input...";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RadioButton inputDeviceRadioButton1;
        public System.Windows.Forms.ComboBox keyBox;
        public System.Windows.Forms.Label keyLabel;
        public System.Windows.Forms.RadioButton inputDeviceRadioButton3;
        public System.Windows.Forms.TextBox resultText;
        public System.Windows.Forms.Label resultLabel;
        public System.Windows.Forms.Button okButton;
        public System.Windows.Forms.Button cancelButton;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button getInputButton;
    }
}