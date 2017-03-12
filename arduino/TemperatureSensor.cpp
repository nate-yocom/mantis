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
 
#include <DallasTemperature.h>  // Temperature library for our temperature sensors
#include <OneWire.h>            // Communication library for the Dallas one wire devices

#include "debug.h"
#include "TemperatureSensor.h"

/** Static **/
bool TemperatureSensor::s_sensorsArrayInited = false;
TemperatureSensor* TemperatureSensor::s_sensors[MAX_TEMP_SENSORS_ATTACHED] = {0};

void TemperatureSensor::InitStatic()
{
    if(!s_sensorsArrayInited)
    {
        for(int x = 0; x < MAX_TEMP_SENSORS_ATTACHED; x++)
        {
            s_sensors[x] = NULL;
        }
        s_sensorsArrayInited = true;
    }
}       
                  
void TemperatureSensor::AddSensor(const char *name, int pin)
{
    InitStatic();
          
    TemperatureSensor * sensor = new TemperatureSensor(name, pin);
    for(int x = 0; x < MAX_TEMP_SENSORS_ATTACHED; x++)
    {
        if(s_sensors[x] == NULL)
        {
            s_sensors[x] = sensor;
            return;       
        }
    }
}
        
TemperatureSensor * TemperatureSensor::GetSensor(const char *name)
{
    for(int x = 0; x < MAX_TEMP_SENSORS_ATTACHED; x++)
    {
        if(s_sensors[x] != NULL && strcmp(name, s_sensors[x]->Name()) == 0)
            return s_sensors[x];
    }
            
    DEBUG(F("Unable to find sensor object for: %s!"), name);
    return NULL;
}
            
            
/** Instance **/

TemperatureSensor::TemperatureSensor(const char *name, unsigned int pin)
{
    m_name = name;    
    m_oneWire = new OneWire(pin);
    m_dallas = new DallasTemperature(m_oneWire);
    ReInit();
}
        
float TemperatureSensor::Read()
{
    if(millis() - m_lastReadingTS >= 500)
    {        
        m_lastReadingTS = millis();
                
        float tempReading = m_dallas->getTempFByIndex(0);
        if (tempReading <= -127)
        {
            if(m_connected == true)
            {
                DEBUG(F("Probe for %s is no longer connected"), m_name);
                m_connected = false;
            }
            m_lastReading = DISCONNECTED_TEMPERATURE;                                
            return DISCONNECTED_TEMPERATURE;
        }
                
        if (tempReading >= -127 && m_connected == false)
        {
            DEBUG(F("%s probe inserted, initializing sensor"), m_name);
            m_lastReading = 0.0;
            ReInit();
            m_connected = true;              
            delay(500);        
        }
                
        m_lastReading = tempReading;
        m_dallas->requestTemperatures();        
    }        
                    
    return m_lastReading;    
}      
        
bool TemperatureSensor::IsConnected()
{
    return m_connected;
}
        
const char * TemperatureSensor::Name()
{
    return m_name;
}

unsigned long TemperatureSensor::LastReadingTS()
{
    return m_lastReadingTS;
}

void TemperatureSensor::ReInit()
{
    DeviceAddress sensorAddress;        
    DEBUG(F("Initializing temperature sensor: %s"), m_name);        
    m_dallas->begin();
    m_dallas->getAddress(sensorAddress, 0);
    m_dallas->setResolution(sensorAddress, TEMP_SENSOR_RESOLUTION);
    m_dallas->setWaitForConversion(false);
    m_dallas->requestTemperatures();    
    DEBUG(F("%s initialized"), m_name);
}
