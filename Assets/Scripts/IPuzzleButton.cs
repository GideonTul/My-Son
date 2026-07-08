using System;

// Anything the puzzle controller can treat as a "step" — a SequenceButton,
// a PianoKeyInteractable, or anything else you build later — implements this.
public interface IPuzzleButton
{
    int ButtonId { get; }
    NoteName Note { get; }

    // Fired when the player successfully interacts with this button.
    event Action<int> OnButtonPressed;

    // Called by the controller to lock/unlock this button during playback.
    void SetInputEnabled(bool enabled);

    // Called by the controller to demonstrate this step (sound + visual),
    // without counting as player input.
    void Activate();
}
