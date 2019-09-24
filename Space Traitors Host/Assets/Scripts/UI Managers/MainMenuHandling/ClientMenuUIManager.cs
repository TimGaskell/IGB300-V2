using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ClientMenuUIManager : MonoBehaviour
{
    private const int PANELS_CLOSED = -1;

    public List<GameObject> instructionPanels;
    public GameObject parentInstructionPanel;
    public GameObject instructionCounter;
    public int currentPanel;

    private void Start()
    {
        currentPanel = PANELS_CLOSED;
        parentInstructionPanel.SetActive(false);

        foreach (GameObject instructionPanel in instructionPanels)
        {
            instructionPanel.SetActive(false);
        }
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
        if (currentPanel == instructionPanels.Count - 1)
        {
            CloseInstructionPanels();
        }
        else
        {
            if (currentPanel == PANELS_CLOSED)
            {
                parentInstructionPanel.SetActive(true);
            }

            else
            {
                instructionPanels[currentPanel].SetActive(false);
            }

            currentPanel++;
            instructionCounter.GetComponent<TextMeshProUGUI>().text = string.Format("{0} / {1}", currentPanel + 1, instructionPanels.Count);
            instructionPanels[currentPanel].SetActive(true);
        }
    }

    public void CloseInstructionPanels()
    {
        instructionPanels[currentPanel].SetActive(false);
        parentInstructionPanel.SetActive(false);
        currentPanel = PANELS_CLOSED;
    }
}
