using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectUIManager : MonoBehaviour
{
    public GameObject serverActivePanel;
    public GameObject noServerPanel;

    public GameObject playerList;

    public GameObject activePlayerPanel;

    private string tempCharacterType;

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

            SetupPlayerList();
            DisplayActivePlayer();
            tempCharacterType = "";
            DisplaySelectedCharacter();
        }
    }

    #region No ServerHandling

    /// <summary>
    /// 
    /// Setups the player list side bar to take the character names from the player array. Also disables any unused players in the list so they are not visible
    /// 
    /// </summary>
    private void SetupPlayerList()
    {
        //Since character selection is in the reverse order to the order of play, will need to start at the top end of the player order list
        int counter = GameManager.instance.numPlayers - 1;

        foreach(Transform playerPanels in playerList.transform)
        {
            if (counter >= 0)
            {
                playerPanels.gameObject.SetActive(true);
                //Since the order of the character selection is randomised, need to obtain the relevant player ID from the player order array, then find that relevant players information
                int playerPointer = GameManager.instance.playerOrder[counter];
                string playerID = GameManager.instance.players[playerPointer].playerID.ToString();
                string playerName = GameManager.instance.players[playerPointer].playerName;

                //Set the player name to be their ID and their name
                playerPanels.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0}, {1}", playerID, playerName);
                //Sets the player's character name to be blank
                playerPanels.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                //Disable any irrelevant player cards
                playerPanels.gameObject.SetActive(false);
            }

            //Work backwards through the player order list
            counter--;
        }
    }

    /// <summary>
    /// 
    /// Displays the ID and name of the player currently selecting their character on the Active Player Panel
    /// 
    /// </summary>
    private void DisplayActivePlayer()
    {
        string playerID = GameManager.instance.ActivePlayer().playerID.ToString();
        string playerName = GameManager.instance.ActivePlayer().playerName;

        activePlayerPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0}, {1}", playerID, playerName);
    }

    /// <summary>
    /// 
    /// Updates the selected character text on the Active Player Panel
    /// 
    /// </summary>
    private void DisplaySelectedCharacter()
    {
        activePlayerPanel.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = tempCharacterType;
    }

    /// <summary>
    /// 
    /// Temporarily stores the character type the player has selected and displays it on the Active Player Panel
    /// 
    /// </summary>
    /// <param name="characterType"></param>
    public void SelectCharacter(string characterType)
    {
        tempCharacterType = characterType;
        DisplaySelectedCharacter();
    }

    /// <summary>
    /// 
    /// Return the selected character to the game manager and loops through to the next player in the character selection order
    /// 
    /// </summary>
    public void ConfirmCharacter()
    {
        //Checks if the player actually has selected a character
        if(tempCharacterType != "")
        {
            //Checks if the character has already been selected
            if (!GameManager.instance.CheckCharacterSelected(tempCharacterType))
            {
                //Updates the player list with the new character selection and assigns the character in the game manager
                UpdatePlayerCharacter();
                GameManager.instance.SelectCharacter(tempCharacterType);
                //If there are no more players to select moves into next scene, otherwise updates variables for next player selection
                if (GameManager.instance.activePlayer < 0)
                {
                    SceneManager.LoadScene("Game Level");
                }
                else
                {
                    tempCharacterType = "";
                    DisplaySelectedCharacter();
                    DisplayActivePlayer();
                }
            }
            else
            {
                Debug.Log("Character already selected. Please select another Character");
            }
        }
        else
        {
            Debug.Log("Please Select a Character");
        }
    }

    /// <summary>
    /// 
    /// Updates the player list to include the newly selected character
    /// 
    /// </summary>
    public void UpdatePlayerCharacter()
    {
        int currentPlayerPos = GameManager.instance.numPlayers - GameManager.instance.activePlayer - 1;
        playerList.transform.GetChild(currentPlayerPos).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = tempCharacterType;
    }

    #endregion
}
