using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ClientUICharacterSelect : MonoBehaviour
{
    public GameObject charInfoPanel;

    public Character.CharacterTypes selectedCharacterType;

    public List<GameObject> characterButtons;

    private void Start()
    {
        InitCharacterSelection();
    }

    private void InitCharacterSelection()
    {
        selectedCharacterType = Character.CharacterTypes.Default;

        charInfoPanel.GetComponent<ClientUICharacterInfoComponents>().confirmButton.GetComponent<Button>().interactable = false;

        charInfoPanel.SetActive(false);
        ResetCharacterButtons();
    }

    public void DisplayCharacterInfo(int buttonID)
    {
        selectedCharacterType = characterButtons[buttonID].GetComponent<ClientUICharacterButton>().characterType;

        foreach (GameObject characterButton in characterButtons)
        {
            if(characterButton.GetComponent<ClientUICharacterButton>().characterType != selectedCharacterType)
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

    public void EnableCharacterSelection()
    {
        charInfoPanel.GetComponent<ClientUICharacterInfoComponents>().confirmButton.GetComponent<Button>().interactable = true;
    }

    public void ConfirmCharacterSelection()
    {
        //Needs to utilise server script from here
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
}
