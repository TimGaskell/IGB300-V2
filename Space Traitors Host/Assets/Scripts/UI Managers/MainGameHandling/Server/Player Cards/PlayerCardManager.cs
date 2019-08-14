﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerCardManager : MonoBehaviour
{
    public GameObject activePlayerPanel;

    public List<GameObject> playerPanels;

    public List<Sprite> playerPortraits;

    public void InitialisePlayerCards()
    {
        //Update the player cards to start the game
        for (int playerIndex = 0; playerIndex < GameManager.instance.MAX_PLAYERS; playerIndex++)
        {
            if (playerIndex < GameManager.instance.numPlayers)
            {
                UpdatePlayerCard(playerIndex);
                UpdateInventoryButton(playerIndex, false);
            }
            else
            {
                //Disable player cards for players which arent playing
                playerPanels[playerIndex].SetActive(false);
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

        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().nameText.GetComponent<TextMeshProUGUI>().text = player.playerName;
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().characterText.GetComponent<TextMeshProUGUI>().text = player.Character.CharacterName;
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().characterPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(player.Character.CharacterType);
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().scrapText.GetComponent<TextMeshProUGUI>().text = player.scrap.ToString();
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().corruptionText.GetComponent<TextMeshProUGUI>().text = player.Corruption.ToString();
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().traitorMarker.SetActive(player.isTraitor);
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().componentMarker.SetActive(player.hasComponent);

        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().specCounters[0].GetComponent<TextMeshProUGUI>().text =
            ObtainSpecInfo(player.ScaledBrawn, player.ModBrawn);
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().specCounters[1].GetComponent<TextMeshProUGUI>().text =
            ObtainSpecInfo(player.ScaledSkill, player.ModSkill);
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().specCounters[2].GetComponent<TextMeshProUGUI>().text =
            ObtainSpecInfo(player.ScaledTech, player.ModTech);
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().specCounters[3].GetComponent<TextMeshProUGUI>().text =
            ObtainSpecInfo(player.ScaledCharm, player.ModCharm);

        string lifePointsString = string.Format("{0} / {1}", player.lifePoints, player.maxLifePoints);
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().lifePointsText.GetComponent<TextMeshProUGUI>().text = lifePointsString;
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


    public void UpdateInventoryButton(int playerIndex, bool enable)
    {
        playerPanels[playerIndex].GetComponent<NSPlayerCardComponents>().inventoryButton.GetComponent<Button>().interactable = enable;
    }
}
