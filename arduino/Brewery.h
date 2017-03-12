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
 
#ifndef _INCLUDE_BREWERY_H_
#define _INCLUDE_BREWERY_H_

#include "InputEvents.h"

class Brewery
{
    public:
        enum Pump2Mode {
            Off     = 0x00,
            Temp    = 0x01,
            Float   = 0x02,
            On      = 0x03
        };

        typedef struct BreweryState
        {    
            float       HltTemp;
            float       HltTargetTemp;
            float       MashTemp;
            float       MashTargetTemp;
            float       BoilTemp;
            bool        MashFloatOn;
            bool        HltFloatOn;
            bool        Pump1On;
            bool        Pump2On;
            bool        BurnerOn;
            bool        Wireless;
            bool        RemoteControlled;            
            Pump2Mode   Pump2AutoMode;
        } BreweryState;
    
        static void Init();
        static BreweryState CurrentState();
        
        static void DeltaMashTarget(float delta);        
        static void DeltaHltTarget(float delta);
        static void SetMashTarget(float value);
        static void SetHltTarget(float value);
                
        static void SetPump1(bool on);        
        static void SetPump2(bool on);
        static void SetPump2Mode(Pump2Mode mode);
        static void SetBurner(bool on);
        static void SetWireless(bool on);
        static void SetRemoteControl(bool on);        
                
        static void Tick();
        static void UpdateDigital();
        
    private:
        static void AutoBrew();
        
        static BreweryState s_state;        
        static unsigned long s_lastPump2Change;
        static unsigned long s_lastBurnerChange;
        
        class BreweryInputListener : public InputEvents::EventListener
        {
            public:
                virtual void HandleEvent(InputEvents::Event, const void *data);
        };
        
        static BreweryInputListener s_eventListener; 
};

#endif