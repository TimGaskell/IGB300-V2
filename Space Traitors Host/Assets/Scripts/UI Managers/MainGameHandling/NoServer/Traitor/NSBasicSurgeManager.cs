<<<<<<< HEAD:Space Traitors Host/Assets/Scripts/UI Managers/MainGameHandling/NoServer/Traitor/NSBasicSurgeManager.cs
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NSBasicSurgeManager : MonoBehaviour
{
    public GameObject powerBar;
    public GameObject powerCounter;
    public GameObject baseIncrease;
    public GameObject playerIncrease;
    public GameObject choiceIncrease;
    public GameObject totalIncrease;

    public void UpdateSurgeValues()
    {
        powerBar.GetComponent<Slider>().value = GameManager.instance.AIPower;
        powerCounter.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.AIPower.ToString());
        baseIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.basePower.ToString());
        playerIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.playerPower.ToString());
        choiceIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.aiPowerChange.ToString());
        totalIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", (GameManager.instance.basePower + GameManager.instance.playerPower + GameManager.instance.aiPowerChange).ToString());
    }
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BasicSurgeManager : MonoBehaviour
{
    public GameObject powerBar;
    public GameObject powerCounter;
    public GameObject baseIncrease;
    public GameObject playerIncrease;
    public GameObject choiceIncrease;
    public GameObject totalIncrease;

    public void UpdateSurgeValues()
    {
        powerBar.GetComponent<Slider>().value = GameManager.instance.AIPower;
        powerCounter.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.AIPower.ToString());
        baseIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.basePower.ToString());
        playerIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.playerPower.ToString());
        choiceIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.aiPowerChange.ToString());
        totalIncrease.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", (GameManager.instance.basePower + GameManager.instance.playerPower + GameManager.instance.aiPowerChange).ToString());
    }
}
>>>>>>> Lachlan's-Branch:Space Traitors Host/Assets/Scripts/UI Managers/MainGameHandling/BasicSurgeManager.cs
