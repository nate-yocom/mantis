Mantis is the pet name of a project undertaken by Nate Yocom with his Sculpture.  The goal of 
mantis is to:

    - Create a simple on-board/out-of-the box UI for automation of a brew day to the same 
        level of automation as previous versions of the sculpture (basic temperature control
        and pump control).
        
    - Add wireless capability such that the sculpture can:
        - Operate as it's own AP and/or join an existing wireless network
        - Be monitored or controlled by an Application running on a seperate device (i.e. phone,
          tablet, computer, etc) on the same network
    
    - Create a library for interacting with the brew sculpture
    
    - Create a desktop / mobile application for automation and brew sculpture control, including:
        - Recording a brew day / recipe for later "replay"
        - System calibration and testing
        - Automated brew day from an imported Beer XML recipe
        
To do this, this repository contains a complete ground-up re-written UI and arduino code base, including support for the ESP8266 wireless chip.  
The code will work without the wireless chip as well, but the payoff is really in including this chip as well.  

I used this chip: https://www.sparkfun.com/products/13678 - though as the enclosure is very good 
at blocking signal, I suggest looking for a version of this which has or allows an external antennea.

See docs/HowTo.txt for more information and instructions.
