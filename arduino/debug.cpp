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
 
#include <stdarg.h>
#include <Arduino.h>
#include "InputEvents.h"


void DEBUG(const __FlashStringHelper *fmt, ...)
{
    char buf[256] = {0}; // resulting string limited to 128 chars
    va_list args;
    va_start (args, fmt);
#ifdef __AVR__
    vsnprintf_P(buf, sizeof(buf) - 1, (const char *)fmt, args); // progmem for AVR
#else
    vsnprintf(buf, sizeof(buf) - 1, (const char *)fmt, args); // for the rest of the world
#endif
    va_end(args);
    char buf2[256] = {0};
    snprintf(buf2, sizeof(buf2) - 1, "[%010ld] %s", millis(), buf);
    Serial.println(buf2);
}

void STATUS(const __FlashStringHelper *fmt, ...)
{
    char buf[256] = {0}; // resulting string limited to 128 chars
    va_list args;
    va_start (args, fmt);
#ifdef __AVR__
    vsnprintf_P(buf, sizeof(buf) - 1, (const char *)fmt, args); // progmem for AVR
#else
    vsnprintf(buf, sizeof(buf) - 1, (const char *)fmt, args); // for the rest of the world
#endif
    va_end(args);
    char buf2[256] = {0};
    snprintf(buf2, sizeof(buf2) - 1, "[%010ld] %s", millis(), buf);
    Serial.println(buf2);
    InputEvents::PublishEvent(InputEvents::StatusTxt, buf);
}