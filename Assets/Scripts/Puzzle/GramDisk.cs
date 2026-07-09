using UnityEngine;

public class GramDisk : MonoBehaviour, IInteractable
{
    [TextArea] public string msg;
    public int VinylID;

    public void Interact()
    {
        UIMessageManager.Instance.ShowMessage(msg, 10f);

        ObjectiveManager.Instance.CollectVinyl(VinylID);
        Destroy(gameObject);
    }
}
