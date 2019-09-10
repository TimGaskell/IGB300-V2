using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrapTrackerController : MonoBehaviour
{
    public GameObject scrapText;

    public void UpdateScrapText()
    {
        scrapText.GetComponent<TextMeshProUGUI>().text = ClientManager.instance.scrap.ToString();
    }
}
