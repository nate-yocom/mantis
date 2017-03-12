/**
 * Copyright (c) 2016, Nate Yocom (nate@yocom.org)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project. 
 **/
 
namespace MantisDotNet.WinForms.Sample
{
    partial class BreweryViewer
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_remoteControl = new System.Windows.Forms.CheckBox();
            this.m_saveSettings = new System.Windows.Forms.Button();
            this.m_rate = new System.Windows.Forms.TextBox();
            this.m_brewer = new System.Windows.Forms.TextBox();
            this.m_network = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.m_networkList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.m_boilTemp = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.m_mashTarget = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.m_mashTemp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_mashFloat = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.m_burner = new System.Windows.Forms.CheckBox();
            this.m_hltTarget = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.m_hltTemp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.m_hltFloat = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.m_pump2On = new System.Windows.Forms.CheckBox();
            this.m_pump1Checkbox = new System.Windows.Forms.CheckBox();
            this.m_pump2ModeCombo = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.m_statusList = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_refresh = new System.Windows.Forms.Button();
            this.m_joinHiddenNet = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_mashTarget)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_hltTarget)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_remoteControl);
            this.groupBox1.Controls.Add(this.m_saveSettings);
            this.groupBox1.Controls.Add(this.m_rate);
            this.groupBox1.Controls.Add(this.m_brewer);
            this.groupBox1.Controls.Add(this.m_network);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(347, 158);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // m_remoteControl
            // 
            this.m_remoteControl.AutoSize = true;
            this.m_remoteControl.Location = new System.Drawing.Point(16, 122);
            this.m_remoteControl.Name = "m_remoteControl";
            this.m_remoteControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.m_remoteControl.Size = new System.Drawing.Size(99, 17);
            this.m_remoteControl.TabIndex = 7;
            this.m_remoteControl.Text = "Remote Control";
            this.m_remoteControl.UseVisualStyleBackColor = true;
            // 
            // m_saveSettings
            // 
            this.m_saveSettings.Location = new System.Drawing.Point(255, 87);
            this.m_saveSettings.Name = "m_saveSettings";
            this.m_saveSettings.Size = new System.Drawing.Size(75, 23);
            this.m_saveSettings.TabIndex = 6;
            this.m_saveSettings.Text = "Save";
            this.m_saveSettings.UseVisualStyleBackColor = true;
            this.m_saveSettings.Click += new System.EventHandler(this.m_saveSettings_Click);
            // 
            // m_rate
            // 
            this.m_rate.Location = new System.Drawing.Point(108, 87);
            this.m_rate.Name = "m_rate";
            this.m_rate.Size = new System.Drawing.Size(106, 20);
            this.m_rate.TabIndex = 5;
            // 
            // m_brewer
            // 
            this.m_brewer.Location = new System.Drawing.Point(108, 57);
            this.m_brewer.Name = "m_brewer";
            this.m_brewer.Size = new System.Drawing.Size(222, 20);
            this.m_brewer.TabIndex = 4;
            // 
            // m_network
            // 
            this.m_network.Location = new System.Drawing.Point(108, 27);
            this.m_network.Name = "m_network";
            this.m_network.ReadOnly = true;
            this.m_network.Size = new System.Drawing.Size(222, 20);
            this.m_network.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Broadcast Rate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Brewer Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Network:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.m_joinHiddenNet);
            this.groupBox2.Controls.Add(this.m_networkList);
            this.groupBox2.Location = new System.Drawing.Point(374, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(374, 158);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Visible Networks";
            // 
            // m_networkList
            // 
            this.m_networkList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.m_networkList.FullRowSelect = true;
            this.m_networkList.Location = new System.Drawing.Point(3, 16);
            this.m_networkList.Name = "m_networkList";
            this.m_networkList.Size = new System.Drawing.Size(368, 107);
            this.m_networkList.TabIndex = 0;
            this.m_networkList.UseCompatibleStateImageBehavior = false;
            this.m_networkList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 122;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Encryption";
            this.columnHeader2.Width = 131;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Signal (dBm)";
            this.columnHeader3.Width = 74;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.m_boilTemp);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(12, 176);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(146, 140);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Boil Kettle";
            // 
            // m_boilTemp
            // 
            this.m_boilTemp.Location = new System.Drawing.Point(87, 29);
            this.m_boilTemp.Name = "m_boilTemp";
            this.m_boilTemp.ReadOnly = true;
            this.m_boilTemp.Size = new System.Drawing.Size(45, 20);
            this.m_boilTemp.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Temperature:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.m_mashTarget);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.m_mashTemp);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.m_mashFloat);
            this.groupBox4.Location = new System.Drawing.Point(169, 176);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(173, 140);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Mash Tun";
            // 
            // m_mashTarget
            // 
            this.m_mashTarget.Location = new System.Drawing.Point(90, 58);
            this.m_mashTarget.Maximum = new decimal(new int[] {
            212,
            0,
            0,
            0});
            this.m_mashTarget.Name = "m_mashTarget";
            this.m_mashTarget.Size = new System.Drawing.Size(59, 20);
            this.m_mashTarget.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Target:";
            // 
            // m_mashTemp
            // 
            this.m_mashTemp.Location = new System.Drawing.Point(90, 29);
            this.m_mashTemp.Name = "m_mashTemp";
            this.m_mashTemp.ReadOnly = true;
            this.m_mashTemp.Size = new System.Drawing.Size(59, 20);
            this.m_mashTemp.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Temperature:";
            // 
            // m_mashFloat
            // 
            this.m_mashFloat.AutoSize = true;
            this.m_mashFloat.Enabled = false;
            this.m_mashFloat.Location = new System.Drawing.Point(17, 100);
            this.m_mashFloat.Name = "m_mashFloat";
            this.m_mashFloat.Size = new System.Drawing.Size(49, 17);
            this.m_mashFloat.TabIndex = 1;
            this.m_mashFloat.Text = "Float";
            this.m_mashFloat.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.m_burner);
            this.groupBox5.Controls.Add(this.m_hltTarget);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.m_hltTemp);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.m_hltFloat);
            this.groupBox5.Location = new System.Drawing.Point(348, 176);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(190, 140);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "HLT";
            // 
            // m_burner
            // 
            this.m_burner.AutoSize = true;
            this.m_burner.Location = new System.Drawing.Point(23, 112);
            this.m_burner.Name = "m_burner";
            this.m_burner.Size = new System.Drawing.Size(57, 17);
            this.m_burner.TabIndex = 14;
            this.m_burner.Text = "Burner";
            this.m_burner.UseVisualStyleBackColor = true;
            // 
            // m_hltTarget
            // 
            this.m_hltTarget.Location = new System.Drawing.Point(96, 58);
            this.m_hltTarget.Maximum = new decimal(new int[] {
            212,
            0,
            0,
            0});
            this.m_hltTarget.Name = "m_hltTarget";
            this.m_hltTarget.Size = new System.Drawing.Size(59, 20);
            this.m_hltTarget.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Target:";
            // 
            // m_hltTemp
            // 
            this.m_hltTemp.Location = new System.Drawing.Point(96, 29);
            this.m_hltTemp.Name = "m_hltTemp";
            this.m_hltTemp.ReadOnly = true;
            this.m_hltTemp.Size = new System.Drawing.Size(59, 20);
            this.m_hltTemp.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Temperature:";
            // 
            // m_hltFloat
            // 
            this.m_hltFloat.AutoSize = true;
            this.m_hltFloat.Enabled = false;
            this.m_hltFloat.Location = new System.Drawing.Point(23, 89);
            this.m_hltFloat.Name = "m_hltFloat";
            this.m_hltFloat.Size = new System.Drawing.Size(49, 17);
            this.m_hltFloat.TabIndex = 2;
            this.m_hltFloat.Text = "Float";
            this.m_hltFloat.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.m_pump2On);
            this.groupBox6.Controls.Add(this.m_pump1Checkbox);
            this.groupBox6.Controls.Add(this.m_pump2ModeCombo);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Location = new System.Drawing.Point(544, 176);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(201, 140);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Pumps";
            // 
            // m_pump2On
            // 
            this.m_pump2On.AutoSize = true;
            this.m_pump2On.Enabled = false;
            this.m_pump2On.Location = new System.Drawing.Point(13, 89);
            this.m_pump2On.Name = "m_pump2On";
            this.m_pump2On.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.m_pump2On.Size = new System.Drawing.Size(62, 17);
            this.m_pump2On.TabIndex = 3;
            this.m_pump2On.Text = "Pump 2";
            this.m_pump2On.UseVisualStyleBackColor = true;
            // 
            // m_pump1Checkbox
            // 
            this.m_pump1Checkbox.AutoSize = true;
            this.m_pump1Checkbox.Location = new System.Drawing.Point(13, 66);
            this.m_pump1Checkbox.Name = "m_pump1Checkbox";
            this.m_pump1Checkbox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.m_pump1Checkbox.Size = new System.Drawing.Size(62, 17);
            this.m_pump1Checkbox.TabIndex = 2;
            this.m_pump1Checkbox.Text = "Pump 1";
            this.m_pump1Checkbox.UseVisualStyleBackColor = true;
            // 
            // m_pump2ModeCombo
            // 
            this.m_pump2ModeCombo.FormattingEnabled = true;
            this.m_pump2ModeCombo.Items.AddRange(new object[] {
            "Off",
            "Temperature",
            "FloatSwitch",
            "On"});
            this.m_pump2ModeCombo.Location = new System.Drawing.Point(95, 29);
            this.m_pump2ModeCombo.Name = "m_pump2ModeCombo";
            this.m_pump2ModeCombo.Size = new System.Drawing.Size(91, 21);
            this.m_pump2ModeCombo.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Pump 2 Mode:";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.m_statusList);
            this.groupBox7.Location = new System.Drawing.Point(16, 359);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(729, 129);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Status";
            // 
            // m_statusList
            // 
            this.m_statusList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.m_statusList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_statusList.Location = new System.Drawing.Point(3, 16);
            this.m_statusList.Name = "m_statusList";
            this.m_statusList.Size = new System.Drawing.Size(723, 110);
            this.m_statusList.TabIndex = 0;
            this.m_statusList.UseCompatibleStateImageBehavior = false;
            this.m_statusList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Timestamp";
            this.columnHeader4.Width = 133;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Message";
            this.columnHeader5.Width = 577;
            // 
            // m_refresh
            // 
            this.m_refresh.Location = new System.Drawing.Point(667, 330);
            this.m_refresh.Name = "m_refresh";
            this.m_refresh.Size = new System.Drawing.Size(75, 23);
            this.m_refresh.TabIndex = 10;
            this.m_refresh.Text = "Full Refresh";
            this.m_refresh.UseVisualStyleBackColor = true;
            this.m_refresh.Click += new System.EventHandler(this.m_refresh_Click);
            // 
            // m_joinHiddenNet
            // 
            this.m_joinHiddenNet.Location = new System.Drawing.Point(170, 129);
            this.m_joinHiddenNet.Name = "m_joinHiddenNet";
            this.m_joinHiddenNet.Size = new System.Drawing.Size(198, 23);
            this.m_joinHiddenNet.TabIndex = 1;
            this.m_joinHiddenNet.Text = "Join Hidden Network";
            this.m_joinHiddenNet.UseVisualStyleBackColor = true;
            this.m_joinHiddenNet.Click += new System.EventHandler(this.m_joinHiddenNet_Click);
            // 
            // BreweryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 502);
            this.Controls.Add(this.m_refresh);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BreweryViewer";
            this.Text = "Brewery View for";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_mashTarget)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_hltTarget)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox m_rate;
        private System.Windows.Forms.TextBox m_brewer;
        private System.Windows.Forms.TextBox m_network;
        private System.Windows.Forms.ListView m_networkList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button m_saveSettings;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox m_mashFloat;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox m_hltFloat;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox m_mashTemp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox m_hltTemp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox m_boilTemp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox m_pump2ModeCombo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox m_pump1Checkbox;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ListView m_statusList;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.CheckBox m_pump2On;
        private System.Windows.Forms.Button m_refresh;
        private System.Windows.Forms.NumericUpDown m_mashTarget;
        private System.Windows.Forms.NumericUpDown m_hltTarget;
        private System.Windows.Forms.CheckBox m_remoteControl;
        private System.Windows.Forms.CheckBox m_burner;
        private System.Windows.Forms.Button m_joinHiddenNet;
    }
}