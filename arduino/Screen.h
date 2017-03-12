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
 
#ifndef _INCLUDE_SCREEN_H_
#define _INCLUDE_SCREEN_H_

#include <inttypes.h>
#include <stdint.h>
#include <genieArduino.h>       // Display library for the screen
#include "BackBufferedObjects.h"
/**
 * A static class for manipulating and updating the screen,
 *  as well as marshalling callbacks for button actions etc.
 */
class Screen
{
    public:
        static void Init();
        static void ShowForm(uint16_t formNumber);
        static void Tick();
        static void AddStatus(const char *line);
        
        static void ScreenEventsHandler();
    private:
        static void ScreenChange();
        static void ScreenEventsBrewery(genieFrame event);
        static void ScreenEventsSettings(genieFrame event);
        static void TickBrewery();
        static void TickSettings();        
        static void SetBrewer(const char *brewer);                        
        static void WriteObject(uint16_t objType, uint16_t objIndex, uint16_t data);
    
        static unsigned long    s_lastPushTick;
        static Genie   *        s_genie;
                        
        class ScreenEventListener : public InputEvents::EventListener
        {
            public:
                virtual void HandleEvent(InputEvents::Event, const void *data);
        };        

        // Strings        
        static BackBufferedString * s_status0;
        static BackBufferedString * s_status1;
        static BackBufferedString * s_status2;
        static BackBufferedString * s_status3;
        static BackBufferedString * s_status4;                
        static BackBufferedString * s_brewer;
        static BackBufferedString * s_brewerEdit;
        static BackBufferedString * s_version;
        static BackBufferedString * s_ap;
        
        // Temperatures
        static BackBufferedData * s_boilTemp;
        static BackBufferedData * s_mashTemp;
        static BackBufferedData * s_hltTemp; 
        
        // Status LEDs
        static BackBufferedData * s_pump1On;
        static BackBufferedData * s_pump2On;
        static BackBufferedData * s_burnerOn;
        static BackBufferedData * s_mashFloatOn;
        static BackBufferedData * s_remoteControlled;                    
        
        static ScreenEventListener s_eventListener;
        
        enum ActivePage
        {
            Main = 0,
            Settings = 1,
            Loading = 2            
        };
        
        static ActivePage    s_activePage;    
};

#endif