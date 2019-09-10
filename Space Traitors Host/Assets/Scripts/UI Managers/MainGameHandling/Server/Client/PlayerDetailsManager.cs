using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetailsManager : MonoBehaviour
{
    public GameObject scrapTracker;
    public GameObject corruptionBar;

    public List<GameObject> specGraphs;

    public void UpdatePlayerDetails()
    {
        scrapTracker.GetComponent<ScrapTrackerController>().UpdateScrapText();
        corruptionBar.GetComponent<CorruptionBarController>().UpdateCorruptionBar();

        foreach (GameObject specGraph in specGraphs)
        {
            specGraph.GetComponent<SpecGraphManager>().UpdateSpecGraph();
        }
    }
}
