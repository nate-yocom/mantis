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
 
#ifndef _INCLUDE_WIRELESS_H
#define _INCLUDE_WIRELESS_H

#include <ArduinoJson.h>
#include "WiFiEsp/WiFiEspUdp.h"
#include "InputEvents.h"
#include "Brewery.h"

class Wireless
{
    public:
        static void Init();
        static void Reset();         
        static void Tick();
        static bool Available(); 
        static void Disconnect();       
        
    private:            
        static const char * Read();
        static void   Write(String output);
        static void   Write(const char *buffer, int offset, int len);
        static void   Send(const char *str);
        static void   Command(const char *command, const char *expectGood);
        static void   BroadcastState(bool force);
        static void   HandleIncoming();
        static void   HandleIncomingInputEvent(JsonObject& root);
        static void   HandleIncomingWirelessCommand(JsonObject& root);
        static void   IPAsString(IPAddress address, char *buf);
        static void   GetSSIDList(char *buf, size_t size);
        static const char * EncTypeToString(int type);
        static void   UpdateNetworkInfo();
        static const char * RssiToQuality(int32_t rssi);
        static void   GetStateBroadcast(char *buf, size_t size);
        static void   GetConfiguration(bool hidePass, char *buffer, size_t size);
        static void   SendPong();        
        
        static unsigned long s_lastBroadcast;               
    
        class WifiInputListener : public InputEvents::EventListener
        {
            public:
                virtual void HandleEvent(InputEvents::Event, const void *data);
        };
                
        enum WirelessCommand {
            GetSSIDs      = InputEvents::Event::MAX + 1,    // 129
            SetSSID,                                        // 130
            SetSSIDPass,                                    // 131
            ResetReconnect,                                 // 132
            ResetToAP,                                      // 133
            RememberStuff,                                  // 134
            SetRemoteConfiguration,                         // 135
            GetRemoteConfiguration,                         // 136
            SetState,                                       // 137
            GetAll,                                         // 138                      
        };
                
        static Brewery::BreweryState s_lastBroadcastState;
        static WifiInputListener s_eventListener;
        static WiFiEspUDP        s_udpServer;
        static bool              s_chipAvailable;
        static IPAddress         s_localIp;
        static IPAddress         s_bcastIp;
        static IPAddress         s_defaultApBcast;        
        static char              s_ap[128];   
        static bool              s_allowStatusSend;  
        static char              s_readBuffer[4 * 1024];
        static char              s_mac[32];         
};

#endif