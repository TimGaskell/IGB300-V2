using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class ClientUIManager : MonoBehaviour
{
    private ClientManager player;

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
    public GameObject ResultsPanel;

    public GameObject nonTraitorVictoryPanel;
    public GameObject traitorVictoryPanel;

    public GameObject sabotagePanel;

    public GameObject inventoryPanel;


    public static ClientUIManager instance = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        player = GameObject.Find("PlayerInfoHolder").GetComponent<ClientManager>();

        if (GameManager.instance.serverActive)
        {
            serverActivePanel.SetActive(true);
            noServerPanel.SetActive(false);
        }
        else
        {
            //Sets up targets for choosing other players on the combat and ability panels
            SetupTargets(interactionPanel.GetComponent<InteractionManager>().targetButtons);
            SetupTargets(abilityPanel.GetComponent<AbilityManager>().targetButtons);

            serverActivePanel.SetActive(false);
            noServerPanel.SetActive(true);

            abilityPanel.SetActive(false);
            actionPointPanel.SetActive(false);
            movementPanel.SetActive(false);
            interactionPanel.SetActive(true);
            interactionPanel.GetComponent<InteractionManager>().InitComponentPanel();
            interactionPanel.SetActive(false);
            basicSurgePanel.SetActive(false);
            attackSurgePanel.SetActive(false);

            nonTraitorVictoryPanel.SetActive(false);
            traitorVictoryPanel.SetActive(false);

            sabotagePanel.SetActive(false);

            inventoryPanel.SetActive(false);

            playerCards.GetComponent<PlayerCardManager>().InitialisePlayerCards();
            DisplayCurrentPhase();
            UpdateAIPower();
            UpdateComponentTracker();
        }


    }

    // Update is called once per frame
    void Update()
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

        UpdatePlayerStats();

    }

    public void UpdatePlayerStats() {




    }

    /// <summary>
    /// Does the action roll for the player and sends the number to the server.
    /// </summary>
    public void ActionRoll() {

        int roll = 0;

        roll = GameManager.instance.RollActionPoints();

        player.ActionPoints = roll;

        Server.Instance.SendActionPoints(roll);


    }

    public void MoveToRoom() {
        
        PlayerMovement.instance.StartMoving = true;
        GameManager.instance.playerMoving = true;
        Server.Instance.SendRoomChoice(GameManager.instance.playerGoalIndex);
        

    }


    /// <summary>
    /// Shows the result of their selected choice on another panel
    /// </summary>
    /// <param name="result">True or false of whether they completed the action successfuly on the server </param>
    public void ShowResult(bool result) {

        ResultsPanel.SetActive(true);
        if (result) {

            ResultsPanel.transform.GetChild(0).GetComponent<Text>().text = "Success";
        }
        else {
            ResultsPanel.transform.GetChild(0).GetComponent<Text>().text = "Failed";
        }
    }

    /// <summary>
    /// Gets rid of the results panel and tells the server that they are moving onto the next phase
    /// </summary>
    public void ResultsButton() {

        ResultsPanel.SetActive(false);
        IncrementPhase();
        Server.Instance.SendNewPhase();

    }


  
    #region Server Handling

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
                abilityPanel.GetComponent<AbilityManager>().SetupAbilities();
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
                interactionPanel.GetComponent<InteractionManager>().InitialiseChoices(GameManager.instance.playerGoalIndex);
                break;
            case (GameManager.TurnPhases.BasicSurge):
                aiPowerPanel.SetActive(false);
                interactionPanel.SetActive(false);
                basicSurgePanel.SetActive(true);
                playerCards.GetComponent<PlayerCardManager>().activePlayerPanel.SetActive(false);
                playerCards.GetComponent<PlayerCardManager>().UpdateAllCards();
                basicSurgePanel.GetComponent<BasicSurgeManager>().UpdateSurgeValues();
                break;
            case (GameManager.TurnPhases.AttackSurge):
                aiPowerPanel.SetActive(false);
                interactionPanel.SetActive(false);
                attackSurgePanel.SetActive(true);
                playerCards.GetComponent<PlayerCardManager>().activePlayerPanel.SetActive(false);
                playerCards.GetComponent<PlayerCardManager>().UpdateAllCards();
                attackSurgePanel.GetComponent<AttackSurgeManager>().UpdateTarget();
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

        Server.Instance.SendSelectedChoice(interactionPanel.GetComponent<InteractionManager>().selectedChoiceID);
       
    }

    /// <summary>
    /// 
    /// Update the AI Power panel with the current AI Power for the slider and the counter
    /// 
    /// </summary>
    private void UpdateAIPower()
    {
        aiPowerPanel.SetActive(true);
        aiPowerPanel.GetComponent<AIPowerComponents>().powerCounter.GetComponent<TextMeshProUGUI>().text = string.Format("{0} %", GameManager.instance.AIPower.ToString());
        aiPowerPanel.GetComponent<AIPowerComponents>().powerSlider.GetComponent<Slider>().value = GameManager.instance.AIPower;
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
        playerCards.GetComponent<PlayerCardManager>().UpdatePlayerCard(GameManager.instance.activePlayer);
        IncrementPhase();
    }

    /// <summary>
    /// 
    /// Exits the combat screen and then checks if the traitor has won before moving to the next phase
    /// 
    /// </summary>
    public void EndCombat()
    {
        interactionPanel.GetComponent<InteractionManager>().CloseCombat();
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
        GameManager.instance.AIAttackPlayer(attackSurgePanel.GetComponent<AttackSurgeManager>().selectedSpec);
        playerCards.GetComponent<PlayerCardManager>().UpdateAllCards();
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
            Player player = GameManager.instance.GetPlayer(targetButton.GetComponent<TargetProperties>().characterType);
            //If the player of the particular type does not exist, disables the target button for the character of that type
            if (player == null)
            {
                targetButton.SetActive(false);
            }
            else
            {
                //Sets the player ID on the target image as well as their name above their image
                targetButton.GetComponent<TargetProperties>().playerID = player.playerID;
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

        inventoryPanel.GetComponent<InventoryManager>().selectedPlayer = GameManager.instance.GetOrderedPlayer(buttonID);
        inventoryPanel.GetComponent<InventoryManager>().StartInventoryPanel();
    }

    public void CloseInventoryPanel()
    {
        inventoryPanel.SetActive(false);
    }

    #endregion
}



