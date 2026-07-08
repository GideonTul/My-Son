using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [SerializeField] private int numOfObjectives;
    private static int objectivesComplete = 0;
    [SerializeField] private GameObject finalObj;
    [SerializeField] private MusicTrack FinalSong;
    [SerializeField] private PlayableDirector FinalCutscene;

    [TextArea] public string text;

    private List<int> vinyls = new List<int>();

    public event EventHandler<bool> MissionFinished;

    private void Awake()
    {
        Instance = this;
        objectivesComplete = 0;

        vinyls = new List<int>();

        if (finalObj != null)
        {
            finalObj.SetActive(false);
        }

    }

    public void Complete()
    {
        

        objectivesComplete++;
        Debug.Log("Objs Complete: " + objectivesComplete);
        if (objectivesComplete >= numOfObjectives) { 
            UIMessageManager.Instance.ShowMessage(text, 10f); 
            if (finalObj != null)
            {
                finalObj.SetActive(true);
            }
        }
    }


    public void MissionComplete()
    {
        if (FinalSong != null)
        {
            AudioManager.Instance.PlayMusic(FinalSong, 2f, 1f);
        }
        if (FinalCutscene != null)
        {
            FinalCutscene.Play();
        }
        MissionFinished?.Invoke(this, true);
    }

    

    public void CollectVinyl(int vinylID)
    {
        vinyls.Add(vinylID);

    }

    public bool isVinylCollected(int id)
    {
        return vinyls.Contains(id);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

}
