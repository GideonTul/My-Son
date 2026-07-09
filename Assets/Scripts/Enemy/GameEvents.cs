using System;
using UnityEngine;
using UnityEngine.Playables;

public static class GameEvents
{
    public static Action<bool> OnPlayerSafeChanged;

    public static Action<Vector3, float> OnNoiseMade;

    public static Action<PlayableDirector> OnPlayerKilled;
    public static Action OnPlayerRespawned;

    public static Action OnEnemyStartedChasing;
    public static Action OnEnemyStoppedChasing;
}
