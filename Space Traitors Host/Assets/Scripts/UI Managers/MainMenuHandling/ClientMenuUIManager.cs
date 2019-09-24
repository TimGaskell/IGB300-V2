using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientMenuUIManager : MonoBehaviour
{
    private const int PANELS_CLOSED = -1;

    public List<GameObject> instructionPanels;
    public int currentPanel;

    private void Start()
    {
        currentPanel = PANELS_CLOSED;
    }

    public void GameStart()
    {
        SceneManager.LoadScene(ClientManager.LOBBY_SCENE);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenNextInstructionPanel()
    {
        instructionPanels[currentPanel].SetActive(false);
        currentPanel++;
        instructionPanels[currentPanel].SetActive(true);
    }

    public void CloseInstructionPanels()
    {
        instructionPanels[currentPanel].SetActive(false);
        currentPanel = PANELS_CLOSED;
    }
}
