using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class ClientUIManager : MonoBehaviour
{

    public GameObject test;
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

        GameManager.instance.roomList = GameObject.Find("Rooms");
        instance = this;

        player = GameObject.Find("ClientManager").GetComponent<ClientManager>();

        if (GameManager.instance.serverActive)
        {
            serverActivePanel.SetActive(true);
            noServerPanel.SetActive(false);

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
            DisplayCurrentPhase();
  
        }
        else
        {
           

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
        }


    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerStats();
        if (GameManager.instance.serverActive)
        {
            
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


    /// <summary>
    /// Updates all values on the player card based on what is in the client manager
    /// </summary>
    public void UpdatePlayerStats() {

        playerCards.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = ClientManager.instance.scaledBrawn.ToString();
        playerCards.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = ClientManager.instance.scaledSkill.ToString();
        playerCards.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = ClientManager.instance.scaledTech.ToString();
        playerCards.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = ClientManager.instance.scaledCharm.ToString();

        //Sets amount of scrap and lifepoints of player
        playerCards.transform.GetChild(6).GetChild(0).GetComponent<Text>().text = ClientManager.instance.scrap.ToString();
        playerCards.transform.GetChild(7).GetChild(0).GetComponent<Text>().text = ClientManager.instance.lifePoints.ToString();

        //Sets component if the player currently has one
        if (ClientManager.instance.hasComponent) {
            playerCards.transform.GetChild(8).GetChild(0).GetComponent<Text>().text = "Have component";

        }
        else {
            playerCards.transform.GetChild(8).GetChild(0).GetComponent<Text>().text = "Don't have component";
        }

        //Sets corruption of the player
        playerCards.transform.GetChild(9).GetChild(0).GetComponent<Text>().text = ClientManager.instance.corruption.ToString();

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
    public void DisplayCurrentPhase()
    {
        switch (GameManager.instance.currentPhase)
        {
            case (GameManager.TurnPhases.Default):
                basicSurgePanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                interactionPanel.SetActive(false);
                break;
            case (GameManager.TurnPhases.Abilities):
                basicSurgePanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                interactionPanel.SetActive(false);
                abilityPanel.SetActive(true);
                break;
            case (GameManager.TurnPhases.ActionPoints):
                abilityPanel.SetActive(false);
                actionPointPanel.SetActive(true);
                RollActionPoints.instance.ResetRoll();
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
        Server.Instance.SendNewPhase();
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
    /// Installs a component for a player
    /// 
    /// </summary>
    public void InstallComponent()
    {
        if (!GameManager.instance.InstallComponent())
        {
            sabotagePanel.SetActive(true);
        }
 
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
    public void SetupTargets(List<GameObject> targetButtons)
    {
        foreach (GameObject targetButton in targetButtons)
        {
            PlayerData player = ClientManager.instance.GetPlayer(targetButton.GetComponent<TargetProperties>().characterType);  // GameManager.instance.GetPlayer(targetButton.GetComponent<TargetProperties>().characterType);
            //If the player of the particular type does not exist, disables the target button for the character of that type
            if (player == null)
            {
                targetButton.SetActive(false);
            }
            else
            {
                //Sets the player ID on the target image as well as their name above their image
                targetButton.GetComponent<TargetProperties>().playerID = player.PlayerID;
                targetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.PlayerName;
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



