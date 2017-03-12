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

using MantisDotNet.Network;
using MantisDotNet.Network.Protocol;
using MantisDotNet;

namespace MantisDotNet
{
    // Higher level stuff should go here... like discover, get list of breweries, etc
    public static class Mantis
    {
        private static ConcurrentDictionary<string, Brewery> s_breweries = new ConcurrentDictionary<string, Brewery>();

        public class BreweryTopologyEventArgs : EventArgs
        {
            public Brewery Brewery { get; internal set; }
        }

        public class MantisDiagnosticsEvent : EventArgs
        {
            public enum EventLevel
            {
                Error,
                Warning,
                Info,
                Debug,                
            };

            public DateTime Timestamp { get; internal set; }
            public EventLevel Level { get; internal set; }
            public string Message { get; internal set; }

            internal static void Raise(EventLevel level, string messageFormat, params object[] args)
            {
                MantisDiagnosticsEvent evt = new MantisDiagnosticsEvent()
                {
                    Timestamp = DateTime.Now,
                    Level = level,
                    Message = string.Format(messageFormat, args)
                };

                OnDiagnosticsEvent?.Invoke(null, evt);

                switch(level)
                {
                    case EventLevel.Debug:
                        System.Diagnostics.Debug.WriteLine(messageFormat, args);
                        break;
                    case EventLevel.Error:
                        System.Diagnostics.Trace.TraceError(messageFormat, args);
                        break;
                    case EventLevel.Warning:
                        System.Diagnostics.Trace.TraceWarning(messageFormat, args);
                        break;
                    case EventLevel.Info:
                        System.Diagnostics.Trace.TraceInformation(messageFormat, args);
                        break;
                }                
            }
        }

        public delegate void BreweryTopologyEventHandler(object sender, BreweryTopologyEventArgs e);
        public static event BreweryTopologyEventHandler OnNewBrewery;        
        public static event BreweryTopologyEventHandler OnBreweryLeave;

        public delegate void MantisDiagnosticsEventHandler(object sender, MantisDiagnosticsEvent e);
        public static event MantisDiagnosticsEventHandler OnDiagnosticsEvent;

        internal static void Debug(string format, params object[] args)
        {
            MantisDiagnosticsEvent.Raise(MantisDiagnosticsEvent.EventLevel.Debug, format, args);
        }

        internal static void Error(string format, params object[] args)
        {
            MantisDiagnosticsEvent.Raise(MantisDiagnosticsEvent.EventLevel.Error, format, args);
        }

        internal static void Warning(string format, params object[] args)
        {
            MantisDiagnosticsEvent.Raise(MantisDiagnosticsEvent.EventLevel.Warning, format, args);
        }

        internal static void Info(string format, params object[] args)
        {
            MantisDiagnosticsEvent.Raise(MantisDiagnosticsEvent.EventLevel.Info, format, args);
        }

        public static void Initialize()
        {
            Network.NetworkPump.Initialize();

            // We hook incoming brewery messages so we can auto-add/discover breweries, and deliver messages
            Network.NetworkPump.HookBreweryMessages(HandleIncomingBreweryMessage);            
        }

        public static void Discover()
        {
            Discover(3, 500);            
        }

        public static void Discover(int pingCount, int delayBetweenMilliseconds)
        {
            for (int x = 0; x < pingCount; x++)
            {
                Debug("Discover ping send {0}/{1}", x, pingCount);
                NetworkPump.Send(ControlMessage.CreateBroadcastControlMessage(ControlMessageId.Ping));
                if (delayBetweenMilliseconds > 0)
                {
                    System.Threading.Thread.Sleep(delayBetweenMilliseconds);
                }
            }
        }

        public static IEnumerable<Brewery> Breweries()
        {
            foreach (Brewery b in s_breweries.Values)
                yield return b;
        }

        private static void HandleIncomingBreweryMessage(BreweryMessage message)
        {
            IncomingMessage incomingMessage = message as IncomingMessage;
            if(message != null)
            {
                // If the message is goodbye, get and remove from list
                if(incomingMessage.MessageType == BreweryMessageType.Goodbye)
                {
                    Info("Goodbye from {0}", incomingMessage.SourceBreweryID);

                    // Only signal if we already knew about it
                    Brewery targetBrewery = null;
                    if(s_breweries.TryRemove(incomingMessage.SourceBreweryID, out targetBrewery))
                    {
                        targetBrewery.HandleIncomingMessage(message);
                        OnBreweryLeave?.Invoke(null, new BreweryTopologyEventArgs() { Brewery = targetBrewery });
                    }
                }
                else
                {
                    bool isNew = false;
                    Brewery targetBrewery = s_breweries.GetOrAdd(incomingMessage.SourceBreweryID, (key) =>
                    {
                        // We add to the dictionary, but note that its new
                        isNew = true;
                        Debug("Message from previously unknown ID: {0} => {1}", incomingMessage.SourceBreweryID, incomingMessage.MessageType);
                        return new Brewery(key);
                    });

                    // Fire event outside of dictionary add, as delegate may want the dictionary elemetn
                    if (isNew)
                    {
                        Info("New brewery discovered, sending full refresh: {0}", targetBrewery.ID);
                        targetBrewery.OnRefreshComplete += NewBreweryRefreshComplete;
                        targetBrewery.FullRefreshAsync();
                    }

                    // Deliver message to the target brewery object - TBD                    
                    targetBrewery.HandleIncomingMessage(message);
                }
            }

            PruneOldBreweries();
        }   

        private static void NewBreweryRefreshComplete(object sender, Brewery.BreweryEventArgs args)
        {
            Info("Refresh of {0} complete, now available as new brewery", args.Brewery.ID);
            args.Brewery.OnRefreshComplete -= NewBreweryRefreshComplete;
            // Fire event when full refresh has completed
            OnNewBrewery?.Invoke(null, new BreweryTopologyEventArgs() { Brewery = args.Brewery });
        }
        
        private static void PruneOldBreweries()
        {
            // Any brewery not heard from in > 15 minutes is awol, assume it's left.
            List<Brewery> deadBreweries = new List<Brewery>();
            foreach(Brewery brewery in s_breweries.Values)
            {
                if(DateTime.Now - brewery.LastMessage > TimeSpan.FromMinutes(15))
                {
                    deadBreweries.Add(brewery);
                }
            }

            foreach(Brewery brewery in deadBreweries)
            {
                Brewery tmp = null;
                if(s_breweries.TryRemove(brewery.ID, out tmp))
                {
                    Info("{0} has not been heard from in over 15 minutes, pruning", tmp.ID);
                    OnBreweryLeave?.Invoke(null, new BreweryTopologyEventArgs() { Brewery = tmp });
                }
            }
        }                     
    }
}
