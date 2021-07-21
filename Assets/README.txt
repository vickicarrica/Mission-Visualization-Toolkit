Disclaimer:
    As you can probably tell, I'm very new to this, so please do make any corrections/changes as you see fit.
    Feel free to change the units, code, or anything else.


Hello, this doc was created with the purpose of (hopefully) helping you better understand this program.

Table of Contents:
    Coordinate System 
    Scaling/Units 
    Suggested Syntax 
    Design Philosophy 
    Helpful Resources


=============================
Coordinate System:
=============================
Y
|   /
|  / Z
| /
|/
+------------X

Y is up due to how Unity sets the axises.
You will notice within several functions that the Z and Y values are flipped. This is to accommodate the transition between normal coordinate systems (XYZ) and unity's (XZY).


=============================
Scaling/Units:
=============================
Unless otherwise stated, these apply to all vars.

Scaling                 AU                  See master.unitMultiplier
Distance                km
Time                    seconds
Angles                  radians
Lat/Lon                 degrees
Mass                    kg


=============================
Suggested Syntax:
=============================
(This is optional, I just think it looks nice)

Functions               Allman              If function is a one liner, put it as a single line
Names                   CamelCase
Major Classes           All Caps            "Major" is arbitary -> you can decide what is major or not


=============================
Design Philosophy
=============================
Math and displaying stuff in game should be COMPLETELY seperate (or as seperate as possible)
This means that math that is not associated with displaying the bodies should not be scaled, touched, or in anyway modified from elements associated with displaying.
Essentially, the idea is to be able to recreate everything at a 1:1 scale from the data within the classes, and then modify the data so that it is presentable
    Example: The ORBIT class is never scaled until it is shown on screen


=============================
Helpful Resources
=============================
I'm not sure how comfortable you are with the math/physics that are used here, but for me at least this was the first time I went over any of this.
These were just the links I used to learn, so they be far too simple for you.

Kepler
    Orbital Elements
        https://www.youtube.com/watch?v=AReKBoiph6g
    Kepler's Equation
        http://www.csun.edu/~hcmth017/master/node16.html
    Orbital Elements to Cart 
        https://drive.google.com/file/d/1so93guuhCO94PEU8vFvDLv_-k9vJBcFs/view
        http://www.stargazing.net/kepler/ellipse.html#twig04
Doxygen
    Setup/usage
        https://www.youtube.com/watch?v=TtRn3HsOm1s [0.00 - 6.06]