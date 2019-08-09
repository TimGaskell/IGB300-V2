using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class NSInventoryManager : MonoBehaviour
{
    public GameObject playerText;
    public GameObject itemText;

    public List<GameObject> equippedButtons;
    public List<GameObject> storageButtons;

    private List<GameObject> itemButtons;

    public GameObject discardButton;

    private Item selectedItem;
    public Player selectedPlayer;

    private enum ItemTypes { Default, Storage, Equipped };
    private ItemTypes selectedItemType;

    public void StartInventoryPanel()
    {
        selectedItemType = ItemTypes.Default;

        playerText.GetComponent<TextMeshProUGUI>().text = selectedPlayer.playerName;
        ResetSelectedItem();

        discardButton.GetComponent<Button>().interactable = false;

        UpdateInventoryButtons();
    }
    
    private void UpdateInventoryButtons()
    {
        List<Item> equippedItems = selectedPlayer.GetItems(true);
        List<Item> storedItems = selectedPlayer.GetItems(false);

        DisplayItemNames(equippedButtons, equippedItems);
        DisplayItemNames(storageButtons, storedItems);


    }

    private void DisplayItemNames(List<GameObject> buttons, List<Item> items)
    {
        int counter = 0;
        string displayText;
        foreach (GameObject button in buttons)
        {
            if(counter >= items.Count)
            {
                displayText = "";
                button.GetComponent<NSItemButtonComponents>().item = new Item();
            }
            else
            {
                displayText = items[counter].ItemName;
                button.GetComponent<NSItemButtonComponents>().item = items[counter];
            }

            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = displayText;

            counter++;
        }
    }

    public void SelectItemType(string ItemType)
    {
        selectedItemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), ItemType);
    }

    public void SelectItem(int buttonID)
    {
        if(selectedItem.ItemType == Item.ItemTypes.Default)
        {
            switch (selectedItemType)
            {
                case (ItemTypes.Equipped):
                    selectedItem = equippedButtons[buttonID].GetComponent<NSItemButtonComponents>().item;
                    break;
                case (ItemTypes.Storage):
                    selectedItem = storageButtons[buttonID].GetComponent<NSItemButtonComponents>().item;
                    break;
                default:
                    throw new NotImplementedException("Not a valid Item Type");
            }

            if(selectedItem.ItemType != Item.ItemTypes.Default)
            {
                itemText.GetComponent<TextMeshProUGUI>().text = selectedItem.ItemName;
            }
        }
    }

    public void DiscardItem()
    {

    }

    private void ResetSelectedItem()
    {
        itemText.GetComponent<TextMeshProUGUI>().text = "";
        selectedItem = new Item();
    }
}
