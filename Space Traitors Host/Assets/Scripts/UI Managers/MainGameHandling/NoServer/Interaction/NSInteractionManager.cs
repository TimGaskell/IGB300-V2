using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class NSInteractionManager : MonoBehaviour
{
    public GameObject standardChoiceUI;
    public GameObject escapeRoomUI;

    public GameObject playerCards;

    public GameObject choice0ButtonText;
    public GameObject choice1ButtonText;

    public GameObject choiceInfoPanel;

    public Room currentRoom;
    public int selectedChoiceID;

    private List<int> attackablePlayers;

    public GameObject componentPanelsParent;
    public GameObject componentPanelPrefab;
    private List<GameObject> componentPanels;
    public GameObject installButton;

    public GameObject combatButton;

    public GameObject targetPanel;
    public GameObject combatPanel;
    public GameObject stealPanel;

    public List<GameObject> targetButtons;
    public GameObject targetName;

    public GameObject targetButton;

    private int selectedTarget;

    public List<Sprite> characterPortraits;

    private GameManager.SpecScores attackerSpecScore;
    private GameManager.SpecScores defenderSpecScore;

    private enum ParticipantTypes { Attacker, Defender }

    /// <summary>
    /// 
    /// Update the UI to allow the player to select the choices relevant to a particular room.
    /// 
    /// </summary>
    /// <param name="roomIndex">The index of the room the player is in</param>
    public void InitialiseChoices(int roomIndex)
    {
        //Disable the combat panels
        targetPanel.SetActive(false);
        combatPanel.SetActive(false);
        stealPanel.SetActive(false);

        //Escape room is for installing components, so requies a different screen. The Escape Room ID is where the players start
        if (roomIndex == Player.STARTING_ROOM_ID)
        {
            escapeRoomUI.SetActive(true);
            standardChoiceUI.SetActive(false);

            int counter = 0;
            foreach (GameObject compPanel in componentPanels)
            {
                if (counter < GameManager.instance.installedComponents)
                {
                    compPanel.transform.GetChild(0).gameObject.SetActive(true);
                }
                counter++;
            }

            installButton.GetComponent<Button>().interactable = GameManager.instance.CanInstallComponent();
        }
        else
        {
            standardChoiceUI.SetActive(true);
            escapeRoomUI.SetActive(false);

            choiceInfoPanel.SetActive(false);

            currentRoom = GameManager.instance.GetRoom(roomIndex);

            //Update button text
            choice0ButtonText.GetComponent<TextMeshProUGUI>().text = currentRoom.roomChoices[0].choiceName;
            choice1ButtonText.GetComponent<TextMeshProUGUI>().text = currentRoom.roomChoices[1].choiceName;
        }

        //Checks is combat is available for the active player, enabling the attack button if it is
        attackablePlayers = GameManager.instance.CheckCombat();
        if (attackablePlayers.Count == 0)
        {
            combatButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            combatButton.GetComponent<Button>().interactable = true;
        }
    }

    #region Standard Choice
    /// <summary>
    /// 
    /// Update the choice information panel to suit the players selected choice to display
    /// 
    /// </summary>
    /// <param name="choiceID">The ID of the choice being displayed</param>
    public void DisplayChoice(int choiceID)
    {
        choiceInfoPanel.SetActive(true);
        selectedChoiceID = choiceID;
        Choice selectedChoice = currentRoom.roomChoices[selectedChoiceID];

        choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().choiceHeader.GetComponent<TextMeshProUGUI>().text = selectedChoice.choiceName;

        //If the choice is not a spec challenge, updates the display text to suit the choice.
        if (selectedChoice.specChallenge == GameManager.SpecScores.Default)
        {
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().nonSpecChoiceText.SetActive(true);
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().specChallengeGroup.SetActive(false);

            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().nonSpecChoiceText.GetComponent<TextMeshProUGUI>().text = selectedChoice.SuccessText();
        }
        //If the choice is a spec challenge, displays the success and failure state for the spec challenge choice
        else
        {
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().nonSpecChoiceText.SetActive(false);
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().specChallengeGroup.SetActive(true);

            //Find the players relevant spec score need for the choice
            float playerScore = GameManager.instance.GetActivePlayer().GetScaledSpecScore(selectedChoice.specChallenge);

            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().specScoreText.GetComponent<TextMeshProUGUI>().text =
                string.Format("Spec: {0}", selectedChoice.specChallenge.ToString());
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().specChanceText.GetComponent<TextMeshProUGUI>().text =
                string.Format("Chance: {0}%", Mathf.Round(GameManager.SpecChallengeChance(playerScore, selectedChoice.targetScore)).ToString());
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().specSuccessText.GetComponent<TextMeshProUGUI>().text = selectedChoice.SuccessText();
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().specFailureText.GetComponent<TextMeshProUGUI>().text = selectedChoice.FailText();
        }

        //Test if the choice is available to the player and displays the reason it cannot be selected if not.
        Choice.IsAvailableTypes isAvailable = selectedChoice.IsAvailable();

        if (isAvailable == Choice.IsAvailableTypes.available)
        {
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().errorText.SetActive(false);
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().choiceSelectButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().errorText.SetActive(true);
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().errorText.GetComponent<TextMeshProUGUI>().text = selectedChoice.ConvertErrorText(isAvailable);
            choiceInfoPanel.GetComponent<NSChoiceInfoComponents>().choiceSelectButton.GetComponent<Button>().interactable = false;
        }
    }

    #endregion

    #region Escape Room Installation

    public void InitComponentPanel()
    {
        //Initialise the component panels on the interaction UI
        componentPanels = new List<GameObject>();

        for (int componentIndex = 0; componentIndex < GameManager.instance.NumComponents; componentIndex++)
        {
            componentPanels.Add(Instantiate(componentPanelPrefab, componentPanelsParent.transform));
        }
    }

    #endregion

    #region Combat Handling

    /// <summary>
    /// 
    /// Setup the target panel to allow the player to select a valid target
    /// 
    /// </summary>
    public void DisplayTargets()
    {
        targetPanel.SetActive(true);

        //Sets the target to be default values.
        targetName.GetComponent<TextMeshProUGUI>().text = "";
        selectedTarget = GameManager.DEFAULT_PLAYER_ID;
        targetButton.GetComponent<Button>().interactable = false;

        //Loops through each of the target buttons and sets them to be non-interactable if the character is not a valid target
        foreach (GameObject targetButton in targetButtons)
        {
            if (!attackablePlayers.Exists(x => x == targetButton.GetComponent<NSTargetProperties>().playerID))
            {
                targetButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                targetButton.GetComponent<Button>().interactable = true;
            }
        }
    }

    /// <summary>
    /// 
    /// Sets the selected target to be attacked
    /// 
    /// </summary>
    /// <param name="buttonID">The ID of the button. Used to get the associated player ID from the image</param>
    public void SelectTarget(int buttonID)
    {
        //Gets the relevant player information
        Player targetPlayer = GameManager.instance.GetPlayer(targetButtons[buttonID].GetComponent<NSTargetProperties>().playerID);

        selectedTarget = targetPlayer.playerID;
        targetName.GetComponent<TextMeshProUGUI>().text = targetPlayer.playerName;

        //Allow the player to confirm their target for the attack
        targetButton.GetComponent<Button>().interactable = true;
    }

    /// <summary>
    /// 
    /// Confirms the selected target and opens the combat screen
    /// 
    /// </summary>
    public void ConfirmTarget()
    {
        //Gets the relevant player information. The attacker will always be the active player and the defending will be the selected target
        //from the target panel.
        Player attackingPlayer = GameManager.instance.GetActivePlayer();
        Player defendingPlayer = GameManager.instance.GetPlayer(selectedTarget);

        combatPanel.SetActive(true);

        //Sets the default screen status for the combats
        combatPanel.GetComponent<NSCombatComponents>().attackerName.GetComponent<TextMeshProUGUI>().text = attackingPlayer.playerName;
        combatPanel.GetComponent<NSCombatComponents>().defenderName.GetComponent<TextMeshProUGUI>().text = defendingPlayer.playerName;
        combatPanel.GetComponent<NSCombatComponents>().attackerPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(attackingPlayer.Character.CharacterType);
        combatPanel.GetComponent<NSCombatComponents>().defenderPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(defendingPlayer.Character.CharacterType);
        combatPanel.GetComponent<NSCombatComponents>().attackerSpec.GetComponent<TextMeshProUGUI>().text = "";
        combatPanel.GetComponent<NSCombatComponents>().defenderSpec.GetComponent<TextMeshProUGUI>().text = "";
        combatPanel.GetComponent<NSCombatComponents>().winnerText.GetComponent<TextMeshProUGUI>().text = "";
        combatPanel.GetComponent<NSCombatComponents>().continueButton.GetComponent<Button>().interactable = false;

        //Reenables the spec score buttons
        for (int buttonID = 0; buttonID < 4; buttonID++)
        {
            combatPanel.GetComponent<NSCombatComponents>().attackerSpecButtons[buttonID].GetComponent<Button>().interactable = true;
            combatPanel.GetComponent<NSCombatComponents>().defenderSpecButtons[buttonID].GetComponent<Button>().interactable = true;
        }

        attackerSpecScore = GameManager.SpecScores.Default;
        defenderSpecScore = GameManager.SpecScores.Default;
    }

    /// <summary>
    /// 
    /// If one of the attacker spec scores is selected updates the display and store the spec score to use in the combat
    /// 
    /// </summary>
    /// <param name="specScore">The name of the spec score for the button</param>
    public void AttackerSpec(string specScore)
    {
        GameManager.SpecScores chosenSpec = (GameManager.SpecScores)Enum.Parse(typeof(GameManager.SpecScores), specScore);

        combatPanel.GetComponent<NSCombatComponents>().attackerSpec.GetComponent<TextMeshProUGUI>().text = specScore;
        attackerSpecScore = chosenSpec;

        //Disables the buttons to prevent the value from being changed
        foreach (GameObject specButton in combatPanel.GetComponent<NSCombatComponents>().attackerSpecButtons)
        {
            specButton.GetComponent<Button>().interactable = false;
        }

        //If both spec scores have been selected, starts the combat and displays the combat
        if (!(defenderSpecScore == GameManager.SpecScores.Default))
        {
            DisplayWinner();
        }
    }

    /// <summary>
    /// 
    /// If one of the defender spec scores is selected updates the display and store the spec score to use in the combat
    /// 
    /// </summary>
    /// <param name="specScore">The name of the spec score for the button</param>
    public void DefenderSpec(string specScore)
    {
        GameManager.SpecScores chosenSpec = (GameManager.SpecScores)Enum.Parse(typeof(GameManager.SpecScores), specScore);

        combatPanel.GetComponent<NSCombatComponents>().defenderSpec.GetComponent<TextMeshProUGUI>().text = specScore;
        defenderSpecScore = chosenSpec;

        //Disables the buttons to prevent the value from being changed
        foreach (GameObject specButton in combatPanel.GetComponent<NSCombatComponents>().defenderSpecButtons)
        {
            specButton.GetComponent<Button>().interactable = false;
        }

        //If both spec scores have been selected, starts the combat and displays the combat
        if (!(attackerSpecScore == GameManager.SpecScores.Default))
        {
            DisplayWinner();
        }
    }

    /// <summary>
    /// 
    /// Starts a combat and displays the winner of the combat
    /// 
    /// </summary>
    private void DisplayWinner()
    {
        combatPanel.GetComponent<NSCombatComponents>().continueButton.GetComponent<Button>().interactable = true;

        //If perform combat returns true, means the attacker wins
        if (GameManager.instance.PerformCombat(attackerSpecScore, selectedTarget, defenderSpecScore, out float successChance))
        {
            combatPanel.GetComponent<NSCombatComponents>().winnerText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetActivePlayer().playerName;
            //Sends the IDs of the relevant players to the stealing manager
            stealPanel.GetComponent<NSStealingManager>().winner = GameManager.instance.GetActivePlayer();
            stealPanel.GetComponent<NSStealingManager>().loser = GameManager.instance.GetPlayer(selectedTarget);
        }
        else
        {
            combatPanel.GetComponent<NSCombatComponents>().winnerText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetPlayer(selectedTarget).playerName;
            //Sends the IDs of the relevant players to the stealing manager
            stealPanel.GetComponent<NSStealingManager>().winner = GameManager.instance.GetPlayer(selectedTarget);
            stealPanel.GetComponent<NSStealingManager>().loser = GameManager.instance.GetActivePlayer();
        }

        playerCards.GetComponent<NSPlayerCardManager>().UpdateAllCards();
    }

    /// <summary>
    /// 
    /// Opens the steal panel for the winner
    /// 
    /// </summary>
    public void OpenStealPanel()
    {
        stealPanel.SetActive(true);
        stealPanel.GetComponent<NSStealingManager>().StartStealPanel();
    }

    /// <summary>
    /// 
    /// Closes the combat panels
    /// 
    /// </summary>
    public void CloseCombat()
    {
        targetPanel.SetActive(false);
        combatPanel.SetActive(false);
        stealPanel.SetActive(false);
    }

    #endregion
}
