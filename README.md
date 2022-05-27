# MR-Inspection-Tool

Hello! Welcome to our project, a mixed reality inspection tool for Industry 4.0 environments.
The project is a proof of concept demonstrating a functional connection and information pipeline between Unity, a machine learning server,
an Intel RealSense D455 Depth Camera, and a Microsoft HoloLens 2, as well as visualization and interactivity with the pipeline.

## Requirements

You will ***need*** the following software:

- Unity Hub
- Unity 2020.3.20f1 LTS, configured with the Universal Windows Platform development package.
- Windows 10 OR Windows 11
- The Microsoft Mixed Reality Tool Kit (MRTK) is used heavily in this project, but its installation should be managed entirely by Unity.


You will ***need*** the following hardware:

- A high-end desktop running the aforementioned Windows 10 OR Windows 11, with a capable GPU to run the machine learning server's container.
The project was benchmarked using an RTX 3090.
- An Intel RealSense D455 Depth Camera.


We *recommend* the following software for debugging and modification:

- Docker Desktop
- Any text editor, preferably Visual Studio (this is typically bundled with the Unity UWP development package).
- The full Intel RealSense SDK 2.0 and its Unity Wrapper.

## Installation - Client

1. To open the project, clone the repo:

```git clone https://github.com/Juichilee/MR-Inspection-Tool-Hololens```

2. Use GitHub Desktop or the command line to switch branches:

```git checkout InputManager-B```

3. Your files will be updated. This was our last working directory when we finished our capstone.

You'll need to add this project via the Open > Add Project From Disk in the Projects tab of the Unity Hub.

Add the root directory created by the clone operation.

## Installation - Server

1. ```git clone https://github.com/[REPO FOR MACHINE LEARNING SERVER]```
2. ```docker compose up```
3. The machine learning server will start and await a connection.

## Running the Client and Server

1. Open both the Unity project and the directory of the machine learning server repo in PowerShell.
2. Connect the Intel RealSense camera to the PC. Be sure to use the provided USB-C to USB-A cable.
3. In PowerShell: ```docker compose up```

This will start the server and print status information. It is now awaiting a connection.

4. In the Unity Client, enter Game Mode by pressing the Play button at the top of the screen, and then enter the keys ```Z``` and ```X``` in quick succession.
5. A connection will be initiated. In some cases, the server will refuse to connect. While keeping the client running, quit (Control + C) to relaunch the server and restart the connection process.

## Adding the HoloLens 2

1. 

## Troubleshooting

- Some firewalls and WiFi networks may be incompatible with the Holographic Remoting protocol used by our project. For instance, only the visitor WiFi or Robotics Wifi at our lab at OSU worked for this project. If you experience repeated failed connections or degraded performance, such as accumulating lag, switch networks.


## Unrealized Features
Some features that we would have worked on, had we had the time:
- Make the Intel RealSense image input independent of the hardware, e.g. make a generic webcam handler.
- Improve the calibration process.
- Allow another camera to supply its own coordinates to the system, eliminating the need to calibrate entirely.


## Extending to Other Projects

Some other projects that could build on our work include:
- A full-fledged inspection tool that uses the output of the machine learning server to recognize steps in an inspection process.
- A testbed for a machine learning server to improve the reliability of object recognition.
- A training tool for new workers in an industrial environment that recognizes certain motions via a supplied dataset.
- Integrating the object recognition with multiple cameras.

