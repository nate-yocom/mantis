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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MantisDotNet.Network;
using MantisDotNet.WinForms.Sample.Utilities;

namespace MantisDotNet.WinForms.Sample
{
    public partial class MainForm : Form
    {
        private ConcurrentDictionary<string, BreweryViewer> m_breweryViewers = new ConcurrentDictionary<string, BreweryViewer>();

        public MainForm()
        {            
            InitializeComponent();

            Mantis.Initialize();

            m_diagList.View = View.Details;
            Mantis.OnDiagnosticsEvent += (sender, arg) =>
            {
                m_diagList.Invoke(() =>
                {
                    ListViewItem item = new ListViewItem(arg.Timestamp.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                    item.SubItems.AddRange(new string[]
                    {                        
                        arg.Level.ToString(),
                        arg.Message
                    });
                    item.ToolTipText = string.Format("{0} {1}\t{2}\r\n", arg.Timestamp.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), arg.Level, arg.Message);
                    m_diagList.Items.Add(item);
                    m_diagList.Items[m_diagList.Items.Count - 1].EnsureVisible();
                });                    
            };

            MantisDotNet.Network.NetworkPump.HookRawMessageSends((ep, buffer) =>
            {
                if (RecordingInProgress && m_fileName != null)
                {
                    lock (m_recordingFileLock)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(m_fileName, true))
                        {
                            var record = new { Direction = "Send", Timestamp = DateTime.Now, Endpoint = ep.ToString(), Buffer = buffer, Text = Encoding.ASCII.GetString(buffer) };
                            file.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(record));
                        }
                    }
                }
            });

            MantisDotNet.Network.NetworkPump.HookRawMessagePump((result) =>
            {
                if (RecordingInProgress && m_fileName != null)
                {
                    lock (m_recordingFileLock)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(m_fileName, true))
                        {
                            var record = new { Direction = "Recv", Timestamp = DateTime.Now, Endpoint = result.RemoteEndPoint.ToString(), Buffer = result.Buffer, Text = Encoding.ASCII.GetString(result.Buffer) };
                            file.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(record));
                        }
                    }
                }

                m_networkList.Invoke(() =>
                {
                    DateTime now = DateTime.Now;
                    ListViewItem item = new ListViewItem(now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                    item.SubItems.AddRange(new string[]
                    {
                        "RECV",
                        result.RemoteEndPoint.ToString(),
                        Encoding.ASCII.GetString(result.Buffer)                        
                    });
                    item.ToolTipText = string.Format("{0} {1}\tRECV\t{2}\r\n", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), result.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result.Buffer));
                    m_networkList.Items.Add(item);
                    m_networkList.Items[m_networkList.Items.Count - 1].EnsureVisible();                    
                });                
            });

            MantisDotNet.Network.NetworkPump.HookRawMessageSends((endpoint, data) =>
            {
                m_networkList.Invoke(() =>
                {
                    DateTime now = DateTime.Now;
                    ListViewItem item = new ListViewItem(now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                    item.SubItems.AddRange(new string[]
                    {
                        "SEND",
                        endpoint.ToString(),
                        Encoding.ASCII.GetString(data)
                    });
                    item.ToolTipText = string.Format("{0} {1}\tSEND\t{2}\r\n", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), endpoint.ToString(), Encoding.ASCII.GetString(data));                    
                    m_networkList.Items.Add(item);
                    m_networkList.Items[m_networkList.Items.Count - 1].EnsureVisible();
                });
            });

            Mantis.OnNewBrewery += (sender, arg) =>
            {
                this.Invoke(() =>
                {
                    ToolStripItem item = m_breweriesMenu.DropDownItems.Find(arg.Brewery.ID, true).ToList().FirstOrDefault();
                    if (item == null)
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem();
                        menuItem.Name = arg.Brewery.ID;
                        menuItem.Text = "View " + arg.Brewery.ID + "...";
                        bool breweryViewerExists = m_breweryViewers.ContainsKey(arg.Brewery.ID);
                        menuItem.Click += (send, args) =>
                        {
                            // Already available? Show it, otherwise create                            
                            BreweryViewer viewer = m_breweryViewers.GetOrAdd(arg.Brewery.ID, (key) =>
                            {                                
                                BreweryViewer v = new BreweryViewer(arg.Brewery);
                                v.FormClosed += (e, a) =>
                                {
                                    BreweryViewer tmp = null;
                                    m_breweryViewers.TryRemove(arg.Brewery.ID, out tmp);
                                };
                                v.Show();
                                return v;
                            });
                            
                            viewer.WindowState = FormWindowState.Normal;
                            viewer.BringToFront();
                        };

                        if(breweryViewerExists)
                        {
                            BreweryViewer existingViewer = null;
                            if (m_breweryViewers.TryGetValue(arg.Brewery.ID, out existingViewer))
                                existingViewer.UpdateBrewery(arg.Brewery);                            
                        }
                        m_breweriesMenu.DropDownItems.Add(menuItem);
                    }
                });
            };
            
            Mantis.OnBreweryLeave += (sender, arg) =>
            {
                this.Invoke(() =>
                {
                    ToolStripItem item = m_breweriesMenu.DropDownItems.Find(arg.Brewery.ID, true).ToList().FirstOrDefault();
                    if(item != null)
                    {
                        m_breweriesMenu.DropDownItems.Remove(item);
                    }                    
                });
            };
        }

        private void discoverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mantis.Discover();
        }

        private bool m_recordingInProgress = false;
        private bool RecordingInProgress
        {
            get {  lock(m_recordingFileLock) { return m_recordingInProgress; } }
            set {  lock(m_recordingFileLock) { m_recordingInProgress = value; } }
        }

        private object m_recordingFileLock = new object();
        private string m_fileName = null;

        private void m_startRecordingRaw_Click(object sender, EventArgs e)
        {
            if(!RecordingInProgress)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.AddExtension = true;
                dialog.CheckFileExists = false;
                dialog.DefaultExt = "rec";
                dialog.OverwritePrompt = false;
                dialog.RestoreDirectory = true;
                dialog.Filter = "Network Recording (.rec)|*.rec|All Files (*.*)|*.*";
                dialog.Title = "Save recording to file ...";
                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    m_fileName = dialog.FileName;
                    RecordingInProgress = true;
                    m_startRecordingRaw.Text = "Stop Current Recording";
                }                
            }
            else
            {
                RecordingInProgress = false;
                m_startRecordingRaw.Text = "Start Recording...";                
            }
        }

        private void viewRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
