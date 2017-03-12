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
    partial class WirelessJoinDialog
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
            this.m_label = new System.Windows.Forms.Label();
            this.m_ssidPassword = new System.Windows.Forms.TextBox();
            this.m_ok = new System.Windows.Forms.Button();
            this.m_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_label
            // 
            this.m_label.AutoSize = true;
            this.m_label.Location = new System.Drawing.Point(27, 24);
            this.m_label.Name = "m_label";
            this.m_label.Size = new System.Drawing.Size(207, 13);
            this.m_label.TabIndex = 0;
            this.m_label.Text = "Joining network - please provide password";
            // 
            // m_ssidPassword
            // 
            this.m_ssidPassword.Location = new System.Drawing.Point(30, 49);
            this.m_ssidPassword.Name = "m_ssidPassword";
            this.m_ssidPassword.PasswordChar = '*';
            this.m_ssidPassword.Size = new System.Drawing.Size(298, 20);
            this.m_ssidPassword.TabIndex = 1;
            // 
            // m_ok
            // 
            this.m_ok.Location = new System.Drawing.Point(253, 75);
            this.m_ok.Name = "m_ok";
            this.m_ok.Size = new System.Drawing.Size(75, 23);
            this.m_ok.TabIndex = 2;
            this.m_ok.Text = "Join";
            this.m_ok.UseVisualStyleBackColor = true;
            this.m_ok.Click += new System.EventHandler(this.m_ok_Click);
            // 
            // m_cancel
            // 
            this.m_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_cancel.Location = new System.Drawing.Point(172, 75);
            this.m_cancel.Name = "m_cancel";
            this.m_cancel.Size = new System.Drawing.Size(75, 23);
            this.m_cancel.TabIndex = 3;
            this.m_cancel.Text = "Cancel";
            this.m_cancel.UseVisualStyleBackColor = true;
            this.m_cancel.Click += new System.EventHandler(this.m_cancel_Click);
            // 
            // WirelessJoinDialog
            // 
            this.AcceptButton = this.m_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_cancel;
            this.ClientSize = new System.Drawing.Size(365, 126);
            this.ControlBox = false;
            this.Controls.Add(this.m_cancel);
            this.Controls.Add(this.m_ok);
            this.Controls.Add(this.m_ssidPassword);
            this.Controls.Add(this.m_label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WirelessJoinDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Joining ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_label;
        private System.Windows.Forms.TextBox m_ssidPassword;
        private System.Windows.Forms.Button m_ok;
        private System.Windows.Forms.Button m_cancel;
    }
}