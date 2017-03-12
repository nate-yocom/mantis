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
 
#include <malloc.h>
#include <stdlib.h>
#include <stdio.h>
#include "Memory.h"
#include "debug.h"

extern char _end;
extern "C" char *sbrk(int i);

#define RAM_START 0x20070000
#define RAM_END 0x20088000

int Memory::GetUsedDynamic()
{
    struct mallinfo mi = mallinfo();
    return mi.uordblks;		
}

int Memory::GetUsedProgramStatic()
{
    return (&_end - (char *)RAM_START);
}

int Memory::GetUsedStack()
{
    register char * stack_ptr asm ("sp");
    return ((char *)RAM_END - stack_ptr);
}

int Memory::GetFree()
{
    char *heapend=sbrk(0);
	register char * stack_ptr asm ("sp");
	struct mallinfo mi = mallinfo();
    return stack_ptr - heapend + mi.fordblks;
}

void Memory::Dump()
{
    char *heapend=sbrk(0);
	register char * stack_ptr asm ("sp");
	struct mallinfo mi = mallinfo();
    DEBUG(F("****************** MEMORY INFO ********************"));
	DEBUG(F("  Dynamic:         %d"),mi.uordblks);
	DEBUG(F("  Static:          %d"),&_end - (char *)RAM_START); 
	DEBUG(F("  Stack:           %d"), (char *)RAM_END - stack_ptr);
    DEBUG(F("  Gap:             %d"), stack_ptr - heapend);
    DEBUG(F("  Arena:           %d"), mi.arena); 
    DEBUG(F("  Free Chunks:     %d"), mi.ordblks);
    DEBUG(F("  Free Fastbin:    %d"), mi.smblks);
    DEBUG(F("  Max allocated:   %d"), mi.usmblks);
    DEBUG(F("  Total allocated: %d"), mi.uordblks);
    DEBUG(F("  Free:            %d"), mi.fordblks);
    DEBUG(F(" "));
	DEBUG(F("Total Free:      %d"), stack_ptr - heapend + mi.fordblks);
    DEBUG(F("***************************************************"));
}