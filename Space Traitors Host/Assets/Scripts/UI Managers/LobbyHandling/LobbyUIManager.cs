using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    public GameObject serverActivePanel;
    public GameObject noServerPanel;

    public GameObject playerNumPanel;
    public GameObject playerNamePanel;

    private Transform nameEntryFields;

    private void Start()
    {
        if (GameManager.instance.serverActive)
        {
            serverActivePanel.SetActive(true);
            noServerPanel.SetActive(false);
        }
        else
        {
            serverActivePanel.SetActive(false);
            noServerPanel.SetActive(true);

            playerNumPanel.GetComponent<CanvasGroup>().interactable = true;
            playerNamePanel.GetComponent<CanvasGroup>().interactable = false;

            nameEntryFields = playerNamePanel.transform.GetChild(1);
            ChangeInputFields(0);
        }
    }

    #region No Server Handling

    public void ConfirmPlayerNumbers(GameObject textField)
    {
        string playerNum = textField.GetComponent<TMP_InputField>().text;

        if (int.TryParse(playerNum, out int convertedNum) && convertedNum <= GameManager.instance.MAX_PLAYERS && convertedNum >= GameManager.instance.MIN_PLAYERS) 
        {
            GameManager.instance.numPlayers = convertedNum;
            ChangeInputFields(convertedNum);
            playerNumPanel.GetComponent<CanvasGroup>().interactable = false;
            playerNamePanel.GetComponent<CanvasGroup>().interactable = true;
        }
        else
        {
            Debug.Log("Not a valid player number");
        }
    }

    private void ChangeInputFields(int playerNum)
    {
        int counter = 0;

        foreach (Transform entryField in nameEntryFields)
        {
            if(counter >= playerNum)
            {
                entryField.gameObject.SetActive(false);
            }
            else
            {
                entryField.gameObject.SetActive(true);
            }
            counter++;
        }
    }

    public void ConfirmPlayerNames(GameObject textFields)
    {
        int counter = 0;
        GameManager.instance.ResetPlayers();

        foreach (Transform entryField in textFields.transform)
        {
            if (entryField.gameObject.activeSelf)
            {
                string tempPlayerName = entryField.GetComponent<TMP_InputField>().text;
                if (tempPlayerName != "")
                {
                    GameManager.instance.GeneratePlayer(counter, tempPlayerName);
                }
                else
                {
                    Debug.Log(string.Format("Invalid Player Name for Player {0}", counter));
                    break;
                }
            }
            else
            {
                break;
            }
            counter++;

            if (counter == GameManager.instance.numPlayers)
            {
                SceneManager.LoadScene("Character SelectionV2");
            }
        }
    }
    
    #endregion
}
