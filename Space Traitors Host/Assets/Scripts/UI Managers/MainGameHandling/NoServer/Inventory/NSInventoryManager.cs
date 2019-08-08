using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NSInventoryManager : MonoBehaviour
{
    public GameObject playerText;
    public GameObject itemText;

    public List<GameObject> equippedButtons;
    public List<GameObject> storedButtons;

    public GameObject discardButton;

    private Item selectedItem;
    public Player selectedPlayer;

    public void StartInventoryPanel()
    {
        playerText.GetComponent<TextMeshProUGUI>().text = selectedPlayer.playerName;
        itemText.GetComponent<TextMeshProUGUI>().text = "";

        discardButton.GetComponent<Button>().interactable = false;

        UpdateInventoryButtons();
    }
    
    private void UpdateInventoryButtons()
    {
        List<Item> equippedItems = selectedPlayer.GetItems(true);
        List<Item> storedItems = selectedPlayer.GetItems(false);

        DisplayItemNames(equippedButtons, equippedItems);
        DisplayItemNames(storedButtons, storedItems);
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
            }
            else
            {
                displayText = items[counter].ItemName;
            }

            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = displayText;

            counter++;
        }
    }

    public void SelectItem(int buttonID)
    {

    }

    public void DiscardItem()
    {

    }
}
