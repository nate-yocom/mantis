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
 
#include <Arduino.h>
#include <ArduinoJson.h>

#include "WiFiEsp/WiFiEsp.h"
#include "WiFiEsp/WiFiEspUdp.h"
       
#include "debug.h"
#include "Brewery.h"
#include "Wireless.h"
#include "Settings.h"
#include "Memory.h"

#define ESP8266_BAUD_RATE 9600

Brewery::BreweryState Wireless::s_lastBroadcastState;
Wireless::WifiInputListener Wireless::s_eventListener;
WiFiEspUDP Wireless::s_udpServer; 
unsigned long Wireless::s_lastBroadcast = 0;
bool Wireless::s_chipAvailable = false;
IPAddress Wireless::s_localIp;
IPAddress Wireless::s_bcastIp;
IPAddress Wireless::s_defaultApBcast(192, 168, 4, 255);
char Wireless::s_mac[32] = {0};
char Wireless::s_ap[128] = {0};
bool Wireless::s_allowStatusSend = false;
char Wireless::s_readBuffer[4 * 1024] = {0};

void Wireless::Init()
{             
    Serial3.begin(ESP8266_BAUD_RATE);
    Settings::Get();    // Make sure settings are loaded                               
    DEBUG(F("Adding listener"));  
    InputEvents::AddListener(&s_eventListener);
    strcpy(s_ap, "<disabled>");        
}

void Wireless::Reset()
{
    s_allowStatusSend = false;
    STATUS(F("Initializing wifi chip..."));    
    WiFi.init(&Serial3);
        
    if(WiFi.status() != WL_NO_SHIELD)
    {
        s_chipAvailable = true;
                
        if(Settings::Get().ssid.length() > 0)
        {
            STATUS(F("Joining AP %s"), Settings::Get().ssid.c_str());
            int status = WL_IDLE_STATUS;
            while( status != WL_CONNECTED )
            {
                if(Settings::Get().pass.length() > 0)
                {
                    status = WiFi.begin((char *)Settings::Get().ssid.c_str(), (char *)Settings::Get().pass.c_str());
                }
                else
                {
                    status = WiFi.begin((char *)Settings::Get().ssid.c_str(), NULL);
                }
            }
            STATUS(F("Joining AP %s COMPLETE"), Settings::Get().ssid.c_str());
            strcpy(s_ap, Settings::Get().ssid.c_str());
        }
        else
        {
            STATUS(F("Chip found, setting into AP mode"));        
            WiFi.beginAP("morebeer", 10, "", 0, false);
            strcpy(s_ap, "morebeer (AP Mode)");    
        }        
        
        s_localIp = WiFi.localIP();
        s_bcastIp = s_localIp;
        s_bcastIp[3] = 255; 
        char localIpBuf[16];
        IPAsString(s_localIp, localIpBuf);
        char bcastIpBuf[16];
        IPAsString(s_bcastIp, bcastIpBuf);
        STATUS(F("Configured with local IP: %s Broadcast: %s"), localIpBuf, bcastIpBuf);
        
        STATUS(F("Chip initialized and ready, starting UDP listen on %d"), Settings::Get().portnum);        
        s_udpServer.begin(Settings::Get().portnum);
        
        // Stash mac address
        byte mac[6];
        WiFi.macAddress(mac);        
        sprintf(s_mac, "%02X:%02X:%02X:%02X:%02X:%02X", mac[5], mac[4], mac[3], mac[2], mac[1], mac[0]);           
        
        UpdateNetworkInfo();

        SendPong();                       
    }
    else
    {
        s_chipAvailable = false;
        STATUS(F("No wifi chip found"));
        strcpy(s_ap, "<disabled>");
        UpdateNetworkInfo();            
    } 
         
    s_allowStatusSend = true;   
}

void Wireless::UpdateNetworkInfo()
{
    if(strlen(s_ap) != 0)
    {
        char buff[256] = {0};                
        char * currSsid = WiFi.SSID();
        if(currSsid[0] == 0)
        {
            sprintf(buff, "SSID: %s", s_ap);                    
        }
        else
        {
            int32_t rssi = WiFi.RSSI();            
            sprintf(buff, "SSID: %s (%s %d dBm)", currSsid, RssiToQuality(rssi), rssi);            
        }
        InputEvents::PublishEvent(InputEvents::NetworkChange, buff);
    }    
    else
    {
        InputEvents::PublishEvent(InputEvents::NetworkChange, "                                             ");
    }
}

const char * Wireless::RssiToQuality(int32_t rssi)
{
    if(rssi > -55) return "High";
    if(rssi > -70) return "Medium";
    if(rssi > -90) return "Low";
    return "Very Low";
}

void Wireless::Disconnect()
{
    if(Available())
    {
        s_allowStatusSend = false;
        char broadme[2048] = {0}; 
        StaticJsonBuffer<256> jsonBuffer;
        JsonObject& root = jsonBuffer.createObject();
        root["MessageType"] = "Goodbye";                        
        root["Version"] = 1;
        root["TS"] = millis();
        root["ID"] = s_mac;                
        root.printTo((char *) broadme, sizeof(broadme));                    
        Write(broadme);
        delay(250);        
        WiFi.disconnect(); 
        WiFi.init(&Serial3);
        memset(s_ap, 0, sizeof(s_ap));
        UpdateNetworkInfo();       
    }
}

const char * Wireless::Read()
{        
    memset(s_readBuffer, 0, sizeof(s_readBuffer));
    int packetSize = s_udpServer.parsePacket();    
    if(packetSize)
    {
        char rip[16] = {0};
        IPAsString(s_udpServer.remoteIP(), rip);
        DEBUG(F("Read sees %d bytes from %s:%d"), packetSize, rip, s_udpServer.remotePort());        
        int readSize = min(packetSize, 4 * 1024 - 1);        
        int readSoFar = 0;
        int readThisTime = 0;

        while(readSoFar < readSize && readThisTime != -1)
        {
            readThisTime = s_udpServer.read(s_readBuffer + readSoFar, readSize - readSoFar);            
            if(readThisTime >= 0)
            {
                readSoFar += readThisTime;
            }                           
        }                                                 

        if(readThisTime == -1)
        {
            memset(s_readBuffer, 0, sizeof(s_readBuffer));
        }                
    }    
    return s_readBuffer;
}

void Wireless::Write(String output)
{
    Write(output.c_str(), 0, output.length()); 
}

void Wireless::Write(const char *buffer, int offset, int len)
{                
    s_udpServer.beginPacket(s_bcastIp, Settings::Get().portnum);
    s_udpServer.write(buffer + offset, len);
    s_udpServer.endPacket();
}  

void Wireless::IPAsString(IPAddress ip, char *buf)
{    
    sprintf(buf, "%d.%d.%d.%d", ip[0], ip[1], ip[2], ip[3]);    
}

bool Wireless::Available()
{
    return s_chipAvailable;
}

void Wireless::Tick()
{        
    // If no wifi, just skip..
    if(Available() && Brewery::CurrentState().Wireless)
    {    
        HandleIncoming();            
        BroadcastState(false);        
    }          
}

void Wireless::GetStateBroadcast(char *buffer, size_t size)
{
    // Get the latest state
    Brewery::BreweryState currState = Brewery::CurrentState();
    char localIp[16] = {0};
    IPAsString(s_localIp, localIp); 
              
    StaticJsonBuffer<512> jsonBuffer;
    JsonObject& root = jsonBuffer.createObject();
    root["MessageType"] = "State";
    root["Version"] = 1;
    root["ID"] = s_mac;
    root["IP"] = localIp;
    root["TS"] = millis();
    root["FM"] = (int) Memory::GetFree();
    JsonObject& stateObj = root.createNestedObject("CurrentState");
    stateObj["HT"] = currState.HltTemp;
    stateObj["HTT"] = currState.HltTargetTemp;
    stateObj["MT"] = currState.MashTemp;
    stateObj["MTT"] = currState.MashTargetTemp;
    stateObj["KT"] = currState.BoilTemp;
    stateObj["MF"] = currState.MashFloatOn;
    stateObj["HF"] = currState.HltFloatOn;
    stateObj["P1"] = currState.Pump1On;
    stateObj["P2"] = currState.Pump2On;
    stateObj["B"] = currState.BurnerOn;
    stateObj["P2M"] = (int) currState.Pump2AutoMode;
    stateObj["RC"] = currState.RemoteControlled;    
                
    root.printTo(buffer, size);           
}

void Wireless::BroadcastState(bool force)
{                             
    // We broadcast, in order of priority:

    //  When called with force = true
    bool shouldBroadcast = force;

    //  If theres a delta between last broadcast state and current - no more than once a second
    Brewery::BreweryState currState = Brewery::CurrentState();          
    if( 
        abs(currState.HltTemp - s_lastBroadcastState.HltTemp) > 1.0 ||
        abs(currState.MashTemp - s_lastBroadcastState.MashTemp) > 1.0 ||
        abs(currState.BoilTemp - s_lastBroadcastState.BoilTemp) > 1.0 ||
        currState.HltTargetTemp != s_lastBroadcastState.HltTargetTemp ||
        currState.MashTargetTemp != s_lastBroadcastState.MashTargetTemp ||
        currState.MashFloatOn != s_lastBroadcastState.MashFloatOn ||
        currState.HltFloatOn != s_lastBroadcastState.HltFloatOn ||
        currState.Pump1On != s_lastBroadcastState.Pump1On ||
        currState.Pump2On != s_lastBroadcastState.Pump2On ||
        currState.BurnerOn != s_lastBroadcastState.BurnerOn ||
        currState.Wireless != s_lastBroadcastState.Wireless ||
        currState.RemoteControlled != s_lastBroadcastState.RemoteControlled ||
        currState.Pump2AutoMode != s_lastBroadcastState.Pump2AutoMode)
    {        
        DEBUG(F("Wireless tick sees delta between current state and previous, delta time is: %d"), millis() - s_lastBroadcast);
        if(millis() - s_lastBroadcast >= 1000)
        {
            shouldBroadcast = true;
        }
    }

    //  Once every 'broadcast rate' milliseconds
    shouldBroadcast = (s_lastBroadcast == 0 || millis() - s_lastBroadcast >= Settings::Get().bcastrate) ? true : shouldBroadcast;
        
    // BroadcastState       
    if(shouldBroadcast)
    {
        char buffer[1024] = {0};
        s_lastBroadcast = millis();
        s_lastBroadcastState = currState;        
        GetStateBroadcast(buffer, sizeof(buffer));                             
        Write(buffer, 0, strlen(buffer) + 1);
    }
}

void Wireless::GetSSIDList(char *buffer, size_t size)
{
    // Create a json list of ssid, signal, encryption
    DEBUG(F("Getting SSID list"));
    
    memset(buffer, 0, size);
    
    StaticJsonBuffer<256> jsonBuffer;
    JsonObject& root = jsonBuffer.createObject();
    root["MessageType"] = "SSIDs";
    root["ID"] = s_mac;
    root["TS"] = millis();
    root["Version"] = 1;
    
    JsonArray& ssidArray = root.createNestedArray("Nets");
    
    int numNets = WiFi.scanNetworks();
    if(numNets > 0)
    {
        for(int x = 0; x < numNets; x++)
        {
            JsonObject &net = ssidArray.createNestedObject();
            net["Name"] = WiFi.SSID(x);
            net["RSSI"] = WiFi.RSSI(x);
            net["Encryption"] = EncTypeToString(WiFi.encryptionType(x));                                                                
        }
    }
                
    root.printTo((char *) buffer, size);    
}

const char * Wireless::EncTypeToString(int type)
{
   switch (type) {
    case ENC_TYPE_WEP:
      return "WEP";
    case ENC_TYPE_WPA_PSK:
      return "WPA_PSK";     
    case ENC_TYPE_WPA2_PSK:
      return "WPA2_PSK";      
    case ENC_TYPE_WPA_WPA2_PSK:
      return "WPA_WPA2_PSK";            
    case ENC_TYPE_NONE:
      return "None";            
  }
}

void Wireless::HandleIncoming()
{   
    int messageCount = 0;
    const char * rawMessage = Read();             
    while(strlen(rawMessage) > 0)
    {        
        messageCount++;
        DEBUG(F("Msg[%d] len: %d"), messageCount, strlen(rawMessage));
        DEBUG(F("Msg[%d]: *%s*"), messageCount, rawMessage);
        
        StaticJsonBuffer<512> jsonBuffer;
        JsonObject& root = jsonBuffer.parseObject(rawMessage);
        
        const char * messageType = root["MessageType"].asString();
        if(stricmp(messageType, "ControlMessage") == 0)
        {
            int controlId = root["ControlId"];
            const char * messageTarget = root["Target"].asString();
                    
            // Target must match our mac address, or *
            if(stricmp(messageTarget, "*") == 0 || stricmp(messageTarget, s_mac) == 0)
            {        
                if(controlId > InputEvents::Event::MAX)
                {
                    HandleIncomingWirelessCommand(root);   
                }
                else
                {
                    HandleIncomingInputEvent(root);
                }
            }    
            else
            {
                DEBUG(F("Ignoring message %d for target other than us: %s"), controlId, rawMessage);
            }                   
        }
        else
        {
            DEBUG(F("Ignoring non-control message %s"), messageType);
        }
        
        rawMessage = Read();
    }    
}

void Wireless::GetConfiguration(bool hidePass, char *buffer, size_t size)
{    
    StaticJsonBuffer<256> jsonBuffer;
    JsonObject& root = jsonBuffer.createObject();
    root["MessageType"] = "Settings";                        
    root["Version"] = 1;
    root["ID"] = s_mac;    
    root["Data"] = Settings::GetJson(hidePass);    
    root.printTo(buffer, size);                     
}

void Wireless::HandleIncomingWirelessCommand(JsonObject& root)
{
    s_allowStatusSend = false;
    int type = root["ControlId"];
    Wireless::WirelessCommand cmd = (Wireless::WirelessCommand) type;    
    switch(cmd)
    {
        case Wireless::WirelessCommand::GetSSIDs:
            {
                STATUS(F("Retrieving SSID list for network client"));
                char buff[2048] = {0};
                GetSSIDList(buff, sizeof(buff));
                Write(buff, 0, strlen(buff) + 1);
            }
            break;
        case Wireless::WirelessCommand::SetSSID:            
            STATUS(F("Setting SSID to %s for network client"), root["Data"].asString());
            Settings::Get().ssid = root["Data"].asString();
            break;
        case Wireless::WirelessCommand::SetSSIDPass:  
            STATUS(F("Setting network password for client"));          
            Settings::Get().pass = root["Data"].asString();
            break;
        case Wireless::WirelessCommand::ResetReconnect:
            STATUS(F("Resetting network connection"));
            Reset();
            break;
        case Wireless::WirelessCommand::ResetToAP:
            STATUS(F("Resetting network connection to AP mode"));
            Settings::Get().ssid = "";
            Settings::Get().pass = "";
            Reset();
            break;   
        case Wireless::WirelessCommand::RememberStuff:
            STATUS(F("Saving settings"));
            Settings::Save();
            break;    
        case Wireless::WirelessCommand::SetRemoteConfiguration:
            {                         
                STATUS(F("Network client provided configuration, saving"));  
                String curSsid = Settings::Get().ssid;
                String curPass = Settings::Get().pass;      
                Settings::FromJson(root["Data"].asString());
                Settings::Save();
                InputEvents::PublishEvent(InputEvents::SetBrewer, Settings::Get().brewer.c_str());                
                if(curSsid != Settings::Get().ssid || curPass != Settings::Get().pass)
                {
                    STATUS(F("Network changed, resetting"));
                    Reset();
                }             
            }
            break;
        case Wireless::WirelessCommand::GetRemoteConfiguration:
            {    
                STATUS(F("Sending configuration to network"));
                char buffer[2048] = {0};

                // Joined net gets to see the AP password, cuz they must already have it
                if(s_bcastIp != s_defaultApBcast)
                {                    
                    GetConfiguration(false, buffer, sizeof(buffer));                                                            
                }     
                else
                {
                    // Private net does not, anyone can join that - we shouldn't leak the password there
                    GetConfiguration(true, buffer, sizeof(buffer));                                        
                } 
                Write(buffer, 0, strlen(buffer) + 1);                                                       
            }
            break;
        case Wireless::WirelessCommand::SetState:
            {                
                STATUS(F("Changing state to that provided by network client"));
                DEBUG(F("Remote state provided: %s"), root["Data"].asString());
                String jsonString = root["Data"].asString();
                StaticJsonBuffer<512> jsonBuffer;
                JsonObject& stateRoot = jsonBuffer.parseObject(jsonString.c_str());
                
                float flt = stateRoot["HTT"].as<float>();
                InputEvents::PublishEvent(InputEvents::HltTargetSet, &flt);
                flt = stateRoot["MTT"].as<float>();
                InputEvents::PublishEvent(InputEvents::MashTargetSet, &flt);
                bool on = stateRoot["P1"].as<bool>();
                InputEvents::PublishEvent(InputEvents::Pump1, &on);
                on = stateRoot["P2"].as<bool>();
                InputEvents::PublishEvent(InputEvents::Pump2, &on);
                on = stateRoot["B"].as<bool>();
                InputEvents::PublishEvent(InputEvents::Burner, &on);
                on = stateRoot["RC"].as<bool>();
                InputEvents::PublishEvent(InputEvents::RemoteControl, &on);
                Brewery::Pump2Mode mode = (Brewery::Pump2Mode) stateRoot["P2M"].as<int>();
                InputEvents::PublishEvent(InputEvents::Pump2ModeChange, &mode);
            }
            break;
        case Wireless::WirelessCommand::GetAll:
            {
                STATUS(F("Sending refresh information to network clients"));
                char sendBuf[8 * 1024] = {0};
                StaticJsonBuffer<512> jsonBuffer;
                JsonObject& root = jsonBuffer.createObject();
                root["MessageType"] = "AllInfo";
                root["ID"] = s_mac;
                root["TS"] = millis();

                char statebuffer[1024] = {0};
                GetStateBroadcast(statebuffer, sizeof(statebuffer));
                
                char ssids[2048] = {0}; 
                GetSSIDList(ssids, sizeof(ssids));

                root["State"] = statebuffer;
                root["SSIDs"] = ssids;

                char configbuffer[4096] = {0};
                if(s_bcastIp != s_defaultApBcast)
                {
                    GetConfiguration(false, configbuffer, sizeof(configbuffer));                                                                                        
                }
                else
                {                    
                    GetConfiguration(true, configbuffer, sizeof(configbuffer));                    
                }

                root["Config"] = configbuffer;
                root.printTo((char *) sendBuf, sizeof(sendBuf));                
                Write(sendBuf, 0, strlen(sendBuf) + 1);
            }
            break;                   
    }
    s_allowStatusSend = true;
}

void Wireless::HandleIncomingInputEvent(JsonObject& root)
{        
    int type = root["ControlId"];
    InputEvents::Event evt = (InputEvents::Event) type;
    
    switch(evt)
    {
        case InputEvents::MashTargetSet:
        case InputEvents::HltTargetSet:
            {
                float target = (float) root["Data"].as<float>();
                InputEvents::PublishEvent(evt, &target);
            }
            break;                            
        case InputEvents::Pump1:
        case InputEvents::Pump2:
        case InputEvents::Burner:
        case InputEvents::RemoteControl:            
            {
                bool on = root["Data"].as<bool>();
                InputEvents::PublishEvent(evt, &on);  
            }   
            break;     
        case InputEvents::DebugTxt:
        case InputEvents::StatusTxt:
            {
                s_allowStatusSend = false;
                InputEvents::PublishEvent(evt, root["Data"].asString());
                s_allowStatusSend = true;    
            }
            break;
        case InputEvents::Pump2ModeChange:
            {
                Brewery::Pump2Mode mode = (Brewery::Pump2Mode) root["Data"].as<int>();
                InputEvents::PublishEvent(evt, &mode);
            }
            break;
        case InputEvents::BroadcastReq:
        case InputEvents::Ping:
            {
                InputEvents::PublishEvent(evt, NULL);
            }                
            break;
        case InputEvents::SetBrewer:
            InputEvents::PublishEvent(evt, root["Data"].asString());
            break;                                                
    } 
}

void Wireless::SendPong()
{
    char broadme[2048] = {0};
    StaticJsonBuffer<256> jsonBuffer;
    JsonObject& root = jsonBuffer.createObject();
    root["MessageType"] = "Pong";                        
    root["Version"] = 1;
    root["TS"] = millis();
    root["ID"] = s_mac;
    root["FM"] = (int) Memory::GetFree();                        
    root.printTo((char *) broadme, sizeof(broadme));                    
    Write(broadme);
}

void Wireless::WifiInputListener::HandleEvent(InputEvents::Event event, const void *data)
{                       
    if(Available())
    {                 
        switch(event)
        {
            case InputEvents::StatusTxt:        
                {
                    // Only if we are connected and wrunning
                    if(Available() && Brewery::CurrentState().Wireless && s_allowStatusSend)
                    {
                        char broadme[2048] = {0};
                        StaticJsonBuffer<256> jsonBuffer;
                        JsonObject& root = jsonBuffer.createObject();
                        root["MessageType"] = "StatusTxt";                        
                        root["Version"] = 1;
                        root["ID"] = s_mac;
                        root["TS"] = millis();        
                        root["Data"] = (const char *) data;
                        root.printTo((char *) broadme, sizeof(broadme));                    
                        Write(broadme);
                    }                                                                                
                }            
                break;            
            case InputEvents::BroadcastReq:
                DEBUG(F("Broadcasting on request"));
                Wireless::BroadcastState(true);
                break;    
            case InputEvents::Ping:
                DEBUG(F("Ping'd"));
                SendPong();
                break;                                
        }
    }
}