using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    private List<GameObject> healthPoints;

    public GameObject pointBackground;
    public GameObject initialPoint;

    private const int backgroundIncrement = 60;

    private void Awake()
    {
        healthPoints = new List<GameObject>();

        healthPoints.Add(initialPoint);
    }

    public void UpdateHealthPoints()
    {
        //Add new health points if the UI has less than the max number of hit points represented
        RectTransform backgroundRectTrans = pointBackground.GetComponent<RectTransform>();

        while (healthPoints.Count < ClientManager.instance.maxLifePoints)
        {
            healthPoints.Add(Instantiate(initialPoint, pointBackground.transform));

            float currentWidth = backgroundRectTrans.sizeDelta.x;
            float currentHeight = backgroundRectTrans.sizeDelta.y;

            pointBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(currentWidth + backgroundIncrement, currentHeight);
        }

        //
        for (int healthIndex = 0; healthIndex < ClientManager.instance.maxLifePoints; healthIndex++)
        {
            GameObject pointMarker = healthPoints[healthIndex].transform.GetChild(0).gameObject;
            pointMarker.SetActive(healthIndex < ClientManager.instance.lifePoints);
        }
    }
}
