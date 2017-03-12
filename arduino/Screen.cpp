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
#include "Brewery.h"
#include "debug.h"
#include "Screen.h"
#include "InputEvents.h"
#include "Settings.h"

unsigned long   Screen::s_lastPushTick = 0;
Genie *         Screen::s_genie = NULL;

BackBufferedString * Screen::s_status0 = NULL;
BackBufferedString * Screen::s_status1 = NULL;
BackBufferedString * Screen::s_status2 = NULL;
BackBufferedString * Screen::s_status3 = NULL;
BackBufferedString * Screen::s_status4 = NULL;
BackBufferedString * Screen::s_brewer = NULL;
BackBufferedString * Screen::s_brewerEdit = NULL;
BackBufferedString * Screen::s_version = NULL;
BackBufferedString * Screen::s_ap = NULL;
BackBufferedData * Screen::s_boilTemp  = NULL;
BackBufferedData * Screen::s_mashTemp = NULL;
BackBufferedData * Screen::s_hltTemp = NULL;                 
BackBufferedData * Screen::s_pump1On = NULL;
BackBufferedData * Screen::s_pump2On = NULL;
BackBufferedData * Screen::s_burnerOn = NULL;
BackBufferedData * Screen::s_mashFloatOn = NULL;
BackBufferedData * Screen::s_remoteControlled = NULL;
                        
Screen::ActivePage    Screen::s_activePage = Screen::ActivePage::Main;
        
Screen::ScreenEventListener Screen::s_eventListener;

// UI Types
#define UI_KNOB     3
#define UI_BUTTON   6
#define UI_FORM     10
#define UI_KYBD     13
#define UI_FLIP     30


// UI Indexes
#define UI_TARGET_MASH_UP      0x01 
#define UI_TARGET_MASH_DOWN    0x02
#define UI_TARGET_HLT_UP       0x03
#define UI_TARGET_HLT_DOWN     0x04
#define UI_PUMP_1              0x00 
#define UI_WIRELESS            0x01
#define UI_PUMP_2              0x00
#define UI_SETTINGS_KYBD       0x00
#define UI_SETTINGS_CLEAR      0x06
#define UI_SETTINGS_SAVE       0x07
#define UI_SETTINGS_RESET      0x08

static void EventCallback()
{
    Screen::ScreenEventsHandler();
}
        
void Screen::Init()
{
    Serial1.begin(115200); //Display serial port
    s_genie = new Genie();
    s_genie->assignDebugPort(Serial);
    s_genie->Begin(Serial1);  //Start Display comms on Serial1   
    s_genie->AttachEventHandler(EventCallback);  //event handler for receiving things from the display        
    
    s_status0 = new BackBufferedString(s_genie, 0x00);
    s_status1 = new BackBufferedString(s_genie, 0x05);
    s_status2 = new BackBufferedString(s_genie, 0x06);
    s_status3 = new BackBufferedString(s_genie, 0x07);
    s_status4 = new BackBufferedString(s_genie, 0x08);

    s_brewer = new BackBufferedString(s_genie, 0x01);
    s_version = new BackBufferedString(s_genie, 0x02);
    s_brewerEdit = new BackBufferedString(s_genie, 0x03);    
    s_ap = new BackBufferedString(s_genie, 0x04);

    s_boilTemp = new BackBufferedData(s_genie, GENIE_OBJ_LED_DIGITS, 0x00);
    s_mashTemp = new BackBufferedData(s_genie, GENIE_OBJ_LED_DIGITS, 0x01);
    s_hltTemp = new BackBufferedData(s_genie, GENIE_OBJ_LED_DIGITS, 0x02);                 
    s_pump1On = new BackBufferedData(s_genie, GENIE_OBJ_USER_LED, 0x00);
    s_pump2On = new BackBufferedData(s_genie, GENIE_OBJ_USER_LED, 0x01);
    s_burnerOn = new BackBufferedData(s_genie, GENIE_OBJ_USER_LED, 0x02);
    s_mashFloatOn = new BackBufferedData(s_genie, GENIE_OBJ_USER_LED, 0x03);
    s_remoteControlled = new BackBufferedData(s_genie, GENIE_OBJ_USER_LED, 0x04);
        
    //Reset the Display (change D4 to D2 if you have original 4D Arduino Adaptor)
    DEBUG(F("Reseting display..."));
    pinMode(4, OUTPUT);  // Set D4 on Arduino to Output (4D Arduino Adaptor V2 - Display Reset)
    digitalWrite(4, 1);  // Reset the Display via D4
    delay(100);
    digitalWrite(4, 0);  // unReset the Display via D4  
    delay(3500); //let the display start up   
    s_genie->WriteContrast(15); //Set the display to maximum brightness.   
    delay(2000);  //Another one second delay in start up to allow things to come up.
    
    s_status0->SetValue(" ");
    s_status1->SetValue(" ");
    s_status2->SetValue(" ");
    s_status3->SetValue(" ");
    s_status4->SetValue(" ");
    s_ap->SetValue(" ");
                    
    SetBrewer(Settings::Get().brewer.c_str());
    s_version->SetValue(Settings::GetVersion());

    InputEvents::AddListener(&s_eventListener);
    
    Brewery::BreweryState currState = Brewery::CurrentState();
    InputEvents::PublishEvent(InputEvents::MashTargetSet, &currState.MashTargetTemp);    
    InputEvents::PublishEvent(InputEvents::HltTargetSet, &currState.HltTargetTemp);
    
    WriteObject(GENIE_OBJ_USERIMAGES, 0x00, Settings::Get().logoidx);
    STATUS(F("System initialized"));
}
 
void Screen::ShowForm(uint16_t formNumber)
{
    WriteObject(GENIE_OBJ_FORM, formNumber, 0);
    s_activePage = (Screen::ActivePage) formNumber;
    ScreenChange();
}

void Screen::SetBrewer(const char *brewer)
{
    s_brewer->SetValue(brewer);  

    // Hack, but wireless always sets brewer when remote config changes, so we force logo correct here too
    WriteObject(GENIE_OBJ_USERIMAGES, 0x00, Settings::Get().logoidx);  
}

void Screen::WriteObject(uint16_t objType, uint16_t objIndex, uint16_t data)
{
    s_genie->WriteObject(objType, objIndex, data);
}

void Screen::ScreenEventsHandler()
{    
    genieFrame event;
    while(s_genie->DequeueEvent(&event))
    {
        DEBUG(F("UI event, command: %d object: %d index: %d MSB: x%02x LSB: x%02x"), 
            event.reportObject.cmd, event.reportObject.object, event.reportObject.index, event.reportObject.data_msb, event.reportObject.data_lsb);
               
        if(event.reportObject.cmd == GENIE_REPORT_EVENT && event.reportObject.object == UI_FORM)
        {            
            InputEvents::PublishEvent(InputEvents::ScreenChange, &event.reportObject.index);            
        }                    
        
        ScreenEventsBrewery(event);
        ScreenEventsSettings(event);                                                                                                
    }    
}

void Screen::ScreenEventsSettings(genieFrame event)
{
    // Keyboard event    
    if(event.reportObject.cmd == GENIE_REPORT_EVENT && event.reportObject.object == UI_KYBD)
    {
        switch(event.reportObject.index)
        {
            case UI_SETTINGS_KYBD:
                char character = (char) event.reportObject.data_lsb;
                char tmp[256] = {0};                                
                if(character == 0x08)   // backspace
                {
                    if(strlen(s_brewerEdit->GetValue()) > 1)
                    {                   
                        strncpy(tmp, s_brewerEdit->GetValue(), strlen(s_brewerEdit->GetValue()) - 1);
                        s_brewerEdit->SetValue(tmp);                    
                    }             
                    else
                    {
                        s_brewerEdit->SetValue("");                            
                    }                                                                   
                }
                else
                {
                    strncpy(tmp, s_brewerEdit->GetValue(), sizeof(tmp) - 2);
                    tmp[strlen(tmp)] = character;
                    s_brewerEdit->SetValue(tmp);                       
                }                                
                break;
        }
    }
    
    if(event.reportObject.cmd == GENIE_REPORT_EVENT && event.reportObject.object == UI_BUTTON)
    {
        switch(event.reportObject.index)
        {
            case UI_SETTINGS_CLEAR:
                s_brewerEdit->SetValue("");                
                break;
            case UI_SETTINGS_SAVE:            
                InputEvents::PublishEvent(InputEvents::Event::SetBrewer, s_brewerEdit->GetValue());
                ShowForm(0);                                
                break;
            case UI_SETTINGS_RESET:
                s_brewerEdit->SetValue("");
                InputEvents::PublishEvent(InputEvents::Event::SetBrewer, s_brewerEdit->GetValue());
                Settings::ResetToFactory();
                ShowForm(0);                
                break;    
        }
    }
}

void Screen::ScreenEventsBrewery(genieFrame event)
{
    // If we are remote controlled, the only option you can tweak via the UI is to turn off wireless,
    //  which then disables remote control:
    Brewery::BreweryState currState = Brewery::CurrentState();

    if(event.reportObject.cmd == GENIE_REPORT_EVENT && event.reportObject.object == UI_FLIP)
    {
        switch(event.reportObject.index)
        {
            case UI_WIRELESS:
                bool on = event.reportObject.data_lsb != 0;
                InputEvents::PublishEvent(InputEvents::Wireless, &on);
                break;
        }
    }
        
    if(!currState.RemoteControlled)
    {       
        if(event.reportObject.cmd == GENIE_REPORT_EVENT && event.reportObject.object == UI_FLIP)
        {
            switch(event.reportObject.index)
            {
                case UI_PUMP_1:   
                    bool on = event.reportObject.data_lsb != 0;
                    InputEvents::PublishEvent(InputEvents::Pump1, &on);                                     
                    break;                                
            }
        }
            
        if(event.reportObject.cmd == GENIE_REPORT_EVENT && event.reportObject.object == UI_BUTTON)
        {
            switch(event.reportObject.index)
            {
                case UI_TARGET_MASH_UP:
                    {
                        float newVal = currState.MashTargetTemp + 1;
                        InputEvents::PublishEvent(InputEvents::MashTargetSet, &newVal);
                    }                        
                    break;
                case UI_TARGET_MASH_DOWN:
                    {
                        float newVal = currState.MashTargetTemp - 1;
                        InputEvents::PublishEvent(InputEvents::MashTargetSet, &newVal);
                    }
                    break;
                case UI_TARGET_HLT_UP:
                    {
                        float newVal = currState.HltTargetTemp + 1;
                        InputEvents::PublishEvent(InputEvents::HltTargetSet, &newVal);
                    }
                    break;
                case UI_TARGET_HLT_DOWN:
                    {
                        float newVal = currState.HltTargetTemp - 1;
                        InputEvents::PublishEvent(InputEvents::HltTargetSet, &newVal);
                    }
                    break;
            }
        }
            
        if(event.reportObject.cmd == GENIE_REPORT_EVENT && event.reportObject.object == UI_KNOB)
        {
            switch(event.reportObject.index)
            {
                case UI_PUMP_2:                     
                    Brewery::Pump2Mode mode = (Brewery::Pump2Mode) event.reportObject.data_lsb;
                    InputEvents::PublishEvent(InputEvents::Pump2ModeChange, &mode);                 
                    break;
            }
        } 
    }   
}

void Screen::Tick()
{
    s_genie->DoEvents(); 

    switch(s_activePage)
    {
        case Screen::ActivePage::Main:
            TickBrewery();
            break;
        case Screen::ActivePage::Settings:                
            TickSettings();
            break;
    }                      
}

void Screen::TickBrewery()
{
    // Now get the latest state and update our backbuffered bits
    Brewery::BreweryState currState = Brewery::CurrentState();

    s_boilTemp->SetValue(currState.BoilTemp);
    s_mashTemp->SetValue(currState.MashTemp);
    s_hltTemp->SetValue(currState.HltTemp);
    s_pump1On->SetValue(currState.Pump1On);
    s_pump2On->SetValue(currState.Pump2On);
    s_burnerOn->SetValue(currState.BurnerOn);
    s_mashFloatOn->SetValue(currState.MashFloatOn);
    s_remoteControlled->SetValue(currState.RemoteControlled);
                
    // We only write to the screen at most 2 times a second, to avoid flooding it
    //  in the face of little delta.  We also only write deltas.
    if(s_lastPushTick == 0 || millis() - s_lastPushTick >= 500)
    {                
        s_lastPushTick = millis();

        s_boilTemp->Tick();
        s_mashTemp->Tick();
        s_hltTemp->Tick();
        s_pump1On->Tick();
        s_pump2On->Tick();
        s_burnerOn->Tick();
        s_mashFloatOn->Tick();
        s_remoteControlled->Tick();

        s_status0->Tick();
        s_status1->Tick();
        s_status2->Tick();
        s_status3->Tick();
        s_status4->Tick();
        s_brewer->Tick();
        s_version->Tick();        
        s_ap->Tick();                                                                          
    }   
}

void Screen::TickSettings()
{
    if(s_lastPushTick == 0 || millis() - s_lastPushTick > 500)
    {
        s_lastPushTick = millis();
        s_brewerEdit->Tick();        
    }
}

void Screen::AddStatus(const char *line)
{               
    // Roll up!
    s_status0->SetValue(s_status1->GetValue());
    s_status1->SetValue(s_status2->GetValue());
    s_status2->SetValue(s_status3->GetValue());
    s_status3->SetValue(s_status4->GetValue());
    s_status4->SetValue(line);                                         
}

void Screen::ScreenChange()
{    
    if(s_activePage == Screen::ActivePage::Main)
    {
        s_boilTemp->Refresh();
        s_mashTemp->Refresh();
        s_hltTemp->Refresh();
        s_pump1On->Refresh();
        s_pump2On->Refresh();
        s_burnerOn->Refresh();
        s_mashFloatOn->Refresh();
        s_remoteControlled->Refresh();
        s_status0->Refresh();
        s_status1->Refresh();
        s_status2->Refresh();
        s_status3->Refresh();
        s_status4->Refresh();
        s_brewer->Refresh();
        s_version->Refresh();                
        s_ap->Refresh();
    } 
    else if(s_activePage == Screen::ActivePage::Settings)
    {
        s_brewerEdit->SetValue(s_brewer->GetValue());
        s_brewerEdit->Refresh();                    
    } 
}

void Screen::ScreenEventListener::HandleEvent(InputEvents::Event event, const void *data)
{                                        
    switch(event)
    {
        case InputEvents::DebugTxt:
        case InputEvents::StatusTxt:
            {
                Screen::AddStatus((const char *) data);                                                                
            }            
            break; 
            
        case InputEvents::SetBrewer:     
            Settings::Get().brewer = (const char*)data;
            Settings::Save();               
            Screen::SetBrewer((const char *)data);
            break;

        case InputEvents::NetworkChange:
            s_ap->SetValue((const char *)data);
            break;

        case InputEvents::Pump1:                    
            WriteObject(UI_FLIP, UI_PUMP_1, *((bool *) data));
            break;

        case InputEvents::Pump2ModeChange:
            {            
                uint8_t val = (uint8_t)(*((Brewery::Pump2Mode *) data));
                WriteObject(GENIE_OBJ_ROTARYSW, 0x00, val);
            }
            break;
        case InputEvents::MashTargetSet:
            WriteObject(GENIE_OBJ_LED_DIGITS, 0x04, *((float *)data));            
            break;
        case InputEvents::HltTargetSet:
            WriteObject(GENIE_OBJ_LED_DIGITS, 0x05, *((float *)data));            
            break;                                    
        case InputEvents::ScreenChange:
            s_activePage = *((Screen::ActivePage *)data);
            ScreenChange(); 
            break; 
    }
}
