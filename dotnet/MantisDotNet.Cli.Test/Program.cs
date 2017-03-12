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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantisDotNet.Cli.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter network name: ");
            string network = Console.ReadLine();
            Console.Write("Enter network pass: ");
            string pass = Console.ReadLine();
            Console.Write("Enter brewer name: ");
            string brewerName = Console.ReadLine();

            object fileLock = new object();

            MantisDotNet.Mantis.OnDiagnosticsEvent += (sender, arg) =>
            {
                Console.WriteLine("DIAG: {0} {1}\t{2}", arg.Timestamp.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), arg.Level, arg.Message);
            };

            MantisDotNet.Mantis.Initialize();

            MantisDotNet.Network.NetworkPump.HookRawMessagePump((result) =>
            {
                lock(fileLock)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter("network.rec", true))
                    {
                        var record = new { TS = DateTime.Now, EP = result.RemoteEndPoint.ToString(), B = Encoding.ASCII.GetString(result.Buffer) };
                        file.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(record));
                    }
                }
                
                string message = Encoding.ASCII.GetString(result.Buffer);
                Console.WriteLine("Raw message from: {0} message: {1}", result.RemoteEndPoint, Encoding.ASCII.GetString(result.Buffer));                
            });

            MantisDotNet.Network.NetworkPump.HookRawMessageSends((endpoint, data) =>
            {
                //string message = Encoding.ASCII.GetString(data);
                //Console.WriteLine("Raw message send to: {0} len: {1} message: {2}", endpoint, message.Length, message);
            });

            MantisDotNet.Network.NetworkPump.HookBreweryMessages((m) =>
            {
                //Console.WriteLine("Got {0} from {1} => {2}", m.MessageType, (m as Network.Protocol.IncomingMessage).SourceBreweryID, m.ToString());                
            });

            Mantis.OnNewBrewery += (o, arg) =>
            {
                Console.WriteLine("New brewery discovered: {0}", arg.Brewery.ID);

                // arg.Brewery.OnNetworkMessage += (sender, bearg) => { Console.WriteLine("{0} Got a network message: {1}", bearg.Brewery.ID, bearg.Message.MessageType); };

                arg.Brewery.OnGoodbye += (sender, bearg) => { Console.WriteLine("{0} said goodbye", bearg.Brewery.ID); };                
                arg.Brewery.OnNetworksReceived += (sender, bearg) => { Console.WriteLine("{0} Got network list: {1} networks", bearg.Brewery.ID, bearg.Brewery.Networks.Count()); };
                arg.Brewery.OnPongReceived += (sender, bearg) => { Console.WriteLine("{0} said Pong", bearg.Brewery.ID); };
                arg.Brewery.OnSettingsReceived += (sender, bearg) => { Console.WriteLine("{0} sent configuration: {1}", bearg.Brewery.ID, bearg.Brewery.Settings.ToString()); };
                arg.Brewery.OnStateReceived += (sender, bearg) => { Console.WriteLine("{0} sent state: {1}", bearg.Brewery.ID, bearg.Brewery.RemoteState.ToString()); };
                arg.Brewery.OnStatusText += (sender, bearg) => { Console.WriteLine("{0} said: {1}", bearg.Brewery.ID, bearg.Message); };
            };

            Mantis.OnBreweryLeave += (o, arg) =>
            {
                Console.WriteLine("Brewery leaving: {0}", arg.Brewery.ID);
            };

            Mantis.Discover();

            Console.ReadLine();

            foreach (Brewery brewery in Mantis.Breweries())
            {
                Console.WriteLine("Configuring {0}", brewery.ID);
                brewery.PushSettingsAsync(new Network.Protocol.SettingsData()
                {
                    BroadcastRate = 30000,
                    CustomBrewer = brewerName,
                    LogoIndex = 0,
                    Network = network,
                    Password = pass,
                }).Wait();
            }

            while (true)
            {
                Console.ReadLine();

                foreach (Brewery brewery in Mantis.Breweries())
                {
                    Console.WriteLine("Requesting refresh from {0}", brewery.ID);
                    brewery.RefreshStateAsync();
                    Console.WriteLine("Refresh request sent");
                }                
            }
        }
    }
}
