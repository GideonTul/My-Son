using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [SerializeField] private int numOfObjectives;
    private static int objectivesComplete = 0;
    private bool missionComplete = false;

    private List<int> vinyls = new List<int>();

    private void Awake()
    {
        Instance = this;

    }

    public void Complete()
    {
        objectivesComplete++;

        if (objectivesComplete >= numOfObjectives) missionComplete = true;
    }
    public bool MissionComplete()
    {
        // maybe add call to a MissionComplete interface instead


        return missionComplete;
    } 

    public void CollectVinyl(int vinylID)
    {
        vinyls.Add(vinylID);

    }

    public bool isVinylCollected(int id)
    {
        return vinyls.Contains(id);
    }

}
