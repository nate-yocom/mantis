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
 
#ifndef DEFINED_STUFF_H
#define DEFINED_STUFF_H

// Define output pin numbers
#define OUT_D4_DISPLAY_PIN          4
#define OUT_SPEAKER_PIN             11
#define OUT_MASH_ONEWIRE_BUS_PIN    24
#define OUT_KETTLE_ONEWIRE_BUS_PIN  26
#define OUT_HLT_ONEWIRE_BUS_PIN     28
#define OUT_HLT_BURNER_STATUS_PIN   32
#define OUT_HLT_BURNER_PIN          44
#define OUT_MASH_PUMP_PIN           46 
#define OUT_PUMP2_PIN               46
#define OUT_WORT_PUMP_PIN           48
#define OUT_PUMP1_PIN               48
#define OUT_ALARM_PIN               49

// Define input pin numbers
#define INPUT_HLT_FLOAT_PIN         22
#define INPUT_MASH_FLOAT_PIN        23

// Unused pins, init'd only
#define UNUSED_AUX_PIN_2   47
#define UNUSED_AUX_PIN_3   45
#define UNUSED_AUX_PIN_4   43

#define MASH_SENSOR     "Mash"
#define HLT_SENSOR      "HLT"
#define KETTLE_SENSOR   "Kettle"

#endif