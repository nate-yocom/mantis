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
 
using Newtonsoft.Json;

namespace MantisDotNet.Network.Protocol
{
    public enum BreweryMessageType
    {
        State,
        SSIDs,
        Settings,
        StatusTxt,
        Pong,
        Goodbye,
        AllInfo,
        ControlMessage
    }
    
    public enum ControlMessageId
    {
        SetMashTarget = 0,
        SetHltTarget = 1,
        SetPump1State = 2,
        SetPump2Mode = 3,
        SetBurner = 4,
        SetRemoteControl = 6,
        StatusText = 8,
        RequestBroadcast = 9,
        Ping = 10,
        SetBrewerName = 11,
        SetPump2State = 12,
        RequestSSIDs = 129,
        SetSSID = 130,
        SetPass = 131,
        ResetReconnect = 132,
        ResetToApMode = 133,
        SaveSettings = 134,
        SetConfiguration = 135,
        GetConfiguration = 136,
        SetState = 137,
        GetAll,
    }

    public enum AutoBrewPumpMode
    {
        Off = 0,
        Temperature = 1,
        FloatSwitch = 2,
        On = 3
    }

    public class SSID
    {
        public string Name { get; set; }
        public int RSSI { get; set; }
        public string Encryption { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public static class Constants
    {
        public const int DEFAULT_PORT = 12982;
    }

    public class WriteableBreweryState
    {        
        [JsonProperty(PropertyName = "HTT")]
        public float HltTarget { get; set; }        
        [JsonProperty(PropertyName = "MTT")]
        public float MashTarget { get; set; }        
        [JsonProperty(PropertyName = "P1")]
        public bool Pump1On { get; set; }
        [JsonProperty(PropertyName = "P2")]
        public bool Pump2On { get; set; }
        [JsonProperty(PropertyName = "B")]
        public bool BurnerOn { get; set; }
        [JsonProperty(PropertyName = "P2M")]
        public AutoBrewPumpMode Pump2AutoMode { get; set; }
        [JsonProperty(PropertyName = "RC")]
        public bool RemoteControlled { get; set; }

        public virtual WriteableBreweryState GetCopy()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<WriteableBreweryState>(this.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            WriteableBreweryState state = obj as WriteableBreweryState;
            if (state == null)
                return false;

            return state.HltTarget == HltTarget &&
                state.MashTarget == MashTarget &&
                state.Pump1On == Pump1On &&
                state.Pump2On == Pump2On &&
                state.Pump2AutoMode == Pump2AutoMode &&
                state.RemoteControlled == RemoteControlled;
        }

        public static bool operator ==(WriteableBreweryState lhs, WriteableBreweryState rhs)
        {
            if (ReferenceEquals(lhs, rhs)) return true;
            if (((object)lhs == null) || ((object)rhs == null)) return false;

            return lhs.Equals(rhs);
        }

        public static bool operator !=(WriteableBreweryState lhs, WriteableBreweryState rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class BreweryState : WriteableBreweryState
    {
        [JsonProperty(PropertyName = "HT")]
        public float HltTemp { get; set; }
        [JsonProperty(PropertyName = "MT")]
        public float MashTemp { get; set; }
        [JsonProperty(PropertyName = "KT")]
        public float KettleTemp { get; set; }
        [JsonProperty(PropertyName = "MF")]
        public bool MashFloat { get; set; }
        [JsonProperty(PropertyName = "HF")]
        public bool HltFloat { get; set; }

        public override WriteableBreweryState GetCopy()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<BreweryState>(this.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            BreweryState state = obj as BreweryState;
            if (state == null)
                return false;

            if (!base.Equals(obj))
                return false;

            return state.HltTemp == HltTemp &&
                state.MashTemp == MashTemp &&
                state.KettleTemp == KettleTemp &&
                state.MashFloat == MashFloat &&
                state.HltFloat == HltFloat;
        }        

        public static bool operator == (BreweryState lhs, BreweryState rhs)
        {
            if (ReferenceEquals(lhs, rhs)) return true;
            if (((object)lhs == null) || ((object)rhs == null)) return false;

            return lhs.Equals(rhs);
        }

        public static bool operator !=(BreweryState lhs, BreweryState rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class SettingsData
    {
        public SettingsData()
        {
            BroadcastRate = 30000;
            PortNumber = Constants.DEFAULT_PORT;
            AdditionalConfiguration = "";
            LogoIndex = 0;
        }
        
        public SettingsData GetCopy()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsData>(this.ToString());
        }

        [JsonProperty(PropertyName="ssid")]
        public string Network { get; set; }

        [JsonProperty(PropertyName = "pass")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "brewer")]
        public string CustomBrewer { get; set; }

        [JsonProperty(PropertyName = "portnum")]
        public int PortNumber { get; set; }

        [JsonProperty(PropertyName = "remote")]
        public string AdditionalConfiguration { get; set; }

        [JsonProperty(PropertyName = "bcastrate")]
        public int BroadcastRate { get; set; }

        [JsonProperty(PropertyName = "logoidx")]
        public int LogoIndex { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SettingsData rhs = obj as SettingsData;
            if (rhs == null)
                return false;

            return rhs.GetHashCode() == GetHashCode();
        }

        public static bool operator ==(SettingsData lhs, SettingsData rhs)
        {
            if (ReferenceEquals(lhs, rhs)) return true;
            if ( ((object) lhs == null) || ((object) rhs == null)) return false;

            return lhs.Equals(rhs);
        }

        public static bool operator !=(SettingsData lhs, SettingsData rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }    
}
