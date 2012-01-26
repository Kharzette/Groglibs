﻿namespace SharedForms
{
	partial class BSPForm
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
			this.GroupFileIO = new System.Windows.Forms.GroupBox();
			this.LightGBSP = new System.Windows.Forms.Button();
			this.BuildGBSP = new System.Windows.Forms.Button();
			this.OpenBrushFile = new System.Windows.Forms.Button();
			this.SaveGBSP = new System.Windows.Forms.Button();
			this.StatsGroupBox = new System.Windows.Forms.GroupBox();
			this.LabelNumPortals = new System.Windows.Forms.Label();
			this.NumPortals = new System.Windows.Forms.TextBox();
			this.LabelNumCollisionFaces = new System.Windows.Forms.Label();
			this.LabelNumDrawFaces = new System.Windows.Forms.Label();
			this.NumClusters = new System.Windows.Forms.TextBox();
			this.NumVerts = new System.Windows.Forms.TextBox();
			this.LabelNumRawFaces = new System.Windows.Forms.Label();
			this.NumPlanes = new System.Windows.Forms.TextBox();
			this.GroupBuildSettings = new System.Windows.Forms.GroupBox();
			this.label11 = new System.Windows.Forms.Label();
			this.WarpAsMirror = new System.Windows.Forms.CheckBox();
			this.MaxThreads = new System.Windows.Forms.NumericUpDown();
			this.SlickAsGouraud = new System.Windows.Forms.CheckBox();
			this.FixTJunctions = new System.Windows.Forms.CheckBox();
			this.VerboseEntity = new System.Windows.Forms.CheckBox();
			this.VerboseBSP = new System.Windows.Forms.CheckBox();
			this.LightSettingsGroupBox = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SurfaceLightStrength = new System.Windows.Forms.NumericUpDown();
			this.SurfaceLighting = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.NumSamples = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.LightGridSize = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.MaxIntensity = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.MinLightZ = new System.Windows.Forms.NumericUpDown();
			this.MinLightX = new System.Windows.Forms.NumericUpDown();
			this.MinLightY = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.SurfaceLightFrequency = new System.Windows.Forms.NumericUpDown();
			this.SeamCorrection = new System.Windows.Forms.CheckBox();
			this.mTips = new System.Windows.Forms.ToolTip(this.components);
			this.GroupFileIO.SuspendLayout();
			this.StatsGroupBox.SuspendLayout();
			this.GroupBuildSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MaxThreads)).BeginInit();
			this.LightSettingsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SurfaceLightStrength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NumSamples)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.LightGridSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MaxIntensity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightZ)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SurfaceLightFrequency)).BeginInit();
			this.SuspendLayout();
			// 
			// GroupFileIO
			// 
			this.GroupFileIO.Controls.Add(this.LightGBSP);
			this.GroupFileIO.Controls.Add(this.BuildGBSP);
			this.GroupFileIO.Controls.Add(this.OpenBrushFile);
			this.GroupFileIO.Controls.Add(this.SaveGBSP);
			this.GroupFileIO.Location = new System.Drawing.Point(12, 12);
			this.GroupFileIO.Name = "GroupFileIO";
			this.GroupFileIO.Size = new System.Drawing.Size(88, 137);
			this.GroupFileIO.TabIndex = 22;
			this.GroupFileIO.TabStop = false;
			this.GroupFileIO.Text = "File IO";
			// 
			// LightGBSP
			// 
			this.LightGBSP.Location = new System.Drawing.Point(6, 106);
			this.LightGBSP.Name = "LightGBSP";
			this.LightGBSP.Size = new System.Drawing.Size(75, 23);
			this.LightGBSP.TabIndex = 18;
			this.LightGBSP.Text = "Light GBSP";
			this.mTips.SetToolTip(this.LightGBSP, "Generate lightmaps for the level.  Needs vis data first.");
			this.LightGBSP.UseVisualStyleBackColor = true;
			this.LightGBSP.Click += new System.EventHandler(this.OnLightGBSP);
			// 
			// BuildGBSP
			// 
			this.BuildGBSP.Enabled = false;
			this.BuildGBSP.Location = new System.Drawing.Point(6, 48);
			this.BuildGBSP.Name = "BuildGBSP";
			this.BuildGBSP.Size = new System.Drawing.Size(75, 23);
			this.BuildGBSP.TabIndex = 17;
			this.BuildGBSP.Text = "Build GBSP";
			this.mTips.SetToolTip(this.BuildGBSP, "Build loaded data into saveable bsp and portal data");
			this.BuildGBSP.UseVisualStyleBackColor = true;
			this.BuildGBSP.Click += new System.EventHandler(this.OnBuildGBSP);
			// 
			// OpenBrushFile
			// 
			this.OpenBrushFile.Location = new System.Drawing.Point(6, 19);
			this.OpenBrushFile.Name = "OpenBrushFile";
			this.OpenBrushFile.Size = new System.Drawing.Size(75, 23);
			this.OpenBrushFile.TabIndex = 0;
			this.OpenBrushFile.Text = "Open Map";
			this.mTips.SetToolTip(this.OpenBrushFile, "Load convex volumes from .map or .vmf source files");
			this.OpenBrushFile.UseVisualStyleBackColor = true;
			this.OpenBrushFile.Click += new System.EventHandler(this.OnOpenMap);
			// 
			// SaveGBSP
			// 
			this.SaveGBSP.Enabled = false;
			this.SaveGBSP.Location = new System.Drawing.Point(6, 77);
			this.SaveGBSP.Name = "SaveGBSP";
			this.SaveGBSP.Size = new System.Drawing.Size(75, 23);
			this.SaveGBSP.TabIndex = 15;
			this.SaveGBSP.Text = "Save GBSP";
			this.mTips.SetToolTip(this.SaveGBSP, "Save built data to a .gbsp file");
			this.SaveGBSP.UseVisualStyleBackColor = true;
			this.SaveGBSP.Click += new System.EventHandler(this.OnSaveGBSP);
			// 
			// StatsGroupBox
			// 
			this.StatsGroupBox.Controls.Add(this.LabelNumPortals);
			this.StatsGroupBox.Controls.Add(this.NumPortals);
			this.StatsGroupBox.Controls.Add(this.LabelNumCollisionFaces);
			this.StatsGroupBox.Controls.Add(this.LabelNumDrawFaces);
			this.StatsGroupBox.Controls.Add(this.NumClusters);
			this.StatsGroupBox.Controls.Add(this.NumVerts);
			this.StatsGroupBox.Controls.Add(this.LabelNumRawFaces);
			this.StatsGroupBox.Controls.Add(this.NumPlanes);
			this.StatsGroupBox.Location = new System.Drawing.Point(106, 12);
			this.StatsGroupBox.Name = "StatsGroupBox";
			this.StatsGroupBox.Size = new System.Drawing.Size(134, 127);
			this.StatsGroupBox.TabIndex = 30;
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
			this.NumPortals.TabStop = false;
			// 
			// LabelNumCollisionFaces
			// 
			this.LabelNumCollisionFaces.AutoSize = true;
			this.LabelNumCollisionFaces.Location = new System.Drawing.Point(6, 74);
			this.LabelNumCollisionFaces.Name = "LabelNumCollisionFaces";
			this.LabelNumCollisionFaces.Size = new System.Drawing.Size(44, 13);
			this.LabelNumCollisionFaces.TabIndex = 18;
			this.LabelNumCollisionFaces.Text = "Clusters";
			// 
			// LabelNumDrawFaces
			// 
			this.LabelNumDrawFaces.AutoSize = true;
			this.LabelNumDrawFaces.Location = new System.Drawing.Point(6, 48);
			this.LabelNumDrawFaces.Name = "LabelNumDrawFaces";
			this.LabelNumDrawFaces.Size = new System.Drawing.Size(31, 13);
			this.LabelNumDrawFaces.TabIndex = 17;
			this.LabelNumDrawFaces.Text = "Verts";
			// 
			// NumClusters
			// 
			this.NumClusters.Location = new System.Drawing.Point(51, 71);
			this.NumClusters.Name = "NumClusters";
			this.NumClusters.ReadOnly = true;
			this.NumClusters.Size = new System.Drawing.Size(76, 20);
			this.NumClusters.TabIndex = 15;
			this.NumClusters.TabStop = false;
			this.mTips.SetToolTip(this.NumClusters, "Visibility clusters (leafs glommed together by contents)");
			// 
			// NumVerts
			// 
			this.NumVerts.Location = new System.Drawing.Point(51, 45);
			this.NumVerts.Name = "NumVerts";
			this.NumVerts.ReadOnly = true;
			this.NumVerts.Size = new System.Drawing.Size(76, 20);
			this.NumVerts.TabIndex = 14;
			this.NumVerts.TabStop = false;
			this.mTips.SetToolTip(this.NumVerts, "Vert count");
			// 
			// LabelNumRawFaces
			// 
			this.LabelNumRawFaces.AutoSize = true;
			this.LabelNumRawFaces.Location = new System.Drawing.Point(6, 22);
			this.LabelNumRawFaces.Name = "LabelNumRawFaces";
			this.LabelNumRawFaces.Size = new System.Drawing.Size(39, 13);
			this.LabelNumRawFaces.TabIndex = 12;
			this.LabelNumRawFaces.Text = "Planes";
			// 
			// NumPlanes
			// 
			this.NumPlanes.Cursor = System.Windows.Forms.Cursors.Default;
			this.NumPlanes.Location = new System.Drawing.Point(51, 19);
			this.NumPlanes.Name = "NumPlanes";
			this.NumPlanes.ReadOnly = true;
			this.NumPlanes.Size = new System.Drawing.Size(76, 20);
			this.NumPlanes.TabIndex = 13;
			this.NumPlanes.TabStop = false;
			this.mTips.SetToolTip(this.NumPlanes, "Shared plane count");
			// 
			// GroupBuildSettings
			// 
			this.GroupBuildSettings.Controls.Add(this.label11);
			this.GroupBuildSettings.Controls.Add(this.WarpAsMirror);
			this.GroupBuildSettings.Controls.Add(this.MaxThreads);
			this.GroupBuildSettings.Controls.Add(this.SlickAsGouraud);
			this.GroupBuildSettings.Controls.Add(this.FixTJunctions);
			this.GroupBuildSettings.Controls.Add(this.VerboseEntity);
			this.GroupBuildSettings.Controls.Add(this.VerboseBSP);
			this.GroupBuildSettings.Location = new System.Drawing.Point(246, 12);
			this.GroupBuildSettings.Name = "GroupBuildSettings";
			this.GroupBuildSettings.Size = new System.Drawing.Size(137, 163);
			this.GroupBuildSettings.TabIndex = 31;
			this.GroupBuildSettings.TabStop = false;
			this.GroupBuildSettings.Text = "Build Settings";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(57, 136);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(69, 13);
			this.label11.TabIndex = 34;
			this.label11.Text = "Max Threads";
			// 
			// WarpAsMirror
			// 
			this.WarpAsMirror.AutoSize = true;
			this.WarpAsMirror.Location = new System.Drawing.Point(6, 111);
			this.WarpAsMirror.Name = "WarpAsMirror";
			this.WarpAsMirror.Size = new System.Drawing.Size(90, 17);
			this.WarpAsMirror.TabIndex = 24;
			this.WarpAsMirror.Text = "Warp = Mirror";
			this.mTips.SetToolTip(this.WarpAsMirror, "Convert Q2 warp surface flags into mirror surfaces");
			this.WarpAsMirror.UseVisualStyleBackColor = true;
			// 
			// MaxThreads
			// 
			this.MaxThreads.Location = new System.Drawing.Point(6, 134);
			this.MaxThreads.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
			this.MaxThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.MaxThreads.Name = "MaxThreads";
			this.MaxThreads.Size = new System.Drawing.Size(45, 20);
			this.MaxThreads.TabIndex = 33;
			this.mTips.SetToolTip(this.MaxThreads, "Limit the active threads so builds don\'t cripple the machine");
			this.MaxThreads.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// SlickAsGouraud
			// 
			this.SlickAsGouraud.AutoSize = true;
			this.SlickAsGouraud.Location = new System.Drawing.Point(6, 88);
			this.SlickAsGouraud.Name = "SlickAsGouraud";
			this.SlickAsGouraud.Size = new System.Drawing.Size(102, 17);
			this.SlickAsGouraud.TabIndex = 23;
			this.SlickAsGouraud.Text = "Slick = Gouraud";
			this.mTips.SetToolTip(this.SlickAsGouraud, "Convert the Q2 slick flag into gouraud shading");
			this.SlickAsGouraud.UseVisualStyleBackColor = true;
			// 
			// FixTJunctions
			// 
			this.FixTJunctions.AutoSize = true;
			this.FixTJunctions.Checked = true;
			this.FixTJunctions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.FixTJunctions.Location = new System.Drawing.Point(6, 65);
			this.FixTJunctions.Name = "FixTJunctions";
			this.FixTJunctions.Size = new System.Drawing.Size(94, 17);
			this.FixTJunctions.TabIndex = 22;
			this.FixTJunctions.Text = "Fix TJunctions";
			this.mTips.SetToolTip(this.FixTJunctions, "Split faces with edges that form a T causing visible cracks in the world");
			this.FixTJunctions.UseVisualStyleBackColor = true;
			// 
			// VerboseEntity
			// 
			this.VerboseEntity.AutoSize = true;
			this.VerboseEntity.Location = new System.Drawing.Point(6, 42);
			this.VerboseEntity.Name = "VerboseEntity";
			this.VerboseEntity.Size = new System.Drawing.Size(104, 17);
			this.VerboseEntity.TabIndex = 21;
			this.VerboseEntity.Text = "BModel Verbose";
			this.mTips.SetToolTip(this.VerboseEntity, "Spam for the moving brush models (doors / elevators etc)");
			this.VerboseEntity.UseVisualStyleBackColor = true;
			// 
			// VerboseBSP
			// 
			this.VerboseBSP.AutoSize = true;
			this.VerboseBSP.Location = new System.Drawing.Point(6, 19);
			this.VerboseBSP.Name = "VerboseBSP";
			this.VerboseBSP.Size = new System.Drawing.Size(65, 17);
			this.VerboseBSP.TabIndex = 20;
			this.VerboseBSP.Text = "Verbose";
			this.mTips.SetToolTip(this.VerboseBSP, "Turns on a lot of spam, which can slow down the build");
			this.VerboseBSP.UseVisualStyleBackColor = true;
			this.VerboseBSP.CheckedChanged += new System.EventHandler(this.OnVerbose);
			// 
			// LightSettingsGroupBox
			// 
			this.LightSettingsGroupBox.Controls.Add(this.label1);
			this.LightSettingsGroupBox.Controls.Add(this.SurfaceLightStrength);
			this.LightSettingsGroupBox.Controls.Add(this.SurfaceLighting);
			this.LightSettingsGroupBox.Controls.Add(this.label10);
			this.LightSettingsGroupBox.Controls.Add(this.NumSamples);
			this.LightSettingsGroupBox.Controls.Add(this.label9);
			this.LightSettingsGroupBox.Controls.Add(this.LightGridSize);
			this.LightSettingsGroupBox.Controls.Add(this.label8);
			this.LightSettingsGroupBox.Controls.Add(this.MaxIntensity);
			this.LightSettingsGroupBox.Controls.Add(this.label6);
			this.LightSettingsGroupBox.Controls.Add(this.label5);
			this.LightSettingsGroupBox.Controls.Add(this.label4);
			this.LightSettingsGroupBox.Controls.Add(this.MinLightZ);
			this.LightSettingsGroupBox.Controls.Add(this.MinLightX);
			this.LightSettingsGroupBox.Controls.Add(this.MinLightY);
			this.LightSettingsGroupBox.Controls.Add(this.label3);
			this.LightSettingsGroupBox.Controls.Add(this.SurfaceLightFrequency);
			this.LightSettingsGroupBox.Controls.Add(this.SeamCorrection);
			this.LightSettingsGroupBox.Location = new System.Drawing.Point(12, 181);
			this.LightSettingsGroupBox.Name = "LightSettingsGroupBox";
			this.LightSettingsGroupBox.Size = new System.Drawing.Size(371, 132);
			this.LightSettingsGroupBox.TabIndex = 32;
			this.LightSettingsGroupBox.TabStop = false;
			this.LightSettingsGroupBox.Text = "Light Settings";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(167, 47);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 42;
			this.label1.Text = "Surf Strength";
			// 
			// SurfaceLightStrength
			// 
			this.SurfaceLightStrength.Location = new System.Drawing.Point(116, 45);
			this.SurfaceLightStrength.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
			this.SurfaceLightStrength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.SurfaceLightStrength.Name = "SurfaceLightStrength";
			this.SurfaceLightStrength.Size = new System.Drawing.Size(45, 20);
			this.SurfaceLightStrength.TabIndex = 41;
			this.mTips.SetToolTip(this.SurfaceLightStrength, "Strength of emitted tiny surface lights");
			this.SurfaceLightStrength.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
			// 
			// SurfaceLighting
			// 
			this.SurfaceLighting.AutoSize = true;
			this.SurfaceLighting.Checked = true;
			this.SurfaceLighting.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SurfaceLighting.Location = new System.Drawing.Point(6, 43);
			this.SurfaceLighting.Name = "SurfaceLighting";
			this.SurfaceLighting.Size = new System.Drawing.Size(103, 17);
			this.SurfaceLighting.TabIndex = 40;
			this.SurfaceLighting.Text = "Surface Lighting";
			this.mTips.SetToolTip(this.SurfaceLighting, "Surfaces marked light emit light.  This can get very slow");
			this.SurfaceLighting.UseVisualStyleBackColor = true;
			this.SurfaceLighting.CheckedChanged += new System.EventHandler(this.OnSurfaceLighting);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(167, 73);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(47, 13);
			this.label10.TabIndex = 39;
			this.label10.Text = "Samples";
			// 
			// NumSamples
			// 
			this.NumSamples.Location = new System.Drawing.Point(116, 71);
			this.NumSamples.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this.NumSamples.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.NumSamples.Name = "NumSamples";
			this.NumSamples.Size = new System.Drawing.Size(45, 20);
			this.NumSamples.TabIndex = 38;
			this.mTips.SetToolTip(this.NumSamples, "Extra ray samples to soften the results");
			this.NumSamples.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(293, 21);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(68, 13);
			this.label9.TabIndex = 37;
			this.label9.Text = "Light Density";
			// 
			// LightGridSize
			// 
			this.LightGridSize.Location = new System.Drawing.Point(242, 19);
			this.LightGridSize.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.LightGridSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.LightGridSize.Name = "LightGridSize";
			this.LightGridSize.Size = new System.Drawing.Size(45, 20);
			this.LightGridSize.TabIndex = 36;
			this.mTips.SetToolTip(this.LightGridSize, "World units per light map texel");
			this.LightGridSize.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(167, 99);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(69, 13);
			this.label8.TabIndex = 35;
			this.label8.Text = "Max Intensity";
			// 
			// MaxIntensity
			// 
			this.MaxIntensity.Location = new System.Drawing.Point(116, 97);
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
			this.mTips.SetToolTip(this.MaxIntensity, "Maximum light intensity");
			this.MaxIntensity.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(293, 99);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 13);
			this.label6.TabIndex = 33;
			this.label6.Text = "Min Light B";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(293, 73);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(61, 13);
			this.label5.TabIndex = 32;
			this.label5.Text = "Min Light G";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(293, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(61, 13);
			this.label4.TabIndex = 31;
			this.label4.Text = "Min Light R";
			// 
			// MinLightZ
			// 
			this.MinLightZ.Location = new System.Drawing.Point(242, 97);
			this.MinLightZ.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.MinLightZ.Name = "MinLightZ";
			this.MinLightZ.Size = new System.Drawing.Size(45, 20);
			this.MinLightZ.TabIndex = 29;
			this.mTips.SetToolTip(this.MinLightZ, "Level wide ambient blue");
			// 
			// MinLightX
			// 
			this.MinLightX.Location = new System.Drawing.Point(242, 45);
			this.MinLightX.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.MinLightX.Name = "MinLightX";
			this.MinLightX.Size = new System.Drawing.Size(45, 20);
			this.MinLightX.TabIndex = 30;
			this.mTips.SetToolTip(this.MinLightX, "Level wide ambient red");
			// 
			// MinLightY
			// 
			this.MinLightY.Location = new System.Drawing.Point(242, 71);
			this.MinLightY.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.MinLightY.Name = "MinLightY";
			this.MinLightY.Size = new System.Drawing.Size(45, 20);
			this.MinLightY.TabIndex = 28;
			this.mTips.SetToolTip(this.MinLightY, "Level wide ambient green");
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(167, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(50, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Surf Freq";
			// 
			// SurfaceLightFrequency
			// 
			this.SurfaceLightFrequency.Increment = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.SurfaceLightFrequency.Location = new System.Drawing.Point(116, 19);
			this.SurfaceLightFrequency.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.SurfaceLightFrequency.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.SurfaceLightFrequency.Name = "SurfaceLightFrequency";
			this.SurfaceLightFrequency.Size = new System.Drawing.Size(45, 20);
			this.SurfaceLightFrequency.TabIndex = 8;
			this.mTips.SetToolTip(this.SurfaceLightFrequency, "Emit a light every x world units for surface lighting");
			this.SurfaceLightFrequency.Value = new decimal(new int[] {
            48,
            0,
            0,
            0});
			// 
			// SeamCorrection
			// 
			this.SeamCorrection.AutoSize = true;
			this.SeamCorrection.Checked = true;
			this.SeamCorrection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SeamCorrection.Location = new System.Drawing.Point(6, 20);
			this.SeamCorrection.Name = "SeamCorrection";
			this.SeamCorrection.Size = new System.Drawing.Size(104, 17);
			this.SeamCorrection.TabIndex = 1;
			this.SeamCorrection.Text = "Seam Correction";
			this.mTips.SetToolTip(this.SeamCorrection, "Shifts light points around a bit to help points just inside solid");
			this.SeamCorrection.UseVisualStyleBackColor = true;
			// 
			// BSPForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(394, 323);
			this.ControlBox = false;
			this.Controls.Add(this.LightSettingsGroupBox);
			this.Controls.Add(this.GroupBuildSettings);
			this.Controls.Add(this.StatsGroupBox);
			this.Controls.Add(this.GroupFileIO);
			this.Name = "BSPForm";
			this.Text = "BSPForm";
			this.GroupFileIO.ResumeLayout(false);
			this.StatsGroupBox.ResumeLayout(false);
			this.StatsGroupBox.PerformLayout();
			this.GroupBuildSettings.ResumeLayout(false);
			this.GroupBuildSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.MaxThreads)).EndInit();
			this.LightSettingsGroupBox.ResumeLayout(false);
			this.LightSettingsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.SurfaceLightStrength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NumSamples)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.LightGridSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MaxIntensity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightZ)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MinLightY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SurfaceLightFrequency)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox GroupFileIO;
		private System.Windows.Forms.Button LightGBSP;
		private System.Windows.Forms.Button BuildGBSP;
		private System.Windows.Forms.Button OpenBrushFile;
		private System.Windows.Forms.Button SaveGBSP;
		private System.Windows.Forms.GroupBox StatsGroupBox;
		private System.Windows.Forms.Label LabelNumPortals;
		private System.Windows.Forms.TextBox NumPortals;
		private System.Windows.Forms.Label LabelNumCollisionFaces;
		private System.Windows.Forms.Label LabelNumDrawFaces;
		private System.Windows.Forms.TextBox NumClusters;
		private System.Windows.Forms.TextBox NumVerts;
		private System.Windows.Forms.Label LabelNumRawFaces;
		private System.Windows.Forms.TextBox NumPlanes;
		private System.Windows.Forms.GroupBox GroupBuildSettings;
		private System.Windows.Forms.CheckBox WarpAsMirror;
		private System.Windows.Forms.CheckBox SlickAsGouraud;
		private System.Windows.Forms.CheckBox FixTJunctions;
		private System.Windows.Forms.CheckBox VerboseEntity;
		private System.Windows.Forms.CheckBox VerboseBSP;
		private System.Windows.Forms.GroupBox LightSettingsGroupBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown LightGridSize;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown MaxIntensity;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown MinLightZ;
		private System.Windows.Forms.NumericUpDown MinLightX;
		private System.Windows.Forms.NumericUpDown MinLightY;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown SurfaceLightFrequency;
		private System.Windows.Forms.CheckBox SeamCorrection;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown NumSamples;
		private System.Windows.Forms.NumericUpDown MaxThreads;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox SurfaceLighting;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown SurfaceLightStrength;
		private System.Windows.Forms.ToolTip mTips;
	}
}