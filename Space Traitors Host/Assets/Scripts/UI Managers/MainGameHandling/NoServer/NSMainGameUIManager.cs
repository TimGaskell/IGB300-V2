using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class NSMainGameUIManager : MonoBehaviour
{
    public GameObject serverActivePanel;
    public GameObject noServerPanel;

    public GameObject playerCards;

    public GameObject aiPowerPanel;
    public GameObject componentTrackerPanel;

    public GameObject abilityPanel;
    public GameObject actionPointPanel;
    public GameObject movementPanel;
    public GameObject interactionPanel;
    public GameObject basicSurgePanel;
    public GameObject attackSurgePanel;

    public GameObject nonTraitorVictoryPanel;
    public GameObject traitorVictoryPanel;

    public GameObject sabotagePanel;

    public GameObject inventoryPanel;

    private void Start()
    {
        if (GameManager.instance.serverActive)
        {
            serverActivePanel.SetActive(true);
            noServerPanel.SetActive(false);
        }
        else
        {
            //Sets up targets for choosing other players on the combat and ability panels
            SetupTargets(interactionPanel.GetComponent<NSInteractionManager>().targetButtons);
            SetupTargets(abilityPanel.GetComponent<NSAbilityManager>().targetButtons);

            serverActivePanel.SetActive(false);
            noServerPanel.SetActive(true);

            abilityPanel.SetActive(false);
            actionPointPanel.SetActive(false);
            movementPanel.SetActive(false);
            interactionPanel.SetActive(true);
            interactionPanel.GetComponent<NSInteractionManager>().InitComponentPanel();
            interactionPanel.SetActive(false);
            basicSurgePanel.SetActive(false);
            attackSurgePanel.SetActive(false);

            nonTraitorVictoryPanel.SetActive(false);
            traitorVictoryPanel.SetActive(false);

            sabotagePanel.SetActive(false);

            inventoryPanel.SetActive(false);

            playerCards.GetComponent<NSPlayerCardManager>().InitialisePlayerCards();
            DisplayCurrentPhase();
            UpdateAIPower();
            UpdateComponentTracker();
        }
    }

    private void Update()
    {
        if (GameManager.instance.serverActive)
        {
            throw new NotImplementedException("Server Not Implemented");
        }
        else
        {
            //Detects if the movement phase is over when the player model has stopped moving.
            //Need to detect if room selection is true otherwise will entirely skip over the movement phase.
            if (!GameManager.instance.playerMoving && !GameManager.instance.roomSelection &&
                GameManager.instance.currentPhase == GameManager.TurnPhases.Movement)
            {
                IncrementPhase();
            }
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
        switch (GameManager.instance.currentPhase)
        {
            case (GameManager.TurnPhases.Abilities):
                UpdateAIPower();
                basicSurgePanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                interactionPanel.SetActive(false);
                abilityPanel.SetActive(true);
                abilityPanel.GetComponent<NSAbilityManager>().SetupAbilities();
                playerCards.GetComponent<NSPlayerCardManager>().UpdateActivePlayer();
                playerCards.GetComponent<NSPlayerCardManager>().UpdateInventoryButton(GameManager.instance.activePlayer, true);
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
                interactionPanel.GetComponent<NSInteractionManager>().InitialiseChoices(GameManager.instance.playerGoalIndex);
                playerCards.GetComponent<NSPlayerCardManager>().UpdateInventoryButton(GameManager.instance.activePlayer, false);
                break;
            case (GameManager.TurnPhases.BasicSurge):
                aiPowerPanel.SetActive(false);
                interactionPanel.SetActive(false);
                basicSurgePanel.SetActive(true);
                playerCards.GetComponent<NSPlayerCardManager>().activePlayerPanel.SetActive(false);
                playerCards.GetComponent<NSPlayerCardManager>().UpdateAllCards();
                basicSurgePanel.GetComponent<NSBasicSurgeManager>().UpdateSurgeValues();
                break;
            case (GameManager.TurnPhases.AttackSurge):
                aiPowerPanel.SetActive(false);
                interactionPanel.SetActive(false);
                attackSurgePanel.SetActive(true);
                playerCards.GetComponent<NSPlayerCardManager>().activePlayerPanel.SetActive(false);
                playerCards.GetComponent<NSPlayerCardManager>().UpdateAllCards();
                attackSurgePanel.GetComponent<NSAttackSurgeManager>().UpdateTarget();
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
        GameManager.instance.IncrementPhase();
        if (GameManager.instance.CurrentVictory == GameManager.VictoryTypes.NonTraitor)
        {
            nonTraitorVictoryPanel.SetActive(true);
        }
        else if (GameManager.instance.CurrentVictory == GameManager.VictoryTypes.Traitor)
        {
            traitorVictoryPanel.SetActive(true);
            traitorVictoryPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0} Wins!", GameManager.instance.GetPlayer(GameManager.instance.traitorWinID).playerName);
        }
        else
        {
            DisplayCurrentPhase();
        }
    }

    /// <summary>
    /// 
    /// Updates the players resources based on the choice they have selected and moves into the next phase.
    /// 
    /// </summary>
    public void SelectChoice()
    {
        interactionPanel.GetComponent<NSInteractionManager>().currentRoom.roomChoices[interactionPanel.GetComponent<NSInteractionManager>().selectedChoiceID].SelectChoice();
        playerCards.GetComponent<NSPlayerCardManager>().UpdatePlayerCard(GameManager.instance.activePlayer);
        IncrementPhase();
    }

    /// <summary>
    /// 
    /// Update the AI Power panel with the current AI Power for the slider and the counter
    /// 
    /// </summary>
    private void UpdateAIPower()
    {
        aiPowerPanel.SetActive(true);
        aiPowerPanel.GetComponent<NSAIPowerComponents>().powerCounter.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.AIPower.ToString());
        aiPowerPanel.GetComponent<NSAIPowerComponents>().powerSlider.GetComponent<Slider>().value = GameManager.instance.AIPower;
    }

    private void UpdateComponentTracker()
    {
        if (GameManager.instance.CheckInstalledComponents())
        {
            componentTrackerPanel.GetComponent<NSComponentTrackerComponents>().header.SetActive(false);
            componentTrackerPanel.GetComponent<NSComponentTrackerComponents>().tracker.SetActive(false);
            componentTrackerPanel.GetComponent<NSComponentTrackerComponents>().victoryText.SetActive(true);
        }
        else
        {
            componentTrackerPanel.GetComponent<NSComponentTrackerComponents>().header.SetActive(true);
            componentTrackerPanel.GetComponent<NSComponentTrackerComponents>().tracker.SetActive(true);
            componentTrackerPanel.GetComponent<NSComponentTrackerComponents>().victoryText.SetActive(false);

            componentTrackerPanel.GetComponent<NSComponentTrackerComponents>().tracker.GetComponent<TextMeshProUGUI>().text = 
                string.Format("{0} / {1}", GameManager.instance.installedComponents, GameManager.instance.NumComponents);
        }
    }

    /// <summary>
    /// 
    /// Installs a component for a player
    /// 
    /// </summary>
    public void InstallComponent()
    {
        if (!GameManager.instance.InstallComponent())
        {
            sabotagePanel.SetActive(true);
        }
        UpdateComponentTracker();
        playerCards.GetComponent<NSPlayerCardManager>().UpdatePlayerCard(GameManager.instance.activePlayer);
        IncrementPhase();
    }

    /// <summary>
    /// 
    /// Exits the combat screen and then checks if the traitor has won before moving to the next phase
    /// 
    /// </summary>
    public void EndCombat()
    {
        interactionPanel.GetComponent<NSInteractionManager>().CloseCombat();
        IncrementPhase();
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

    /// <summary>
    /// 
    /// Confirms the spec score the player wants to use when targetted by an AI Attack
    /// 
    /// </summary>
    public void ConfirmSpecSelection()
    {
        GameManager.instance.AIAttackPlayer(attackSurgePanel.GetComponent<NSAttackSurgeManager>().selectedSpec);
        playerCards.GetComponent<NSPlayerCardManager>().UpdateAllCards();
        IncrementPhase();
    }

    /// <summary>
    /// 
    /// Setups the target panel to show the players which are in the game (disabling those which are not) as well as setting up their names above
    /// their portraits. Only needs to be called once when the game is started
    /// 
    /// </summary>
    private void SetupTargets(List<GameObject> targetButtons)
    {
        foreach (GameObject targetButton in targetButtons)
        {
            Player player = GameManager.instance.GetPlayer(targetButton.GetComponent<NSTargetProperties>().characterType);
            //If the player of the particular type does not exist, disables the target button for the character of that type
            if (player == null)
            {
                targetButton.SetActive(false);
            }
            else
            {
                //Sets the player ID on the target image as well as their name above their image
                targetButton.GetComponent<NSTargetProperties>().playerID = player.playerID;
                targetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.playerName;
            }
        }
    }

    public void CloseSabotagePanel()
    {
        sabotagePanel.SetActive(false);
    }

    public void OpenInventoryPanel(int buttonID)
    {
        inventoryPanel.SetActive(true);

        inventoryPanel.GetComponent<NSInventoryManager>().selectedPlayer = GameManager.instance.GetOrderedPlayer(buttonID);
        inventoryPanel.GetComponent<NSInventoryManager>().StartInventoryPanel();
    }

    public void CloseInventoryPanel()
    {
        inventoryPanel.SetActive(false);
    }

    #endregion
}
