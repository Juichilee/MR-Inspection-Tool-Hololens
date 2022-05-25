# MR-Inspection-Tool

Hello! Welcome to our project, a mixed reality inspection tool for Industry 4.0 environments.
The project is a proof of concept demonstrating a functional connection and information pipeline between Unity, a machine learning server,
an Intel RealSense D455 Depth Camera, and a Microsoft HoloLens 2, as well as visualization and interactivity with the pipeline.


You will ***need*** the following software:

- Unity Hub (manage your install from here)
- Unity 2020.3.20f1 LTS, configured with the Universal Windows Platform development package.
- Windows 10 OR Windows 11
- The Microsoft Mixed Reality Tool Kit (MRTK) is used heavily in this project, but its installation should be managed entirely by Unity.


You will ***need*** the following hardware:

- A high-end desktop running the aforementioned Windows 10 OR Windows 11, with a capable GPU to run the machine learning server's container.
- (The project was benchmarked using an RTX 3090.)
- An Intel RealSense D455 Depth Camera.


We *recommend* the following software for debugging and modification:

- Docker Desktop
- Any text editor, preferably Visual Studio (this is typically bundled with the Unity UWP development package).
- The full Intel RealSense SDK 2.0 and its Unity Wrapper.


To open the project, clone the repo:

```git clone https://github.com/Juichilee/MR-Inspection-Tool-Hololens```

Either use GitHub Desktop or the command line to switch branches:

```git checkout InputManager-B```

Your files will be updated. This was our last working directory when we finished our capstone.

You'll need to add this project via the Open > Add Project From Disk in the Projects tab of the Unity Hub.

Add using the root directory created by the clone operation. 


To load the project in Unity:

1. Download the files contained in this repository. Keep them in their parent folder.

2. Navigate to the parent folder (MR-Inspection-Tool) from Unity Hub > Open > Open Project from Disk.

