using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class NSAbilityManager : MonoBehaviour
{
    public List<GameObject> abilityButtons;
    public GameObject selectedText;
    public GameObject selectButton;

    private Ability selectedAbility;

    public GameObject abilityActiveDisplay;
    public GameObject playerTargetDisplay;
    public GameObject resourceTargetDisplay;

    public List<GameObject> targetButtons;
    public GameObject playerSelectedButton;
    public GameObject resourceSelectedButton;

    private int selectedPlayer;
    private Ability.ScanResources selectedResource;

    public GameObject abilityInfoText;

    public GameObject playerCards;

    public void SetupAbilities()
    {
        selectedAbility = new Ability();

        abilityInfoText.SetActive(false);

        selectedText.GetComponent<TextMeshProUGUI>().text = "";
        selectButton.GetComponent<Button>().interactable = false;

        abilityActiveDisplay.SetActive(false);
        playerTargetDisplay.SetActive(false);
        resourceTargetDisplay.SetActive(false);

        int counter = 0;

        foreach (GameObject abilityButton in abilityButtons)
        {
            Ability currentAbility = GameManager.instance.GetActivePlayer().GetAbility(counter);
            abilityButton.GetComponent<Button>().interactable = true;

            abilityButton.GetComponent<NSAbilityButtonComponents>().abilityNameText.GetComponent<TextMeshProUGUI>().text = currentAbility.AbilityName;
            abilityButton.GetComponent<NSAbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().text = string.Format("{0}%", currentAbility.corruptionRequirement.ToString());
            abilityButton.GetComponent<NSAbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().text = currentAbility.scrapCost.ToString();

            if (currentAbility.CheckScrap())
            {
                abilityButton.GetComponent<NSAbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                abilityButton.GetComponent<Button>().interactable = false;
                abilityButton.GetComponent<NSAbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            if (currentAbility.CheckCorruption())
            {                
                abilityButton.GetComponent<NSAbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                abilityButton.GetComponent<Button>().interactable = false;
                abilityButton.GetComponent<NSAbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            counter++;
        }
    }

    public void SelectAbility(int buttonID)
    {
        selectedAbility = GameManager.instance.GetActivePlayer().GetAbility(buttonID);

        selectedText.GetComponent<TextMeshProUGUI>().text = selectedAbility.AbilityName;
        selectButton.GetComponent<Button>().interactable = true;
    }

    public void ConfirmAbility()
    {
        selectedPlayer = GameManager.DEFAULT_PLAYER_ID;
        selectedResource = Ability.ScanResources.Default;

        switch (selectedAbility.abilityType)
        {
            case (Ability.AbilityTypes.Sabotage):
                selectedAbility.Activate();
                DisplayActiveAbility();
                break;

            case (Ability.AbilityTypes.Secret_Paths):
            case (Ability.AbilityTypes.Power_Boost):
            case (Ability.AbilityTypes.Encouraging_Song):
            case (Ability.AbilityTypes.Muddle_Sensors):
            case (Ability.AbilityTypes.Code_Inspection):
            case (Ability.AbilityTypes.Supercharge):

                playerTargetDisplay.SetActive(true);
                playerSelectedButton.GetComponent<Button>().interactable = false;
                playerSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "No target selected";
                break;

            case (Ability.AbilityTypes.Sensor_Scan):

                resourceTargetDisplay.SetActive(true);
                resourceSelectedButton.GetComponent<Button>().interactable = false;
                resourceSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "No resource selected";
                break;

            default:
                throw new NotImplementedException("Ability Not Defined");
        }
    }

    public void SelectTarget(int buttonID)
    {
        selectedPlayer = targetButtons[buttonID].GetComponent<NSTargetProperties>().playerID;
        playerSelectedButton.GetComponent<Button>().interactable = true;
        playerSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("Selected: {0}", GameManager.instance.GetPlayer(selectedPlayer).playerName);
    }

    public void ConfirmTarget()
    {
        if(selectedAbility.abilityType == Ability.AbilityTypes.Code_Inspection)
        {
            selectedAbility.Activate(selectedPlayer, out bool isTraitor);
            string traitorString = "";
            if (!isTraitor)
            {
                traitorString = "not ";
            }
            abilityInfoText.SetActive(true);
            abilityInfoText.GetComponent<TextMeshProUGUI>().text = string.Format("{0} is {1}a traitor", GameManager.instance.GetPlayer(selectedPlayer), traitorString);
        }
        else
        {
            selectedAbility.Activate(selectedPlayer);
        }

        DisplayActiveAbility();
    }

    public void SelectResource(int buttonID)
    {
        switch (buttonID)
        {
            case (0):
                selectedResource = Ability.ScanResources.Scrap;
                break;
            case (1):
                selectedResource = Ability.ScanResources.Items;
                break;
            case (2):
                selectedResource = Ability.ScanResources.Components;
                break;
        }

        resourceSelectedButton.GetComponent<Button>().interactable = true;
        resourceSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("Selected: {0}", selectedResource.ToString());
    }

    public void ConfirmResource()
    {
        List<int> scannedRooms = selectedAbility.Activate(selectedResource);
        List<string> scannedRoomsString = new List<string>();
        foreach (int roomID in scannedRooms)
        {
            scannedRoomsString.Add(roomID.ToString());
        }
        abilityInfoText.SetActive(true);
        abilityInfoText.GetComponent<TextMeshProUGUI>().text = string.Join(" ", scannedRoomsString);
        DisplayActiveAbility();
    }

    public void CancelSelection()
    {
        playerTargetDisplay.SetActive(false);
        resourceTargetDisplay.SetActive(false);
    }

    private void DisplayActiveAbility()
    {
        abilityActiveDisplay.SetActive(true);
        abilityActiveDisplay.GetComponent<NSActiveAbilityDisplay>().UpdateActiveText(selectedAbility);
        playerCards.GetComponent<NSPlayerCardManager>().UpdateAllCards();
    }
}
