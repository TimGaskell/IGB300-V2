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

    public GameObject winText;
    public GameObject confirmButton;

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

        winText.GetComponent<TextMeshProUGUI>().text = "";
        confirmButton.GetComponent<Button>().interactable = false;
    }

    public void SetWinText(bool hasWon)
    {
        if (hasWon)
        {
            winText.GetComponent<TextMeshProUGUI>().text = "You Won!";
        }
        else
        {
            winText.GetComponent<TextMeshProUGUI>().text = "You Lost";
        }
    }

    public void EndAttackSurge()
    {
        //I believe this should work but might be a better way of doing it
        //Would ideally be handled using the clients confirming they are all finished then
        //having the server update from there.
        Server.Instance.NewPhase(0, 0, 0, new NewPhase());
    }
}
