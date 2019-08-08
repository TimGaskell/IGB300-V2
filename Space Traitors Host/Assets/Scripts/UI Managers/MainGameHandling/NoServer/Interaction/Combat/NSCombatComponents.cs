using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSCombatComponents : MonoBehaviour
{
    public GameObject attackerName;
    public GameObject attackerPortrait;
    public GameObject defenderName;
    public GameObject defenderPortrait;
    public GameObject attackerSpec;
    public GameObject defenderSpec;

    public List<GameObject> attackerSpecButtons;
    public List<GameObject> defenderSpecButtons;

    public GameObject winnerText;
    public GameObject continueButton;
}
