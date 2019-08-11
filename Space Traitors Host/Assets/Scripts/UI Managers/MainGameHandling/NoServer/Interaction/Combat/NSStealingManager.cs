using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NSStealingManager : MonoBehaviour
{
    public GameObject playerCards;

    public Player winner;
    public Player loser;

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

    public void StartStealPanel()
    {
        winnerText.GetComponent<TextMeshProUGUI>().text = winner.playerName;
        loserText.GetComponent<TextMeshProUGUI>().text = loser.playerName;

        UpdateItemButtons();
        loserItemParent.GetComponent<CanvasGroup>().interactable = true;
    }

    private void UpdateItemButtons()
    {
        string winnerDisplayText;
        string loserDisplayText;

        SetErrorText("");
        ResetSelectedItem();

        for (int itemIndex = 0; itemIndex < Player.MAX_ITEMS; itemIndex++)
        {
            if(itemIndex >= winner.items.Count)
            {
                winnerDisplayText = "";
                winnerItemButtons[itemIndex].GetComponent<NSItemButtonComponents>().item = new Item();
                winnerItemButtons[itemIndex].GetComponent<Image>().color = Color.white;
            }
            else
            {
                winnerDisplayText = winner.items[itemIndex].ItemName;
                winnerItemButtons[itemIndex].GetComponent<NSItemButtonComponents>().item = winner.items[itemIndex];
                if (winner.items[itemIndex].isEquipped)
                {
                    winnerItemButtons[itemIndex].GetComponent<Image>().color = Color.green;
                }
                else
                {
                    winnerItemButtons[itemIndex].GetComponent<Image>().color = Color.red;
                }
            }

            winnerItemButtons[itemIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = winnerDisplayText;


            if (itemIndex >= loser.items.Count)
            {
                loserDisplayText = "";
                loserItemButtons[itemIndex].GetComponent<NSItemButtonComponents>().item = new Item();
                loserItemButtons[itemIndex].GetComponent<Image>().color = Color.white;
            }
            else
            {
                loserDisplayText = loser.items[itemIndex].ItemName;
                loserItemButtons[itemIndex].GetComponent<NSItemButtonComponents>().item = loser.items[itemIndex];
                if (loser.items[itemIndex].isEquipped)
                {
                    loserItemButtons[itemIndex].GetComponent<Image>().color = Color.green;
                }
                else
                {
                    loserItemButtons[itemIndex].GetComponent<Image>().color = Color.red;
                }
            }

            loserItemButtons[itemIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = loserDisplayText;

            playerCards.GetComponent<NSPlayerCardManager>().UpdateAllCards();
        }
    }

    private void ResetSelectedItem()
    {
        selectedItemText.GetComponent<TextMeshProUGUI>().text = "";
        selectedItem = new Item();
        selectedID = -1;
        winnerItemActions.SetActive(false);
        loserItemActions.SetActive(false);
    }

    public void SelectLoserItem(int buttonID)
    {
        Item buttonItem = loserItemButtons[buttonID].GetComponent<NSItemButtonComponents>().item;

        if(buttonItem.ItemType != Item.ItemTypes.Default)
        {
            if(selectedItem.ItemType == Item.ItemTypes.Default)
            {
                selectedItem = buttonItem;
                selectedID = buttonID;
                selectedItemText.GetComponent<TextMeshProUGUI>().text = selectedItem.ItemName;
                SetErrorText("");
                loserItemActions.SetActive(true);
            }
            else
            {
                SetErrorText("Item Already Selected");
                ResetSelectedItem();
            }
        }
    }

    public void SelectWinnerItem(int buttonID)
    {
        Item buttonItem = winnerItemButtons[buttonID].GetComponent<NSItemButtonComponents>().item;

        if (buttonItem.ItemType != Item.ItemTypes.Default)
        {
            if (selectedItem.ItemType == Item.ItemTypes.Default)
            {
                selectedItem = buttonItem;
                selectedID = buttonID;
                selectedItemText.GetComponent<TextMeshProUGUI>().text = selectedItem.ItemName;
                SetErrorText("");
                winnerItemActions.SetActive(true);

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

    private void SetErrorText(string errorString)
    {
        errorText.GetComponent<TextMeshProUGUI>().text = errorString;
    }

    public void StealItem()
    {
        if (winner.GiveItem(selectedItem))
        {
            loser.RemoveItem(selectedID);
            loserItemParent.GetComponent<CanvasGroup>().interactable = false;
            UpdateItemButtons();
        }
        else
        {
            SetErrorText("Too many items");
        }
    }

    public void StealDiscardItem()
    {
        loser.DiscardItem(selectedID);
        loserItemParent.GetComponent<CanvasGroup>().interactable = false;
        UpdateItemButtons();
    }

    public void EquipItem()
    {
        if (selectedItem.isEquipped)
        {
            winner.UnequipItem(selectedID);
            UpdateItemButtons();
        }
        else
        {
            Player.EquipErrors equipStatus = winner.EquipItem(selectedID);

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
        winner.DiscardItem(selectedID);
        UpdateItemButtons();
    }
}
