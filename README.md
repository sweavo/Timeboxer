Timeboxer
=========

https://github.com/sweavo/Timeboxer

Only-just-good-enough kitchen timer app. This seeks to get as close as possible to the simplicity of a mechanical kitchen timer.

It has no pause, no options, no installer and three features: it counts down to zero, goes ping and stops. 

It has two bonus features: it can be moved and it can be quit.


Installation
------------

Put `Timeboxer.exe` somewhere on your machine.

You're done.


Operation
---------

Run `Timeboxer.exe`

To move the timer, drag it using the left mouse.

To set the time, use the right mousebutton to drag the sweep anywhere between 0 and 60m.  Release and the timer begins.

To quit, right-double-click the face.

To run multiple timers, run multiple instances of `Timeboxer.exe`


Highlander update!
------------------

If you provide an integral argument to Timeboxer, then it kills all other instances of Timeboxer (There shall be only one!)

Then it sets itself to the number of minutes provided on the command line.  If you provide zero, then you end up with no Timeboxers.

Suggestion: use shortcuts or your stream deck to call:

* `Timeboxer.exe 25` - call it "Pomodoro"
* `Timeboxer.exe 5`  - call it "Short Break"
* `Timeboxer.exe 0`  - call it "No Pomodoro"
