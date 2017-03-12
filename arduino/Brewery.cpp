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
#include "debug.h"
#include "defines.h"
#include "TemperatureSensor.h"
#include "Brewery.h"
#include "InputEvents.h"
#include "Wireless.h"

Brewery::BreweryState Brewery::s_state = {0};
Brewery::BreweryInputListener Brewery::s_eventListener;
unsigned long Brewery::s_lastPump2Change = 0;
unsigned long Brewery::s_lastBurnerChange = 0;

void Brewery::Init()
{
    DEBUG(F("Brewery initalizing state..."));
    s_state.HltTemp = 0;
    s_state.HltTargetTemp = 162;
    s_state.MashTemp = 0;
    s_state.MashTargetTemp = 150;
    s_state.BoilTemp = 0;
    s_state.MashFloatOn = false;
    s_state.HltFloatOn = false;
    s_state.Pump1On = false;
    s_state.Pump2On = false;
    s_state.Pump2AutoMode = Off;
    s_state.Wireless = false;
    s_state.RemoteControlled = false;
    s_state.BurnerOn = false;
    
    SetPump1(false);
    SetPump2(false);
    SetBurner(false);
    
    // Initialize temperature sensors
    DEBUG(F("Setup and init temperature sensors..."));
    TemperatureSensor::AddSensor(MASH_SENSOR, OUT_MASH_ONEWIRE_BUS_PIN);
    TemperatureSensor::AddSensor(HLT_SENSOR, OUT_HLT_ONEWIRE_BUS_PIN);
    TemperatureSensor::AddSensor(KETTLE_SENSOR, OUT_KETTLE_ONEWIRE_BUS_PIN);
    
    InputEvents::AddListener(&s_eventListener);
}

Brewery::BreweryState Brewery::CurrentState()
{
    Brewery::BreweryState stateCopy = s_state;
    return stateCopy;    
}
        
void Brewery::DeltaMashTarget(float delta)
{    
    s_state.MashTargetTemp += delta;
}   
     
void Brewery::DeltaHltTarget(float delta)
{
    s_state.HltTargetTemp += delta;
}

void Brewery::SetMashTarget(float value)
{
    s_state.MashTargetTemp = value;
}

void Brewery::SetHltTarget(float value)
{
    s_state.HltTargetTemp = value;
}
                
void Brewery::SetPump1(bool on)
{                 
    digitalWrite(OUT_PUMP1_PIN, on ? HIGH : LOW);
    s_state.Pump1On = on;                
}

void Brewery::SetPump2(bool on)
{
    s_lastPump2Change = millis();
    digitalWrite(OUT_PUMP2_PIN, on ? HIGH : LOW);
    s_state.Pump2On = on;    
}
        
void Brewery::SetPump2Mode(Pump2Mode mode)
{    
    s_state.Pump2AutoMode = mode;    
}

void Brewery::SetBurner(bool on)
{
    s_lastBurnerChange = millis();
    s_state.BurnerOn = on;
    digitalWrite(OUT_HLT_BURNER_PIN, on ? HIGH : LOW);
    digitalWrite(OUT_HLT_BURNER_STATUS_PIN, on ? HIGH : LOW);
}

void Brewery::SetWireless(bool on)
{
    // If turning on, and not already on, reset wireless to bring it into life
    if(!s_state.Wireless && on)
    {        
        Wireless::Reset();
        if(!Wireless::Available())
        {
            STATUS(F("Wireless mode cannot be changed, no wireless chip available"));
            return;
        }           
    }  
    
    if(s_state.Wireless && !on)
    {
        Wireless::Disconnect();
        STATUS(F("Wireless disconnected"));
    }      
        
    s_state.Wireless = on;
    if(s_state.RemoteControlled && !s_state.Wireless)
    {
        SetRemoteControl(false);
    }    
}

void Brewery::SetRemoteControl(bool on)
{
    if(on && s_state.Wireless)
    {
        STATUS(F("Network controller attached, remote control on"));
        s_state.RemoteControlled = on;
    } 
    else    
    {
        STATUS(F("Network controller detached, remote control off"));
        s_state.RemoteControlled = false;
    }
}

void Brewery::UpdateDigital()
{
    s_state.MashFloatOn = digitalRead(INPUT_MASH_FLOAT_PIN) != 0;
    s_state.HltFloatOn = digitalRead(INPUT_HLT_FLOAT_PIN) != 0;            
    s_state.Pump1On = digitalRead(OUT_PUMP1_PIN) != LOW;    
    s_state.Pump2On = digitalRead(OUT_PUMP2_PIN) != LOW;    
    s_state.BurnerOn = digitalRead(OUT_HLT_BURNER_PIN) != LOW;   
}

void Brewery::Tick()
{
    s_state.HltTemp = TemperatureSensor::GetSensor(HLT_SENSOR)->Read();
    s_state.MashTemp = TemperatureSensor::GetSensor(MASH_SENSOR)->Read();    
    s_state.BoilTemp = TemperatureSensor::GetSensor(KETTLE_SENSOR)->Read();
    
    UpdateDigital();
            
    AutoBrew();              
}

void Brewery::AutoBrew()
{    
    if(!s_state.RemoteControlled)
    {
        switch(s_state.Pump2AutoMode)
        {
            case Pump2Mode::Off:
                if(s_state.Pump2On)
                {
                    SetPump2(false);  
                } 
                break;
            case Pump2Mode::On:
                if(!s_state.Pump2On)
                {                
                    SetPump2(true);
                }
                break;            
            case Pump2Mode::Temp:
                if(s_state.MashTemp <= s_state.MashTargetTemp - 0.50)
                {
                    // If we just turned it off, don't let it come back on for another 5 secs or so'
                    if(!s_state.Pump2On && millis() - s_lastPump2Change >= 5000)
                    {
                        STATUS(F("Mash temp too low, and pump not running - starting pump"));
                        SetPump2(true);
                    }
                }
                else if(s_state.Pump2On)
                {
                    STATUS(F("Mash temp good, and running - stopping pump"));
                    SetPump2(false);
                }
                break;
            case Pump2Mode::Float:
                if(!s_state.MashFloatOn)
                {
                    // If we just turned it off, give it at least 2 seconds before starting again
                    if(!s_state.Pump2On && millis() - s_lastPump2Change >= 2000)
                    {
                        STATUS(F("Mash water too low, and pump not running - starting pump"));
                        SetPump2(true);
                    }
                }
                else if(s_state.Pump2On)
                {
                    STATUS(F("Mash water good, and pump running - stopping pump"));
                    SetPump2(false);
                }
                break;            
        }
    }
        
    // If the HLT float is too low, done, its off, period
    if(!s_state.HltFloatOn)
    {
        if(s_state.BurnerOn)
        {
            STATUS(F("HLT float low, burning running, stopping burner"));            
            SetBurner(false);
        }
    }
    else if(!s_state.RemoteControlled)
    {
        // Always try and get HLT to target temp
        if(s_state.HltTemp <= s_state.HltTargetTemp - 0.50)
        {
            // Must be at least 5 seconds from last state change
            if(!s_state.BurnerOn && millis() - s_lastBurnerChange > 5000)
            {
                STATUS(F("HLT temp low, burner not running, starting burner"));
                SetBurner(true);
            }            
        }             
        else if(s_state.BurnerOn)
        {
            STATUS(F("HLT temp good, burner running, stopping burner"));
            SetBurner(false);
        }
    }      
}

void Brewery::BreweryInputListener::HandleEvent(InputEvents::Event event, const void *data)
{                                        
    switch(event)
    {
        case InputEvents::MashTargetSet:
            Brewery::SetMashTarget(*((float *)data));
            break;
        case InputEvents::HltTargetSet:
            Brewery::SetHltTarget(*((float *)data));
            break;
        case InputEvents::Pump2ModeChange:            
            Brewery::SetPump2Mode(*((Brewery::Pump2Mode *) data)); 
            break; 
        case InputEvents::Pump1:
            Brewery::SetPump1(*((bool *) data));
            break;
        case InputEvents::Pump2:
            Brewery::SetPump2(*((bool *) data));
            break;                    
        case InputEvents::Wireless:
            Brewery::SetWireless(*((bool *) data));
            break;                    
        case InputEvents::Burner:
            Brewery::SetBurner(*((bool *) data));
            break;
        case InputEvents::RemoteControl:
            Brewery::SetRemoteControl(*((bool *) data));
            break;            
    }
}