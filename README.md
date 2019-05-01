# agi18
AGI18 Harpmonics  
A project for the course Advanced Graphic and Interaction given at KTH.

## Midi Setup
Suggested setup required for the instrument stage:

1. Download loopMidi, install, start & hit + button to add a port and leave running  
[website](https://www.tobias-erichsen.de/software/loopmidi.html)  
[download](http://www.tobias-erichsen.de/wp-content/uploads/2015/08/loopMIDISetup_1_0_13_24.zip)

2. Download VSTHost and unzip at desired location  
[website](http://www.hermannseib.com/english/vsthost.htm)  
[download](http://www.hermannseib.com/programs/vsthostx64.zip)

3. Download Synth1 VST plugin and unzip at the same location as where VSTHost was unzipped  
See 3rdPartySoftware for Synth1 zip

4. Launch VSTHost, hit menu -> Devices -> Midi -> Input and make sure `loopMIDI port` is selected, hit Devices -> Wave and set `Input Port = No Wave`, `OutputPort = DS:<Your Prefered Device>`, `Buffer = 500 + samples`

5. Hit Ctrl-N in VSTHost, load `Synth1 VST64.dll` from where you unzipped Synth1

6. Click the gray button (MIDI input filters) in the middle of the three buttons to the left in the little window that popped up after loading the Synth1 plugin, make sure `loopMIDI Port` is selected

7. Check the `Scene MIDI Output` gameobject in the Unity inspector, make sure you see `loopMIDI Port` in the list called `Available Midi Devices`, set the field `Midi Output Device Id` to the Element number of the entry for `loopMIDI Port`

8. Click the 2nd (green with a small dial) of the four tiny buttons in the top right of the Synth1 window, opening up the Synth1 interface. Click the button `OPT` near the bottom, select the MIDI tab, click the Load button in the `Control Change Map` section, navigate to where you unzipped Synth1 and load `Synth1/settings/nordlead2.ccm`

Using MME as the Wave Output in VSTHost works for some people, a lower latency can be achieved with it.
If there are no sounds presets available in Synth1 you need to set the path to at least one soundbank (see images below), there is at least one soundbank (soundbank00) included in the Synth1 zip file and you'll find it in the directory where Synth1 was unzipped.

![](Images/tinywindow.png)  
*The tiny plugin window in VSTHost*

![](Images/synth1gui.png)  
*Synth1 GUI help*

![](Images/synth1opts1.png)  
*Synth1 options 1 of 2*

![](Images/synth1opts2.png)  
*Synth1 options 2 of 2*
