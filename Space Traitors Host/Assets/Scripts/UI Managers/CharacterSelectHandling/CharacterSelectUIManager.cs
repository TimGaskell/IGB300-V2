using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUIManager : MonoBehaviour
{
    public GameObject serverActivePanel;
    public GameObject noServerPanel;

    public GameObject activePlayerPanel;
    public GameObject SelectButton;
    private GameObject PlayerObject;

    private Character.CharacterTypes tempCharacterType;

    private void Start()
    {
        if (GameManager.instance.serverActive)
        {
            serverActivePanel.SetActive(true);
            noServerPanel.SetActive(false);
            PlayerObject = GameObject.Find("PlayerInfoHolder");
        }
       
    }

    #region SeverHandeling



    /// <summary>
    /// 
    /// Activates the player when its their turn to pick
    /// 
    /// </summary>
    public void DisplayActivePlayer()
    {       
        SelectButton.GetComponent<Button>().enabled = true;
        SelectButton.GetComponent<Image>().color = Color.white;
        activePlayerPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("Your Turn");
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
            Server.Instance.SendCharacterSelected((int)tempCharacterType);
          
        }
        else
        {
            Debug.Log("Please Select a Character");
        }

        
    }

    public void EndSelection() {
        
        PlayerObject.GetComponent<Player>().Character = new Character((Character.CharacterTypes)(int)tempCharacterType);
        SelectButton.GetComponent<Button>().enabled = false;
        SelectButton.GetComponent<Image>().color = Color.gray;
        activePlayerPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("Please wait");
    }
    #endregion

   
}
