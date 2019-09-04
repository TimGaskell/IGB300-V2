using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIPowerBar : MonoBehaviour
{
    public List<GameObject> barObjects;
    private List<Image> barImages;

    public GameObject powerText;

    private List<int> multiplier;

    float baseX;
    float baseY;
    
    void Start()
    {
        barImages = new List<Image>();

        baseX = barObjects[0].GetComponent<RectTransform>().rect.width;
        baseY = barObjects[0].GetComponent<RectTransform>().anchoredPosition.y;

        multiplier = new List<int> { 1, -1 };

        foreach (GameObject barObject in barObjects)
        {
            barImages.Add(barObject.GetComponent<Image>());
        }
    }

    public void UpdateAIPower()
    {
        float aiPower = 0.5f;
        //float aiPower = GameManager.instance.AIPower / 100;

        powerText.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", (aiPower * 100).ToString());

        //Always gets the amount of AI Power in the Game Manager
        //Maximum AI Power is 100, fill amount is between 0 and 1. 
        for (int i = 0; i < barObjects.Count; i++)
        {
            barImages[i].fillAmount = aiPower;
            barObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(multiplier[i] * baseX * (1 - aiPower), baseY);
        }
    }
}
