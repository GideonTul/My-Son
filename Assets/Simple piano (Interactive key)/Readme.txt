🎹 Simple Interactive Piano
- A lightweight interactive piano system for Unity.
Includes click input, physics-based interaction (VR / collider),
and full 88-key audio support.

✨ Features
- 88-key piano (white & black keys)
- Mouse click input support
- Physics-based interaction (VR / collider)
- Toggleable input system via PianoController
- Smooth key press animation
- Configurable fade-out audio
- Centralized control via PianoController

📦 Included
- Piano_Interactive (Fully functional piano)
- Piano_Static (Visual only)
- Piano_Static_Collider (Physical blocking version)
- Piano_Controller
- TestHand (Demo input object)
- Demo_Scene

🚀 How to Use
1.Drag Piano_Interactive into your scene
2.Press Play and interact:
  -Mouse click → play notes
  -Collider with tag Finger → trigger notes

⚙️ Input Settings
You can control input behavior from Piano_Controller:
- Enable Click Input → mouse interaction
- Enable Physics Input → collider-based interaction

🎧 Audio
-Place audio clips inside:
Resources/Piano/
-Supported naming format:
A0, A#0, B0, C1, C#1 ... C8
*Default volume is set lower for comfort. Adjust via PianoController if needed.

🧪 Demo Notes
- TestHand is a simple demo object for testing trigger-based interaction
- It is not a full physics-based hand controller
- It uses a kinematic Rigidbody and may pass through objects depending on setup
- For physics interaction, objects should have:
  -A Collider
  -A Rigidbody (recommended for consistent trigger detection)
  -Tag set to Finger
👉 For production use, replace it with your own controller or hand system

🧱 Static Versions
- Piano_Static → visual only (no interaction, no collider)
- Piano_Static_Collider → includes colliders for physical blocking

⚠️ Important
- Interactive keys use Trigger Colliders
- Objects will pass through keys by design
- For physical blocking, use Piano_Static_Collider

🎮 Input System Compatibility
- Compatible with both Legacy Input System and New Input System
- Demo input (TestHand) works without requiring changes to Player Settings

📄 License
Audio samples are used under MIT License.
See included license file for details.