# Motion Capture Glove Tracking

A Unity-based system for real-time gesture recognition and interaction with immersive spatial audio environments, developed for the SADIE Cube Festival Performance at Virginia Tech. The system allows performers to manipulate 3D sound fields using natural hand movements captured in full 6DoF space.

## Overview

This project contains the gesture recognition and tracking logic for a custom motion-capture glove that enables expressive 3D hand interaction in immersive spatial audio environments, without requiring a traditional screen.

The glove uses reflective markers tracked by an external motion capture system. Real-time 3D coordinates of hand/finger points are processed to detect intuitive gestures such as pinch, zoom, grab, point, and other expressive motions for controlling sound spatialization, effects, or immersive exocentric aural scenes.


## Project Context

The **SADIE** (Spatial Audio Data Immersive Experience) project explores immersive data sonification: transforming complex, multidimensional datasets into spatialized sound fields. By assigning distinct auditory cues to various data variables and rendering them simultaneously via a high-density loudspeaker array, users can immerse themselves in the data and leverage human auditory perception to discover correlations, patterns, and relationships that may be difficult to detect visually.

This implementation was created for **Cube Fest 2019**, an international spatial music festival held at Virginia Tech's Moss Arts Center.

### The Cube

The Cube is a four-story (50' × 40' × 32') immersive environment at Virginia Tech featuring:
- 128+ channel spatial audio system with over 140 loudspeakers
- Motion tracking capabilities
- Real-time audio/visual rendering systems
  
This venue serves as both a performance space and a research laboratory for immersive audio, virtual reality, and interactive media.

## Features
- Real-time Gesture Recognition: Processes 3D coordinate data from motion capture marker positions to identify hand gestures
- Spatial Audio Interaction: Maps hand movements to control parameters in immersive sound environments; Designed for low-latency interaction in live performance settings
- Unity Integration: Built using Unity for seamless integration with real-time rendering systems (via included BespokeUnityLibrary.unitypackage)
- Multiple Gesture Support: Recognizes various gestures, including:
  - Pinch detection
  - Distance-based zoom/scale manipulations
  - Directional pointing
  -  Other 3D spatial interactions
 
## Technical Stack
- Unity: Primary development platform
- C#: Main programming language (93.9% of codebase)
- BespokeUnityLibrary: Custom Unity package for specialized functionality
- Motion Capture Integration: Compatible with optical motion capture systems

## Repository Structure
motion-capture-glove-tracking/

  ├── SADIE - CubeFest - 02Aug2019/     # Main project performance-specific files, patches, configuration, or assets used during the CubeFest presentation.
  
  ├── BespokeUnityLibrary.unitypackage  # Custom Unity library package containing scripts/components for receiving and interpreting motion capture data inside Unity.
  
  └── README.md                         # This file

## Requirements

- Motion capture system capable of tracking reflective markers (e.g., Qualisys, OptiTrack, Vicon, or similar)
- Unity 2018+ (recommended version from 2019 era for compatibility)
- Hardware: Custom glove with strategically placed reflective markers on fingers/thumb/back of the hand

<p align="center">
 <img width="45%" height="auto" alt="glove image" src="https://github.com/user-attachments/assets/d33e4053-e273-4243-9e40-01e541d43495" />
</p>

## Installation
1. Clone the Repository
   - git clone https://github.com/disha13sardana/motion-capture-glove-tracking.git
   - cd motion-capture-glove-tracking

2. Unity Setup
     - Open Unity (recommended version: 2018+)
     - Import the 'BespokeUnityLibrary.unitypackage' into your Unity project
     -  Import the project folder: 'SADIE - CubeFest - 02Aug2019'
         - The glove tracking script will be added to your scene
         - The recognized gestures will be mapped to sound parameters (spatialization, reverb, granular synthesis, etc.)

3. Motion Capture System
   - Ensure your motion capture hardware is properly configured
   - Set up tracking markers on glove elements according to your system's requirements. Once set up, configure marker IDs/ rigid body names corresponding to your glove markers and calibrate
  <p align="center">
   <img width="40%" height="auto" alt="tracking-markers image" src="https://github.com/user-attachments/assets/261ccc71-5c41-40f9-a9e5-6de9d213bd40" />
  </p>

  _Streaming to Unity_
    - - Install a motion capture server/streaming software that outputs marker positions (e.g., via OSC, NatNet, VRPN, or custom UDP stream)
      - Configure coordinate streaming to Unity
      - Run the motion capture system → Unity should receive live hand data gestures
      
  <p align="center">
   <img width="50%" height="auto" alt="tracking-in-unity image" src="https://github.com/user-attachments/assets/7b635197-a6ae-4bfd-9168-75aaefa17bcc" />
  </p>


Example gesture logic (pseudocode):

```csharp
if (PinchDistance(thumbTip, indexTip) < 0.03f) {
    OnPinchDetected();
}

if (IsPointingDirectionValid(indexTip, indexProximal)) {
    UpdateSoundDirection(indexTip.position);
}
```

## Research & Publications
For more information on the research context, see:
- Sardana, D., Joo, W., Bukvic, I. I., & Earle, G. (2020). Perception of Spatial Data Properties in an Immersive Multi-layered Auditory Environment. In Proceedings of the 15th International Audio Mostly Conference (**AM 2020**) (pp. 30–37). Association for Computing Machinery, New York, NY, USA.
- Bukvic, I. I., Sardana, D., & Joo, W. (2020). New Interfaces for Spatial Musical Expression. In Proceedings of the International Conference on New Interfaces for Musical Expression (**NIME 2020**) (pp. 249-254).
- Bukvic, I. I., Earle, G., Sardana, D., & Joo, W. (2019). Studies in Spatial Aural Perception: Establishing Foundations for Immersive Sonification. In The 25th International Conference on Auditory Display (**ICAD 2019**).
- Sardana, D., Joo, W., Bukvic, I. I., & Earle, G. (2019). Introducing Locus: A NIME for Immersive Exocentric Aural Environments. In Proceedings of the International Conference on New Interfaces for Musical Expression (**NIME 2019**).

## Demo/ Media Links
- CubeFest 2019 performance video: https://www.youtube.com/watch?v=jPBzkMkQlq4
- NSF Award #: https://www.nsf.gov/awardsearch/show-award/?AWD_ID=1748667
- NPR Article: https://www.wvtf.org/news/2018-01-18/music-of-the-spheres-big-data-meets-big-sound#stream/0
- Project Website: https://disha-sardana.squarespace.com/work/sadie

## Contributing
Contributions are welcome! Areas for potential contribution:
- Additional gesture recognition patterns
- Machine learning-based gesture classification
- Support for multiple users/gloves simultaneously
- Integration with other spatial audio platforms
- Documentation improvements
- Example scenes and tutorials

## Acknowledgments
- Virginia Tech Institute for Creativity, Arts, and Technology (ICAT)
- The Cube at Moss Arts Center
- SADIE Project team at Virginia Tech
- Cube Fest 2019 organizers and participants

## License
(coming up...)

## Contact
For questions or collaboration opportunities:
- Repository: github.com/disha13sardana/motion-capture-glove-tracking
- Issues: Use GitHub Issues for bug reports and feature requests
- Email: disha13sardana@gmail.com


Note: This project represents research and development work in immersive spatial audio interaction, originally developed in 2019 as part of NSF-funded SADIE research. System requirements and performance may vary based on your specific motion capture hardware and Unity configuration.
