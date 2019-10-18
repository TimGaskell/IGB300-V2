using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerCardManager : MonoBehaviour
{
    public GameObject activePlayerPanel;

    public static PlayerCardManager instance = null;

    public GameObject initialPlayerCard;
    public List<GameObject> playerCards;

    private int activePlayerCounter;


    private void Start() {

        instance = this;
        activePlayerCounter = -1;
    }

    public void InitialisePlayerCards()
    {
        playerCards.Add(initialPlayerCard);
        UpdatePlayerCard(1);

        for(int playerIndex = 2; playerIndex < GameManager.instance.numPlayers + 1; playerIndex++)
        {
            GameObject playercard = Instantiate(initialPlayerCard, initialPlayerCard.transform.parent);
            playerCards.Add(playercard);
            UpdatePlayerCard(playerIndex);

        }
    }

    /// <summary>
    /// 
    /// Update the active player text to display the current active player
    /// 
    /// </summary>
    public void UpdateActivePlayer()
    {
        activePlayerPanel.SetActive(true);
        activePlayerPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetActivePlayer().playerName;
        if(activePlayerCounter != GameManager.instance.numPlayers)
        {
            activePlayerCounter++;
        }
        else
        {
            activePlayerCounter = 0;
        }
        Debug.Log(activePlayerCounter);
        ActivePlayerColourSwitch();
    }

    /// <summary>
    /// 
    /// Update a particular player card using the reference of the ordered player index
    /// 
    /// </summary>
    /// <param name="playerIndex">The ordered player index</param>
    public void UpdatePlayerCard(int playerIndex)
    {
        Player player = GameManager.instance.GetOrderedPlayer(playerIndex);

        playerCards[playerIndex-1].GetComponent<PlayerCardComponents>().nameText.GetComponent<TextMeshProUGUI>().text = player.playerName;
        playerCards[playerIndex-1].GetComponent<PlayerCardComponents>().characterPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(player.Character.CharacterType);
        playerCards[playerIndex-1].GetComponent<PlayerCardComponents>().scrapText.GetComponent<TextMeshProUGUI>().text = player.scrap.ToString();
        playerCards[playerIndex-1].GetComponent<PlayerCardComponents>().componentMarker.SetActive(player.hasComponent);

        string lifePointsString = string.Format("{0} / {1}", player.lifePoints, player.maxLifePoints);
        playerCards[playerIndex-1].GetComponent<PlayerCardComponents>().lifePointsText.GetComponent<TextMeshProUGUI>().text = lifePointsString;
    }

    public void UpdateAllCards()
    {
        for (int playerIndex = 1; playerIndex < GameManager.instance.numPlayers + 1; playerIndex++)
        {
            UpdatePlayerCard(playerIndex);
        }
    }

    /// <summary>
    /// 
    /// Return a string which displays the characters scaled spec score as well as their spec score which 
    /// has not been scaled because of corruption
    /// 
    /// </summary>
    /// <param name="scaledSpec">The scaled spec score</param>
    /// <param name="modSpec">The unscaled spce score, but with base modifiers</param>
    /// <returns>The string to display</returns>
    private string ObtainSpecInfo(float scaledSpec, int modSpec)
    {
        return string.Format("{0} / {1}", scaledSpec, modSpec);
    }

    private void ActivePlayerColourSwitch()
    {
        for (int i = 0; i < playerCards.Count; i++)
        {
            Color backColour;

            if (i == activePlayerCounter)
            {
                 backColour = Color.green;
            }
            else
            {
                backColour = Color.white;
            }

            playerCards[i].GetComponent<Image>().color = backColour;
        }
    }

}
