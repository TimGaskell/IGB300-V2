using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActiveAbilityDisplay : MonoBehaviour
{
    public GameObject activeAbilityText;

    public void UpdateActiveText(Ability activeAbility)
    {
        string activeText = string.Format("{0} has activated {1}", GameManager.instance.GetActivePlayer().playerName, activeAbility.AbilityName);

        activeAbilityText.GetComponent<TextMeshProUGUI>().text = activeText;
    }
}
