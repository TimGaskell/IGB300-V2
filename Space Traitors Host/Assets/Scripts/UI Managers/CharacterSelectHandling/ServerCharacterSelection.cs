using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ServerCharacterSelection : MonoBehaviour {
    public GameObject serverActivePanel;

    public GameObject playerList;

    public GameObject activePlayerPanel;

    public List<String> DisabledCharacters;

    public Character.CharacterTypes tempCharacterType;

    private void Start() {
        if (GameManager.instance.serverActive) {
            serverActivePanel.SetActive(true);
            SetupPlayerList();
            DisplayActivePlayer();
            tempCharacterType = Character.CharacterTypes.Default;
            DisplaySelectedCharacter();
        }
        else {
            serverActivePanel.SetActive(false);
     

            SetupPlayerList();
            DisplayActivePlayer();
            tempCharacterType = Character.CharacterTypes.Default;
            DisplaySelectedCharacter();
        }
    }

    private void Update() {
        
        if(GameManager.instance.activePlayer == 0) {

            //Send message to every player's client to move onto next scene
            Server.Instance.SendChangeScene("GameLevel");
            //Change to the character select
            SceneManager.LoadScene("GameLevel");

        }
    }



    #region No ServerHandling

    /// <summary>
    /// 
    /// Setups the player list to take the player names from the player array. Also disables any unused players in the list so they are not visible
    /// 
    /// </summary>
    private void SetupPlayerList() {
        //Since character selection is in the reverse order to the order of play, will need to start at the top end of the player order list
        int counter = GameManager.instance.numPlayers;
        foreach (Transform playerPanels in playerList.transform) {
            if (counter > 0) {
                playerPanels.gameObject.SetActive(true);
                //Need to sort the panels by the order of the character selection, not by their ID, so need to get the information from an ordered player
                Player playerInfo = GameManager.instance.GetOrderedPlayer(counter);
                int playerID = playerInfo.playerID;
                string playerName = playerInfo.playerName;    
                //Set the player name to be their ID and their name
                playerPanels.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0}, {1}", playerID, playerName);
                //Sets the player's character name to be blank
                playerPanels.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";      
            }
            else {
                //Disable any irrelevant player cards
                playerPanels.gameObject.SetActive(false);
            }
            //Work backwards through the player order list
            counter--;
        }

        Server.Instance.SendActivePlayer(GameManager.instance.GetActivePlayer().playerID);

    }

    /// <summary>
    /// 
    /// Displays the ID and name of the player currently selecting their character on the Active Player Panel
    /// 
    /// </summary>
    private void DisplayActivePlayer() {
        //Find the active player in the game manager and displays its information
        Player activePlayer = GameManager.instance.GetActivePlayer();
        string playerID = activePlayer.playerID.ToString();
        string playerName = activePlayer.playerName;

        activePlayerPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0}, {1}", playerID, playerName);
    }

    /// <summary>
    /// 
    /// Updates the selected character text on the Active Player Panel
    /// 
    /// </summary>
    public void DisplaySelectedCharacter() {
        //If the character type is default, displays an empty string- otherwise displays the selected character
        activePlayerPanel.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = tempCharacterType == Character.CharacterTypes.Default ? "" : tempCharacterType.ToString();
    }

    /// <summary>
    /// 
    /// Temporarily stores the character type the player has selected and displays it on the Active Player Panel
    /// 
    /// </summary>
    /// <param name="characterType"></param>
    public void SelectCharacter(string characterType) {
        //Button input functions cannot take enums as parameters. As such have to convert to enum from string input
        tempCharacterType = (Character.CharacterTypes)Enum.Parse(typeof(Character.CharacterTypes), characterType);
        DisplaySelectedCharacter();
    }

    

    /// <summary>
    /// 
    /// Updates the player list to include the newly selected character
    /// 
    /// </summary>
    public void UpdatePlayerCharacter() {
        tempCharacterType = GameManager.instance.GetOrderedPlayer(GameManager.instance.activePlayer).Character.CharacterType;
        int currentPlayerPos = GameManager.instance.numPlayers - GameManager.instance.activePlayer;
        playerList.transform.GetChild(currentPlayerPos).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = tempCharacterType.ToString();
    }

    #endregion

    #region ServerHandling


    #endregion
}

