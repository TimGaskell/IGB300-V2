using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionPanelHandler : MonoBehaviour
{
    private const int PANELS_CLOSED = -1;

    public List<GameObject> instructionPanels;
    public GameObject parentInstructionPanel;
    public GameObject instructionCounter;
    public int currentPanel;

    public void Awake()
    {
        currentPanel = PANELS_CLOSED;

        foreach (GameObject instructionPanel in instructionPanels)
        {
            instructionPanel.SetActive(false);
        }
    }

    public void OpenNextInstructionPanel()
    {
        if (currentPanel == instructionPanels.Count - 1)
        {
            CloseInstructionPanels();
        }
        else
        {
            if (currentPanel != PANELS_CLOSED)
            {
                //parentInstructionPanel.SetActive(true);
                instructionPanels[currentPanel].SetActive(false);
            }

            else
            {
                
            }

            currentPanel++;
            instructionCounter.GetComponent<TextMeshProUGUI>().text = string.Format("{0} / {1}", currentPanel + 1, instructionPanels.Count);
            instructionPanels[currentPanel].SetActive(true);
        }
    }

    public void OpenPreviousInstructionPanel()
    {
        if(currentPanel == 0)
        {
            CloseInstructionPanels();
        }
        else
        {
            instructionPanels[currentPanel].SetActive(false);
            currentPanel--;
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
