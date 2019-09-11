using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServerBasicSurgeManager : MonoBehaviour
{

    public GameObject powerBar;
    public GameObject baseIncrease;
    public GameObject playerIncrease;
    public GameObject choiceIncrease;
    public GameObject totalIncrease;
    public GameObject confirmButton;


    private void Start() {
       
    }

    public void UpdateSurgeValues()
    {
        powerBar.GetComponent<AIPowerBar>().UpdateAIPower();
        baseIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.basePower.ToString());
        playerIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.playerPower.ToString());
        choiceIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.aiPowerChange.ToString());
        totalIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", (GameManager.instance.basePower + GameManager.instance.playerPower + GameManager.instance.aiPowerChange).ToString());
    }

    public void EndBasicSurge()
    {
        //I believe this should work but might be a better way of doing it
        //Would ideally be handled using the clients confirming they are all finished then
        //having the server update from there.
        Server.Instance.NewPhase(0, 0, 0, new NewPhase());
    }
}
