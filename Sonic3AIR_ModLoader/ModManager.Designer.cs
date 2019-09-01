namespace Sonic3AIR_ModLoader
{
    partial class ModManager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModManager));
            this.addMods = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.modContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openModFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAndLoadButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.modPage = new System.Windows.Forms.TabPage();
            this.modPanel = new System.Windows.Forms.Panel();
            this.downloadButtonTest = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.modInfoTextBox = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.modTechnicalNameLabel = new System.Windows.Forms.Label();
            this.modNameLabel = new System.Windows.Forms.Label();
            this.ModList = new System.Windows.Forms.CheckedListBox();
            this.moveDownButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.moveUpButton = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.deleteRecordingButton = new System.Windows.Forms.Button();
            this.copyRecordingFilePath = new System.Windows.Forms.Button();
            this.gameRecordingList = new System.Windows.Forms.ListBox();
            this.refreshDebugButton = new System.Windows.Forms.Button();
            this.uploadButton = new System.Windows.Forms.Button();
            this.openRecordingButton = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.showLogFileButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.openModdingTemplatesFolder = new System.Windows.Forms.Button();
            this.openModDocumentationButton = new System.Windows.Forms.Button();
            this.openSampleModsFolderButton = new System.Windows.Forms.Button();
            this.openUserManualButton = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.autoLaunchDelayLabel = new System.Windows.Forms.Label();
            this.autoLaunchDelayUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.keepOpenOnQuitCheckBox = new System.Windows.Forms.CheckBox();
            this.keepLoaderOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.autoRunCheckbox = new System.Windows.Forms.CheckBox();
            this.updateSonic3AIRPathButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.carJemTwitterButton = new System.Windows.Forms.Button();
            this.eukaTwitterButton = new System.Windows.Forms.Button();
            this.gamebannaButton = new System.Windows.Forms.Button();
            this.s3AIRWebsiteButton = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.openModsFolder = new System.Windows.Forms.Button();
            this.changeRomPathButton = new System.Windows.Forms.Button();
            this.openConfigFile = new System.Windows.Forms.Button();
            this.openAppDataFolderButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.romPathBox = new System.Windows.Forms.TextBox();
            this.openEXEFolderButton = new System.Windows.Forms.Button();
            this.failSafeModeCheckbox = new System.Windows.Forms.CheckBox();
            this.fixGlitchesCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.sonic3AIRPathBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.modStackOffRadioButton = new System.Windows.Forms.RadioButton();
            this.modStackOnRadioButton = new System.Windows.Forms.RadioButton();
            this.saveButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.moreModOptionsButton = new System.Windows.Forms.Button();
            this.moreModOptionsMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gamebannaURLHandlerOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modContextMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.modPage.SuspendLayout();
            this.modPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.autoLaunchDelayUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.moreModOptionsMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // addMods
            // 
            this.addMods.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addMods.Location = new System.Drawing.Point(3, 6);
            this.addMods.Name = "addMods";
            this.addMods.Size = new System.Drawing.Size(25, 25);
            this.addMods.TabIndex = 2;
            this.addMods.Text = "+";
            this.addMods.UseVisualStyleBackColor = true;
            this.addMods.Click += new System.EventHandler(this.AddMods_Click);
            // 
            // removeButton
            // 
            this.removeButton.Enabled = false;
            this.removeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeButton.Location = new System.Drawing.Point(3, 37);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(25, 25);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "-";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // modContextMenuStrip
            // 
            this.modContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openModFolderToolStripMenuItem,
            this.removeModToolStripMenuItem,
            this.openModURLToolStripMenuItem});
            this.modContextMenuStrip.Name = "modContextMenuStrip";
            this.modContextMenuStrip.Size = new System.Drawing.Size(168, 70);
            // 
            // openModFolderToolStripMenuItem
            // 
            this.openModFolderToolStripMenuItem.Name = "openModFolderToolStripMenuItem";
            this.openModFolderToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.openModFolderToolStripMenuItem.Text = "Open Mod Folder";
            this.openModFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenModFolderToolStripMenuItem_Click);
            // 
            // removeModToolStripMenuItem
            // 
            this.removeModToolStripMenuItem.Name = "removeModToolStripMenuItem";
            this.removeModToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.removeModToolStripMenuItem.Text = "Remove Mod";
            this.removeModToolStripMenuItem.Click += new System.EventHandler(this.RemoveModToolStripMenuItem_Click);
            // 
            // openModURLToolStripMenuItem
            // 
            this.openModURLToolStripMenuItem.Name = "openModURLToolStripMenuItem";
            this.openModURLToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.openModURLToolStripMenuItem.Text = "Open Mod URL";
            this.openModURLToolStripMenuItem.Click += new System.EventHandler(this.OpenModURLToolStripMenuItem_Click);
            // 
            // saveAndLoadButton
            // 
            this.saveAndLoadButton.Location = new System.Drawing.Point(232, 478);
            this.saveAndLoadButton.Name = "saveAndLoadButton";
            this.saveAndLoadButton.Size = new System.Drawing.Size(81, 23);
            this.saveAndLoadButton.TabIndex = 5;
            this.saveAndLoadButton.Text = "Save & Load";
            this.saveAndLoadButton.UseMnemonic = false;
            this.saveAndLoadButton.UseVisualStyleBackColor = true;
            this.saveAndLoadButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.modPage);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(311, 460);
            this.tabControl1.TabIndex = 7;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // modPage
            // 
            this.modPage.Controls.Add(this.modPanel);
            this.modPage.Location = new System.Drawing.Point(4, 22);
            this.modPage.Name = "modPage";
            this.modPage.Padding = new System.Windows.Forms.Padding(3);
            this.modPage.Size = new System.Drawing.Size(303, 434);
            this.modPage.TabIndex = 0;
            this.modPage.Text = "Mods";
            this.modPage.UseVisualStyleBackColor = true;
            // 
            // modPanel
            // 
            this.modPanel.BackColor = System.Drawing.Color.Transparent;
            this.modPanel.Controls.Add(this.moreModOptionsButton);
            this.modPanel.Controls.Add(this.downloadButtonTest);
            this.modPanel.Controls.Add(this.groupBox3);
            this.modPanel.Controls.Add(this.ModList);
            this.modPanel.Controls.Add(this.moveDownButton);
            this.modPanel.Controls.Add(this.refreshButton);
            this.modPanel.Controls.Add(this.addMods);
            this.modPanel.Controls.Add(this.moveUpButton);
            this.modPanel.Controls.Add(this.removeButton);
            this.modPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modPanel.Location = new System.Drawing.Point(3, 3);
            this.modPanel.Name = "modPanel";
            this.modPanel.Size = new System.Drawing.Size(297, 428);
            this.modPanel.TabIndex = 14;
            // 
            // downloadButtonTest
            // 
            this.downloadButtonTest.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.downloadButtonTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadButtonTest.Location = new System.Drawing.Point(3, 180);
            this.downloadButtonTest.Name = "downloadButtonTest";
            this.downloadButtonTest.Size = new System.Drawing.Size(25, 25);
            this.downloadButtonTest.TabIndex = 16;
            this.downloadButtonTest.Text = "⭳";
            this.downloadButtonTest.UseVisualStyleBackColor = true;
            this.downloadButtonTest.Click += new System.EventHandler(this.DownloadButtonTest_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.modInfoTextBox);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.modTechnicalNameLabel);
            this.groupBox3.Controls.Add(this.modNameLabel);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox3.Location = new System.Drawing.Point(3, 211);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(291, 175);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Mod Properties";
            // 
            // modInfoTextBox
            // 
            this.modInfoTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.modInfoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.modInfoTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.modInfoTextBox.Location = new System.Drawing.Point(6, 45);
            this.modInfoTextBox.Name = "modInfoTextBox";
            this.modInfoTextBox.ReadOnly = true;
            this.modInfoTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.modInfoTextBox.Size = new System.Drawing.Size(279, 124);
            this.modInfoTextBox.TabIndex = 16;
            this.modInfoTextBox.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(3, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 13);
            this.label6.TabIndex = 13;
            this.label6.Tag = "";
            // 
            // modTechnicalNameLabel
            // 
            this.modTechnicalNameLabel.AutoSize = true;
            this.modTechnicalNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.modTechnicalNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modTechnicalNameLabel.Location = new System.Drawing.Point(3, 29);
            this.modTechnicalNameLabel.Name = "modTechnicalNameLabel";
            this.modTechnicalNameLabel.Size = new System.Drawing.Size(99, 13);
            this.modTechnicalNameLabel.TabIndex = 15;
            this.modTechnicalNameLabel.Tag = "";
            this.modTechnicalNameLabel.Text = "Technical Name";
            this.modTechnicalNameLabel.UseMnemonic = false;
            // 
            // modNameLabel
            // 
            this.modNameLabel.AutoSize = true;
            this.modNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.modNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modNameLabel.Location = new System.Drawing.Point(3, 16);
            this.modNameLabel.Name = "modNameLabel";
            this.modNameLabel.Size = new System.Drawing.Size(39, 13);
            this.modNameLabel.TabIndex = 11;
            this.modNameLabel.Tag = "";
            this.modNameLabel.Text = "Name";
            this.modNameLabel.UseMnemonic = false;
            // 
            // ModList
            // 
            this.ModList.ContextMenuStrip = this.modContextMenuStrip;
            this.ModList.FormattingEnabled = true;
            this.ModList.Location = new System.Drawing.Point(34, 6);
            this.ModList.Name = "ModList";
            this.ModList.ScrollAlwaysVisible = true;
            this.ModList.Size = new System.Drawing.Size(260, 199);
            this.ModList.TabIndex = 15;
            this.ModList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ModsList_ItemCheck);
            this.ModList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ModsList_MouseClick);
            this.ModList.SelectedIndexChanged += new System.EventHandler(this.ModsList_SelectedIndexChanged);
            this.ModList.SelectedValueChanged += new System.EventHandler(this.ModsList_SelectedValueChanged);
            this.ModList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ModList_MouseDown);
            // 
            // moveDownButton
            // 
            this.moveDownButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.moveDownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.moveDownButton.Location = new System.Drawing.Point(3, 68);
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(25, 25);
            this.moveDownButton.TabIndex = 13;
            this.moveDownButton.Text = "↓";
            this.moveDownButton.UseVisualStyleBackColor = true;
            this.moveDownButton.Click += new System.EventHandler(this.MoveDownButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(132, 392);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 7;
            this.refreshButton.Text = "Reload";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.moveUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.moveUpButton.Location = new System.Drawing.Point(3, 99);
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(25, 25);
            this.moveUpButton.TabIndex = 12;
            this.moveUpButton.Text = "↑";
            this.moveUpButton.UseVisualStyleBackColor = true;
            this.moveUpButton.Click += new System.EventHandler(this.MoveUpButton_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(303, 434);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Recordings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.deleteRecordingButton);
            this.groupBox2.Controls.Add(this.copyRecordingFilePath);
            this.groupBox2.Controls.Add(this.gameRecordingList);
            this.groupBox2.Controls.Add(this.refreshDebugButton);
            this.groupBox2.Controls.Add(this.uploadButton);
            this.groupBox2.Controls.Add(this.openRecordingButton);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(291, 270);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Game Recordings";
            // 
            // deleteRecordingButton
            // 
            this.deleteRecordingButton.Enabled = false;
            this.deleteRecordingButton.Location = new System.Drawing.Point(210, 198);
            this.deleteRecordingButton.Name = "deleteRecordingButton";
            this.deleteRecordingButton.Size = new System.Drawing.Size(75, 23);
            this.deleteRecordingButton.TabIndex = 5;
            this.deleteRecordingButton.Text = "Delete";
            this.deleteRecordingButton.UseVisualStyleBackColor = true;
            this.deleteRecordingButton.Click += new System.EventHandler(this.DeleteRecordingButton_Click);
            // 
            // copyRecordingFilePath
            // 
            this.copyRecordingFilePath.Enabled = false;
            this.copyRecordingFilePath.Location = new System.Drawing.Point(129, 227);
            this.copyRecordingFilePath.Name = "copyRecordingFilePath";
            this.copyRecordingFilePath.Size = new System.Drawing.Size(75, 34);
            this.copyRecordingFilePath.TabIndex = 4;
            this.copyRecordingFilePath.Text = "Copy File Path";
            this.copyRecordingFilePath.UseVisualStyleBackColor = true;
            this.copyRecordingFilePath.Click += new System.EventHandler(this.CopyRecordingFilePath_Click);
            // 
            // gameRecordingList
            // 
            this.gameRecordingList.FormattingEnabled = true;
            this.gameRecordingList.Location = new System.Drawing.Point(6, 19);
            this.gameRecordingList.Name = "gameRecordingList";
            this.gameRecordingList.Size = new System.Drawing.Size(279, 173);
            this.gameRecordingList.TabIndex = 0;
            this.gameRecordingList.SelectedIndexChanged += new System.EventHandler(this.GameRecordingList_SelectedIndexChanged);
            // 
            // refreshDebugButton
            // 
            this.refreshDebugButton.Location = new System.Drawing.Point(6, 198);
            this.refreshDebugButton.Name = "refreshDebugButton";
            this.refreshDebugButton.Size = new System.Drawing.Size(75, 23);
            this.refreshDebugButton.TabIndex = 1;
            this.refreshDebugButton.Text = "Refresh";
            this.refreshDebugButton.UseVisualStyleBackColor = true;
            this.refreshDebugButton.Click += new System.EventHandler(this.RefreshDebugButton_Click);
            // 
            // uploadButton
            // 
            this.uploadButton.Enabled = false;
            this.uploadButton.Location = new System.Drawing.Point(129, 198);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(75, 23);
            this.uploadButton.TabIndex = 3;
            this.uploadButton.Text = "Upload";
            this.uploadButton.UseVisualStyleBackColor = true;
            this.uploadButton.Click += new System.EventHandler(this.UploadButton_Click);
            // 
            // openRecordingButton
            // 
            this.openRecordingButton.Enabled = false;
            this.openRecordingButton.Location = new System.Drawing.Point(210, 227);
            this.openRecordingButton.Name = "openRecordingButton";
            this.openRecordingButton.Size = new System.Drawing.Size(75, 34);
            this.openRecordingButton.TabIndex = 2;
            this.openRecordingButton.Text = "Open";
            this.openRecordingButton.UseVisualStyleBackColor = true;
            this.openRecordingButton.Click += new System.EventHandler(this.OpenRecordingButton_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.showLogFileButton);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(303, 434);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Debug";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // showLogFileButton
            // 
            this.showLogFileButton.Location = new System.Drawing.Point(9, 407);
            this.showLogFileButton.Name = "showLogFileButton";
            this.showLogFileButton.Size = new System.Drawing.Size(279, 21);
            this.showLogFileButton.TabIndex = 6;
            this.showLogFileButton.Text = "Show Log File";
            this.showLogFileButton.UseVisualStyleBackColor = true;
            this.showLogFileButton.Click += new System.EventHandler(this.ShowLogFileButton_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(9, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(279, 240);
            this.label5.TabIndex = 7;
            this.label5.Text = resources.GetString("label5.Text");
            this.label5.UseMnemonic = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.openModdingTemplatesFolder);
            this.groupBox6.Controls.Add(this.openModDocumentationButton);
            this.groupBox6.Controls.Add(this.openSampleModsFolderButton);
            this.groupBox6.Controls.Add(this.openUserManualButton);
            this.groupBox6.Location = new System.Drawing.Point(3, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(294, 141);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Guides";
            // 
            // openModdingTemplatesFolder
            // 
            this.openModdingTemplatesFolder.Location = new System.Drawing.Point(6, 106);
            this.openModdingTemplatesFolder.Name = "openModdingTemplatesFolder";
            this.openModdingTemplatesFolder.Size = new System.Drawing.Size(279, 23);
            this.openModdingTemplatesFolder.TabIndex = 4;
            this.openModdingTemplatesFolder.Text = "Open Modding Templates Folder";
            this.openModdingTemplatesFolder.UseVisualStyleBackColor = true;
            this.openModdingTemplatesFolder.Click += new System.EventHandler(this.OpenModdingTemplatesFolder_Click);
            // 
            // openModDocumentationButton
            // 
            this.openModDocumentationButton.Location = new System.Drawing.Point(6, 77);
            this.openModDocumentationButton.Name = "openModDocumentationButton";
            this.openModDocumentationButton.Size = new System.Drawing.Size(279, 23);
            this.openModDocumentationButton.TabIndex = 2;
            this.openModDocumentationButton.Text = "Sonic 3 A.I.R Modding Instructions";
            this.openModDocumentationButton.UseVisualStyleBackColor = true;
            this.openModDocumentationButton.Click += new System.EventHandler(this.OpenModDocumentationButton_Click);
            // 
            // openSampleModsFolderButton
            // 
            this.openSampleModsFolderButton.Location = new System.Drawing.Point(6, 19);
            this.openSampleModsFolderButton.Name = "openSampleModsFolderButton";
            this.openSampleModsFolderButton.Size = new System.Drawing.Size(279, 23);
            this.openSampleModsFolderButton.TabIndex = 3;
            this.openSampleModsFolderButton.Text = "Open Sample Mods Folder";
            this.openSampleModsFolderButton.UseVisualStyleBackColor = true;
            this.openSampleModsFolderButton.Click += new System.EventHandler(this.OpenSampleModsFolderButton_Click);
            // 
            // openUserManualButton
            // 
            this.openUserManualButton.Location = new System.Drawing.Point(6, 48);
            this.openUserManualButton.Name = "openUserManualButton";
            this.openUserManualButton.Size = new System.Drawing.Size(279, 23);
            this.openUserManualButton.TabIndex = 1;
            this.openUserManualButton.Text = "Sonic 3 A.I.R User Manual";
            this.openUserManualButton.UseVisualStyleBackColor = true;
            this.openUserManualButton.Click += new System.EventHandler(this.OpenUserManualButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.autoLaunchDelayLabel);
            this.tabPage2.Controls.Add(this.autoLaunchDelayUpDown);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.keepOpenOnQuitCheckBox);
            this.tabPage2.Controls.Add(this.keepLoaderOpenCheckBox);
            this.tabPage2.Controls.Add(this.autoRunCheckbox);
            this.tabPage2.Controls.Add(this.updateSonic3AIRPathButton);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.sonic3AIRPathBox);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(303, 434);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // autoLaunchDelayLabel
            // 
            this.autoLaunchDelayLabel.AutoSize = true;
            this.autoLaunchDelayLabel.Location = new System.Drawing.Point(6, 66);
            this.autoLaunchDelayLabel.Name = "autoLaunchDelayLabel";
            this.autoLaunchDelayLabel.Size = new System.Drawing.Size(113, 13);
            this.autoLaunchDelayLabel.TabIndex = 14;
            this.autoLaunchDelayLabel.Text = "Boot Delay (Seconds):";
            // 
            // autoLaunchDelayUpDown
            // 
            this.autoLaunchDelayUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Sonic3AIR_ModLoader.Properties.Settings.Default, "AutoLaunchDelay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.autoLaunchDelayUpDown.Location = new System.Drawing.Point(125, 64);
            this.autoLaunchDelayUpDown.Name = "autoLaunchDelayUpDown";
            this.autoLaunchDelayUpDown.Size = new System.Drawing.Size(91, 20);
            this.autoLaunchDelayUpDown.TabIndex = 13;
            this.autoLaunchDelayUpDown.Value = global::Sonic3AIR_ModLoader.Properties.Settings.Default.AutoLaunchDelay;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Mod Manager Options:";
            // 
            // keepOpenOnQuitCheckBox
            // 
            this.keepOpenOnQuitCheckBox.AutoSize = true;
            this.keepOpenOnQuitCheckBox.Checked = global::Sonic3AIR_ModLoader.Properties.Settings.Default.KeepOpenOnQuit;
            this.keepOpenOnQuitCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Sonic3AIR_ModLoader.Properties.Settings.Default, "KeepOpenOnQuit", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.keepOpenOnQuitCheckBox.Location = new System.Drawing.Point(9, 136);
            this.keepOpenOnQuitCheckBox.Name = "keepOpenOnQuitCheckBox";
            this.keepOpenOnQuitCheckBox.Size = new System.Drawing.Size(142, 17);
            this.keepOpenOnQuitCheckBox.TabIndex = 10;
            this.keepOpenOnQuitCheckBox.Text = "Stay Open on Game Exit";
            this.keepOpenOnQuitCheckBox.UseVisualStyleBackColor = true;
            // 
            // keepLoaderOpenCheckBox
            // 
            this.keepLoaderOpenCheckBox.AutoSize = true;
            this.keepLoaderOpenCheckBox.Checked = global::Sonic3AIR_ModLoader.Properties.Settings.Default.KeepOpenOnLaunch;
            this.keepLoaderOpenCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Sonic3AIR_ModLoader.Properties.Settings.Default, "KeepOpenOnLaunch", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.keepLoaderOpenCheckBox.Location = new System.Drawing.Point(9, 113);
            this.keepLoaderOpenCheckBox.Name = "keepLoaderOpenCheckBox";
            this.keepLoaderOpenCheckBox.Size = new System.Drawing.Size(161, 17);
            this.keepLoaderOpenCheckBox.TabIndex = 9;
            this.keepLoaderOpenCheckBox.Text = "Stay Open on Game Launch";
            this.keepLoaderOpenCheckBox.UseVisualStyleBackColor = true;
            // 
            // autoRunCheckbox
            // 
            this.autoRunCheckbox.AutoSize = true;
            this.autoRunCheckbox.Checked = global::Sonic3AIR_ModLoader.Properties.Settings.Default.AutoLaunch;
            this.autoRunCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Sonic3AIR_ModLoader.Properties.Settings.Default, "AutoLaunch", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.autoRunCheckbox.Location = new System.Drawing.Point(9, 46);
            this.autoRunCheckbox.Name = "autoRunCheckbox";
            this.autoRunCheckbox.Size = new System.Drawing.Size(139, 17);
            this.autoRunCheckbox.TabIndex = 8;
            this.autoRunCheckbox.Text = "Enable Auto Boot Mode";
            this.autoRunCheckbox.UseVisualStyleBackColor = true;
            this.autoRunCheckbox.CheckedChanged += new System.EventHandler(this.AutoRunCheckbox_CheckedChanged);
            this.autoRunCheckbox.CheckStateChanged += new System.EventHandler(this.AutoRunCheckbox_CheckedChanged);
            // 
            // updateSonic3AIRPathButton
            // 
            this.updateSonic3AIRPathButton.Location = new System.Drawing.Point(270, 19);
            this.updateSonic3AIRPathButton.Name = "updateSonic3AIRPathButton";
            this.updateSonic3AIRPathButton.Size = new System.Drawing.Size(27, 20);
            this.updateSonic3AIRPathButton.TabIndex = 7;
            this.updateSonic3AIRPathButton.Text = "...";
            this.updateSonic3AIRPathButton.UseVisualStyleBackColor = true;
            this.updateSonic3AIRPathButton.Click += new System.EventHandler(this.UpdateSonic3AIRPathButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.carJemTwitterButton);
            this.groupBox1.Controls.Add(this.eukaTwitterButton);
            this.groupBox1.Controls.Add(this.gamebannaButton);
            this.groupBox1.Controls.Add(this.s3AIRWebsiteButton);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.openModsFolder);
            this.groupBox1.Controls.Add(this.changeRomPathButton);
            this.groupBox1.Controls.Add(this.openConfigFile);
            this.groupBox1.Controls.Add(this.openAppDataFolderButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.romPathBox);
            this.groupBox1.Controls.Add(this.openEXEFolderButton);
            this.groupBox1.Controls.Add(this.failSafeModeCheckbox);
            this.groupBox1.Controls.Add(this.fixGlitchesCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(6, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 269);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced Settings";
            // 
            // carJemTwitterButton
            // 
            this.carJemTwitterButton.Location = new System.Drawing.Point(149, 232);
            this.carJemTwitterButton.Name = "carJemTwitterButton";
            this.carJemTwitterButton.Size = new System.Drawing.Size(133, 24);
            this.carJemTwitterButton.TabIndex = 19;
            this.carJemTwitterButton.Text = "CarJem @ Twitter";
            this.carJemTwitterButton.UseVisualStyleBackColor = true;
            this.carJemTwitterButton.Click += new System.EventHandler(this.CarJemTwitterButton_Click);
            // 
            // eukaTwitterButton
            // 
            this.eukaTwitterButton.Location = new System.Drawing.Point(149, 202);
            this.eukaTwitterButton.Name = "eukaTwitterButton";
            this.eukaTwitterButton.Size = new System.Drawing.Size(133, 24);
            this.eukaTwitterButton.TabIndex = 18;
            this.eukaTwitterButton.Text = "Eukaryot3K @ Twitter";
            this.eukaTwitterButton.UseVisualStyleBackColor = true;
            this.eukaTwitterButton.Click += new System.EventHandler(this.EukaTwitterButton_Click);
            // 
            // gamebannaButton
            // 
            this.gamebannaButton.Location = new System.Drawing.Point(149, 172);
            this.gamebannaButton.Name = "gamebannaButton";
            this.gamebannaButton.Size = new System.Drawing.Size(133, 24);
            this.gamebannaButton.TabIndex = 17;
            this.gamebannaButton.Text = "S3AIR @ Gamebanna";
            this.gamebannaButton.UseVisualStyleBackColor = true;
            this.gamebannaButton.Click += new System.EventHandler(this.GamebannaButton_Click);
            // 
            // s3AIRWebsiteButton
            // 
            this.s3AIRWebsiteButton.Location = new System.Drawing.Point(149, 142);
            this.s3AIRWebsiteButton.Name = "s3AIRWebsiteButton";
            this.s3AIRWebsiteButton.Size = new System.Drawing.Size(133, 24);
            this.s3AIRWebsiteButton.TabIndex = 16;
            this.s3AIRWebsiteButton.Text = "Sonic 3 AIR Homepage";
            this.s3AIRWebsiteButton.UseVisualStyleBackColor = true;
            this.s3AIRWebsiteButton.Click += new System.EventHandler(this.S3AIRWebsiteButton_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Location = new System.Drawing.Point(6, 142);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(139, 88);
            this.groupBox5.TabIndex = 15;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "About";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 65);
            this.label4.TabIndex = 12;
            this.label4.Text = "Sonic 3 A.I.R \r\nMod Manager\r\n\r\nVersion v.0.11 BETA\r\nBy CarJem Generations\r\n";
            // 
            // openModsFolder
            // 
            this.openModsFolder.Location = new System.Drawing.Point(3, 112);
            this.openModsFolder.Name = "openModsFolder";
            this.openModsFolder.Size = new System.Drawing.Size(133, 24);
            this.openModsFolder.TabIndex = 10;
            this.openModsFolder.Text = "Open Mods Folder";
            this.openModsFolder.UseVisualStyleBackColor = true;
            this.openModsFolder.Click += new System.EventHandler(this.OpenModsFolder_Click);
            // 
            // changeRomPathButton
            // 
            this.changeRomPathButton.Location = new System.Drawing.Point(255, 33);
            this.changeRomPathButton.Name = "changeRomPathButton";
            this.changeRomPathButton.Size = new System.Drawing.Size(27, 20);
            this.changeRomPathButton.TabIndex = 8;
            this.changeRomPathButton.Text = "...";
            this.changeRomPathButton.UseVisualStyleBackColor = true;
            this.changeRomPathButton.Click += new System.EventHandler(this.ChangeRomPathButton_Click);
            // 
            // openConfigFile
            // 
            this.openConfigFile.Location = new System.Drawing.Point(3, 82);
            this.openConfigFile.Name = "openConfigFile";
            this.openConfigFile.Size = new System.Drawing.Size(133, 24);
            this.openConfigFile.TabIndex = 9;
            this.openConfigFile.Text = "Open Settings File";
            this.openConfigFile.UseVisualStyleBackColor = true;
            this.openConfigFile.Click += new System.EventHandler(this.OpenConfigFile_Click);
            // 
            // openAppDataFolderButton
            // 
            this.openAppDataFolderButton.Location = new System.Drawing.Point(149, 82);
            this.openAppDataFolderButton.Name = "openAppDataFolderButton";
            this.openAppDataFolderButton.Size = new System.Drawing.Size(133, 24);
            this.openAppDataFolderButton.TabIndex = 9;
            this.openAppDataFolderButton.Text = "Open AppData Folder";
            this.openAppDataFolderButton.UseVisualStyleBackColor = true;
            this.openAppDataFolderButton.Click += new System.EventHandler(this.OpenAppDataFolderButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Sonic 3K ROM Path:";
            // 
            // romPathBox
            // 
            this.romPathBox.Location = new System.Drawing.Point(6, 33);
            this.romPathBox.Name = "romPathBox";
            this.romPathBox.ReadOnly = true;
            this.romPathBox.Size = new System.Drawing.Size(243, 20);
            this.romPathBox.TabIndex = 5;
            // 
            // openEXEFolderButton
            // 
            this.openEXEFolderButton.Location = new System.Drawing.Point(149, 112);
            this.openEXEFolderButton.Name = "openEXEFolderButton";
            this.openEXEFolderButton.Size = new System.Drawing.Size(133, 24);
            this.openEXEFolderButton.TabIndex = 8;
            this.openEXEFolderButton.Text = "Open EXE Folder";
            this.openEXEFolderButton.UseVisualStyleBackColor = true;
            this.openEXEFolderButton.Click += new System.EventHandler(this.OpenEXEFolderButton_Click);
            // 
            // failSafeModeCheckbox
            // 
            this.failSafeModeCheckbox.AutoSize = true;
            this.failSafeModeCheckbox.Location = new System.Drawing.Point(95, 59);
            this.failSafeModeCheckbox.Name = "failSafeModeCheckbox";
            this.failSafeModeCheckbox.Size = new System.Drawing.Size(97, 17);
            this.failSafeModeCheckbox.TabIndex = 4;
            this.failSafeModeCheckbox.Text = "Fail Safe Mode";
            this.failSafeModeCheckbox.UseVisualStyleBackColor = true;
            this.failSafeModeCheckbox.Click += new System.EventHandler(this.FailSafeModeCheckbox_Click);
            // 
            // fixGlitchesCheckbox
            // 
            this.fixGlitchesCheckbox.AutoSize = true;
            this.fixGlitchesCheckbox.Location = new System.Drawing.Point(9, 59);
            this.fixGlitchesCheckbox.Name = "fixGlitchesCheckbox";
            this.fixGlitchesCheckbox.Size = new System.Drawing.Size(80, 17);
            this.fixGlitchesCheckbox.TabIndex = 3;
            this.fixGlitchesCheckbox.Text = "Fix Glitches";
            this.fixGlitchesCheckbox.UseVisualStyleBackColor = true;
            this.fixGlitchesCheckbox.Click += new System.EventHandler(this.FixGlitchesCheckbox_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sonic 3 A.I.R Path:";
            // 
            // sonic3AIRPathBox
            // 
            this.sonic3AIRPathBox.Location = new System.Drawing.Point(9, 19);
            this.sonic3AIRPathBox.Name = "sonic3AIRPathBox";
            this.sonic3AIRPathBox.ReadOnly = true;
            this.sonic3AIRPathBox.Size = new System.Drawing.Size(255, 20);
            this.sonic3AIRPathBox.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.modStackOffRadioButton);
            this.groupBox4.Controls.Add(this.modStackOnRadioButton);
            this.groupBox4.Location = new System.Drawing.Point(176, 97);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(118, 56);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Enable Mod Stacking";
            // 
            // modStackOffRadioButton
            // 
            this.modStackOffRadioButton.AutoSize = true;
            this.modStackOffRadioButton.Location = new System.Drawing.Point(53, 33);
            this.modStackOffRadioButton.Name = "modStackOffRadioButton";
            this.modStackOffRadioButton.Size = new System.Drawing.Size(45, 17);
            this.modStackOffRadioButton.TabIndex = 16;
            this.modStackOffRadioButton.TabStop = true;
            this.modStackOffRadioButton.Text = "OFF";
            this.modStackOffRadioButton.UseVisualStyleBackColor = true;
            this.modStackOffRadioButton.CheckedChanged += new System.EventHandler(this.ModStackRadioButtons_CheckedChanged);
            // 
            // modStackOnRadioButton
            // 
            this.modStackOnRadioButton.AutoSize = true;
            this.modStackOnRadioButton.Location = new System.Drawing.Point(6, 33);
            this.modStackOnRadioButton.Name = "modStackOnRadioButton";
            this.modStackOnRadioButton.Size = new System.Drawing.Size(41, 17);
            this.modStackOnRadioButton.TabIndex = 15;
            this.modStackOnRadioButton.TabStop = true;
            this.modStackOnRadioButton.Text = "ON";
            this.modStackOnRadioButton.UseVisualStyleBackColor = true;
            this.modStackOnRadioButton.CheckedChanged += new System.EventHandler(this.ModStackRadioButtons_CheckedChanged);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(151, 478);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 16;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(70, 478);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 13;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // moreModOptionsButton
            // 
            this.moreModOptionsButton.ContextMenuStrip = this.moreModOptionsMenuStrip;
            this.moreModOptionsButton.Location = new System.Drawing.Point(213, 392);
            this.moreModOptionsButton.Name = "moreModOptionsButton";
            this.moreModOptionsButton.Size = new System.Drawing.Size(81, 23);
            this.moreModOptionsButton.TabIndex = 18;
            this.moreModOptionsButton.Text = "More...";
            this.moreModOptionsButton.UseVisualStyleBackColor = true;
            this.moreModOptionsButton.Click += new System.EventHandler(this.MoreModOptionsButton_Click);
            // 
            // moreModOptionsMenuStrip
            // 
            this.moreModOptionsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gamebannaURLHandlerOptionsToolStripMenuItem});
            this.moreModOptionsMenuStrip.Name = "moreModOptionsMenuStrip";
            this.moreModOptionsMenuStrip.Size = new System.Drawing.Size(262, 48);
            // 
            // gamebannaURLHandlerOptionsToolStripMenuItem
            // 
            this.gamebannaURLHandlerOptionsToolStripMenuItem.Name = "gamebannaURLHandlerOptionsToolStripMenuItem";
            this.gamebannaURLHandlerOptionsToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.gamebannaURLHandlerOptionsToolStripMenuItem.Text = "Gamebanna URL Handler Options...";
            this.gamebannaURLHandlerOptionsToolStripMenuItem.Click += new System.EventHandler(this.AddRemoveURLHandlerButton_Click);
            // 
            // ModManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 513);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.saveAndLoadButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sonic 3 A.I.R Mod Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModManager_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.ModManager_VisibleChanged);
            this.modContextMenuStrip.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.modPage.ResumeLayout(false);
            this.modPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.autoLaunchDelayUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.moreModOptionsMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button addMods;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button saveAndLoadButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage modPage;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox romPathBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox failSafeModeCheckbox;
        private System.Windows.Forms.CheckBox fixGlitchesCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sonic3AIRPathBox;
        private System.Windows.Forms.Button changeRomPathButton;
        private System.Windows.Forms.Button updateSonic3AIRPathButton;
        private System.Windows.Forms.Button openConfigFile;
        private System.Windows.Forms.Label modNameLabel;
        private System.Windows.Forms.Label modTechnicalNameLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button openAppDataFolderButton;
        private System.Windows.Forms.Button openEXEFolderButton;
        private System.Windows.Forms.Button moveDownButton;
        private System.Windows.Forms.Button moveUpButton;
        private System.Windows.Forms.CheckBox autoRunCheckbox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button refreshDebugButton;
        private System.Windows.Forms.ListBox gameRecordingList;
        private System.Windows.Forms.Button uploadButton;
        private System.Windows.Forms.Button openRecordingButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button openModsFolder;
        private System.Windows.Forms.Button copyRecordingFilePath;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.ContextMenuStrip modContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openModFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeModToolStripMenuItem;
        private System.Windows.Forms.CheckBox keepLoaderOpenCheckBox;
        private System.Windows.Forms.CheckBox keepOpenOnQuitCheckBox;
        private System.Windows.Forms.Panel modPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button deleteRecordingButton;
        private System.Windows.Forms.Label autoLaunchDelayLabel;
        private System.Windows.Forms.NumericUpDown autoLaunchDelayUpDown;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button showLogFileButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckedListBox ModList;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.RichTextBox modInfoTextBox;
        private System.Windows.Forms.ToolStripMenuItem openModURLToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button openModDocumentationButton;
        private System.Windows.Forms.Button openSampleModsFolderButton;
        private System.Windows.Forms.Button openUserManualButton;
        private System.Windows.Forms.Button openModdingTemplatesFolder;
        private System.Windows.Forms.Button carJemTwitterButton;
        private System.Windows.Forms.Button eukaTwitterButton;
        private System.Windows.Forms.Button gamebannaButton;
        private System.Windows.Forms.Button s3AIRWebsiteButton;
        private System.Windows.Forms.Button downloadButtonTest;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton modStackOffRadioButton;
        private System.Windows.Forms.RadioButton modStackOnRadioButton;
        private System.Windows.Forms.Button moreModOptionsButton;
        private System.Windows.Forms.ContextMenuStrip moreModOptionsMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem gamebannaURLHandlerOptionsToolStripMenuItem;
    }
}

