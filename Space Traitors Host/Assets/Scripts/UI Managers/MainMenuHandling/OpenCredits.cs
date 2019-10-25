using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCredits : MonoBehaviour
{
    GameObject creditsPanel;
    // Start is called before the first frame update
    void Start()
    {
        creditsPanel.SetActive(false);
    }

    // Update is called once per frame


    public void OpenPanel()
    {
        creditsPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        creditsPanel.SetActive(true);
    }
}
