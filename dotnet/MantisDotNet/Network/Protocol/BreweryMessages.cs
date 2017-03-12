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
using Newtonsoft.Json;

namespace MantisDotNet.Network.Protocol
{
    public abstract class BreweryMessage
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public BreweryMessageType MessageType { get; set; }
        
        private static Dictionary<BreweryMessageType, Type> EnumTypeToObjectType = new Dictionary<BreweryMessageType, Type>()
        {
            { BreweryMessageType.State, typeof(StateBreweryMessage) },
            { BreweryMessageType.SSIDs, typeof(SsidBreweryMessage) },
            { BreweryMessageType.Settings, typeof(SettingsBreweryMessage) },
            { BreweryMessageType.StatusTxt, typeof(StatusTextBreweryMessage) },
            { BreweryMessageType.Pong, typeof(PongBreweryMessage) },
            { BreweryMessageType.Goodbye, typeof(GoodbyeBreweryMessage) },
            { BreweryMessageType.AllInfo, typeof(AllInfoBreweryMessage) },
            { BreweryMessageType.ControlMessage, typeof(ControlMessage) }
        };

        internal static Type GetMessageType(BreweryMessageType type)
        {
            return EnumTypeToObjectType[type];
        }

        public static BreweryMessage FromString(string jsonString)
        {
            try
            {
                Dictionary<string, object> props = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                if (props.ContainsKey("MessageType"))
                {
                    Protocol.BreweryMessageType messageType;
                    if (Enum.TryParse<Protocol.BreweryMessageType>(props["MessageType"] as string, out messageType))
                    {
                        return (BreweryMessage) Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, Protocol.BreweryMessage.GetMessageType(messageType));
                    }
                }
            }
            catch
            {
                // Nothing to be done here?
            }

            return null;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class IncomingMessage : BreweryMessage
    {
        [JsonProperty(PropertyName = "ID")]
        public string SourceBreweryID { get; set; }
        [JsonProperty(PropertyName = "TS")]
        public int TimeStamp { get; set; }
    }

    public class StateBreweryMessage : IncomingMessage
    {
        public BreweryState CurrentState;
        public string IP { get; set; }

        [JsonProperty(PropertyName = "FM")]
        public int FreeMemory { get; set; }

    }

    public class SsidBreweryMessage : IncomingMessage
    {
        [JsonProperty(PropertyName = "Nets")]
        public SSID[] Networks { get; set; }
    }

    public class SettingsBreweryMessage : IncomingMessage
    {        
        public string Data { get; set; }        
        public SettingsData Settings
        {
            get { return Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsData>(Data); }
        }            
    }

    public class StatusTextBreweryMessage : IncomingMessage
    {
        public string Data { get; set; }
    }

    public class PongBreweryMessage : IncomingMessage
    {
        [JsonProperty(PropertyName = "FM")]
        public int FreeMemory { get; set; }
    }

    public class GoodbyeBreweryMessage : IncomingMessage
    {
    }

    public class AllInfoBreweryMessage : IncomingMessage
    {
        [JsonProperty(PropertyName = "State")]
        public string EmbeddedState { get; set; }
        [JsonProperty(PropertyName = "Config")]
        public string EmbeddedConfig { get; set; }
        [JsonProperty(PropertyName = "SSIDs")]
        public string EmbeddedNetworks { get; set; }

        public StateBreweryMessage StateMessage()
        {
            return FromString(EmbeddedState) as StateBreweryMessage;
        }

        public SettingsBreweryMessage SettingsMessage()
        {
            return FromString(EmbeddedConfig) as SettingsBreweryMessage;
        }

        public SsidBreweryMessage SsidsMessage()
        {
            return FromString(EmbeddedNetworks) as SsidBreweryMessage;
        }
    }
}
