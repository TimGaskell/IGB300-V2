using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject serverActiveToggle;

    private void Start()
    {
        serverActiveToggle.GetComponent<Toggle>().isOn = false;
    }

    public void UpdateServerStatus()
    {
        GameManager.instance.serverActive = serverActiveToggle.GetComponent<Toggle>().isOn;
    }

    public void GameStart()
    {
        if (GameManager.instance.serverActive)
        {
            SceneManager.LoadScene(GameManager.LobbyScene);
        }
        else
        {
            SceneManager.LoadScene(GameManager.NoServerLobbyScene);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
