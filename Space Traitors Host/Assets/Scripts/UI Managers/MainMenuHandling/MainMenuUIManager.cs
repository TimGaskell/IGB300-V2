﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{

    public void GameStart()
    {
        SceneManager.LoadScene(GameManager.LobbyScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
