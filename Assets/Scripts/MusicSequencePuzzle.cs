using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicSequencePuzzle : MonoBehaviour
{
    public enum PuzzleState { Idle, Playing, WaitingForInput, Solved, Failed }
    public enum SequenceMode { Random, Fixed }

    [Header("Setup")]
    [Tooltip("Drag in any component that implements IPuzzleButton.")]
    [SerializeField] private List<MonoBehaviour> buttonComponents;

    private List<IPuzzleButton> buttons;

    [Header("Sequence Source")]
    [SerializeField] private SequenceMode sequenceMode = SequenceMode.Fixed;

    [Tooltip("Used when Sequence Mode is Random.")]
    [SerializeField] private int sequenceLength = 5;

    [Tooltip("Used when Sequence Mode is Fixed.")]
    [SerializeField] private List<NoteName> fixedSequence;

    [Header("Timing")]
    [SerializeField] private float noteInterval = 0.6f;
    [SerializeField] private float delayBeforePlayback = 1f;
    [SerializeField] private float delayBeforeRetry = 1.5f;

    [Header("Events")]
    [SerializeField] private UnityEvent onPuzzleSolved;
    [SerializeField] private UnityEvent onPuzzleFailed;
    [SerializeField] private UnityEvent onSequenceReplaying;

    // Stores BUTTON INDICES, not ButtonIds.
    private readonly List<int> sequence = new List<int>();

    private int playerIndex = 0;
    private bool acceptingInput = false;

    public PuzzleState State { get; private set; } = PuzzleState.Idle;

    private void Awake()
    {
        Debug.Log($"MusicSequencePuzzle Awake on {name}");
        buttons = new List<IPuzzleButton>();

        foreach (var comp in buttonComponents)
        {
            if (comp is IPuzzleButton pb)
            {
                buttons.Add(pb);
            }
            else
            {
                Debug.LogError($"'{comp.name}' does not implement IPuzzleButton.");
            }
        }
    }

    private void OnEnable()
    {
        foreach (var b in buttons)
        {
            b.OnButtonPressed -= HandleButtonPressed;
            b.OnButtonPressed += HandleButtonPressed;
        }
    }

    private void OnDisable()
    {
        foreach (var b in buttons)
            b.OnButtonPressed -= HandleButtonPressed;
    }

    public void StartPuzzle()
    {
        GenerateSequence();

        StopAllCoroutines();
        StartCoroutine(PlaySequenceRoutine());
    }

    private void GenerateSequence()
    {
        sequence.Clear();

        if (sequenceMode == SequenceMode.Fixed)
            BuildFixedSequence();
        else
            BuildRandomSequence();

        playerIndex = 0;
    }

    private void BuildRandomSequence()
    {
        for (int i = 0; i < sequenceLength; i++)
            sequence.Add(UnityEngine.Random.Range(0, buttons.Count));
    }

    private void BuildFixedSequence()
    {
        // Map each note to its BUTTON INDEX.
        Dictionary<NoteName, int> noteToButtonIndex = new Dictionary<NoteName, int>();

        for (int i = 0; i < buttons.Count; i++)
        {
            if (!noteToButtonIndex.ContainsKey(buttons[i].Note))
            {
                noteToButtonIndex.Add(buttons[i].Note, i);
            }
            else
            {
                Debug.LogWarning($"Multiple buttons use note {buttons[i].Note}. The first one will be used.");
            }
        }

        foreach (NoteName note in fixedSequence)
        {
            if (noteToButtonIndex.TryGetValue(note, out int buttonIndex))
            {
                sequence.Add(buttonIndex);
            }
            else
            {
                Debug.LogError($"No button found for note {note}.");
            }
        }
    }

    private IEnumerator PlaySequenceRoutine()
    {
        State = PuzzleState.Playing;
        SetInputEnabled(false);

        yield return new WaitForSeconds(delayBeforePlayback);

        foreach (int buttonIndex in sequence)
        {
            buttons[buttonIndex].Activate();
            yield return new WaitForSeconds(noteInterval);
        }

        State = PuzzleState.WaitingForInput;
        SetInputEnabled(true);
    }

    private void SetInputEnabled(bool enabled)
    {
        acceptingInput = enabled;

        foreach (var b in buttons)
            b.SetInputEnabled(enabled);
    }
    private int handleCalls = 0;
    private void HandleButtonPressed(int pressedButtonId)
    {
        Debug.Log($"HandleButtonPressed called.\n{Environment.StackTrace}");
        handleCalls++;
        Debug.Log($"HandleButtonPressed Call #{handleCalls}");

        Debug.Log($"Pressed {pressedButtonId}");
        if (!acceptingInput)
        {
            Debug.Log("Puzzle not accepting input.");
            return;
        }
        int expectedButtonId = buttons[sequence[playerIndex]].ButtonId;
        Debug.Log($"Expected {expectedButtonId}");

        if (pressedButtonId == expectedButtonId)
        {
            Debug.Log("Correct");
            playerIndex++;

            if (playerIndex >= sequence.Count)
                Solve();
        }
        else
        {
            Debug.Log("Wrong!");
            Fail();
        }
    }

    private void Solve()
    {
        State = PuzzleState.Solved;
        SetInputEnabled(false);
        onPuzzleSolved?.Invoke();
    }

    private void Fail()
    {
        State = PuzzleState.Failed;
        SetInputEnabled(false);
        Debug.Log("Before invoke");
        onPuzzleFailed?.Invoke();
        Debug.Log("After invoke");
        StartCoroutine(RetryAfterDelay());
        
    }

    public void RetryPuzzle()
    {
        StartCoroutine(RetryAfterDelay());
    }

    private IEnumerator RetryAfterDelay()
    {
        Debug.Log("RETRY CALLED");
        yield return new WaitForSeconds(delayBeforeRetry);

        onSequenceReplaying?.Invoke();

        StartPuzzle();
    }
}