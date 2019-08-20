using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerCardManager : MonoBehaviour
{
    public GameObject activePlayerPanel;

    public GameObject playerCardPrefab;
    private List<GameObject> playerCards;

    public List<Sprite> playerPortraits;

    public void InitialisePlayerCards()
    {
  
        
        for(int playerIndex = 0; playerIndex < GameManager.instance.MAX_PLAYERS; playerIndex++)
        {
            GameObject playercard = Instantiate(playerCardPrefab);
            playercard.transform.parent = GameObject.Find("Cards").transform;
            playerCards.Add(playercard);

        }
        
        
        //Update the player cards to start the game
        for (int playerIndex = 0; playerIndex < GameManager.instance.MAX_PLAYERS; playerIndex++)
        {
            if (playerIndex < GameManager.instance.numPlayers)
            {
                UpdatePlayerCard(playerIndex);
            }

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

        playerCards[playerIndex].GetComponent<PlayerCardComponents>().nameText.GetComponent<TextMeshProUGUI>().text = player.playerName;
        playerCards[playerIndex].GetComponent<PlayerCardComponents>().characterText.GetComponent<TextMeshProUGUI>().text = player.Character.CharacterName;
        playerCards[playerIndex].GetComponent<PlayerCardComponents>().characterPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(player.Character.CharacterType);
        playerCards[playerIndex].GetComponent<PlayerCardComponents>().scrapText.GetComponent<TextMeshProUGUI>().text = player.scrap.ToString();
        playerCards[playerIndex].GetComponent<PlayerCardComponents>().componentMarker.SetActive(player.hasComponent);

        string lifePointsString = string.Format("{0} / {1}", player.lifePoints, player.maxLifePoints);
        playerCards[playerIndex].GetComponent<PlayerCardComponents>().lifePointsText.GetComponent<TextMeshProUGUI>().text = lifePointsString;
    }

    public void UpdateAllCards()
    {
        for (int playerIndex = 0; playerIndex < GameManager.instance.numPlayers; playerIndex++)
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

}
