using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyUIManager : NetworkBehaviour
{
    private bool prerequisitesNotLoaded;

    public GameObject playerNumPanel;
    public GameObject playerNamePanel;

    public GameObject NetworkManagerObject;

    private Transform nameEntryFields;

    public GameObject confirmNamesButton;

    public int currentNamePos;

    public GameObject connectedText;
    public GameObject numberErrorText;

    private void Start()
    {
        NetworkManagerObject = GameObject.Find("NetworkManager");
        playerNumPanel.SetActive(true);
        playerNamePanel.SetActive(false);

        nameEntryFields = playerNamePanel.transform.GetChild(1);
        ChangeInputFields(0);

        currentNamePos = 0;

        connectedText.SetActive(false);

        numberErrorText.SetActive(false);
    }

    void Update()
    {



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
            playerNumPanel.SetActive(false);
            playerNamePanel.SetActive(true);
            NetworkManagerObject.GetComponent<CustomNetworkDiscovery>().StartHost();

        }
        else
        {
            numberErrorText.SetActive(true);
            textField.GetComponent<TMP_InputField>().text = "";
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

        Debug.Log(playerNum);

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


    public void AddPlayerNames(int playerID)
    {

        string tempPlayername = GameManager.instance.GetPlayer(playerID).playerName;

        //if (GameManager.instance.CheckNameEntry())
        //{
        //    confirmNamesButton.GetComponent<Button>().interactable = true;
        //}

        int counter = 0;

        foreach (Transform entryField in nameEntryFields.transform)
        {
            if (currentNamePos == counter)
            {
                entryField.GetChild(0).GetComponent<TextMeshProUGUI>().text = tempPlayername;
                entryField.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                if(counter == GameManager.instance.numPlayers - 1)
                {
                    //confirmNamesButton.GetComponent<Button>().interactable = true;
                    connectedText.SetActive(true);
                    StartCoroutine("WaitStartGame");
                }
                currentNamePos++;
                break;
            }

            counter++;
        }

    }

    IEnumerator WaitStartGame()
    {
        yield return new WaitForSeconds(3.0f);
        Server.Instance.StartGame();
    }


    /// <summary>
    /// 
    /// Assigns the given player names to new players based on the given inputs
    /// 
    /// </summary>
    /// <param name="textFields">The parent object of the input fields</param>
    public void ConfirmPlayerNames(GameObject textFields)
    {
        ////counter for keeping track of the playerIDs
        //int counter = 0;

        ////Obtains each child object of the textFields object
        //foreach (Transform entryField in textFields.transform)
        //{
        //    string tempPlayerName = entryField.GetComponent<TMP_InputField>().text;
        //    //If any of the input fields are empty, stops the process and presents an error. Otherwise generates a new player in the game manager
        //    if (tempPlayerName != "")
        //    {
        //        counter++;
        //    }
        //    else
        //    {
        //        Debug.Log(string.Format("Invalid Player Name for Player {0}", counter));
        //        break;
        //    }


        //    //If all the needed players are accounted for, loads the next scene and breaks from the loop to prevent it running in the background
        //    if (counter == GameManager.instance.numPlayers)
        //    {
        //        Server.Instance.StartGame();

        //    }

        //}

        Server.Instance.StartGame();
    }

    #endregion
}
