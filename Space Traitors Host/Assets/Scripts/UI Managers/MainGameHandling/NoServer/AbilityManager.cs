using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    public List<GameObject> abilityButtons;
    public GameObject selectedText;
    public GameObject selectButton;

    private Ability selectedAbility;

    private void Start()
    {
        selectedAbility = new Ability();
    }

    public void SetupAbilities()
    {
        selectedText.GetComponent<TextMeshProUGUI>().text = "";
        selectButton.GetComponent<Button>().interactable = false;

        int counter = 0;

        foreach (GameObject abilityButton in abilityButtons)
        {
            Ability currentAbility = GameManager.instance.GetActivePlayer().GetAbility(counter);
            abilityButton.GetComponent<Button>().interactable = true;

            abilityButton.GetComponent<AbilityButtonComponents>().abilityNameText.GetComponent<TextMeshProUGUI>().text = currentAbility.AbilityName;
            abilityButton.GetComponent<AbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().text = string.Format("{0}%", currentAbility.corruptionRequirement.ToString());
            abilityButton.GetComponent<AbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().text = currentAbility.scrapCost.ToString();

            if (currentAbility.CheckScrap())
            {
                abilityButton.GetComponent<AbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                abilityButton.GetComponent<Button>().interactable = false;
                abilityButton.GetComponent<AbilityButtonComponents>().scrapText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            if (currentAbility.CheckCorruption())
            {                
                abilityButton.GetComponent<AbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                abilityButton.GetComponent<Button>().interactable = false;
                abilityButton.GetComponent<AbilityButtonComponents>().corruptionText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            counter++;
        }
    }

    public void SelectAbility(int buttonID)
    {
        selectedAbility = GameManager.instance.GetActivePlayer().GetAbility(buttonID);

        selectedText.GetComponent<TextMeshProUGUI>().text = selectedAbility.AbilityName;
        selectButton.GetComponent<Button>().interactable = true;
    }

    public void ActivateAbility()
    {

    }
}
