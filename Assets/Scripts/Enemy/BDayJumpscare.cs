using System.Collections.Generic;
using UnityEngine;

public class BDayJumpscare : MonoBehaviour
{
    [SerializeField] private AudioClip jumpscare;
    [SerializeField] private List<HeadLook> objectsToLook;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(jumpscare, 0.3f);
            foreach (HeadLook obj in objectsToLook)
            {
                
                obj.LookAt(other.transform);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (HeadLook obj in objectsToLook)
            {
                obj.StopLooking();
            }
        }
    }

}
