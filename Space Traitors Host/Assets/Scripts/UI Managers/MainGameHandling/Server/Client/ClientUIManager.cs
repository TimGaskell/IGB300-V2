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
    public GameObject nonTraitorLosePanel;
    public GameObject TraitorsLosePanel;

    public GameObject sabotagePanel;

    public GameObject corruptionBar;
    public GameObject scrapTracker;
    public GameObject healthTracker;
    public GameObject characterPortrait;

    public GameObject inventoryPanel;


    public static ClientUIManager instance = null;

    // Start is called before the first frame update
    void Start()
    {

        GameManager.instance.roomList = GameObject.Find("Rooms");
        instance = this;

        player = GameObject.Find("ClientManager").GetComponent<ClientManager>();

        characterPortrait.GetComponent<Image>().sprite = ClientManager.instance.GetCharacterPortrait(ClientManager.instance.PlayerCharacter.CharacterType);

        serverActivePanel.SetActive(true);
        noServerPanel.SetActive(false);

        abilityPanel.SetActive(false);
        actionPointPanel.SetActive(false);
        movementPanel.SetActive(false);
        interactionPanel.SetActive(false);
        basicSurgePanel.SetActive(false);
        attackSurgePanel.SetActive(false);

        nonTraitorVictoryPanel.SetActive(false);
        traitorVictoryPanel.SetActive(false);

        sabotagePanel.SetActive(false);
        inventoryPanel.SetActive(false);
        DisplayCurrentPhase();
    }

    // Update is called once per frame
    void Update()
    {
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
    public void UpdatePlayerStats()
    {
        UpdateComponentTracker();
        corruptionBar.GetComponent<CorruptionBarController>().UpdateCorruptionBar();
        scrapTracker.GetComponent<ScrapTrackerController>().UpdateScrapText();
        healthTracker.GetComponent<HealthBarManager>().UpdateHealthPoints();
    }



    public void MoveToRoom()
    {

        PlayerMovement.instance.StartMoving = true;
        GameManager.instance.playerMoving = true;
        Server.Instance.SendRoomChoice(GameManager.instance.playerGoalIndex);


    }


    /// <summary>
    /// Shows the result of their selected choice on another panel
    /// </summary>
    /// <param name="result">True or false of whether they completed the action successfuly on the server </param>
    public void ShowResult(bool result)
    {

        ResultsPanel.SetActive(true);
        if (result)
        {

            ResultsPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Success";
        }
        else
        {
            ResultsPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Failed";
        }
    }

    /// <summary>
    /// Gets rid of the results panel and tells the server that they are moving onto the next phase
    /// </summary>
    public void ResultsButton()
    {

        ResultsPanel.SetActive(false);
        IncrementPhase();
        Server.Instance.SendNewPhase();

    }

    public void ActivateInventoryPanel()
    {
        bool inventoryOpen = !inventoryPanel.activeSelf;

        inventoryPanel.SetActive(inventoryOpen);

        if (inventoryOpen)
        {
            inventoryPanel.GetComponent<InventoryManager>().StartInventoryPanel();
        }

        //Disable corruption bar and scrap tracker when inventory is open
        corruptionBar.SetActive(!inventoryOpen);
        scrapTracker.SetActive(!inventoryOpen);
    }

    #region Server Handling

    /// <summary>
    /// 
    /// Enables or disables the panels pertaining to the current phase of the game
    /// 
    /// </summary>
    public void DisplayCurrentPhase()
    {
        UpdatePlayerStats();
        switch (GameManager.instance.currentPhase)
        {
            case (GameManager.TurnPhases.Default):
                basicSurgePanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                interactionPanel.SetActive(false);
                abilityPanel.SetActive(false);
                actionPointPanel.SetActive(false);
                movementPanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                basicSurgePanel.SetActive(false);
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
                basicSurgePanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                interactionPanel.SetActive(false);
                abilityPanel.SetActive(false);
                actionPointPanel.SetActive(false);
                movementPanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                basicSurgePanel.SetActive(false);
                break;
            case (GameManager.TurnPhases.AttackSurge):
                interactionPanel.SetActive(false);
                attackSurgePanel.SetActive(true);
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
        Server.Instance.InstallComponent();

        //playerCards.GetComponent<PlayerCardManager>().UpdatePlayerCard(GameManager.instance.activePlayer);
        //IncrementPhase();
    }

    /// <summary>
    /// 
    /// Exits the combat screen and then checks if the traitor has won before moving to the next phase
    /// 
    /// </summary>
    public void EndCombat()
    {
        interactionPanel.GetComponent<InteractionManager>().CloseCombat();

        if(GameManager.instance.currentPhase == GameManager.TurnPhases.Interaction){
            IncrementPhase();
        }
        else if (GameManager.instance.currentPhase == GameManager.TurnPhases.AttackSurge) {

            InteractionManager.instance.EndAttack();

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

    /// <summary>
    /// 
    /// Confirms the spec score the player wants to use when targetted by an AI Attack
    /// 
    /// </summary>
    public void ConfirmSpecSelection()
    {
        //GameManager.instance.AIAttackPlayer(attackSurgePanel.GetComponent<AttackSurgeManager>().selectedSpec);
        //playerCards.GetComponent<PlayerCardManager>().UpdateAllCards();
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

    public void ClosePanel(GameObject panel)
    {

        panel.SetActive(false);

    }

    public void DisplaySurge()
    {

        interactionPanel.SetActive(false);
        basicSurgePanel.SetActive(true);
        basicSurgePanel.GetComponent<ClientBasicSurgeManager>().UpdateSurgeValues();

    }

    public void DisplayAttackSurge()
    {

        interactionPanel.SetActive(false);
        attackSurgePanel.SetActive(true);
        attackSurgePanel.GetComponent<AttackSurgeManager>().UpdateTarget();

    }

    public void UpdateComponentTracker()
    {
        ClientComponentTracker componentTracker = componentTrackerPanel.GetComponent<ClientComponentTracker>();

        componentTracker.numberText.GetComponent<TextMeshProUGUI>().text = string.Format("{0} / {1}", ClientManager.instance.componentsInstalled, ClientManager.instance.numPlayers);

        string objectiveText;

        if (ClientManager.instance.isTraitor)
        {
            if (ClientManager.instance.CheckComponentInstalled())
            {
                objectiveText = "The others have repaired the escape shuttle. Stop them from escaping!";
            }
            else
            {
                objectiveText = "Find the other robots and eliminate them! Don't let them repair the shuttle!";
            }
                
        }
        else
        {
            if (ClientManager.instance.CheckComponentInstalled())
            {
                objectiveText = "All components installed. Get to the Shuttle!";
            }
            else
            {
                objectiveText = "Find the components and install them on the escape shuttle.";
            }
        }

        componentTracker.objectiveText.GetComponent<TextMeshProUGUI>().text = objectiveText;
    }



    #endregion
}



