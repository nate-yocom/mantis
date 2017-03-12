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
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MantisDotNet;
using MantisDotNet.WinForms.Sample.Utilities;

namespace MantisDotNet.WinForms.Sample
{
    public partial class BreweryViewer : Form
    {
        private Brewery m_brewery = null;
        private bool m_inRefresh = false;
        private bool m_breweryAvailable = true;

        public void UpdateBrewery(Brewery newBrewery)
        {
            m_brewery = newBrewery;            
            this.Invoke(() =>
            {
                m_breweryAvailable = true;
                MessageBox.Show("Brewery said hello, edits now allowed.");
                AvailableChanged();
                RefreshUI();
            });
            HookBrewery();
        }

        private void HookBrewery()
        {
            m_brewery.OnNetworkMessage += BreweryNetworkMessage;
            m_brewery.OnStatusText += BreweryStatusMessage;
            m_brewery.OnGoodbye += (o, a) =>
            {
                m_breweryAvailable = false;
                this.Invoke(() =>
                {
                    MessageBox.Show("Brewery said goodbye, no more edits allowed.");
                    AvailableChanged();                    
                });
                
            };
        }

        public BreweryViewer(Brewery brewery)
        {                        
            InitializeComponent();
            m_brewery = brewery;

            this.Text = "Brewery Viewer for " + m_brewery.ID;
            RefreshUI();
            HookBrewery();
            
            this.FormClosing += (sender, arg) =>
            {
                m_brewery.OnNetworkMessage -= BreweryNetworkMessage;
                m_brewery.OnStatusText -= BreweryStatusMessage;
            };

            m_networkList.DoubleClick += AssignNetworkPrompt;

            m_mashTarget.ValueChanged += (o, a) =>
            {
                if (!m_inRefresh && m_breweryAvailable)
                {
                    m_mashTarget.Enabled = false;
                    m_brewery.SetMashTargetAsync((float)m_mashTarget.Value).ContinueWith(t =>
                    {
                        m_mashTarget.Invoke(() => m_mashTarget.Enabled = true);
                    });
                }
            };

            m_hltTarget.ValueChanged += (o, a) =>
            {
                if (!m_inRefresh && m_breweryAvailable)
                {
                    m_hltTarget.Enabled = false;
                    m_brewery.SetHltTargetAsync((float)m_hltTarget.Value).ContinueWith(t =>
                    {
                        m_hltTarget.Invoke(() => m_hltTarget.Enabled = true);
                    });
                }
            };

            m_pump2ModeCombo.SelectedIndexChanged += (o, a) =>
            {
                if (!m_inRefresh && !m_brewery.RemoteState.RemoteControlled && m_breweryAvailable)
                {
                    m_pump2ModeCombo.Enabled = false;
                    MantisDotNet.Network.Protocol.AutoBrewPumpMode pumpMode = (Network.Protocol.AutoBrewPumpMode)Enum.Parse(typeof(Network.Protocol.AutoBrewPumpMode), (string)m_pump2ModeCombo.SelectedItem);
                    m_brewery.SetPump2ModeAsync(pumpMode).ContinueWith(t =>
                    {
                        m_pump2ModeCombo.Invoke(() => m_pump2ModeCombo.Enabled = true);
                    });
                }
            };

            m_remoteControl.CheckedChanged += (o, a) =>
            {
                if (!m_inRefresh && m_breweryAvailable)
                {
                    m_remoteControl.Enabled = false;
                    m_brewery.SetRemoteControlAsync(m_remoteControl.Checked).ContinueWith(t =>
                    {
                        m_remoteControl.Invoke(() => m_remoteControl.Enabled = true);
                    });
                }
            };

            m_pump1Checkbox.CheckedChanged += (o, a) =>
            {
                if (!m_inRefresh && m_breweryAvailable)
                {
                    m_pump1Checkbox.Enabled = false;
                    m_brewery.SetPump1StateAsync(m_pump1Checkbox.Checked).ContinueWith(t =>
                    {
                        m_pump1Checkbox.Invoke(() => m_pump1Checkbox.Enabled = true);
                    });
                }
            };

            m_pump2On.CheckedChanged += (o, a) =>
            {
                if (!m_inRefresh && m_breweryAvailable && m_brewery.RemoteState.RemoteControlled)
                {
                    m_pump2On.Enabled = false;
                    m_brewery.SetPump2StateAsync(m_pump2On.Checked).ContinueWith(t =>
                    {
                        m_pump2On.Invoke(() => m_pump2On.Enabled = true);
                    });
                }
            };

            m_burner.CheckedChanged += (o, a) =>
            {
                if (!m_inRefresh && m_brewery.RemoteState.RemoteControlled && m_breweryAvailable)
                {
                    m_burner.Enabled = false;
                    m_brewery.SetBurnerAsync(m_burner.Checked).ContinueWith(t =>
                    {
                        m_burner.Invoke(() => m_burner.Enabled = true);
                    });
                }
            };            
        }

        private void AssignNetworkPrompt(object sender, EventArgs e)
        {
            if(m_networkList.SelectedItems.Count > 0)
            {
                // Prompt user for password for the selected network, then join it
                MantisDotNet.Network.Protocol.SSID net = m_networkList.SelectedItems[0].Tag as MantisDotNet.Network.Protocol.SSID;
                if(net != null)
                {
                    WirelessJoinDialog dialog = new WirelessJoinDialog(net.Name);
                    if(dialog.ShowDialog() == DialogResult.OK)
                    {
                        m_brewery.SetWirelessNetworkAsync(net.Name, dialog.Password);
                    }
                }
            }
        }

        private void BreweryNetworkMessage(object sender, MantisDotNet.Brewery.BreweryNetworkEventArgs args)
        {
            this.Invoke(() => RefreshUI());
        }

        private void BreweryStatusMessage(object sender, MantisDotNet.Brewery.BreweryStatusTextEventArgs args)
        {
            m_statusList.Invoke(() =>
            {
                ListViewItem item = new ListViewItem(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                item.SubItems.Add(args.Message);
                m_statusList.Items.Add(item);
                m_statusList.Items[m_statusList.Items.Count - 1].EnsureVisible();
            });
        }

        private void AvailableChanged()
        {
            m_network.Enabled = m_breweryAvailable;
            m_brewer.Enabled = m_breweryAvailable;
            m_rate.Enabled = m_breweryAvailable;           
            m_mashTarget.Enabled = m_breweryAvailable;
            m_mashFloat.Enabled = m_breweryAvailable;            
            m_hltTarget.Enabled = m_breweryAvailable;
            m_hltFloat.Enabled = m_breweryAvailable;
            m_boilTemp.Enabled = m_breweryAvailable;
            m_pump1Checkbox.Enabled = m_breweryAvailable;
            m_pump2ModeCombo.Enabled = m_breweryAvailable;
            m_pump2On.Enabled = m_breweryAvailable;
            m_burner.Enabled = m_breweryAvailable;
            m_remoteControl.Enabled = m_breweryAvailable;
            m_networkList.Enabled = m_breweryAvailable;
            m_saveSettings.Enabled = m_breweryAvailable;
            m_refresh.Enabled = m_breweryAvailable;
        }
        
        private void RefreshUI()
        {
            m_inRefresh = true;
                       
            m_network.Text = m_brewery.Settings.Network;
            m_brewer.Text = m_brewery.Settings.CustomBrewer;            
            m_rate.Text = m_brewery.Settings.BroadcastRate.ToString();

            m_mashTemp.Text = m_brewery.RemoteState.MashTemp.ToString();
            m_mashTarget.Value = (decimal) m_brewery.RemoteState.MashTarget;            
            m_mashFloat.Checked = m_brewery.RemoteState.MashFloat;

            m_hltTemp.Text = m_brewery.RemoteState.HltTemp.ToString();
            m_hltTarget.Value = (decimal)m_brewery.RemoteState.HltTarget;
            m_hltFloat.Checked = m_brewery.RemoteState.HltFloat;

            m_boilTemp.Text = m_brewery.RemoteState.KettleTemp.ToString();

            m_pump2ModeCombo.SelectedItem = m_brewery.RemoteState.Pump2AutoMode.ToString();
            m_pump1Checkbox.Checked = m_brewery.RemoteState.Pump1On;
            m_pump2On.Checked = m_brewery.RemoteState.Pump2On;

            m_burner.Checked = m_brewery.RemoteState.BurnerOn;
            m_remoteControl.Checked = m_brewery.RemoteState.RemoteControlled;

            if(m_brewery.RemoteState.RemoteControlled)
            {
                m_pump2On.Enabled = true;
                m_pump2ModeCombo.Enabled = false;
                m_burner.Enabled = true;
            }
            else
            {
                m_pump2On.Enabled = false;
                m_pump2ModeCombo.Enabled = true;
                m_burner.Enabled = false;
            }

            m_networkList.BeginUpdate();            
            m_networkList.Items.Clear();
            foreach (MantisDotNet.Network.Protocol.SSID net in m_brewery.Networks)
            {
                if (net.Name != null)
                {
                    ListViewItem item = new ListViewItem(net.Name);
                    item.SubItems.AddRange(new string[]
                    {
                        net.Encryption,
                        net.RSSI.ToString()
                    });
                    item.Tag = net;
                    m_networkList.Items.Add(item);
                }
            }
            
            m_networkList.EndUpdate();
            m_inRefresh = false;
        }

        private void m_saveSettings_Click(object sender, EventArgs e)
        {
            var settings = m_brewery.Settings.GetCopy();
            settings.CustomBrewer = m_brewer.Text;
            settings.BroadcastRate = int.Parse(m_rate.Text);
            m_brewery.PushSettingsAsync(settings).ContinueWith((t) =>
            {
                this.Invoke(() => MessageBox.Show("Settings Saved"));
            });
        }

        private void m_refresh_Click(object sender, EventArgs e)
        {
            m_brewery.FullRefreshAsync().ContinueWith(t =>
            {
                this.Invoke(() => MessageBox.Show("Full refresh completed"));
            });
        }

        private void m_joinHiddenNet_Click(object sender, EventArgs e)
        {
            // Prompt user for password for the selected network, then join it            
            WirelessJoinHiddenDialog dialog = new WirelessJoinHiddenDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                m_brewery.SetWirelessNetworkAsync(dialog.Network, dialog.Password);
            }            
        }
    }
}
