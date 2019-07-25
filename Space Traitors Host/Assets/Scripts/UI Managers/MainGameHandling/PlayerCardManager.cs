using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCardManager : MonoBehaviour
{
    public GameObject activePlayerPanel;

    public List<GameObject> playerPanels;

    private void Start()
    {
        for (int playerIndex = 0; playerIndex < GameManager.instance.MAX_PLAYERS; playerIndex++)
        {
            if (playerIndex < GameManager.instance.numPlayers)
            {
                UpdatePlayerCard(playerIndex);
            }
            else
            {
                playerPanels[playerIndex].SetActive(false);
            }
        }
    }

    public void UpdateActivePlayer()
    {
        activePlayerPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetActivePlayer().playerName;
    }

    public void UpdatePlayerCard(int playerIndex)
    {
        Player player = GameManager.instance.GetOrderedPlayer(playerIndex);

        playerPanels[playerIndex].GetComponent<PlayerCardComponents>().nameText.GetComponent<TextMeshProUGUI>().text = player.playerName;
        playerPanels[playerIndex].GetComponent<PlayerCardComponents>().characterText.GetComponent<TextMeshProUGUI>().text = player.Character.CharacterName;
        playerPanels[playerIndex].GetComponent<PlayerCardComponents>().scrapText.GetComponent<TextMeshProUGUI>().text = player.scrap.ToString();
        playerPanels[playerIndex].GetComponent<PlayerCardComponents>().corruptionText.GetComponent<TextMeshProUGUI>().text = player.corruption.ToString();

        string lifePointsString = string.Format("{0} / {1}", player.lifePoints, player.maxLifePoints);
        playerPanels[playerIndex].GetComponent<PlayerCardComponents>().lifePointsText.GetComponent<TextMeshProUGUI>().text = lifePointsString;
    }
}
