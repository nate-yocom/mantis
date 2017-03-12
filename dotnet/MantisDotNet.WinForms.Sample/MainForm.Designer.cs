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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.m_breweriesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.discoverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.recordingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_startRecordingRaw = new System.Windows.Forms.ToolStripMenuItem();
            this.m_mainTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.m_diagList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.m_networkList = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.m_mainTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_breweriesMenu,
            this.recordingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(788, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // m_breweriesMenu
            // 
            this.m_breweriesMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.discoverToolStripMenuItem,
            this.toolStripSeparator1});
            this.m_breweriesMenu.Name = "m_breweriesMenu";
            this.m_breweriesMenu.Size = new System.Drawing.Size(69, 20);
            this.m_breweriesMenu.Text = "Breweries";
            // 
            // discoverToolStripMenuItem
            // 
            this.discoverToolStripMenuItem.Name = "discoverToolStripMenuItem";
            this.discoverToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.discoverToolStripMenuItem.Text = "Discover";
            this.discoverToolStripMenuItem.Click += new System.EventHandler(this.discoverToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(116, 6);
            // 
            // recordingsToolStripMenuItem
            // 
            this.recordingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewRecordingToolStripMenuItem,
            this.m_startRecordingRaw});
            this.recordingsToolStripMenuItem.Name = "recordingsToolStripMenuItem";
            this.recordingsToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.recordingsToolStripMenuItem.Text = "Recordings";
            // 
            // viewRecordingToolStripMenuItem
            // 
            this.viewRecordingToolStripMenuItem.Name = "viewRecordingToolStripMenuItem";
            this.viewRecordingToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.viewRecordingToolStripMenuItem.Text = "View Recording...";
            this.viewRecordingToolStripMenuItem.Click += new System.EventHandler(this.viewRecordingToolStripMenuItem_Click);
            // 
            // m_startRecordingRaw
            // 
            this.m_startRecordingRaw.Name = "m_startRecordingRaw";
            this.m_startRecordingRaw.Size = new System.Drawing.Size(165, 22);
            this.m_startRecordingRaw.Text = "Start Recording...";
            this.m_startRecordingRaw.Click += new System.EventHandler(this.m_startRecordingRaw_Click);
            // 
            // m_mainTabs
            // 
            this.m_mainTabs.Controls.Add(this.tabPage1);
            this.m_mainTabs.Controls.Add(this.tabPage2);
            this.m_mainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_mainTabs.Location = new System.Drawing.Point(0, 24);
            this.m_mainTabs.Name = "m_mainTabs";
            this.m_mainTabs.SelectedIndex = 0;
            this.m_mainTabs.Size = new System.Drawing.Size(788, 451);
            this.m_mainTabs.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.m_diagList);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(780, 425);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Diagnostics";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // m_diagList
            // 
            this.m_diagList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.m_diagList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_diagList.FullRowSelect = true;
            this.m_diagList.Location = new System.Drawing.Point(3, 3);
            this.m_diagList.Name = "m_diagList";
            this.m_diagList.ShowItemToolTips = true;
            this.m_diagList.Size = new System.Drawing.Size(774, 419);
            this.m_diagList.TabIndex = 0;
            this.m_diagList.UseCompatibleStateImageBehavior = false;
            this.m_diagList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Timestamp";
            this.columnHeader1.Width = 165;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Level";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Message";
            this.columnHeader3.Width = 9999;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.m_networkList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(780, 425);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Network Activity";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // m_networkList
            // 
            this.m_networkList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.m_networkList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_networkList.FullRowSelect = true;
            this.m_networkList.Location = new System.Drawing.Point(3, 3);
            this.m_networkList.Name = "m_networkList";
            this.m_networkList.ShowItemToolTips = true;
            this.m_networkList.Size = new System.Drawing.Size(774, 419);
            this.m_networkList.TabIndex = 3;
            this.m_networkList.UseCompatibleStateImageBehavior = false;
            this.m_networkList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Timestamp";
            this.columnHeader4.Width = 184;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Direction";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Endpoint";
            this.columnHeader6.Width = 157;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Data";
            this.columnHeader7.Width = 10000;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 475);
            this.Controls.Add(this.m_mainTabs);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Brewery Control WinForms Sample";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.m_mainTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem m_breweriesMenu;
        private System.Windows.Forms.ToolStripMenuItem discoverToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TabControl m_mainTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripMenuItem recordingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewRecordingToolStripMenuItem;
        private System.Windows.Forms.ListView m_diagList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView m_networkList;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ToolStripMenuItem m_startRecordingRaw;
    }
}

