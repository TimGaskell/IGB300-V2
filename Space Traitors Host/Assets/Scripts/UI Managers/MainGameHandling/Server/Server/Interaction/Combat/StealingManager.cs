﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StealingManager : MonoBehaviour
{
    public static StealingManager instance = null;
    public GameObject playerCards;
    public GameObject stealComponentButton;

    public Player winner;
    public List<Item> losersItems;
    public bool hasComponent = false;
    public int loserID;

    public GameObject winnerText;
    public GameObject loserText;

    public GameObject loserItemParent;

    public GameObject winnerItemActions;
    public GameObject loserItemActions;

    public GameObject selectedItemText;
    private Item selectedItem;
    private int selectedID;

    public List<GameObject> winnerItemButtons;
    public List<GameObject> loserItemButtons;

    public GameObject errorText;

    public GameObject equipButton;


    private void Start() {
        instance = this;
    }
    /// <summary>
    /// 
    /// Sets up the stealing panel to present the relevant player's information
    /// 
    /// </summary>
    public void StartStealPanel()
    {
        winnerText.GetComponent<TextMeshProUGUI>().text = ClientManager.instance.playerName;
        loserText.GetComponent<TextMeshProUGUI>().text =  ClientManager.instance.GetPlayerData(loserID).PlayerName;

        UpdateItemButtons();
        loserItemParent.GetComponent<CanvasGroup>().interactable = true;
    }

    /// <summary>
    /// 
    /// Updates the item buttons to display each player's inventory
    /// 
    /// </summary>
    public void UpdateItemButtons()
    {
        string winnerDisplayText;
        string loserDisplayText;

        SetErrorText("");
        ResetSelectedItem();

        if(hasComponent) {

            stealComponentButton.SetActive(true);

        }
        else {
            stealComponentButton.SetActive(false);
        }

        //Loops through all the relevant items
        for (int itemIndex = 0; itemIndex < Player.MAX_ITEMS; itemIndex++)
        {

            Debug.Log(itemIndex);
            Debug.Log(ClientManager.instance.inventory.Count);
            //If the current button is one of the empty "slots" in the player's inventory, leaves it blank
            if(itemIndex >= ClientManager.instance.inventory.Count)
            {
                winnerDisplayText = "";
                winnerItemButtons[itemIndex].GetComponent<ItemButtonComponents>().item = new Item();
                winnerItemButtons[itemIndex].GetComponent<Image>().color = Color.white;
            }
            //Otherwise displays the relevant information
            else
            {
                winnerDisplayText = ClientManager.instance.inventory[itemIndex].ItemName;
                winnerItemButtons[itemIndex].GetComponent<ItemButtonComponents>().item = ClientManager.instance.inventory[itemIndex];
                if (ClientManager.instance.inventory[itemIndex].isEquipped)
                {
                    winnerItemButtons[itemIndex].GetComponent<Image>().color = Color.green;
                }
                else
                {
                    winnerItemButtons[itemIndex].GetComponent<Image>().color = Color.red;
                }
            }

            winnerItemButtons[itemIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = winnerDisplayText;

            //Statements below as above for winner, just applied to loser of combat
            if (itemIndex >= losersItems.Count)
            {
                loserDisplayText = "";
                loserItemButtons[itemIndex].GetComponent<ItemButtonComponents>().item = new Item();
                loserItemButtons[itemIndex].GetComponent<Image>().color = Color.white;
            }
            else
            {
                loserDisplayText = losersItems[itemIndex].ItemName;
                loserItemButtons[itemIndex].GetComponent<ItemButtonComponents>().item = losersItems[itemIndex];
                if (losersItems[itemIndex].isEquipped)
                {
                    loserItemButtons[itemIndex].GetComponent<Image>().color = Color.green;
                }
                else
                {
                    loserItemButtons[itemIndex].GetComponent<Image>().color = Color.red;
                }
            }

            loserItemButtons[itemIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = loserDisplayText;
        }
    }

    /// <summary>
    /// 
    /// Resets the selected item and all relevant variables
    /// 
    /// </summary>
    private void ResetSelectedItem()
    {
        selectedItemText.GetComponent<TextMeshProUGUI>().text = "";
        selectedItem = new Item();
        selectedID = -1;
        winnerItemActions.SetActive(false);
        loserItemActions.SetActive(false);
    }

    /// <summary>
    /// 
    /// Selects the item for the loser
    /// 
    /// </summary>
    /// <param name="buttonID">The ID of the button in the panel</param>
    public void SelectLoserItem(int buttonID)
    {
        Item buttonItem = loserItemButtons[buttonID].GetComponent<ItemButtonComponents>().item;

        //Checks if the selected button isn't an "empty" slot
        if(buttonItem.ItemType != Item.ItemTypes.Default)
        {
            //Checks there is a selected item, apply the item if there is. Also display the relevant actions for the player
            if(selectedItem.ItemType == Item.ItemTypes.Default)
            {
                selectedItem = buttonItem;
                selectedID = buttonID;
                selectedItemText.GetComponent<TextMeshProUGUI>().text = selectedItem.ItemName;
                SetErrorText("");
                loserItemActions.SetActive(true);
            }
            //Reseting it if it is not
            else
            {
                SetErrorText("Item Already Selected");
                ResetSelectedItem();
            }
        }
    }

    /// <summary>
    /// 
    /// Selects the item for the winner
    /// 
    /// </summary>
    /// <param name="buttonID">The ID of the button on the panel</param>
    public void SelectWinnerItem(int buttonID)
    {
        Item buttonItem = winnerItemButtons[buttonID].GetComponent<ItemButtonComponents>().item;

        if (buttonItem.ItemType != Item.ItemTypes.Default)
        {
            //Checks there is a selected item, apply the item if there is. Also display the relevant actions for the player
            if (selectedItem.ItemType == Item.ItemTypes.Default)
            {
                selectedItem = buttonItem;
                selectedID = buttonID;
                selectedItemText.GetComponent<TextMeshProUGUI>().text = selectedItem.ItemName;
                SetErrorText("");
                winnerItemActions.SetActive(true);

                //Changes the text of the equip action based on whether or not the item is equipped.
                if (selectedItem.isEquipped)
                {
                    equipButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Unequip Item";
                }
                else
                {
                    equipButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip Item";
                }
            }
            //Resets the selected item if it is not
            else
            {
                SetErrorText("Item Already Selected");
                ResetSelectedItem();
            }
        }
    }

    /// <summary>
    /// 
    /// Sets the error text to a relevant string
    /// 
    /// </summary>
    /// <param name="errorString">The string to set the text to</param>
    private void SetErrorText(string errorString)
    {
        errorText.GetComponent<TextMeshProUGUI>().text = errorString;
    }

    /// <summary>
    /// 
    /// Steals the item from the loser, transfering it to the winner
    /// 
    /// </summary>
    public void StealItem()
    {   
       Server.Instance.StealItem(selectedID, loserID, false);

    }

    /// <summary>
    /// 
    /// Force discards an item from the loser's inventory
    /// 
    /// </summary>
    public void StealDiscardItem()
    {
        Server.Instance.StealDiscardItem(selectedID, loserID);
       
    }

    /// <summary>
    /// 
    /// Equips or unequips the item from the player, displaying an error if they cannot
    /// 
    /// </summary>
    public void EquipItem()
    {
        Server.Instance.EquipItem(selectedID);
    }

    /// <summary>
    /// 
    /// Discards one of the winner's items
    /// 
    /// </summary>
    public void DiscardItem()
    {
        Server.Instance.DiscardItem(selectedID);
    }

    /// <summary>
    /// 
    /// Steal a component from the loser of the combat to give to the winner.
    /// 
    /// </summary>
    public void StealComponent() {
        ResetSelectedItem();
        if (ClientManager.instance.hasComponent) {
            SetErrorText("Already have a component");
        }
        else {
            ClientManager.instance.hasComponent = true;
            Server.Instance.StealItem(0, loserID, true);

        }
    }

}
