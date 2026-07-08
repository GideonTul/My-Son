# Audio System

Portable music/SFX/voice manager for Unity.

## Contents
- `AudioManager.cs` - singleton; crossfading music (intro -> loop clip),
  plus SFX/voice playback and mixer volume control. `PlayMusic(cue, ...)`
  plays that cue's track directly -no queueing/priority logic.
- `MusicLibrary.cs` - maps a cue name (string) -> `MusicTrack`. The class
  is generic; the populated `.asset` instance is project content.
- `MusicTrack.cs` - data asset: an optional intro clip + a loop clip.
- `AudioCueRegistry.cs` - holds the list of valid cue names for a
  project. The script is generic; the populated `.asset` instance is
  project content (lives in `Assets/Audio/Data`, not in this package).
- `AudioCueAttribute.cs` + `Editor/AudioCueDrawer.cs` - `[AudioCue]`
  renders a string field as a dropdown sourced from whatever
  `AudioCueRegistry` asset exists in the project.
- `SceneMusic.cs` - drop on a scene object to set that scene/area's
  music.

## Using this package in a new project
Pick one:
1. **Copy-paste**: drop this whole `com.melodicrum.audiosystem` folder
   into the new project's `Packages/` directory.
2. **Git package**: `Package Manager -> Add package from git URL`.



## Project setup (per-project)
1. Create an `AudioCueRegistry` asset: `Assets > Create > Audio > Cue
   Registry`, place it wherever, and list this
   project's cue names (e.g. `Forest_Ambient`, `Chase`, `Boss`).
2. Create a `MusicTrack` asset: Assets > Create > Audio > Music Track,
   and map AudioClips.
3. Create a `MusicLibrary` asset: `Assets > Create > Audio > Music
   Library`, also under `Assets/Audio/Data/`, and map each cue name to
   a `MusicTrack`.
4. Create an `AudioManager` GameObject in a bootstrap scene, assign the
   `MusicLibrary` asset, the two music `AudioSource`s, SFX source,
   voice source, and your `AudioMixer`.
5. Use `AudioManager.Instance.PlayMusic(cue, fadeTime, volume)` anywhere
   you need to change music.

## AudioManager Setup

Do this once, in whichever scene loads first (a bootstrap/persistent scene
if you have one - `AudioManager` calls `DontDestroyOnLoad`, so it only
needs to exist in the first scene, not every scene).

1. **Create the GameObject.**
   In the Hierarchy: right-click -> `Create Empty`, rename it `AudioManager`.
   Add the `AudioManager` component to it (`Add Component -> Audio Manager`).

2. **Create four child AudioSources.**
   Right-click the `AudioManager` object -> `Create Empty` four times, named:
   `MusicA`, `MusicB`, `SFX`, `Voice`. Add an `AudioSource` component to
   each. Recommended settings on all four:
   - **Play On Awake**: off (the manager calls `Play()` itself)
   - **Spatial Blend**: `0` / 2D (this manager doesn't handle 3D positional
     audio - see note below if you need that)
   - **Loop**: leave off here; `AudioManager` sets `loop` in code as needed

3. **Wire the AudioManager component's fields:**
   - **Mixer** -> your project's `AudioMixer` asset
   - **Music Library** -> the `MusicLibrary.asset` you created in step 2
     above
   - **Music A** / **Music B** -> the two music child `AudioSource`s
   - **SFX Source** -> the `SFX` child `AudioSource`
   - **Voice Source** -> the `Voice` child `AudioSource`

4. **Set up mixer group volume parameters.**
   `AudioManager` calls `mixer.SetFloat("MusicVolume", ...)`,
   `"SFXVolume"`, and `"VoiceVolume"`. In your `AudioMixer`, each of
   Music/SFX/Voice groups needs its **Volume** parameter exposed under
   exactly those names (right-click the group's Volume slider ->
   `Expose 'Volume' to script`, then rename the exposed parameter in the
   Mixer's top-left panel to match). If these names don't match exactly,
   `SetFloat` silently does nothing.

5. **Route each AudioSource's Output** to the matching mixer group
   (`MusicA`/`MusicB` -> your Music group, `SFX` -> SFX group, `Voice` ->
   Voice group) so the exposed volume parameters actually affect them.

6. **Press Play** and check the Console - `AudioManager.Start()` loads
   saved volumes from `PlayerPrefs` (defaulting to `1` the first time),
   so you should hear whatever's assigned once something calls
   `PlayMusic`.

**Note on 3D audio:** this manager's SFX/Voice sources are 2D, singleton,
one-shot-oriented. If you need positional 3D sound (e.g. an object in the
world), that's a separate concern - don't route it through this
`AudioManager`; give that object its own local `AudioSource` instead.