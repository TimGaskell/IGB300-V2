using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject creditsPanel;

    private void Start()
    {
        ClosePanel();
    }

    public void GameStart()
    {
        SceneManager.LoadScene(GameManager.LobbyScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void OpenPanel()
    {
        creditsPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        creditsPanel.SetActive(false);
    }
}
