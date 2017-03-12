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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using MantisDotNet.Network;
using MantisDotNet.Network.Protocol;

namespace MantisDotNet
{    
    // Represents a single brewery and interaction with it
    public class Brewery
    {
        // Properties for getting current state / settings etc
        public string ID { get; private set; }
        public BreweryState RemoteState { get; private set; }
        public SettingsData Settings { get; private set; }
        public DateTime LastMessage { get; private set; }
        public List<SSID> Networks { get; private set; }

        public class BreweryEventArgs : EventArgs
        {
            public Brewery Brewery { get; internal set; }                        
        }

        public class BreweryNetworkEventArgs : BreweryEventArgs
        {
            public IncomingMessage Message { get; internal set; }
        }

        public class BreweryStatusTextEventArgs : BreweryEventArgs
        {
            public string Message { get; internal set; }
        }

        public delegate void BreweryEventHandler(object sender, BreweryEventArgs e);
        public delegate void BreweryNetworkEventHandler(object sender, BreweryNetworkEventArgs e);
        public delegate void BreweryStatusTextEventHandler(object sender, BreweryStatusTextEventArgs e);

        public event BreweryNetworkEventHandler OnNetworkMessage;
        public event BreweryStatusTextEventHandler OnStatusText;
        public event BreweryEventHandler OnSettingsReceived;
        public event BreweryEventHandler OnStateReceived;
        public event BreweryEventHandler OnNetworksReceived;
        public event BreweryEventHandler OnPongReceived;        
        public event BreweryEventHandler OnGoodbye;
        public event BreweryEventHandler OnRefreshComplete;

        public int NetworkTimeout = 15000;

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        private Stopwatch m_stopwatch = Stopwatch.StartNew();
        private ConcurrentDictionary<BreweryMessageType, long> m_receiptStamps = new ConcurrentDictionary<BreweryMessageType, long>();
        
        public Task FullRefreshAsync()
        {            
            return FullRefreshAsync(NetworkTimeout);
        }

        public Task FullRefreshAsync(int timeoutInSeconds)
        {
            return SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.GetAll, ID), BreweryMessageType.AllInfo, timeoutInSeconds);                        
        }

        public Task RefreshStateAsync()
        {
            return SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State);            
        }

        public void RefreshState()
        {
            RefreshStateAsync().Wait();
        }

        public Task PushStateAsync(WriteableBreweryState newState)
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetState, ID, newState.ToString()), () =>
            {
                if (RemoteState == newState)
                    return true;

                SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State).Wait();
                return RemoteState == newState;
            });            
        }

        public void PushState(WriteableBreweryState newState)
        {
            PushStateAsync(newState).Wait();
        }

        public Task PushSettingsAsync(SettingsData settings)
        {
            bool networkChange = settings.Network != Settings.Network || settings.Password != Settings.Password;

            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetConfiguration, ID, settings.ToString()), () =>
            {
                if (!networkChange)
                {
                    SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.GetConfiguration, ID), BreweryMessageType.Settings).Wait();
                    return Settings == settings;
                }

                return true;
            });            
        }

        public void PushSettings(SettingsData settings)
        {
            PushSettingsAsync(settings).Wait();
        }

        public Task RefreshSettingsAsync()
        {
            return SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.GetConfiguration, ID), BreweryMessageType.Settings);
        }

        public void RefreshSettings()
        {
            RefreshSettingsAsync().Wait();
        }
        
        public Task SetMashTargetAsync(float target)
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetMashTarget, ID, target), () =>
            {
                if (RemoteState.MashTarget == target)
                    return true;

                SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State).Wait();
                return RemoteState.MashTarget == target;
            });            
        }

        public void SetMashTarget(float target)
        {
            SetMashTargetAsync(target).Wait();
        }

        public Task SetHltTargetAsync(float target)
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetHltTarget, ID, target), () =>
            {
                // Might have already worked?
                if (RemoteState.HltTarget == target)
                    return true;

                // If not, request a state broadcast, wait for it, then compare
                SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State).Wait();
                return RemoteState.HltTarget == target;                
            });
        }

        public void SetHltTarget(float target)
        {
            SetHltTargetAsync(target).Wait();
        }

        
        public Task SetPump1StateAsync(bool on)
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetPump1State, ID, on), () =>
            {
                if (RemoteState.Pump1On == on)
                    return true;

                SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State).Wait();
                return RemoteState.Pump1On == on;
            });
        }

        public void SetPump1State(bool on)
        {
            SetPump1StateAsync(on).Wait();
        }

        public Task SetPump2ModeAsync(AutoBrewPumpMode mode)
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetPump2Mode, ID, mode), () =>
            {
                if (RemoteState.Pump2AutoMode == mode)
                    return true;

                SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State).Wait();
                return RemoteState.Pump2AutoMode == mode;
            });
        }

        public void SetPump2Mode(AutoBrewPumpMode mode)
        {
            SetPump2ModeAsync(mode).Wait();
        }

        public Task SetPump2StateAsync(bool on)
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetPump2State, ID, on), () =>
            {
                if (RemoteState.Pump2On == on)
                    return true;

                SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State).Wait();
                return RemoteState.Pump2On == on;
            });
        }

        public void SetPump2State(bool on)
        {
            SetPump2StateAsync(on).Wait();
        }


        public Task SetBurnerAsync(bool on)
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetBurner, ID, on), () =>
            {
                if (RemoteState.BurnerOn == on)
                    return true;

                SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State).Wait();
                return RemoteState.BurnerOn == on;
            });
        }

        public void SetBurner(bool on)
        {
            SetBurnerAsync(on).Wait();
        }

        public Task SetRemoteControlAsync(bool on)
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.SetRemoteControl, ID, on), () =>
            {
                if (RemoteState.RemoteControlled == on)
                    return true;

                SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestBroadcast, ID), BreweryMessageType.State).Wait();
                return RemoteState.RemoteControlled == on;
            });
        }

        public void SetRemoteControl(bool on)
        {
            SetRemoteControlAsync(on).Wait();
        }

        public Task PingAsync()
        {
            return SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.Ping, ID), BreweryMessageType.Pong);               
        }

        public void Ping()
        {
            PingAsync().Wait();
        }

        public Task SetBrewerNameAsync(string name)
        {
            SettingsData data = Settings.GetCopy();
            data.CustomBrewer = name;
            return PushSettingsAsync(data);
        }

        public Task SetWirelessNetworkAsync(string ssid, string pass)
        {
            SettingsData data = Settings.GetCopy();
            data.Network = ssid;
            data.Password = pass;                 
            return PushSettingsAsync(data);
        }

        public void SetWirelessNetwork(string ssid, string pass)
        {
            SetWirelessNetworkAsync(ssid, pass).Wait();
        }
        
        public Task ResetNetworkToPrivateAsync()
        {
            return SendAndValidate(ControlMessage.CreateTargetedControlMessage(ControlMessageId.ResetToApMode, ID), () => true);
        }

        public void ResetNetworkToPrivate()
        {
            ResetNetworkToPrivateAsync().Wait();
        }

        public Task RefreshNetworksAsync()
        {
            return SendAndExpect(ControlMessage.CreateTargetedControlMessage(ControlMessageId.RequestSSIDs, ID), BreweryMessageType.SSIDs);
        }

        public void RefreshNetworks()
        {
            RefreshNetworksAsync().Wait();
        }

        internal Brewery(string breweryMacAddress)
        {
            ID = breweryMacAddress;
            RemoteState = new BreweryState();            
            LastMessage = DateTime.Now;
            Settings = new SettingsData();            
        }

        private Task SendAndValidate(ControlMessage messageToSend, Func<bool> validateFunc)
        {
            return SendAndValidate(messageToSend, validateFunc, NetworkTimeout);
        }

        private Task SendAndValidate(ControlMessage messageToSend, Func<bool> validateFunc, int timeoutInSeconds)
        {
            DateTime timeoutAt = DateTime.Now + TimeSpan.FromSeconds(timeoutInSeconds);
            long firstSendAt = m_stopwatch.ElapsedTicks;

            return Task.Run(() =>
            {
                while (timeoutInSeconds == -1 || DateTime.Now < timeoutAt)
                {
                    NetworkPump.Send(messageToSend);
                    System.Threading.Thread.Sleep(1000);
                    
                    if (validateFunc())
                        break;
                }
            });
        }

        private Task SendAndExpect(ControlMessage messageToSend, BreweryMessageType responseExpected)
        {
            return SendAndExpect(messageToSend, responseExpected, NetworkTimeout);
        }

        private Task SendAndExpect(ControlMessage messageToSend, BreweryMessageType responseExpected, int timeoutInSeconds)
        {
            DateTime timeoutAt = DateTime.Now + TimeSpan.FromSeconds(timeoutInSeconds);
            long firstSendAt = m_stopwatch.ElapsedTicks;

            return Task.Run(() =>
            {
                while (timeoutInSeconds == -1 || DateTime.Now < timeoutAt)
                {
                    NetworkPump.Send(messageToSend);
                    System.Threading.Thread.Sleep(1000);

                    long ts = 0;
                    if (m_receiptStamps.TryGetValue(responseExpected, out ts))
                    {
                        if (ts >= firstSendAt)
                            break;
                    }                    
                }
            });
        }

        internal void HandleIncomingMessage(BreweryMessage message)
        {
            LastMessage = DateTime.Now;
            m_receiptStamps.AddOrUpdate(message.MessageType, m_stopwatch.ElapsedTicks, (messageType, oldValue) => m_stopwatch.ElapsedTicks);

            try
            {
                switch (message.MessageType)
                {
                    case BreweryMessageType.Settings:
                        Settings = (message as SettingsBreweryMessage).Settings;
                        OnSettingsReceived?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        break;
                    case BreweryMessageType.State:
                        RemoteState = (message as StateBreweryMessage).CurrentState;
                        OnStateReceived?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        break;
                    case BreweryMessageType.SSIDs:
                        Networks = (message as SsidBreweryMessage).Networks.ToList();
                        OnNetworksReceived?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        break;
                    case BreweryMessageType.AllInfo:
                        AllInfoBreweryMessage allInfo = message as AllInfoBreweryMessage;
                        Settings = allInfo.SettingsMessage().Settings;
                        RemoteState = allInfo.StateMessage().CurrentState;
                        Networks = allInfo.SsidsMessage().Networks.ToList();
                        OnRefreshComplete?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        OnSettingsReceived?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        OnNetworksReceived?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        OnStateReceived?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        break;
                    case BreweryMessageType.Pong:
                        OnPongReceived?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        break;
                    case BreweryMessageType.StatusTxt:
                        OnStatusText?.Invoke(this, new BreweryStatusTextEventArgs() { Brewery = this, Message = (message as StatusTextBreweryMessage).Data });
                        break;
                    case BreweryMessageType.Goodbye:
                        OnGoodbye?.Invoke(this, new BreweryEventArgs() { Brewery = this });
                        break;
                }
            }
            catch(Exception ex)
            {
                Mantis.Error("{0} ignoring brewery delegate exception from message: {1} exception: {2}", ID, message.MessageType, ex);
            }

            if (message is IncomingMessage)
            {
                try
                {
                    OnNetworkMessage?.Invoke(this, new BreweryNetworkEventArgs() { Brewery = this, Message = message as IncomingMessage });
                }
                catch (Exception ex)
                {
                    Mantis.Error("{0} ignoring brewery delegate exception from network delegate, message: {1} exception: {2}", ID, message.MessageType, ex);
                }
            }
        }                        
    }
}
