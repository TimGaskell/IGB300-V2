using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class MainGameUIManager : MonoBehaviour
{
    public GameObject serverActivePanel;
    public GameObject noServerPanel;

    public GameObject playerCards;

    public GameObject aiPowerPanel;
    public GameObject aiPowerBar;
    public GameObject componentTrackerPanel;

    public GameObject abilityPanel;
    public GameObject actionPointPanel;
    public GameObject movementPanel;
    public GameObject interactionPanel;
    public GameObject basicSurgePanel;
    public GameObject attackSurgePanel;

    public GameObject combatPanel;

    public GameObject nonTraitorVictoryPanel;
    public GameObject traitorVictoryPanel;

    public GameObject sabotagePanel;

    public GameObject exitMenu;

    private void Start()
    {
        serverActivePanel.SetActive(true);
        noServerPanel.SetActive(false);

        abilityPanel.SetActive(true);
        abilityPanel.GetComponent<AbilityAnimationController>().InitAbilityAnimations();
        abilityPanel.SetActive(false);
        actionPointPanel.SetActive(false);
        movementPanel.SetActive(false);
        interactionPanel.SetActive(false);
        basicSurgePanel.SetActive(false);
        attackSurgePanel.SetActive(false);

        nonTraitorVictoryPanel.SetActive(false);
        traitorVictoryPanel.SetActive(false);

        sabotagePanel.SetActive(false);

        CloseCombatPanel();

        exitMenu.SetActive(false);

        playerCards.SetActive(true);
        playerCards.GetComponent<PlayerCardManager>().InitialisePlayerCards();
        DisplayCurrentPhase();
        UpdateAIPower();
        UpdateComponentTracker();
    }

    private void Update()
    {
        //Detects if the movement phase is over when the player model has stopped moving.
        //Need to detect if room selection is true otherwise will entirely skip over the movement phase.
        if (!GameManager.instance.playerMoving && !GameManager.instance.roomSelection &&
            GameManager.instance.currentPhase == GameManager.TurnPhases.Movement)
        {
            IncrementPhase();
        }
    }

    #region No Server Handling

    /// <summary>
    /// 
    /// Enables or disables the panels pertaining to the current phase of the game
    /// 
    /// </summary>
    private void DisplayCurrentPhase()
    {
        UpdateComponentTracker();

        switch (GameManager.instance.currentPhase)
        {
            case (GameManager.TurnPhases.Default):

                break;
            case (GameManager.TurnPhases.Abilities):
                CloseCombatPanel();
                aiPowerPanel.SetActive(true);
                UpdateAIPower();
                playerCards.SetActive(true);
                basicSurgePanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                interactionPanel.SetActive(false);
                abilityPanel.SetActive(true);
                //abilityPanel.GetComponent<AbilityManager>().SetupAbilities();
                playerCards.GetComponent<PlayerCardManager>().UpdateActivePlayer();
                break;
            case (GameManager.TurnPhases.ActionPoints):
                abilityPanel.SetActive(false);
                actionPointPanel.SetActive(true);
                break;
            case (GameManager.TurnPhases.Movement):
                actionPointPanel.SetActive(false);
                movementPanel.SetActive(true);
                break;
            case (GameManager.TurnPhases.Interaction):
                movementPanel.SetActive(false);
                interactionPanel.SetActive(true);
                //interactionPanel.GetComponent<InteractionManager>().InitialiseChoices(GameManager.instance.playerGoalIndex);
                break;
            case (GameManager.TurnPhases.BasicSurge):
                //GameManager.instance.numPlayers
                if (GameManager.instance.activePlayer == 1) {

                    basicSurgePanel.SetActive(true);
                    playerCards.GetComponent<PlayerCardManager>().UpdateAllCards();
                    playerCards.SetActive(false);
                    basicSurgePanel.GetComponent<ServerBasicSurgeManager>().UpdateSurgeValues();
                    aiPowerPanel.SetActive(false);
                    interactionPanel.SetActive(false);
                    CloseCombatPanel();
                }

                break;
            case (GameManager.TurnPhases.AttackSurge):
                aiPowerPanel.SetActive(false);
                interactionPanel.SetActive(false);
                attackSurgePanel.SetActive(true);
                playerCards.GetComponent<PlayerCardManager>().UpdateAllCards();
                playerCards.SetActive(false);
                attackSurgePanel.GetComponent<AttackSurgeManager>().UpdateTarget();
                CloseCombatPanel();
                break;
            default:
                throw new NotImplementedException("Not a valid phase");
        }
    }

    /// <summary>
    /// 
    /// Increments the current phase of the game, while also checking if the victory screens are required to be displayed
    /// 
    /// </summary>
    public void IncrementPhase()
    {
        if (GameManager.instance.CurrentVictory == GameManager.VictoryTypes.NonTraitor)
        {
            nonTraitorVictoryPanel.SetActive(true);
            traitorVictoryPanel.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetNonTraitorNames();
        }
        else if (GameManager.instance.CurrentVictory == GameManager.VictoryTypes.Traitor)
        {
            traitorVictoryPanel.SetActive(true);
            traitorVictoryPanel.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Format("{0} Wins!", GameManager.instance.GetPlayer(GameManager.instance.traitorWinID).playerName);
        }
        else
        {
            DisplayCurrentPhase();
        }
    }

    /// <summary>
    /// 
    /// Update the AI Power panel with the current AI Power for the slider and the counter
    /// 
    /// </summary>
    private void UpdateAIPower()
    {
        aiPowerPanel.SetActive(true);
        aiPowerBar.GetComponent<AIPowerBar>().UpdateAIPower();
    }

    private void UpdateComponentTracker()
    {
        if (GameManager.instance.CheckInstalledComponents())
        {
            componentTrackerPanel.GetComponent<ComponentTrackerComponents>().header.SetActive(false);
            componentTrackerPanel.GetComponent<ComponentTrackerComponents>().tracker.SetActive(false);
            componentTrackerPanel.GetComponent<ComponentTrackerComponents>().victoryText.SetActive(true);
        }
        else
        {
            componentTrackerPanel.GetComponent<ComponentTrackerComponents>().header.SetActive(true);
            componentTrackerPanel.GetComponent<ComponentTrackerComponents>().tracker.SetActive(true);
            componentTrackerPanel.GetComponent<ComponentTrackerComponents>().victoryText.SetActive(false);

            componentTrackerPanel.GetComponent<ComponentTrackerComponents>().tracker.GetComponent<TextMeshProUGUI>().text =
                string.Format("{0} / {1}", GameManager.instance.installedComponents, GameManager.instance.NumComponents);
        }
    }

    /// <summary>
    /// 
    /// Return to the main menu
    /// 
    /// </summary>
    public void ExitGame()
    {
        SceneManager.LoadScene(GameManager.MainMenuScene);
    }

    public void SetBasicSurgeButton(bool enabled)
    {
        basicSurgePanel.GetComponent<ServerBasicSurgeManager>().confirmButton.GetComponent<Button>().interactable = enabled;
    }

    public void SetAttackSurgeButton(bool enabled)
    {
        attackSurgePanel.GetComponent<AttackSurgeManager>().confirmButton.GetComponent<Button>().interactable = enabled;
    }

    public void OpenExitMenu()
    {
        if (exitMenu.activeSelf)
        {
            exitMenu.SetActive(false);
        }
        else
        {
            exitMenu.SetActive(true);
        }
    }

    public void InitCombatPanel(int attackerID, int defenderID)
    {
        combatPanel.SetActive(true);
        combatPanel.GetComponent<ServerCombatManager>().InitCombatPanel(attackerID, defenderID);
    }

    public void CloseCombatPanel()
    {
        combatPanel.SetActive(false);
    }

    #endregion
}
