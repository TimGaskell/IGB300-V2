using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AttackSurgeManager : MonoBehaviour
{
    public GameObject playerPortrait;
    public GameObject playerName;

    public GameObject chanceText;
    public GameObject confirmButton;

    public GameManager.SpecScores selectedSpec;

    private Player targetPlayer;

    /// <summary>
    /// 
    /// Update the AI Attack panel for the new target
    /// 
    /// </summary>
    public void UpdateTarget()
    {
        targetPlayer = GameManager.instance.GetPlayer(GameManager.instance.targetPlayer);

        playerPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(targetPlayer.Character.CharacterType);
        playerName.GetComponent<TextMeshProUGUI>().text = targetPlayer.playerName;

        chanceText.GetComponent<TextMeshProUGUI>().text = "";
        confirmButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Confirm";
        confirmButton.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// 
    /// Select a relevant spec score for the player to use for an AI Attack and update the display to inform the player
    /// 
    /// </summary>
    /// <param name="specScore"></param>
    public void SelectSpec(string specScore)
    {
        selectedSpec = (GameManager.SpecScores)Enum.Parse(typeof(GameManager.SpecScores), specScore);

        confirmButton.GetComponent<Button>().interactable = true;
        confirmButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("Confirm: {0}", specScore);
        chanceText.GetComponent<TextMeshProUGUI>().text = string.Format("{0}%",
            GameManager.SpecChallengeChance(targetPlayer.GetScaledSpecScore(selectedSpec), 
            GameManager.instance.aiTargetScore).ToString());
    }
}
