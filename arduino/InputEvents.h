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
 
#ifndef _INCLUDE_INPUT_EVENTS_H
#define _INCLUDE_INPUT_EVENTS_H

class InputEvents
{
    public:
        enum Event {
            MashTargetSet = 0,
            HltTargetSet  = 1,
            Pump1         = 2,
            Pump2ModeChange = 3,
            Burner        = 4,
            Wireless      = 5,
            RemoteControl = 6,
            DebugTxt      = 7,   // Sent out by the system
            StatusTxt     = 8,   // Sent by the system, or by other parties (UI/Wireless)
            BroadcastReq  = 9,
            Ping          = 10,
            SetBrewer     = 11,
            Pump2         = 12,
            ScreenChange  = 13,
            NetworkChange = 14,            
            
            // This should always be last
            MAX           = 128,        
        };
        
        class EventListener
        {
            public:
                virtual void HandleEvent(Event, const void *data); 
        };
        
        static void PublishEvent(Event, const void *data);
        static void AddListener(EventListener *listener);
        
    private:        
        static EventListener *s_listeners[5];
};

#endif