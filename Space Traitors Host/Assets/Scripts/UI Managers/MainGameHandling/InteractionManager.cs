using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public GameObject standardChoiceUI;
    public GameObject escapeRoomUI;

    public GameObject choice0ButtonText;
    public GameObject choice1ButtonText;

    public GameObject attackButton;

    public GameObject choiceInfoPanel;

    public Room currentRoom;
    public int selectedChoiceID;

    private List<int> attackablePlayers;
    
    /// <summary>
    /// 
    /// Update the UI to allow the player to select the choices relevant to a particular room.
    /// 
    /// </summary>
    /// <param name="roomIndex">The index of the room the player is in</param>
    public void InitialiseChoices(int roomIndex)
    {
        //Escape room is for installing components, so requies a different screen. The Escape Room ID is where the players start
        if(roomIndex == Player.STARTING_ROOM_ID)
        {
            escapeRoomUI.SetActive(true);
            standardChoiceUI.SetActive(false);

            throw new NotImplementedException("Escape Room Not Implemented");
        }
        else
        {
            standardChoiceUI.SetActive(true);
            escapeRoomUI.SetActive(false);

            choiceInfoPanel.SetActive(false);
            
            currentRoom = GameManager.instance.GetRoom(roomIndex);

            //Update button text
            choice0ButtonText.GetComponent<TextMeshProUGUI>().text = currentRoom.roomChoices[0].choiceName;
            choice1ButtonText.GetComponent<TextMeshProUGUI>().text = currentRoom.roomChoices[1].choiceName;
        }

        //Checks is combat is available for the active player, enabling the attack button if it is
        attackablePlayers = GameManager.instance.CheckCombat();
        if (attackablePlayers.Count == 0)
        {
            attackButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            attackButton.GetComponent<Button>().interactable = true;
        }
    }

    /// <summary>
    /// 
    /// Update the choice information panel to suit the players selected choice to display
    /// 
    /// </summary>
    /// <param name="choiceID">The ID of the choice being displayed</param>
    public void DisplayChoice(int choiceID)
    {
        choiceInfoPanel.SetActive(true);
        selectedChoiceID = choiceID;
        Choice selectedChoice = currentRoom.roomChoices[selectedChoiceID];

        choiceInfoPanel.GetComponent<ChoiceInfoComponents>().choiceHeader.GetComponent<TextMeshProUGUI>().text = selectedChoice.choiceName;
        
        //If the choice is not a spec challenge, updates the display text to suit the choice.
        if(selectedChoice.specChallenge == GameManager.SpecScores.Default)
        {
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().nonSpecChoiceText.SetActive(true);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specChallengeGroup.SetActive(false);

            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().nonSpecChoiceText.GetComponent<TextMeshProUGUI>().text = selectedChoice.SuccessText();
        }
        //If the choice is a spec challenge, displays the success and failure state for the spec challenge choice
        else
        {
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().nonSpecChoiceText.SetActive(false);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specChallengeGroup.SetActive(true);

            float playerScore;

            //Find the players relevant spec score need for the choice
            switch (selectedChoice.specChallenge)
            {
                case (GameManager.SpecScores.Brawn):
                    playerScore = GameManager.instance.GetActivePlayer().ScaledBrawn;
                    break;
                case (GameManager.SpecScores.Skill):
                    playerScore = GameManager.instance.GetActivePlayer().ScaledSkill;
                    break;
                case (GameManager.SpecScores.Tech):
                    playerScore = GameManager.instance.GetActivePlayer().ScaledTech;
                    break;
                case (GameManager.SpecScores.Charm):
                    playerScore = GameManager.instance.GetActivePlayer().ScaledCharm;
                    break;
                default:
                    throw new NotImplementedException("Not a valid choice");
            }

            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specChanceText.GetComponent<TextMeshProUGUI>().text =
                string.Format("Chance: {0}%", Mathf.Round(GameManager.instance.SpecChallengeChance(playerScore, selectedChoice.targetScore)).ToString());
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specSuccessText.GetComponent<TextMeshProUGUI>().text = selectedChoice.SuccessText();
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specFailureText.GetComponent<TextMeshProUGUI>().text = selectedChoice.FailText();
        }

        //Test if the choice is available to the player and displays the reason it cannot be selected if not.
        Choice.IsAvailableTypes isAvailable = selectedChoice.IsAvailable();

        if(isAvailable == Choice.IsAvailableTypes.available)
        {
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().errorText.SetActive(false);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().choiceSelectButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().errorText.SetActive(true);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().errorText.GetComponent<TextMeshProUGUI>().text = selectedChoice.ConvertErrorText(isAvailable);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().choiceSelectButton.GetComponent<Button>().interactable = false;
        }
    }

    
}
