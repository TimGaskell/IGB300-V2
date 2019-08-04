using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LobbyUIManager : NetworkBehaviour
{
    public GameObject ClientPanel;
    public GameObject ServerPanel;

    public GameObject playerNumPanel;
    public GameObject playerNamePanel;

    private Transform nameEntryFields;

    private void Start()
    {

            playerNumPanel.GetComponent<CanvasGroup>().interactable = true;
            playerNamePanel.GetComponent<CanvasGroup>().interactable = false;

            nameEntryFields = playerNamePanel.transform.GetChild(1);
            ChangeInputFields(0);
        
    }

    void Update() {

        if (NetworkServer.connections.Count > 0) {

            ServerPanel.SetActive(true);
            ClientPanel.SetActive(false);

        }
        else {
            ClientPanel.SetActive(true);
            ServerPanel.SetActive(false);
        }
    }




    #region Server Handling

    /// <summary>
    /// 
    /// Setups the game for a particular number of players based on its entry into an input field
    /// 
    /// </summary>
    /// <param name="textField">The input field which is to be accessed</param>
    public void ConfirmPlayerNumbers(GameObject textField)
    {
        string playerNum = textField.GetComponent<TMP_InputField>().text;

        //Tests if the given number is not valid, either because it is not a number or because it lies outside the valid range
        if (int.TryParse(playerNum, out int convertedNum) && convertedNum <= GameManager.instance.MAX_PLAYERS && convertedNum >= GameManager.instance.MIN_PLAYERS)
        {
            //If the number is valid, assigns the value in the game manager and sets up interface for player name input
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

    /// <summary>
    /// 
    /// Disables player entry fields if they are not to be used (i.e. leave only enough fields active for a particular number of players)
    /// 
    /// </summary>
    /// <param name="playerNum">The numbner of player input fields to leave active</param>
    private void ChangeInputFields(int playerNum)
    {
        int counter = 0;

        foreach (Transform entryField in nameEntryFields)
        {
            if (counter >= playerNum)
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

    /// <summary>
    /// 
    /// Assigns the given player names to new players based on the given inputs
    /// 
    /// </summary>
    /// <param name="textFields">The parent object of the input fields</param>
    public void ConfirmPlayerNames(GameObject textFields)
    {
        //counter for keeping track of the playerIDs
        int counter = 0;
        //Reset the players list for a new game
        GameManager.instance.ResetPlayers();

        //Obtains each child object of the textFields object
        foreach (Transform entryField in textFields.transform)
        {
            string tempPlayerName = entryField.GetComponent<TMP_InputField>().text;
            //If any of the input fields are empty, stops the process and presents an error. Otherwise generates a new player in the game manager
            if (tempPlayerName != "")
            {
                GameManager.instance.GeneratePlayer(counter, tempPlayerName);
            }
            else
            {
                Debug.Log(string.Format("Invalid Player Name for Player {0}", counter));
                break;
            }
            counter++;

            //If all the needed players are accounted for, loads the next scene and breaks from the loop to prevent it running in the background
            if (counter == GameManager.instance.numPlayers)
            {
                NetworkManager.singleton.ServerChangeScene("Character SelectionV2");
            }

        }
    }

    #endregion
}
