using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryManager : MonoBehaviour
{
    public GameObject playerCards;

    public GameObject playerText;
    public GameObject itemText;
    public GameObject errorText;

    public List<GameObject> itemButtons;

    public GameObject discardButton;
    public GameObject equipButton;

    private Item selectedItem;
    private int selectedID;
    public Player selectedPlayer;

    /// <summary>
    /// 
    /// Sets up the inventory panel to display the relevant information
    /// 
    /// </summary>
    public void StartInventoryPanel()
    {
        playerText.GetComponent<TextMeshProUGUI>().text = selectedPlayer.playerName;

        UpdateItemButtons();
    }

    /// <summary>
    /// 
    /// Update the inventory buttons to show the most up to date information about the player's inventory
    /// 
    /// </summary>
    private void UpdateItemButtons()
    {
        int counter = 0;
        //Display text in the string which will appear on the relevant button in the inventory
        string displayText;
        SetErrorText("");
        ResetSelectedItem();
        foreach (GameObject itemButton in itemButtons)
        {
            //Used to account for if there are less than the maximum number of items in a player's inventory
            //For those "empty" slots, sets up a blank button.
            if(counter >= selectedPlayer.items.Count)
            {
                displayText = "";
                itemButton.GetComponent<NSItemButtonComponents>().item = new Item();
                itemButton.GetComponent<Image>().color = Color.white;
            }
            else
            {
                //Presents the information about the current item for the player
                displayText = selectedPlayer.items[counter].ItemName;
                itemButton.GetComponent<NSItemButtonComponents>().item = selectedPlayer.items[counter];
                if (selectedPlayer.items[counter].isEquipped)
                {
                    itemButton.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    itemButton.GetComponent<Image>().color = Color.red;
                }
            }

            itemButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = displayText;

            counter++;
        }

        //Update the current players card with the new spec scores
        playerCards.GetComponent<NSPlayerCardManager>().UpdatePlayerCard(GameManager.instance.activePlayer);
    }

    /// <summary>
    /// 
    /// Used when the player selects one of the item buttons, assigning it to be acted upon by the player
    /// 
    /// </summary>
    /// <param name="buttonID">The ID of the button in the list of the item buttons</param>
    public void SelectItem(int buttonID)
    {
        Item buttonItem = itemButtons[buttonID].GetComponent<NSItemButtonComponents>().item;

        //Need to check if the selected item is one of the empty "slots" in the inventory
        if(buttonItem.ItemType != Item.ItemTypes.Default)
        {
            //If the selected item is "empty" assigns an item here
            if (selectedItem.ItemType == Item.ItemTypes.Default)
            {
                selectedItem = buttonItem;
                //selected ID marks the position in the players inventory as well as its position on the inventory board
                selectedID = buttonID;
                itemText.GetComponent<TextMeshProUGUI>().text = selectedItem.ItemName;
                discardButton.GetComponent<Button>().interactable = true;
                equipButton.GetComponent<Button>().interactable = true;
                SetErrorText("");

                //Used to tell the player if they are equipping or unequipping the selected item
                if (selectedItem.isEquipped)
                {
                    equipButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Unequip Item";
                }
                else
                {
                    equipButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip Item";
                }
            }
            //If an item was already selected, deselects it and informs the player
            else
            {
                SetErrorText("Item Already Selected");
                ResetSelectedItem();
            }
        }
    }

    /// <summary>
    /// 
    /// Equips or unequips the item for the player
    /// 
    /// </summary>
    public void EquipItem()
    {
        //Unequips the item if it is already equipped
        if (selectedItem.isEquipped)
        {
            selectedPlayer.UnequipItem(selectedID);
            UpdateItemButtons();
        }
        //Equips it otherwise
        else
        {
            //Attempts to equip the item for the player, returning the cause if it cannot
            Player.EquipErrors equipStatus = selectedPlayer.EquipItem(selectedID);

            switch (equipStatus)
            {
                case (Player.EquipErrors.Default):
                    UpdateItemButtons();
                    break;
                case (Player.EquipErrors.AlreadyEquipped):
                    SetErrorText("Item is already Equipped");
                    ResetSelectedItem();
                    break;
                case (Player.EquipErrors.TooManyEquipped):
                    SetErrorText("Too Many Items Equipped");
                    ResetSelectedItem();
                    break;
            }
        }
    }

    /// <summary>
    /// 
    /// Discards the item back to its original position in the map
    /// 
    /// </summary>
    public void DiscardItem()
    {
        selectedPlayer.DiscardItem(selectedID);
        UpdateItemButtons();
    }

    /// <summary>
    /// 
    /// Resets the selected item as well as all associated variables
    /// 
    /// </summary>
    private void ResetSelectedItem()
    {
        itemText.GetComponent<TextMeshProUGUI>().text = "";
        selectedItem = new Item();
        selectedID = -1;
        discardButton.GetComponent<Button>().interactable = false;
        equipButton.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// 
    /// Sets the error text to a relevant string
    /// 
    /// </summary>
    /// <param name="errorString">The string to set the error text to</param>
    public void SetErrorText(string errorString)
    {
        errorText.GetComponent<TextMeshProUGUI>().text = errorString;
    }
}
