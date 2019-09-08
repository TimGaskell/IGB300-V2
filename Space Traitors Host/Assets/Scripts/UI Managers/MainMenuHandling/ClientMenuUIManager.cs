using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientMenuUIManager : MonoBehaviour
{

    public void GameStart()
    {
        SceneManager.LoadScene(ClientManager.LOBBY_SCENE);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
