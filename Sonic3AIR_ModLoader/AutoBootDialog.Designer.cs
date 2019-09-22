namespace Sonic3AIR_ModLoader
{
    partial class AutoBootDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoBootDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buildDetails = new System.Windows.Forms.Label();
            this.forceStartButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(0, 375);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(737, 43);
            this.label1.TabIndex = 0;
            this.label1.Tag = "";
            this.label1.Text = "   Launching in: 0:05";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.cancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.Color.Yellow;
            this.cancelButton.Location = new System.Drawing.Point(843, 375);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(0);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(77, 43);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseMnemonic = false;
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.Button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(224)))));
            this.pictureBox1.BackgroundImage = global::Sonic3AIR_ModLoader.Properties.Resources.Sonic3AIRBackground;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Sonic3AIR_ModLoader.Properties.Resources.Sonic3AIRLogov4;
            this.pictureBox1.ImageLocation = "";
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(920, 460);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // buildDetails
            // 
            this.buildDetails.BackColor = System.Drawing.Color.Transparent;
            this.buildDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buildDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildDetails.ForeColor = System.Drawing.Color.Yellow;
            this.buildDetails.Location = new System.Drawing.Point(0, 418);
            this.buildDetails.Margin = new System.Windows.Forms.Padding(0);
            this.buildDetails.Name = "buildDetails";
            this.buildDetails.Size = new System.Drawing.Size(920, 42);
            this.buildDetails.TabIndex = 5;
            this.buildDetails.Text = "Mod Manager Version: {n}\r\nA.I.R. Version: {n}\r\n";
            this.buildDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // forceStartButton
            // 
            this.forceStartButton.BackColor = System.Drawing.Color.Transparent;
            this.forceStartButton.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.forceStartButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.forceStartButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.forceStartButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.forceStartButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.forceStartButton.ForeColor = System.Drawing.Color.Yellow;
            this.forceStartButton.Location = new System.Drawing.Point(737, 375);
            this.forceStartButton.Margin = new System.Windows.Forms.Padding(0);
            this.forceStartButton.Name = "forceStartButton";
            this.forceStartButton.Size = new System.Drawing.Size(96, 43);
            this.forceStartButton.TabIndex = 7;
            this.forceStartButton.Text = "Force Start";
            this.forceStartButton.UseMnemonic = false;
            this.forceStartButton.UseVisualStyleBackColor = false;
            this.forceStartButton.Click += new System.EventHandler(this.ForceStartButton_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(833, 375);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 43);
            this.label2.TabIndex = 8;
            this.label2.Tag = "";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AutoBootDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(920, 460);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.forceStartButton);
            this.Controls.Add(this.buildDetails);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoBootDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label buildDetails;
        public System.Windows.Forms.Button forceStartButton;
        private System.Windows.Forms.Label label2;
    }
}