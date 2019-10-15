﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClientBasicSurgeManager : MonoBehaviour
{
    public static ClientBasicSurgeManager instance = null;

    public GameObject powerBar;
    public GameObject baseIncrease;
    public GameObject playerIncrease;
    public GameObject choiceIncrease;
    public GameObject totalIncrease;
    public GameObject confirmButton;
    public GameObject objectiveText;

    public float power;
    public float basepower;
    public float playerpower;
    public float powerchange;
    public float totalIncreaseUnit;
    public float choiceIncreaseUnit;

    private void Start() {
        instance = this;
    }

    public void UpdateSurgeValues()
    {
        powerBar.GetComponent<AIPowerBar>().UpdateAIPower(power);
        //baseIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", basepower.ToString());
        //playerIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", playerpower.ToString());
        //choiceIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", powerchange.ToString());
        //totalIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", totalIncreaseUnit.ToString());

        string objectiveString;

        if (ClientManager.instance.isTraitor)
        {
            objectiveString = "<color=\"red\">You are a traitor. <color=\"white\">Eliminate all other players.";
        }
        else
        {
            objectiveString = "<color=\"green\">You are not a traitor. <color=\"white\">Find the components and return them to the shuttle.";
        }

        objectiveText.GetComponent<TextMeshProUGUI>().text = objectiveString;
    }

    public void EndBasicSurge()
    {
        GameManager.instance.currentPhase = GameManager.TurnPhases.Default;
        ClientUIManager.instance.DisplayCurrentPhase();
        Server.Instance.SendEndRound();
    }
}
