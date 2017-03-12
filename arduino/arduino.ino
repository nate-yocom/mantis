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

#include <math.h>

#include <DueFlashStorage.h>    // Flash storage for user-customizable bits

#include "defines.h"
#include "debug.h"
#include "Brewery.h"
#include "Screen.h"
#include "TemperatureSensor.h"
#include "InputEvents.h"
#include "Wireless.h"
#include "Memory.h"

void setupInitPins()
{
    DEBUG(F("Initializing pins"));
    
    // Output pins
    pinMode(OUT_HLT_BURNER_PIN, OUTPUT);
    digitalWrite(OUT_HLT_BURNER_PIN, LOW);
   
    pinMode(OUT_MASH_PUMP_PIN, OUTPUT);
    digitalWrite(OUT_MASH_PUMP_PIN, LOW);
   
    pinMode(OUT_WORT_PUMP_PIN, OUTPUT);       
    digitalWrite(OUT_WORT_PUMP_PIN, LOW);
   
    pinMode(OUT_ALARM_PIN, OUTPUT);
    digitalWrite(OUT_ALARM_PIN, LOW);
         
    pinMode(OUT_SPEAKER_PIN, OUTPUT);
    digitalWrite(OUT_SPEAKER_PIN, LOW);  
   
    pinMode(OUT_HLT_BURNER_STATUS_PIN, OUTPUT);
    digitalWrite(OUT_HLT_BURNER_STATUS_PIN, LOW);
   
    // Input pins
    pinMode(INPUT_HLT_FLOAT_PIN, INPUT);
    pinMode(INPUT_MASH_FLOAT_PIN, INPUT);
   
    // Unused pins
    pinMode(UNUSED_AUX_PIN_2, OUTPUT);    
    pinMode(UNUSED_AUX_PIN_3, OUTPUT);    
    pinMode(UNUSED_AUX_PIN_4, OUTPUT);    
}

void setup()
{
    Serial.begin(115200);     // Main serial port / monitor / debug out
    
    // Set our pins to their initial input/output state and values
    setupInitPins();
    
    DEBUG(F("Init brewery..."));
    Brewery::Init();
            
    DEBUG(F("Init wireless component..."));
    Wireless::Init();
    
    DEBUG(F("Setup screen..."));
    Screen::Init();
            
    DEBUG(F("Loading form 0 onto display..."));
    Screen::ShowForm(0);                                                                  
}

unsigned long lastStat = 0;
unsigned long tickStart = 0;
unsigned long tickEnd = 0;

void loop() 
{
    tickStart = millis();
    
    Brewery::Tick();        // Update from the brewery, gets temps, status of floats, etc
    Screen::Tick();         // Tick the screen, publishes and effects callbacks
    Wireless::Tick();       // Update to/from the wireless  
    
    tickEnd = millis();

#if DIAGNOSTICS
    if(millis() - lastStat >= 60000)
    {
        DEBUG(F("Tick: %d (delta: %d) - Mem: %d"), millis(), tickEnd - tickStart, Memory::GetFree());
        Memory::Dump();
        lastStat = millis();
    }  
#endif             
}

