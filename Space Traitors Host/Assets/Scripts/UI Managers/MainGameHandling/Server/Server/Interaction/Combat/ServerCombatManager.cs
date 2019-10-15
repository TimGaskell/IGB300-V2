using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServerCombatManager : MonoBehaviour
{
    public GameObject attackerName;
    public GameObject attackerSpecText;
    public GameObject attackerPortrait;

    public GameObject defenderName;
    public GameObject defenderSpecText;
    public GameObject defenderPortrait;

    public GameObject successChanceText;
    public GameObject winnerText;


    public void InitCombatPanel(int attackerID, int defenderID)
    {
        attackerSpecText.GetComponent<TextMeshProUGUI>().text = "";
        defenderSpecText.GetComponent<TextMeshProUGUI>().text = "";
        successChanceText.GetComponent<TextMeshProUGUI>().text = "";
        winnerText.GetComponent<TextMeshProUGUI>().text = "";

        Player attacker = GameManager.instance.GetPlayer(attackerID);
        Player defender = GameManager.instance.GetPlayer(defenderID);

        attackerName.GetComponent<TextMeshProUGUI>().text = attacker.playerName;
        defenderName.GetComponent<TextMeshProUGUI>().text = defender.playerName;
        attackerPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(attacker.Character.CharacterType);
        defenderPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(defender.Character.CharacterType);
    }

    public void UpdateCombatPanel(GameManager.SpecScores attackerSpec, GameManager.SpecScores defenderSpec, int successChance, int winnerID)
    {
        attackerSpecText.GetComponent<TextMeshProUGUI>().text = attackerSpec.ToString();
        defenderSpecText.GetComponent<TextMeshProUGUI>().text = defenderSpec.ToString();
        successChanceText.GetComponent<TextMeshProUGUI>().text = successChance.ToString();
        winnerText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetPlayer(winnerID).playerName;
    }
}
