# Unity Audio System

A small, dependency-free audio framework: pooled SFX, data-driven sound assets,
crossfading music, mixer-based volume control, and PlayerPrefs-backed settings.
Drop the `Scripts` folder into your project (e.g. `Assets/Scripts/Audio/`).

## Files & responsibilities

| File | Responsibility |
|---|---|
| `SoundData.cs` | ScriptableObject describing **one** sound: clip variations, volume/pitch range, mixer category, loop, spatial settings, cooldown. This is the "data" layer — designers create/tweak these as assets, no code required. |
| `PooledAudioSource.cs` | Wraps a single `AudioSource`. Configures itself from a `SoundData`, optionally follows a moving transform, fades out, and auto-releases itself back to the pool when a one-shot finishes. |
| `AudioSourcePool.cs` | Plain C# object pool of `PooledAudioSource`s. Prewarms a batch, grows on demand up to a cap, then recycles the oldest playing voice instead of growing forever. |
| `AudioManager.cs` | Singleton entry point for SFX/UI/Ambience/Voice. Routes each `SoundData` to the right `AudioMixerGroup`, enforces cooldowns, hands back a `PooledAudioSource` handle so callers can stop/fade a specific instance (e.g. a looping engine sound). |
| `MusicManager.cs` | Separate singleton for music. Exactly two long-lived `AudioSource`s that crossfade between tracks, plus a `Duck()` helper for lowering music under dialogue or big moments. |
| `AudioSettingsController.cs` | Converts linear 0–1 UI slider values to decibels on the `AudioMixer`'s exposed parameters, and persists them with `PlayerPrefs`. |

### Why split Music from SFX?

A one-shot gunshot and a looping music track have almost nothing in common
operationally — one is pooled and short-lived, the other is long-lived and
needs crossfading. Forcing both through the same pooled-`AudioSource` code path
tends to produce awkward special cases. Two small, focused managers are easier
to reason about and extend than one manager trying to do everything.

### Why ScriptableObjects for sounds?

Sounds become **assets**, not code. A designer can create a new `SoundData`,
drag in clips, tune volume/pitch variance and cooldown, and drop it onto a
prefab — all without a programmer touching `AudioManager`. It also means
gameplay code depends on a lightweight data reference, not a raw `AudioClip`
plus a pile of inline playback parameters.

### Why pooling?

Instantiating/destroying GameObjects with `AudioSource`s at runtime causes GC
churn and hitches, especially for frequent sounds like footsteps or gunfire.
The pool prewarms voices up front and reuses them for the life of the game.

## Setup (one-time, in the Unity Editor)

1. **Create the mixer**: `Assets > Create > Audio Mixer`, name it e.g. `MainMixer`.
2. In the Audio Mixer window, add child groups under Master: `Music`, `SFX`, `UI`, `Ambience`, `Voice`.
3. **Expose volume parameters**: for each group, right-click its `Volume` slider → *Expose to script*. Then in the exposed-parameters list (top-left of the Mixer window), rename each to match what `AudioSettingsController` expects: `MasterVolume`, `MusicVolume`, `SFXVolume`, `UIVolume`, `AmbienceVolume`, `VoiceVolume`.
4. **Create the managers**: in your bootstrap/first scene, create an empty GameObject `AudioManager`, add the `AudioManager` component, and assign the mixer + the four non-music `AudioMixerGroup`s. Create another empty GameObject `MusicManager`, add `MusicManager`, assign the `Music` group. Both mark themselves `DontDestroyOnLoad`, so one instance persists across scene loads.
5. **Wire up settings UI** (optional): add `AudioSettingsController` to your settings menu object, assign the mixer, and hook slider `OnValueChanged` events to `SetMasterVolume`, `SetMusicVolume`, etc.
6. **Create sounds**: `Assets > Create > Audio > Sound Data`. Assign one or more clips (multiple = randomized variation), pick a category, tune volume/pitch/cooldown.

## Usage examples

```csharp
// A UI button click (2D, no position needed)
AudioManager.Instance.Play(uiClickSound);

// A one-off positional sound
AudioManager.Instance.PlayAt(explosionSound, hitPoint);

// A looping sound attached to a moving object — keep the handle to stop it later
private PooledAudioSource _engineLoop;
_engineLoop = AudioManager.Instance.PlayAttached(engineLoopSound, transform);
// ...later, e.g. when the engine turns off:
AudioManager.Instance.FadeOutAndStop(_engineLoop, 0.5f);

// Music
MusicManager.Instance.Play(explorationTheme);          // default crossfade
MusicManager.Instance.Play(bossTheme, 2.5f);            // custom crossfade duration
MusicManager.Instance.Duck(0.3f, 0.2f, 1.5f);            // duck to 30% for 1.5s around a cutscene line
```

## Extending it

- **New category**: add a value to the `SoundCategory` enum, a matching `AudioMixerGroup` field on `AudioManager`, and a `case` in `GetGroup()`.
- **Decoupling gameplay code further**: if you want systems to raise sound events without holding a reference to `AudioManager` at all, add a `SoundEventChannel` `ScriptableObject` (a `UnityEvent<SoundData>` asset) that gameplay code raises and `AudioManager` subscribes to — a common pattern for large teams where designers wire up events in the Inspector.
- **Streaming large music files**: mark long music `AudioClip` import settings as *Streaming* (Load Type) so they don't sit fully in memory.
- **Mixer snapshots** (e.g. "Underwater", "Paused"): create `AudioMixerSnapshot`s in the mixer and call `snapshot.TransitionTo(duration)` from wherever that state change happens — no changes to this system needed, since it just plays through the mixer groups you already routed.
