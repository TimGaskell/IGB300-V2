using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyLanUIManager : MonoBehaviour
{
    public GameObject appPanel;
    public GameObject hostPanel;

    private void Start()
    {
        appPanel.SetActive(true);
        hostPanel.SetActive(false);
    }

    public void CloseAppPanel()
    {
        appPanel.SetActive(false);
        hostPanel.SetActive(true);
    }
}
