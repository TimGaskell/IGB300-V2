using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ClientMenuUIManager : MonoBehaviour
{
    public GameObject instructionPanels;
    

    private void Start()
    {
        instructionPanels.SetActive(false);
    }

    public void GameStart()
    {
        SceneManager.LoadScene(ClientManager.LOBBY_SCENE);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenInstructionPanel()
    {
        instructionPanels.SetActive(true);
        instructionPanels.GetComponent<InstructionPanelHandler>().OpenNextInstructionPanel();
    }
}
