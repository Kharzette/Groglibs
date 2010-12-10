﻿namespace BSPBuilder
{
	partial class MainForm
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
			this.OpenBrushFile = new System.Windows.Forms.Button();
			this.LabelNumRawFaces = new System.Windows.Forms.Label();
			this.NumFaces = new System.Windows.Forms.TextBox();
			this.StatsGroupBox = new System.Windows.Forms.GroupBox();
			this.LabelNumPortals = new System.Windows.Forms.Label();
			this.NumPortals = new System.Windows.Forms.TextBox();
			this.LabelNumCollisionFaces = new System.Windows.Forms.Label();
			this.LabelNumDrawFaces = new System.Windows.Forms.Label();
			this.NumAreas = new System.Windows.Forms.TextBox();
			this.NumNodes = new System.Windows.Forms.TextBox();
			this.SaveGBSP = new System.Windows.Forms.Button();
			this.ConsoleOut = new System.Windows.Forms.TextBox();
			this.DrawChoice = new System.Windows.Forms.ComboBox();
			this.MaxCPUCores = new System.Windows.Forms.NumericUpDown();
			this.LabelCPUCores = new System.Windows.Forms.Label();
			this.GroupFileIO = new System.Windows.Forms.GroupBox();
			this.LoadGBSP = new System.Windows.Forms.Button();
			this.LightGBSP = new System.Windows.Forms.Button();
			this.BuildGBSP = new System.Windows.Forms.Button();
			this.VisGBSP = new System.Windows.Forms.Button();
			this.GroupBuildSettings = new System.Windows.Forms.GroupBox();
			this.FixTJunctions = new System.Windows.Forms.CheckBox();
			this.VerboseEntity = new System.Windows.Forms.CheckBox();
			this.VerboseBSP = new System.Windows.Forms.CheckBox();
			this.GroupDrawSettings = new System.Windows.Forms.GroupBox();
			this.Progress1 = new System.Windows.Forms.ProgressBar();
			this.Progress2 = new System.Windows.Forms.ProgressBar();
			this.Progress3 = new System.Windows.Forms.ProgressBar();
			this.Progress4 = new System.Windows.Forms.ProgressBar();
			this.LightSettingsGroupBox = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.MaxIntensity = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.ReflectiveScale = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.MinLightZ = new System.Windows.Forms.NumericUpDown();
			this.MinLightX = new System.Windows.Forms.NumericUpDown();
			this.MinLightY = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.PatchSize = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.LightScale = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.NumBounce = new System.Windows.Forms.NumericUpDown();
			this.Radiosity = new System.Windows.Forms.CheckBox();
			this.ExtraSamples = new System.Windows.Forms.CheckBox();
			this.FastPatch = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SortPortals = new System.Windows.Forms.CheckBox();
			this.FullVis = new System.Windows.Forms.CheckBox();
			this.StatsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MaxCPUCores)).BeginInit();
			this.GroupFileIO.SuspendLayout();
			this.GroupBuildSettings.SuspendLayout();
			this.GroupDrawSettings.SuspendLayout();
			this.LightSettingsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MaxIntensity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ReflectiveScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightZ)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PatchSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.LightScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NumBounce)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// OpenBrushFile
			// 
			this.OpenBrushFile.Location = new System.Drawing.Point(6, 19);
			this.OpenBrushFile.Name = "OpenBrushFile";
			this.OpenBrushFile.Size = new System.Drawing.Size(75, 23);
			this.OpenBrushFile.TabIndex = 0;
			this.OpenBrushFile.Text = "Open Map";
			this.OpenBrushFile.UseVisualStyleBackColor = true;
			this.OpenBrushFile.Click += new System.EventHandler(this.OnOpenBrushFile);
			// 
			// LabelNumRawFaces
			// 
			this.LabelNumRawFaces.AutoSize = true;
			this.LabelNumRawFaces.Location = new System.Drawing.Point(6, 22);
			this.LabelNumRawFaces.Name = "LabelNumRawFaces";
			this.LabelNumRawFaces.Size = new System.Drawing.Size(36, 13);
			this.LabelNumRawFaces.TabIndex = 12;
			this.LabelNumRawFaces.Text = "Faces";
			// 
			// NumFaces
			// 
			this.NumFaces.Location = new System.Drawing.Point(51, 19);
			this.NumFaces.Name = "NumFaces";
			this.NumFaces.ReadOnly = true;
			this.NumFaces.Size = new System.Drawing.Size(76, 20);
			this.NumFaces.TabIndex = 13;
			// 
			// StatsGroupBox
			// 
			this.StatsGroupBox.Controls.Add(this.LabelNumPortals);
			this.StatsGroupBox.Controls.Add(this.NumPortals);
			this.StatsGroupBox.Controls.Add(this.LabelNumCollisionFaces);
			this.StatsGroupBox.Controls.Add(this.LabelNumDrawFaces);
			this.StatsGroupBox.Controls.Add(this.NumAreas);
			this.StatsGroupBox.Controls.Add(this.NumNodes);
			this.StatsGroupBox.Controls.Add(this.LabelNumRawFaces);
			this.StatsGroupBox.Controls.Add(this.NumFaces);
			this.StatsGroupBox.Location = new System.Drawing.Point(192, 12);
			this.StatsGroupBox.Name = "StatsGroupBox";
			this.StatsGroupBox.Size = new System.Drawing.Size(134, 127);
			this.StatsGroupBox.TabIndex = 14;
			this.StatsGroupBox.TabStop = false;
			this.StatsGroupBox.Text = "Statistics";
			// 
			// LabelNumPortals
			// 
			this.LabelNumPortals.AutoSize = true;
			this.LabelNumPortals.Location = new System.Drawing.Point(6, 100);
			this.LabelNumPortals.Name = "LabelNumPortals";
			this.LabelNumPortals.Size = new System.Drawing.Size(39, 13);
			this.LabelNumPortals.TabIndex = 20;
			this.LabelNumPortals.Text = "Portals";
			// 
			// NumPortals
			// 
			this.NumPortals.Location = new System.Drawing.Point(51, 97);
			this.NumPortals.Name = "NumPortals";
			this.NumPortals.ReadOnly = true;
			this.NumPortals.Size = new System.Drawing.Size(76, 20);
			this.NumPortals.TabIndex = 19;
			// 
			// LabelNumCollisionFaces
			// 
			this.LabelNumCollisionFaces.AutoSize = true;
			this.LabelNumCollisionFaces.Location = new System.Drawing.Point(6, 74);
			this.LabelNumCollisionFaces.Name = "LabelNumCollisionFaces";
			this.LabelNumCollisionFaces.Size = new System.Drawing.Size(34, 13);
			this.LabelNumCollisionFaces.TabIndex = 18;
			this.LabelNumCollisionFaces.Text = "Areas";
			// 
			// LabelNumDrawFaces
			// 
			this.LabelNumDrawFaces.AutoSize = true;
			this.LabelNumDrawFaces.Location = new System.Drawing.Point(6, 48);
			this.LabelNumDrawFaces.Name = "LabelNumDrawFaces";
			this.LabelNumDrawFaces.Size = new System.Drawing.Size(38, 13);
			this.LabelNumDrawFaces.TabIndex = 17;
			this.LabelNumDrawFaces.Text = "Nodes";
			// 
			// NumAreas
			// 
			this.NumAreas.Location = new System.Drawing.Point(51, 71);
			this.NumAreas.Name = "NumAreas";
			this.NumAreas.ReadOnly = true;
			this.NumAreas.Size = new System.Drawing.Size(76, 20);
			this.NumAreas.TabIndex = 15;
			// 
			// NumNodes
			// 
			this.NumNodes.Location = new System.Drawing.Point(51, 45);
			this.NumNodes.Name = "NumNodes";
			this.NumNodes.ReadOnly = true;
			this.NumNodes.Size = new System.Drawing.Size(76, 20);
			this.NumNodes.TabIndex = 14;
			// 
			// SaveGBSP
			// 
			this.SaveGBSP.Enabled = false;
			this.SaveGBSP.Location = new System.Drawing.Point(87, 19);
			this.SaveGBSP.Name = "SaveGBSP";
			this.SaveGBSP.Size = new System.Drawing.Size(75, 23);
			this.SaveGBSP.TabIndex = 15;
			this.SaveGBSP.Text = "Save GBSP";
			this.SaveGBSP.UseVisualStyleBackColor = true;
			this.SaveGBSP.Click += new System.EventHandler(this.OnSaveGBSP);
			// 
			// ConsoleOut
			// 
			this.ConsoleOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ConsoleOut.Location = new System.Drawing.Point(12, 396);
			this.ConsoleOut.Multiline = true;
			this.ConsoleOut.Name = "ConsoleOut";
			this.ConsoleOut.ReadOnly = true;
			this.ConsoleOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ConsoleOut.Size = new System.Drawing.Size(452, 153);
			this.ConsoleOut.TabIndex = 16;
			// 
			// DrawChoice
			// 
			this.DrawChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DrawChoice.FormattingEnabled = true;
			this.DrawChoice.Items.AddRange(new object[] {
            "Draw Brushes",
            "Map Brushes",
            "Collision Brushes",
            "Trouble Brushes",
            "Draw Tree",
            "Collision Tree",
            "Portals",
            "Portal Tree"});
			this.DrawChoice.Location = new System.Drawing.Point(6, 19);
			this.DrawChoice.Name = "DrawChoice";
			this.DrawChoice.Size = new System.Drawing.Size(123, 21);
			this.DrawChoice.TabIndex = 17;
			this.DrawChoice.SelectedIndexChanged += new System.EventHandler(this.OnDrawChoiceChanged);
			// 
			// MaxCPUCores
			// 
			this.MaxCPUCores.Enabled = false;
			this.MaxCPUCores.Location = new System.Drawing.Point(6, 19);
			this.MaxCPUCores.Name = "MaxCPUCores";
			this.MaxCPUCores.Size = new System.Drawing.Size(41, 20);
			this.MaxCPUCores.TabIndex = 18;
			// 
			// LabelCPUCores
			// 
			this.LabelCPUCores.AutoSize = true;
			this.LabelCPUCores.Location = new System.Drawing.Point(53, 21);
			this.LabelCPUCores.Name = "LabelCPUCores";
			this.LabelCPUCores.Size = new System.Drawing.Size(57, 13);
			this.LabelCPUCores.TabIndex = 19;
			this.LabelCPUCores.Text = "Max Cores";
			// 
			// GroupFileIO
			// 
			this.GroupFileIO.Controls.Add(this.LoadGBSP);
			this.GroupFileIO.Controls.Add(this.LightGBSP);
			this.GroupFileIO.Controls.Add(this.BuildGBSP);
			this.GroupFileIO.Controls.Add(this.VisGBSP);
			this.GroupFileIO.Controls.Add(this.OpenBrushFile);
			this.GroupFileIO.Controls.Add(this.SaveGBSP);
			this.GroupFileIO.Location = new System.Drawing.Point(12, 12);
			this.GroupFileIO.Name = "GroupFileIO";
			this.GroupFileIO.Size = new System.Drawing.Size(174, 113);
			this.GroupFileIO.TabIndex = 20;
			this.GroupFileIO.TabStop = false;
			this.GroupFileIO.Text = "File IO";
			// 
			// LoadGBSP
			// 
			this.LoadGBSP.Location = new System.Drawing.Point(6, 48);
			this.LoadGBSP.Name = "LoadGBSP";
			this.LoadGBSP.Size = new System.Drawing.Size(75, 23);
			this.LoadGBSP.TabIndex = 19;
			this.LoadGBSP.Text = "Load GBSP";
			this.LoadGBSP.UseVisualStyleBackColor = true;
			this.LoadGBSP.Click += new System.EventHandler(this.OnLoadGBSP);
			// 
			// LightGBSP
			// 
			this.LightGBSP.Location = new System.Drawing.Point(87, 78);
			this.LightGBSP.Name = "LightGBSP";
			this.LightGBSP.Size = new System.Drawing.Size(75, 23);
			this.LightGBSP.TabIndex = 18;
			this.LightGBSP.Text = "Light GBSP";
			this.LightGBSP.UseVisualStyleBackColor = true;
			this.LightGBSP.Click += new System.EventHandler(this.OnLightGBSP);
			// 
			// BuildGBSP
			// 
			this.BuildGBSP.Enabled = false;
			this.BuildGBSP.Location = new System.Drawing.Point(6, 78);
			this.BuildGBSP.Name = "BuildGBSP";
			this.BuildGBSP.Size = new System.Drawing.Size(75, 23);
			this.BuildGBSP.TabIndex = 17;
			this.BuildGBSP.Text = "Build GBSP";
			this.BuildGBSP.UseVisualStyleBackColor = true;
			this.BuildGBSP.Click += new System.EventHandler(this.OnBuildGBSP);
			// 
			// VisGBSP
			// 
			this.VisGBSP.Location = new System.Drawing.Point(87, 48);
			this.VisGBSP.Name = "VisGBSP";
			this.VisGBSP.Size = new System.Drawing.Size(75, 23);
			this.VisGBSP.TabIndex = 16;
			this.VisGBSP.Text = "Vis GBSP";
			this.VisGBSP.UseVisualStyleBackColor = true;
			this.VisGBSP.Click += new System.EventHandler(this.OnVisGBSP);
			// 
			// GroupBuildSettings
			// 
			this.GroupBuildSettings.Controls.Add(this.FixTJunctions);
			this.GroupBuildSettings.Controls.Add(this.VerboseEntity);
			this.GroupBuildSettings.Controls.Add(this.VerboseBSP);
			this.GroupBuildSettings.Controls.Add(this.MaxCPUCores);
			this.GroupBuildSettings.Controls.Add(this.LabelCPUCores);
			this.GroupBuildSettings.Location = new System.Drawing.Point(192, 145);
			this.GroupBuildSettings.Name = "GroupBuildSettings";
			this.GroupBuildSettings.Size = new System.Drawing.Size(115, 123);
			this.GroupBuildSettings.TabIndex = 21;
			this.GroupBuildSettings.TabStop = false;
			this.GroupBuildSettings.Text = "Build Settings";
			// 
			// FixTJunctions
			// 
			this.FixTJunctions.AutoSize = true;
			this.FixTJunctions.Checked = true;
			this.FixTJunctions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.FixTJunctions.Location = new System.Drawing.Point(6, 91);
			this.FixTJunctions.Name = "FixTJunctions";
			this.FixTJunctions.Size = new System.Drawing.Size(94, 17);
			this.FixTJunctions.TabIndex = 22;
			this.FixTJunctions.Text = "Fix TJunctions";
			this.FixTJunctions.UseVisualStyleBackColor = true;
			// 
			// VerboseEntity
			// 
			this.VerboseEntity.AutoSize = true;
			this.VerboseEntity.Location = new System.Drawing.Point(6, 68);
			this.VerboseEntity.Name = "VerboseEntity";
			this.VerboseEntity.Size = new System.Drawing.Size(94, 17);
			this.VerboseEntity.TabIndex = 21;
			this.VerboseEntity.Text = "Entity Verbose";
			this.VerboseEntity.UseVisualStyleBackColor = true;
			// 
			// VerboseBSP
			// 
			this.VerboseBSP.AutoSize = true;
			this.VerboseBSP.Checked = true;
			this.VerboseBSP.CheckState = System.Windows.Forms.CheckState.Checked;
			this.VerboseBSP.Location = new System.Drawing.Point(6, 45);
			this.VerboseBSP.Name = "VerboseBSP";
			this.VerboseBSP.Size = new System.Drawing.Size(65, 17);
			this.VerboseBSP.TabIndex = 20;
			this.VerboseBSP.Text = "Verbose";
			this.VerboseBSP.UseVisualStyleBackColor = true;
			// 
			// GroupDrawSettings
			// 
			this.GroupDrawSettings.Controls.Add(this.DrawChoice);
			this.GroupDrawSettings.Location = new System.Drawing.Point(12, 228);
			this.GroupDrawSettings.Name = "GroupDrawSettings";
			this.GroupDrawSettings.Size = new System.Drawing.Size(135, 58);
			this.GroupDrawSettings.TabIndex = 22;
			this.GroupDrawSettings.TabStop = false;
			this.GroupDrawSettings.Text = "Draw Settings";
			// 
			// Progress1
			// 
			this.Progress1.Location = new System.Drawing.Point(12, 292);
			this.Progress1.Name = "Progress1";
			this.Progress1.Size = new System.Drawing.Size(300, 19);
			this.Progress1.TabIndex = 23;
			// 
			// Progress2
			// 
			this.Progress2.Location = new System.Drawing.Point(12, 317);
			this.Progress2.Name = "Progress2";
			this.Progress2.Size = new System.Drawing.Size(300, 19);
			this.Progress2.TabIndex = 24;
			// 
			// Progress3
			// 
			this.Progress3.Location = new System.Drawing.Point(12, 342);
			this.Progress3.Name = "Progress3";
			this.Progress3.Size = new System.Drawing.Size(300, 19);
			this.Progress3.TabIndex = 25;
			// 
			// Progress4
			// 
			this.Progress4.Location = new System.Drawing.Point(12, 367);
			this.Progress4.Name = "Progress4";
			this.Progress4.Size = new System.Drawing.Size(300, 19);
			this.Progress4.TabIndex = 26;
			// 
			// LightSettingsGroupBox
			// 
			this.LightSettingsGroupBox.Controls.Add(this.label8);
			this.LightSettingsGroupBox.Controls.Add(this.MaxIntensity);
			this.LightSettingsGroupBox.Controls.Add(this.label7);
			this.LightSettingsGroupBox.Controls.Add(this.ReflectiveScale);
			this.LightSettingsGroupBox.Controls.Add(this.label6);
			this.LightSettingsGroupBox.Controls.Add(this.label5);
			this.LightSettingsGroupBox.Controls.Add(this.label4);
			this.LightSettingsGroupBox.Controls.Add(this.MinLightZ);
			this.LightSettingsGroupBox.Controls.Add(this.MinLightX);
			this.LightSettingsGroupBox.Controls.Add(this.MinLightY);
			this.LightSettingsGroupBox.Controls.Add(this.label3);
			this.LightSettingsGroupBox.Controls.Add(this.PatchSize);
			this.LightSettingsGroupBox.Controls.Add(this.label2);
			this.LightSettingsGroupBox.Controls.Add(this.LightScale);
			this.LightSettingsGroupBox.Controls.Add(this.label1);
			this.LightSettingsGroupBox.Controls.Add(this.NumBounce);
			this.LightSettingsGroupBox.Controls.Add(this.Radiosity);
			this.LightSettingsGroupBox.Controls.Add(this.ExtraSamples);
			this.LightSettingsGroupBox.Controls.Add(this.FastPatch);
			this.LightSettingsGroupBox.Location = new System.Drawing.Point(332, 12);
			this.LightSettingsGroupBox.Name = "LightSettingsGroupBox";
			this.LightSettingsGroupBox.Size = new System.Drawing.Size(133, 307);
			this.LightSettingsGroupBox.TabIndex = 27;
			this.LightSettingsGroupBox.TabStop = false;
			this.LightSettingsGroupBox.Text = "Light Settings";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(57, 272);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(69, 13);
			this.label8.TabIndex = 35;
			this.label8.Text = "Max Intensity";
			// 
			// MaxIntensity
			// 
			this.MaxIntensity.Location = new System.Drawing.Point(6, 270);
			this.MaxIntensity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.MaxIntensity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.MaxIntensity.Name = "MaxIntensity";
			this.MaxIntensity.Size = new System.Drawing.Size(45, 20);
			this.MaxIntensity.TabIndex = 34;
			this.MaxIntensity.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(57, 246);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(70, 13);
			this.label7.TabIndex = 28;
			this.label7.Text = "Mirror Reflect";
			// 
			// ReflectiveScale
			// 
			this.ReflectiveScale.DecimalPlaces = 2;
			this.ReflectiveScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.ReflectiveScale.Location = new System.Drawing.Point(6, 244);
			this.ReflectiveScale.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.ReflectiveScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.ReflectiveScale.Name = "ReflectiveScale";
			this.ReflectiveScale.Size = new System.Drawing.Size(45, 20);
			this.ReflectiveScale.TabIndex = 28;
			this.ReflectiveScale.Value = new decimal(new int[] {
            25,
            0,
            0,
            131072});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(57, 220);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 13);
			this.label6.TabIndex = 33;
			this.label6.Text = "Min Light B";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(57, 194);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(61, 13);
			this.label5.TabIndex = 32;
			this.label5.Text = "Min Light G";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(57, 168);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(61, 13);
			this.label4.TabIndex = 31;
			this.label4.Text = "Min Light R";
			// 
			// MinLightZ
			// 
			this.MinLightZ.Location = new System.Drawing.Point(6, 218);
			this.MinLightZ.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.MinLightZ.Name = "MinLightZ";
			this.MinLightZ.Size = new System.Drawing.Size(45, 20);
			this.MinLightZ.TabIndex = 29;
			// 
			// MinLightX
			// 
			this.MinLightX.Location = new System.Drawing.Point(6, 166);
			this.MinLightX.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.MinLightX.Name = "MinLightX";
			this.MinLightX.Size = new System.Drawing.Size(45, 20);
			this.MinLightX.TabIndex = 30;
			// 
			// MinLightY
			// 
			this.MinLightY.Location = new System.Drawing.Point(6, 192);
			this.MinLightY.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.MinLightY.Name = "MinLightY";
			this.MinLightY.Size = new System.Drawing.Size(45, 20);
			this.MinLightY.TabIndex = 28;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(57, 92);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(58, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Patch Size";
			// 
			// PatchSize
			// 
			this.PatchSize.Enabled = false;
			this.PatchSize.Location = new System.Drawing.Point(6, 88);
			this.PatchSize.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.PatchSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.PatchSize.Name = "PatchSize";
			this.PatchSize.Size = new System.Drawing.Size(45, 20);
			this.PatchSize.TabIndex = 8;
			this.PatchSize.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(55, 143);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Light Scale";
			// 
			// LightScale
			// 
			this.LightScale.DecimalPlaces = 2;
			this.LightScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.LightScale.Location = new System.Drawing.Point(6, 140);
			this.LightScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.LightScale.Name = "LightScale";
			this.LightScale.Size = new System.Drawing.Size(45, 20);
			this.LightScale.TabIndex = 6;
			this.LightScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(57, 117);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Bounces";
			// 
			// NumBounce
			// 
			this.NumBounce.Enabled = false;
			this.NumBounce.Location = new System.Drawing.Point(6, 114);
			this.NumBounce.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.NumBounce.Name = "NumBounce";
			this.NumBounce.Size = new System.Drawing.Size(45, 20);
			this.NumBounce.TabIndex = 4;
			// 
			// Radiosity
			// 
			this.Radiosity.AutoSize = true;
			this.Radiosity.Location = new System.Drawing.Point(6, 42);
			this.Radiosity.Name = "Radiosity";
			this.Radiosity.Size = new System.Drawing.Size(69, 17);
			this.Radiosity.TabIndex = 2;
			this.Radiosity.Text = "Radiosity";
			this.Radiosity.UseVisualStyleBackColor = true;
			this.Radiosity.CheckedChanged += new System.EventHandler(this.OnRadiosityChanged);
			// 
			// ExtraSamples
			// 
			this.ExtraSamples.AutoSize = true;
			this.ExtraSamples.Location = new System.Drawing.Point(6, 19);
			this.ExtraSamples.Name = "ExtraSamples";
			this.ExtraSamples.Size = new System.Drawing.Size(93, 17);
			this.ExtraSamples.TabIndex = 1;
			this.ExtraSamples.Text = "Extra Samples";
			this.ExtraSamples.UseVisualStyleBackColor = true;
			// 
			// FastPatch
			// 
			this.FastPatch.AutoSize = true;
			this.FastPatch.Enabled = false;
			this.FastPatch.Location = new System.Drawing.Point(6, 65);
			this.FastPatch.Name = "FastPatch";
			this.FastPatch.Size = new System.Drawing.Size(77, 17);
			this.FastPatch.TabIndex = 0;
			this.FastPatch.Text = "Fast Patch";
			this.FastPatch.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.SortPortals);
			this.groupBox1.Controls.Add(this.FullVis);
			this.groupBox1.Location = new System.Drawing.Point(12, 131);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(93, 64);
			this.groupBox1.TabIndex = 28;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Vis Settings";
			// 
			// SortPortals
			// 
			this.SortPortals.AutoSize = true;
			this.SortPortals.Checked = true;
			this.SortPortals.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SortPortals.Location = new System.Drawing.Point(6, 42);
			this.SortPortals.Name = "SortPortals";
			this.SortPortals.Size = new System.Drawing.Size(80, 17);
			this.SortPortals.TabIndex = 2;
			this.SortPortals.Text = "Sort Portals";
			this.SortPortals.UseVisualStyleBackColor = true;
			// 
			// FullVis
			// 
			this.FullVis.AutoSize = true;
			this.FullVis.Location = new System.Drawing.Point(6, 19);
			this.FullVis.Name = "FullVis";
			this.FullVis.Size = new System.Drawing.Size(59, 17);
			this.FullVis.TabIndex = 0;
			this.FullVis.Text = "Full Vis";
			this.FullVis.UseVisualStyleBackColor = true;
			this.FullVis.CheckedChanged += new System.EventHandler(this.OnFullVisChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(476, 561);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.LightSettingsGroupBox);
			this.Controls.Add(this.Progress4);
			this.Controls.Add(this.Progress3);
			this.Controls.Add(this.Progress2);
			this.Controls.Add(this.Progress1);
			this.Controls.Add(this.GroupFileIO);
			this.Controls.Add(this.StatsGroupBox);
			this.Controls.Add(this.GroupDrawSettings);
			this.Controls.Add(this.GroupBuildSettings);
			this.Controls.Add(this.ConsoleOut);
			this.Name = "MainForm";
			this.ShowInTaskbar = false;
			this.Text = "MainForm";
			this.StatsGroupBox.ResumeLayout(false);
			this.StatsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.MaxCPUCores)).EndInit();
			this.GroupFileIO.ResumeLayout(false);
			this.GroupBuildSettings.ResumeLayout(false);
			this.GroupBuildSettings.PerformLayout();
			this.GroupDrawSettings.ResumeLayout(false);
			this.LightSettingsGroupBox.ResumeLayout(false);
			this.LightSettingsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.MaxIntensity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ReflectiveScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightZ)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PatchSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.LightScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NumBounce)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button OpenBrushFile;
		private System.Windows.Forms.Label LabelNumRawFaces;
		private System.Windows.Forms.TextBox NumFaces;
		private System.Windows.Forms.GroupBox StatsGroupBox;
		private System.Windows.Forms.Label LabelNumCollisionFaces;
		private System.Windows.Forms.Label LabelNumDrawFaces;
		private System.Windows.Forms.TextBox NumAreas;
		private System.Windows.Forms.TextBox NumNodes;
		private System.Windows.Forms.Button SaveGBSP;
		private System.Windows.Forms.TextBox ConsoleOut;
		private System.Windows.Forms.ComboBox DrawChoice;
		private System.Windows.Forms.NumericUpDown MaxCPUCores;
		private System.Windows.Forms.Label LabelCPUCores;
		private System.Windows.Forms.GroupBox GroupFileIO;
		private System.Windows.Forms.Button BuildGBSP;
		private System.Windows.Forms.Button VisGBSP;
		private System.Windows.Forms.GroupBox GroupBuildSettings;
		private System.Windows.Forms.GroupBox GroupDrawSettings;
		private System.Windows.Forms.ProgressBar Progress1;
		private System.Windows.Forms.ProgressBar Progress2;
		private System.Windows.Forms.ProgressBar Progress3;
		private System.Windows.Forms.ProgressBar Progress4;
		private System.Windows.Forms.Button LightGBSP;
		private System.Windows.Forms.Label LabelNumPortals;
		private System.Windows.Forms.TextBox NumPortals;
		private System.Windows.Forms.CheckBox VerboseEntity;
		private System.Windows.Forms.CheckBox VerboseBSP;
		private System.Windows.Forms.GroupBox LightSettingsGroupBox;
		private System.Windows.Forms.CheckBox Radiosity;
		private System.Windows.Forms.CheckBox ExtraSamples;
		private System.Windows.Forms.CheckBox FastPatch;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown LightScale;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown NumBounce;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown PatchSize;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown MinLightZ;
		private System.Windows.Forms.NumericUpDown MinLightX;
		private System.Windows.Forms.NumericUpDown MinLightY;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown ReflectiveScale;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox SortPortals;
		private System.Windows.Forms.CheckBox FullVis;
		private System.Windows.Forms.Button LoadGBSP;
		private System.Windows.Forms.CheckBox FixTJunctions;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown MaxIntensity;
	}
}