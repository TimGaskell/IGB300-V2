using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    public GameObject standardChoiceUI;
    public GameObject escapeRoomUI;

    public GameObject choice0ButtonText;
    public GameObject choice1ButtonText;

    public GameObject attackButton;

    public GameObject choiceInfoPanel;

    private Room currentRoom;
    private int selectedChoice;
    
    public void InitialiseChoices(int roomIndex)
    {
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

            choice0ButtonText.GetComponent<TextMeshProUGUI>().text = currentRoom.roomChoices[0].choiceName;
            choice1ButtonText.GetComponent<TextMeshProUGUI>().text = currentRoom.roomChoices[1].choiceName;
        }
    }

    public void DisplayChoice(int choiceID)
    {
        choiceInfoPanel.SetActive(true);
        selectedChoice = choiceID;

        choiceInfoPanel.GetComponent<ChoiceInfoComponents>().choiceHeader.GetComponent<TextMeshProUGUI>().text = currentRoom.roomChoices[selectedChoice].choiceName;
        choiceInfoPanel.GetComponent<ChoiceInfoComponents>().displayText.GetComponent<TextMeshProUGUI>().text = currentRoom.roomChoices[selectedChoice].SuccessText();
    }

    public void SelectChoice()
    {
        currentRoom.roomChoices[selectedChoice].SelectChoice();
        
    }
}
