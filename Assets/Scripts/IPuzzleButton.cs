using System;

public interface IPuzzleButton
{
    int ButtonId { get; }
    NoteName Note { get; }


    event Action<int> OnButtonPressed;

    void SetInputEnabled(bool enabled);

    void Activate();
}
