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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using MantisDotNet.Utilities;

namespace MantisDotNet.Network
{    
    public static class NetworkPump
    {        
        private static CancellationTokenSource s_tokenSource = new CancellationTokenSource();
        private static CancellationToken s_cancelToken = s_tokenSource.Token;
        private static UdpClient s_client = null;
        private static List<Action<Protocol.BreweryMessage>> s_breweryMessages = new List<Action<Protocol.BreweryMessage>>();
        private static List<Action<UdpReceiveResult>> s_rawMessagePump = new List<Action<UdpReceiveResult>>();
        private static List<Action<IPEndPoint, byte[]>> s_rawMessageSends = new List<Action<IPEndPoint, byte[]>>();        
        private static DateTime s_lastNetworkScan = DateTime.MinValue;

        internal static void Shutdown()
        {
            s_tokenSource.Cancel();                   
        }

        internal static void Initialize()
        {
            s_client = new UdpClient(new IPEndPoint(IPAddress.Any, Protocol.Constants.DEFAULT_PORT));
            s_client.DontFragment = true;
            s_client.EnableBroadcast = true;
            s_client.MulticastLoopback = false;

            AsyncListener();
        }

        private static async void AsyncListener()
        {
            while (!s_cancelToken.IsCancellationRequested)
            {
                s_cancelToken.ThrowIfCancellationRequested();
                UdpReceiveResult result = await s_client.ReceiveAsync();
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    UdpReceiveResult data = (UdpReceiveResult)o;
                    DataReceived(data);
                }, result);                
            }
        }

        public static void HookRawMessagePump(Action<UdpReceiveResult> callback)
        {
            s_rawMessagePump.Add((u) => { try { callback(u); } catch(Exception ex) { Mantis.Warning("Raw message recv consumer may miss message: {0}", ex.Message); } });
        }

        public static void HookRawMessageSends(Action<IPEndPoint, byte[]> callback)
        {
            s_rawMessageSends.Add((e, b) => { try { callback(e, b); } catch (Exception ex) { Mantis.Warning("Raw message send consumer may miss message: {0}", ex.Message); } });
        }

        public static void HookBreweryMessages(Action<Protocol.BreweryMessage> callback)
        {
            s_breweryMessages.Add((b) => { try { callback(b); } catch (Exception ex) { Mantis.Warning("Brewery message recv consumer may miss message: {0}", ex.Message); } });
        }

        public static void Send(Protocol.ControlMessage message)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(json);
            SendOnAll(buffer);
        }
        
        private static IEnumerable<IPEndPoint> GetBroadcastEndpoints()
        {
            return GetLocalAddresses().Select(addr => addr.GetAddressBytes()).Select(b =>
            {
                b[b.Length - 1] = 0xFF;
                return new IPEndPoint(new IPAddress(b), Protocol.Constants.DEFAULT_PORT);
            });            
        }

        private static void SendOnAll(byte[] buffer)
        {
            foreach (IPEndPoint endpoint in GetBroadcastEndpoints())
            {
                Mantis.Debug("Send to {0} => {1}", endpoint.ToString(), Encoding.ASCII.GetString(buffer));
                foreach (var callback in s_rawMessageSends) callback(endpoint, buffer);
                s_client.Send(buffer, buffer.Length, endpoint);            
            }
        }

        private static void DataReceived(UdpReceiveResult data)
        {
            // Ignore messages from self
            if (!GetLocalAddresses().Contains(data.RemoteEndPoint.Address))
            {
                Mantis.Debug("DataReceived => {0}", Encoding.ASCII.GetString(data.Buffer));
                foreach (Action<UdpReceiveResult> callback in s_rawMessagePump) callback(data);

                try
                {
                    string jsonString = Encoding.ASCII.GetString(data.Buffer);
                    Protocol.BreweryMessage message = Protocol.BreweryMessage.FromString(jsonString);

                    if (message != null)
                    {
                        if (message is Protocol.ControlMessage) // We ignore control messages sent to devices
                            return;

                        foreach (var callback in s_breweryMessages) callback(message);
                    }
                }
                catch (Exception ex)
                {
                    Mantis.Error("Dropping incoming packet, exception: {0}", ex.Message);
                }
            }            
        }

        private static IEnumerable<IPAddress> GetLocalAddresses()
        {
            foreach(NetworkInterface iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(iface.OperationalStatus == OperationalStatus.Up)
                {
                    foreach(UnicastIPAddressInformation ip in iface.GetIPProperties().UnicastAddresses)
                    {
                        if(ip.Address.AddressFamily == AddressFamily.InterNetwork && ip.Address.ToString() != "127.0.0.1")
                        {
                            yield return ip.Address;
                        }
                    }
                }
            }            
        }
    }
}
