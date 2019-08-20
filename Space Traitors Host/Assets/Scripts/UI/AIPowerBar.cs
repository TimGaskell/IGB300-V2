using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIPowerBar : MonoBehaviour
{
    private GameObject[] barObjects = new GameObject[2];
    private Image bar1, bar2;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            barObjects[i] = transform.GetChild(i).gameObject;
        }
        bar1 = barObjects[0].GetComponent<Image>();
        bar2 = barObjects[1].GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //Always gets the amount of AI Power in the Game Manager
        //Maximum AI Power is 100, fill amount is between 0 and 1. 
        bar1.fillAmount = GameManager.instance.AIPower / 100;
        bar2.fillAmount = GameManager.instance.AIPower / 100;
    }
}
