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

To cancel the timer, double-click the face.  double-click again to quit.

To run multiple timers, run multiple instances of `Timeboxer.exe`

1.1.3 update 2024-06-02
-----------------------

Now saves its location (in the same folder as the .exe) and restores it, so that when you use it with hotkeys and supply command line args, it doesn't appear to wander around.

Highlander+ update 2023-10-18
-----------------------------

Received a couple of notes about useability, and was gifted an icon!

So now it's clearer what is the remaining time and what is the alarm time, and there's an icon.

Highlander update! 2023-10-12
-----------------------------

### If you provide an integral argument to Timeboxer, then it kills all other instances of Timeboxer (There shall be only one!)

Then it sets itself to the number of minutes provided on the command line.  If you provide zero, then you end up with no Timeboxers.

Suggestion: use shortcuts or your stream deck to call:

* `Timeboxer.exe 25` - call it "Pomodoro"
* `Timeboxer.exe 5`  - call it "Short Break"
* `Timeboxer.exe 0`  - call it "No Pomodoro"


### While moving the sweep, also shows the time-of-day of the alarm.

Now, set your countdown timer to the end of the meeting without doing math in your head.

### Double click with left mouse!

Because of the way I implemented dragging, the double-click to clear the time or close the app had to be with the right mouse button.  No more!
