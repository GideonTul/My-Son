using UnityEngine;

public class GramophonePuzzle : MonoBehaviour, IInteractable
{
    [TextArea] public string noKeyMsg;
    public int VinylIDNeeded;

    public void Interact()
    {
        Debug.Log("Interact called on gramophone");

        

        if (ObjectiveManager.Instance.isVinylCollected(VinylIDNeeded))
        {
            GetComponent<AudioSource>().enabled = true;
            ObjectiveManager.Instance.Complete();
            GetComponent<Collider>().enabled = false;
        }
        else
        {
            UIMessageManager.Instance.ShowMessage(noKeyMsg);
        }
    }



}
