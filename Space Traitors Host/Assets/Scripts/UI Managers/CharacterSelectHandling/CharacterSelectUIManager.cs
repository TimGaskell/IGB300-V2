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

    private Character.CharacterTypes tempCharacterType;

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
            tempCharacterType = Character.CharacterTypes.Default;
            DisplaySelectedCharacter();
        }
    }

    #region No ServerHandling

    /// <summary>
    /// 
    /// Setups the player list to take the player names from the player array. Also disables any unused players in the list so they are not visible
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
                //Need to sort the panels by the order of the character selection, not by their ID, so need to get the information from an ordered player
                Player playerInfo = GameManager.instance.GetOrderedPlayer(counter);
                int playerID = playerInfo.playerID;
                string playerName = playerInfo.playerName;

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
    private void DisplaySelectedCharacter()
    {
        //If the character type is default, displays an empty string- otherwise displays the selected character
        activePlayerPanel.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = tempCharacterType == Character.CharacterTypes.Default ? "": tempCharacterType.ToString();
    }

    /// <summary>
    /// 
    /// Temporarily stores the character type the player has selected and displays it on the Active Player Panel
    /// 
    /// </summary>
    /// <param name="characterType"></param>
    public void SelectCharacter(string characterType)
    {
        //Button input functions cannot take enums as parameters. As such have to convert to enum from string input
        tempCharacterType = (Character.CharacterTypes)Enum.Parse(typeof(Character.CharacterTypes), characterType);
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
        if(tempCharacterType != Character.CharacterTypes.Default)
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
                    SceneManager.LoadScene(GameManager.MainGameScene);
                }
                else
                {
                    tempCharacterType = Character.CharacterTypes.Default;
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
        playerList.transform.GetChild(currentPlayerPos).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = tempCharacterType.ToString();
    }

    #endregion

    #region ServerHandling


    #endregion
}
