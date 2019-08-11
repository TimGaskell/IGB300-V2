using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class NSInventoryManager : MonoBehaviour
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

    public void StartInventoryPanel()
    {
        playerText.GetComponent<TextMeshProUGUI>().text = selectedPlayer.playerName;

        UpdateItemButtons();
    }

    private void UpdateItemButtons()
    {
        int counter = 0;
        string displayText;
        SetErrorText("");
        ResetSelectedItem();
        foreach (GameObject itemButton in itemButtons)
        {
            if(counter >= selectedPlayer.items.Count)
            {
                displayText = "";
                itemButton.GetComponent<NSItemButtonComponents>().item = new Item();
                itemButton.GetComponent<Image>().color = Color.white;
            }
            else
            {
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

        playerCards.GetComponent<NSPlayerCardManager>().UpdatePlayerCard(GameManager.instance.activePlayer);
    }

    public void SelectItem(int buttonID)
    {
        Item buttonItem = itemButtons[buttonID].GetComponent<NSItemButtonComponents>().item;

        if(buttonItem.ItemType != Item.ItemTypes.Default)
        {
            if (selectedItem.ItemType == Item.ItemTypes.Default)
            {
                selectedItem = buttonItem;
                selectedID = buttonID;
                itemText.GetComponent<TextMeshProUGUI>().text = selectedItem.ItemName;
                discardButton.GetComponent<Button>().interactable = true;
                equipButton.GetComponent<Button>().interactable = true;
                SetErrorText("");

                if (selectedItem.isEquipped)
                {
                    equipButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Unequip Item";
                }
                else
                {
                    equipButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip Item";
                }
            }
            else
            {
                SetErrorText("Item Already Selected");
                ResetSelectedItem();
            }
        }
    }

    public void EquipItem()
    {
        if (selectedItem.isEquipped)
        {
            selectedPlayer.UnequipItem(selectedID);
            UpdateItemButtons();
        }
        else
        {
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

    public void DiscardItem()
    {
        selectedPlayer.DiscardItem(selectedID);
        UpdateItemButtons();
    }

    private void ResetSelectedItem()
    {
        itemText.GetComponent<TextMeshProUGUI>().text = "";
        selectedItem = new Item();
        selectedID = -1;
        discardButton.GetComponent<Button>().interactable = false;
        equipButton.GetComponent<Button>().interactable = false;
    }


    public void SetErrorText(string errorString)
    {
        errorText.GetComponent<TextMeshProUGUI>().text = errorString;
    }
}
