using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class AbilityManager : MonoBehaviour
{

    public static AbilityManager instance = null;
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

    public bool[] CheckCorruption;
    public bool[] CheckScrap;

    private void Start() {
        instance = this;
    }

    /// <summary>
    /// 
    /// Setup the abilities panel to display the abilities avaialble to the player, including disabling abilities which they do
    /// not have enough scrap or corruption for.
    /// 
    /// </summary>
    public void SetupAbilities()
    {
        Debug.Log("setting up abilities");
        selectedAbility = new Ability();

        abilityInfoText.SetActive(false);

        selectedText.GetComponent<TextMeshProUGUI>().text = "";
        selectButton.GetComponent<Button>().interactable = false;

        abilityActiveDisplay.SetActive(false);
        playerTargetDisplay.SetActive(false);
        resourceTargetDisplay.SetActive(false);

        int counter = 0;

        Debug.Log("it gets here");

        foreach (GameObject abilityButton in abilityButtons)
        {
            Ability currentAbility = ClientManager.instance.abilities[counter];

            Debug.Log("Ability " + ClientManager.instance.abilities[counter]);
            abilityButton.GetComponent<Button>().interactable = true;
            
            abilityButton.GetComponent<AbilityButtonComponents>().abilityNameText.GetComponent<TextMeshProUGUI>().text = currentAbility.AbilityName;
            abilityButton.GetComponent<AbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().text = string.Format("{0}%", currentAbility.corruptionRequirement.ToString());
            abilityButton.GetComponent<AbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().text = currentAbility.scrapCost.ToString();

            if (CheckScrap[counter])
            {
                abilityButton.GetComponent<AbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                abilityButton.GetComponent<Button>().interactable = false;
                abilityButton.GetComponent<AbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            if (CheckCorruption[counter])
            {                
                abilityButton.GetComponent<AbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                abilityButton.GetComponent<Button>().interactable = false;
                abilityButton.GetComponent<AbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            counter++;
        }
    }

    /// <summary>
    /// 
    /// Pressing one of the ability buttons will select it to be confirmed later.
    /// 
    /// </summary>
    /// <param name="buttonID">The ID of the button selected, which relates to the index of the ability in the players ability list</param>
    public void SelectAbility(int buttonID)
    {
        selectedAbility = ClientManager.instance.abilities[buttonID];

        selectedText.GetComponent<TextMeshProUGUI>().text = selectedAbility.AbilityName;
        selectButton.GetComponent<Button>().interactable = true;
    }

    /// <summary>
    /// 
    /// Confirm the ability to be selected and open the relevant panel for the various scenarios which the abilities present.
    /// 
    /// </summary>
    public void ConfirmAbility()
    {
        selectedPlayer = GameManager.DEFAULT_PLAYER_ID;
        selectedResource = Ability.ScanResources.Default;

        switch (selectedAbility.abilityType)
        {
            //Case for abilities which do not require a selection screen
            case (Ability.AbilityTypes.Sabotage):
                selectedAbility.Activate();
                DisplayActiveAbility();
                Server.Instance.SendAbilityUsed(selectedAbility.abilityType, GameManager.DEFAULT_PLAYER_ID, Ability.ScanResources.Default);
                break;
            
            //Case for abilities which require a player to be targetted
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

            //Case for abilities which require a resource to be selected
            case (Ability.AbilityTypes.Sensor_Scan):

                resourceTargetDisplay.SetActive(true);
                resourceSelectedButton.GetComponent<Button>().interactable = false;
                resourceSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "No resource selected";
                break;

            default:
                throw new NotImplementedException("Ability Not Defined");
        }
    }

    /// <summary>
    /// 
    /// Select a player target to be targeted with the active ability
    /// 
    /// </summary>
    /// <param name="buttonID">The ID of the button being selected</param>
    public void SelectTarget(int buttonID)
    {
        selectedPlayer = targetButtons[buttonID].GetComponent<TargetProperties>().playerID;
        playerSelectedButton.GetComponent<Button>().interactable = true;
        playerSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("Selected: {0}", ClientManager.instance.GetPlayerData(selectedPlayer).PlayerName);  
    }

    /// <summary>
    /// 
    /// Confirm a player target to pass onto the selected ability and activate it.
    /// 
    /// </summary>
    public void ConfirmTarget()
    {
        //For code inspection, need to display if the player is or isn't a traitor
        if(selectedAbility.abilityType == Ability.AbilityTypes.Code_Inspection)
        {
            selectedAbility.Activate(selectedPlayer, out bool isTraitor);
            Server.Instance.SendAbilityUsed(selectedAbility.abilityType, selectedPlayer, Ability.ScanResources.Default);
            //Set up the modifier to the traitor string
            string traitorString = "";
            if (!isTraitor)
            {
                traitorString = "not ";
            }
            abilityInfoText.SetActive(true);
            abilityInfoText.GetComponent<TextMeshProUGUI>().text = string.Format("{0} is {1}a traitor", ClientManager.instance.GetPlayerData(selectedPlayer).PlayerName, traitorString);
        }
        else
        {
            selectedAbility.Activate(selectedPlayer);
            Server.Instance.SendAbilityUsed(selectedAbility.abilityType, selectedPlayer, Ability.ScanResources.Default);
        }

        DisplayActiveAbility();
    }

    /// <summary>
    /// 
    /// Select a resource target to be targeted with the sensor scan ability
    /// 
    /// </summary>
    /// <param name="buttonID">The ID of the button being selected</param>
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

    /// <summary>
    /// 
    /// Confirm the resource target and pass it into the selected ability and activate it
    /// 
    /// </summary>
    public void ConfirmResource()
    {
        //Find the rooms which contain the selected resource and display it to the player
        List<int> scannedRooms = selectedAbility.Activate(selectedResource);
        Server.Instance.SendAbilityUsed(selectedAbility.abilityType, GameManager.DEFAULT_PLAYER_ID, selectedResource);
        List<string> scannedRoomsString = new List<string>();
        foreach (int roomID in scannedRooms)
        {
            scannedRoomsString.Add(roomID.ToString());
        }
        abilityInfoText.SetActive(true);
        abilityInfoText.GetComponent<TextMeshProUGUI>().text = string.Join(" ", scannedRoomsString);
        DisplayActiveAbility();
    }

    /// <summary>
    /// 
    /// Cancels the selection of an ability to go back to the abilities page
    /// 
    /// </summary>
    public void CancelSelection()
    {
        playerTargetDisplay.SetActive(false);
        resourceTargetDisplay.SetActive(false);
    }

    /// <summary>
    /// 
    /// Displays the activated ability to the player
    /// 
    /// </summary>
    private void DisplayActiveAbility()
    {
        abilityActiveDisplay.SetActive(true);
        abilityActiveDisplay.GetComponent<ActiveAbilityDisplay>().UpdateActiveText(selectedAbility);
        playerCards.GetComponent<PlayerCardManager>().UpdateAllCards();
    }
}
