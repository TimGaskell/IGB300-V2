using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    private List<GameObject> healthPoints;

    public GameObject pointBackground;
    public GameObject initialPoint;

    private const int backgroundIncrement = 60;

    private void Start()
    {
        healthPoints = new List<GameObject>();

        healthPoints.Add(initialPoint);
    }

    public void UpdateHealthPoints()
    {
        //Add new health points if the UI has less than the max number of hit points represented
        while (healthPoints.Count < ClientManager.instance.maxLifePoints)
        {
            healthPoints.Add(Instantiate(initialPoint, pointBackground.transform));
        } 

        for (int healthIndex = 0; healthIndex < ClientManager.instance.maxLifePoints; healthIndex++)
        {
            if(healthIndex < ClientManager.instance.lifePoints)
            {
                healthPoints[healthIndex].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
