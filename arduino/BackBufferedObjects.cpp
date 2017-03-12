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
#include "BackBufferedObjects.h"

BackBufferedObject::BackBufferedObject(Genie *genie, uint16_t index) 
{
    m_genie = genie;
    m_index = index;
}

BackBufferedData::BackBufferedData(Genie *genie, uint16_t objectId, uint16_t index)
    : BackBufferedObject(genie, index) 
{
    m_objectId = objectId;
    m_displayedValue = 0;
    m_currentValue = 0;
    m_lastDisplayMillis = 0;
}

void BackBufferedData::Tick() 
{
    if (m_currentValue != m_displayedValue || m_lastDisplayMillis == 0) 
    {
        m_lastDisplayMillis = millis();
        m_genie->WriteObject(m_objectId, m_index, m_currentValue);
        m_displayedValue = m_currentValue;
    }
}

void BackBufferedData::SetValue(uint16_t newValue) 
{
    m_currentValue = newValue;
}

uint16_t BackBufferedData::GetValue() 
{ 
    return m_currentValue; 
}

void BackBufferedData::Refresh() 
{  
    m_lastDisplayMillis = 0;
}

BackBufferedString::BackBufferedString(Genie *genie, uint16_t index)
    : BackBufferedObject(genie, index) 
{
    memset(m_lastValue, 0, sizeof(m_lastValue));
    memset(m_currentValue, 0, sizeof(m_currentValue));
}

void BackBufferedString::Tick() 
{
    if (stricmp(m_currentValue, m_lastValue) != 0) 
    {
        m_genie->WriteStr(m_index, m_currentValue);
        memcpy(m_lastValue, m_currentValue, sizeof(m_currentValue));
    }
}

void BackBufferedString::SetValue(const char *newValue)
{
    memset(m_currentValue, 0, sizeof(m_currentValue));
    if (strlen(newValue) > 0) 
    {
        strncpy(m_currentValue, newValue, sizeof(m_currentValue) - 1);
    }
}

const char *BackBufferedString::GetValue() 
{ 
    return m_currentValue; 
}

void BackBufferedString::Refresh() 
{
    memset(m_lastValue, 0, sizeof(m_lastValue));
    m_lastValue[0] = ' ';
}