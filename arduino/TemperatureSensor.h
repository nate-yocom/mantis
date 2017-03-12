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
 
#ifndef TEMP_SENSORS_H
#define TEMP_SENSORS_H

#include <DallasTemperature.h>  // Temperature library for our temperature sensors
#include <OneWire.h>            // Communication library for the Dallas one wire devices

#define DISCONNECTED_TEMPERATURE    -1.0
#define TEMP_SENSOR_RESOLUTION      10      // Temp sensor resolution for all sensors, 9-12 bit
#define MAX_TEMP_SENSORS_ATTACHED   5

/** 
 * Instance methods for manipulating a OneWire temperature controller,
 *  and static methods for maintaining a collection of up to MAX_TEMP_SENSORS_ATTACHED
 *  in total, access by friend 'name'.
 **/
class TemperatureSensor
{
    public:
        TemperatureSensor(const char *name, unsigned int pin);        
        float Read();        
        bool IsConnected();
        const char * Name();
        unsigned long LastReadingTS();
        static void AddSensor(const char *name, int pin);        
        static TemperatureSensor * GetSensor(const char *name);
        
    private:
        void ReInit();
        static void InitStatic();
                
        OneWire *           m_oneWire;
        DallasTemperature * m_dallas;
        const char *        m_name;        
        float               m_lastReading;
        bool                m_connected;
        unsigned long       m_lastReadingTS;
        
        static bool s_sensorsArrayInited;
        static TemperatureSensor* s_sensors[MAX_TEMP_SENSORS_ATTACHED];
};

#endif