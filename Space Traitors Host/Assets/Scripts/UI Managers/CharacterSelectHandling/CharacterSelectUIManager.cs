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

    public GameObject activePlayerText;
    public GameObject SelectButton;
    public GameObject ErrorText;

    public GameObject charInfoPanel;

    public Character.CharacterTypes selectedCharacterType;

    public List<GameObject> characterButtons;

    private void Start()
    {
        InitCharacterSelection();

        
    }

    #region SeverHandeling



    /// <summary>
    /// 
    /// Activates the player when its their turn to pick
    /// 
    /// </summary>
    public void DisplayActivePlayer()
    {       
        SelectButton.GetComponent<Button>().interactable = true;
        activePlayerText.GetComponent<TextMeshProUGUI>().text = string.Format("Your Turn");
    }

    /// <summary>
    /// 
    /// Return the selected character to the game manager and loops through to the next player in the character selection order
    /// 
    /// </summary>
    public void ConfirmCharacter()
    {
        Server.Instance.SendCharacterSelected((int)selectedCharacterType);
        SelectButton.GetComponent<Button>().interactable = false;

        //Checks if the player actually has selected a character
        //if(tempCharacterType != Character.CharacterTypes.Default)
        //{
        //    Server.Instance.SendCharacterSelected((int)tempCharacterType);

        //}
        //else
        //{
        //    Debug.Log("Please Select a Character");
        //}


    }

    public void ResetCharacterSelection()
    {
        SelectButton.GetComponent<Button>().interactable = true;
    }

    public void EndSelection() {
        
        ClientManager.instance.PlayerCharacter = new Character(selectedCharacterType);
        SelectButton.GetComponent<Button>().interactable = false;
        activePlayerText.GetComponent<TextMeshProUGUI>().text = string.Format("Please wait");
        SetErrorText("");
    }

    private void InitCharacterSelection()
    {
        selectedCharacterType = Character.CharacterTypes.Default;

        charInfoPanel.GetComponent<ClientUICharacterInfoComponents>().confirmButton.GetComponent<Button>().interactable = false;

        charInfoPanel.SetActive(false);
        ResetCharacterButtons();

        activePlayerText.GetComponent<TextMeshProUGUI>().text = string.Format("Please wait");

        SetErrorText("");
    }

    public void DisplayCharacterInfo(int buttonID)
    {
        selectedCharacterType = characterButtons[buttonID].GetComponent<ClientUICharacterButton>().characterType;

        foreach (GameObject characterButton in characterButtons)
        {
            if (characterButton.GetComponent<ClientUICharacterButton>().characterType != selectedCharacterType)
            {
                characterButton.SetActive(false);
            }
        }

        Character selectedCharacter = ClientManager.instance.GetCharacterInfo((int)selectedCharacterType);

        charInfoPanel.SetActive(true);
        ClientUICharacterInfoComponents charInfoComponents = charInfoPanel.GetComponent<ClientUICharacterInfoComponents>();

        charInfoComponents.nameText.GetComponent<TextMeshProUGUI>().text = selectedCharacter.CharacterName;

        charInfoComponents.brawnText.GetComponent<TextMeshProUGUI>().text = selectedCharacter.baseBrawn.ToString();
        charInfoComponents.skillText.GetComponent<TextMeshProUGUI>().text = selectedCharacter.baseSkill.ToString();
        charInfoComponents.techText.GetComponent<TextMeshProUGUI>().text = selectedCharacter.baseTech.ToString();
        charInfoComponents.charmText.GetComponent<TextMeshProUGUI>().text = selectedCharacter.baseCharm.ToString();

        charInfoComponents.abilityName.GetComponent<TextMeshProUGUI>().text = selectedCharacter.characterAbility.AbilityName;
        charInfoComponents.abilityText.GetComponent<TextMeshProUGUI>().text = selectedCharacter.characterAbility.abilityDescription;
        //Need to add in retrieval for ability icons
        //charInfoComponents.abilityIcon.GetComponent<Image>().sprite = 
    }

    public void CloseCharacterInfo()
    {
        charInfoPanel.SetActive(false);
        ResetCharacterButtons();
    }

    private void ResetCharacterButtons()
    {
        foreach (GameObject characterButton in characterButtons)
        {
            characterButton.SetActive(true);
        }
    }

    public void SetErrorText(string errorText)
    {
        ErrorText.GetComponent<TextMeshProUGUI>().text = errorText;
    }
    #endregion


}
