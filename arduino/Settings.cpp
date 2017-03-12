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
#include <DueFlashStorage.h>
#include <ArduinoJson.h>

#include "debug.h"
#include "Settings.h"

Settings::Configuration Settings::s_config;
bool                    Settings::s_loaded = false;
char Settings::s_jsonBuf[4 * 1024] = {0};
DueFlashStorage dueFlashStorage;

#define SETTINGS_ADDR 0
#define DEFAULT_BCAST_RATE 30000
#define DEFAULT_PORT_NUM 12982
#define DEFAULT_LOGO_IDX 0

const char * Settings::GetJson(bool hidePass)
{
    if(!s_loaded) Load();
            
    StaticJsonBuffer<256> jsonBuffer;
    JsonObject& root = jsonBuffer.createObject();
    root["ssid"] = s_config.ssid.c_str();
    
    if(!hidePass)
    {
        root["pass"] = s_config.pass.c_str();
    }
    
    root["brewer"] = s_config.brewer.c_str();
    root["bcastrate"] = s_config.bcastrate;
    root["portnum"] = s_config.portnum;
    root["remote"] = s_config.remoteBucket.c_str();
    root["logoidx"] = s_config.logoidx;

    memset(s_jsonBuf, 0, sizeof(s_jsonBuf));
    root.printTo((char *) s_jsonBuf, sizeof(s_jsonBuf));
        
    return s_jsonBuf;
}

void Settings::FromJson(const char * jsonString)
{
    DEBUG(F("Settings from json: %s"), jsonString);                
    StaticJsonBuffer<256> jsonBuffer;
    JsonObject& root = jsonBuffer.parseObject(jsonString);
    s_config.ssid = root["ssid"].asString();
    s_config.pass = root["pass"].asString();
    s_config.brewer = root["brewer"].asString();
    s_config.remoteBucket = root["remote"].asString();
               
    if(root.containsKey("bcastrate"))
    {
        s_config.bcastrate = root["bcastrate"];
    }
        
    if(root.containsKey("portnum"))
    {
        s_config.portnum = root["portnum"];    
    }            

    if(root.containsKey("logoidx"))
    {
        s_config.logoidx = root["logoidx"];
    }                    

    if(s_config.logoidx != 0 && s_config.logoidx != 1)
    {
        s_config.logoidx = 0;
    }
}

void Settings::Load()
{            
    s_loaded = true;

    s_config.bcastrate = DEFAULT_BCAST_RATE;
    s_config.portnum = DEFAULT_PORT_NUM;
    s_config.logoidx = DEFAULT_LOGO_IDX;
            
    byte *b = dueFlashStorage.readAddress(SETTINGS_ADDR);
    Settings::StoredConfiguration storedConfig = {0};    
    memcpy(&storedConfig, b, sizeof(Settings::StoredConfiguration));
        
    if(storedConfig.invalid == 0x00)
    {                        
        FromJson(storedConfig.jsonString);                                
    }
    else
    {
        DEBUG(F("Fresh flash, no config"));        
    }     
}

void Settings::Save()
{
    Settings::StoredConfiguration storedConfig = {0};    
    byte buffer[sizeof(Settings::StoredConfiguration)] = {0};
    
    const char * json = GetJson();
    storedConfig.invalid = 0x00;        
    memset(storedConfig.jsonString, 0, sizeof(storedConfig.jsonString));
    strncpy(storedConfig.jsonString, json, sizeof(storedConfig.jsonString) - 1);
    DEBUG(F("Saving config: %s"), storedConfig.jsonString);
    
    memcpy(buffer, &storedConfig, sizeof(Settings::StoredConfiguration));
    dueFlashStorage.write(SETTINGS_ADDR, buffer, sizeof(Settings::StoredConfiguration));                     
}

Settings::Configuration& Settings::Get()
{             
    if(!s_loaded) Load();
    return s_config;    
}

void Settings::ResetToFactory()
{
    s_config.portnum = DEFAULT_PORT_NUM;
    s_config.bcastrate = DEFAULT_BCAST_RATE;
    s_config.ssid = "";
    s_config.pass = "";
    s_config.brewer = "";
    s_config.remoteBucket = "";
    s_config.logoidx = 0;
    Save();
}

const char * Settings::GetVersion()
{
    return "Version: 2.1.5";    
}
