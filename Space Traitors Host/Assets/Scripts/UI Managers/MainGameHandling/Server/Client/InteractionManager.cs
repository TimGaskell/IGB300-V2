using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public GameObject standardChoiceUI;
    public GameObject escapeRoomUI;

    public static InteractionManager instance = null;

    public GameObject playerCards;

    public GameObject choice0ButtonText;
    public GameObject choice1ButtonText;

    public GameObject choiceInfoPanel;

    public Room currentRoom;
    public int selectedChoiceID;

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


    //Results
    public GameObject ResultText;
    public GameObject ResultPanel;

    private int selectedTarget;

    public List<Sprite> characterPortraits;

    private GameManager.SpecScores attackerSpecScore;
    private GameManager.SpecScores defenderSpecScore;

    public string[] choiceNames;
    public string[] successTexts;
    public string[] failTexts;
    public Choice.IsAvailableTypes[] isAvailables;
    public GameManager.SpecScores[] specScores;
    public float[] successChances;
    public List<int> attackablePlayers;

    private enum ParticipantTypes { Attacker, Defender }
    public bool AreAttacker;
    public int attackingID;
    public int combatwinnerID;
    public int combatLoserID;

    private void Start() {
        
        instance = this;
    }

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
                if (counter < ClientManager.instance.componentsInstalled)
                {
                    compPanel.transform.GetChild(0).gameObject.SetActive(true);
                }
                counter++;
            }

            

        }
        else
        {
            standardChoiceUI.SetActive(true);
            escapeRoomUI.SetActive(false);

            choiceInfoPanel.SetActive(false);

            currentRoom = GameManager.instance.GetRoom(roomIndex);

            //Update button text
            choice0ButtonText.GetComponent<TextMeshProUGUI>().text = choiceNames[0];
            choice1ButtonText.GetComponent<TextMeshProUGUI>().text = choiceNames[1];
        }

        //Checks is combat is available for the active player, enabling the attack button if it is
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

        choiceInfoPanel.GetComponent<ChoiceInfoComponents>().choiceHeader.GetComponent<TextMeshProUGUI>().text = choiceNames[selectedChoiceID];

        //If the choice is not a spec challenge, updates the display text to suit the choice.
        if (specScores[selectedChoiceID] == GameManager.SpecScores.Default)
        {
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().nonSpecChoiceText.SetActive(true);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specChallengeGroup.SetActive(false);

            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().nonSpecChoiceText.GetComponent<TextMeshProUGUI>().text = successTexts[selectedChoiceID];
        }
        //If the choice is a spec challenge, displays the success and failure state for the spec challenge choice
        else
        {
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().nonSpecChoiceText.SetActive(false);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specChallengeGroup.SetActive(true);

            //Find the players relevant spec score need for the choice
            float playerScore = ClientManager.instance.GetScaledSpecScore(specScores[selectedChoiceID]);  //GameManager.instance.GetActivePlayer().GetScaledSpecScore(specScores[selectedChoiceID]);

            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specScoreText.GetComponent<TextMeshProUGUI>().text =
                string.Format("Spec: {0}", specScores[selectedChoiceID].ToString());
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specChanceText.GetComponent<TextMeshProUGUI>().text =
                string.Format("Chance: {0}%", Mathf.Round(successChances[selectedChoiceID]).ToString());
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specSuccessText.GetComponent<TextMeshProUGUI>().text = successTexts[selectedChoiceID];
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().specFailureText.GetComponent<TextMeshProUGUI>().text = failTexts[selectedChoiceID];
        }

        //Test if the choice is available to the player and displays the reason it cannot be selected if not.

        if (isAvailables[selectedChoiceID] == Choice.IsAvailableTypes.available)
        {
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().errorText.SetActive(false);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().choiceSelectButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().errorText.SetActive(true);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().errorText.GetComponent<TextMeshProUGUI>().text = ConvertErrorText(isAvailables[selectedChoiceID]);
            choiceInfoPanel.GetComponent<ChoiceInfoComponents>().choiceSelectButton.GetComponent<Button>().interactable = false;
        }
    }




    public string ConvertErrorText(Choice.IsAvailableTypes errorType) {
        switch (errorType) {
            case (Choice.IsAvailableTypes.disabled):
                return "Item has already been taken";
            case (Choice.IsAvailableTypes.hasScrap):
                return "You do not have enough scrap";
            case (Choice.IsAvailableTypes.hasComponent):
                return "You already have a component";
            case (Choice.IsAvailableTypes.hasNoDamage):
                return "You already have max life points";
            case (Choice.IsAvailableTypes.hasNoCorruption):
                return "You have no corruption";
            case (Choice.IsAvailableTypes.powerAtMax):
                return "AI Power already at max";
            case (Choice.IsAvailableTypes.maxItems):
                return "You cannot carry any more items";
            default:
                return "Not a valid Error Text";
        }
    }

    #endregion

    #region Escape Room Installation

    public void InitComponentPanel()
    {
        //Initialise the component panels on the interaction UI
        componentPanels = new List<GameObject>();
        Debug.Log("ComponentPanelText");

        for (int componentIndex = 0; componentIndex < ClientManager.instance.numPlayers; componentIndex++)
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
            if (!attackablePlayers.Exists(x => x == targetButton.GetComponent<TargetProperties>().playerID))
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
        int targetPlayer = targetButtons[buttonID].GetComponent<TargetProperties>().playerID;

        selectedTarget = targetPlayer;
        targetName.GetComponent<TextMeshProUGUI>().text = ClientManager.instance.GetPlayerData(targetPlayer).PlayerName;

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
       
        PlayerData defendingPlayer = ClientManager.instance.GetPlayerData(selectedTarget);
        AreAttacker = true;
        combatPanel.SetActive(true);

        //Sets the default screen status for the combats
        combatPanel.GetComponent<CombatComponentsClient>().AttackerOrDefenderTitle.GetComponent<TextMeshProUGUI>().text = "ATTACKING";
        combatPanel.GetComponent<CombatComponentsClient>().attackerName.GetComponent<TextMeshProUGUI>().text = defendingPlayer.PlayerName;      
        combatPanel.GetComponent<CombatComponentsClient>().attackerPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(defendingPlayer.CharacterType);
        combatPanel.GetComponent<CombatComponentsClient>().attackerSpec.GetComponent<TextMeshProUGUI>().text = "";
        combatPanel.GetComponent<CombatComponentsClient>().winnerText.GetComponent<TextMeshProUGUI>().text = "";
        combatPanel.GetComponent<CombatComponentsClient>().continueButton.GetComponent<Button>().interactable = false;

        //Reenables the spec score buttons
        for (int buttonID = 0; buttonID < 4; buttonID++)
        {
            combatPanel.GetComponent<CombatComponentsClient>().attackerSpecButtons[buttonID].GetComponent<Button>().interactable = true;
        }

        Server.Instance.SendCombat(defendingPlayer.PlayerID);

    }

    public void SetupDefence() {

        AreAttacker = false;
        PlayerData attackingPlayer = ClientManager.instance.GetPlayerData(attackingID);

        combatPanel.SetActive(true);

        //Sets the default screen status for the combats
        combatPanel.GetComponent<CombatComponentsClient>().AttackerOrDefenderTitle.GetComponent<TextMeshProUGUI>().text = "DEFENDING";
        combatPanel.GetComponent<CombatComponentsClient>().attackerName.GetComponent<TextMeshProUGUI>().text = attackingPlayer.PlayerName;
        combatPanel.GetComponent<CombatComponentsClient>().attackerPortrait.GetComponent<Image>().sprite = GameManager.instance.GetCharacterPortrait(attackingPlayer.CharacterType);
        combatPanel.GetComponent<CombatComponentsClient>().attackerSpec.GetComponent<TextMeshProUGUI>().text = "";
        combatPanel.GetComponent<CombatComponentsClient>().winnerText.GetComponent<TextMeshProUGUI>().text = "";
        combatPanel.GetComponent<CombatComponentsClient>().continueButton.GetComponent<Button>().interactable = false;

        //Reenables the spec score buttons
        for (int buttonID = 0; buttonID < 4; buttonID++) {
            combatPanel.GetComponent<CombatComponentsClient>().attackerSpecButtons[buttonID].GetComponent<Button>().interactable = true;
        }

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

        combatPanel.GetComponent<CombatComponentsClient>().attackerSpec.GetComponent<TextMeshProUGUI>().text = specScore;
        attackerSpecScore = chosenSpec;

        //Disables the buttons to prevent the value from being changed
        foreach (GameObject specButton in combatPanel.GetComponent<CombatComponentsClient>().attackerSpecButtons)
        {
            specButton.GetComponent<Button>().interactable = false;
        }

        Server.Instance.SendSpecSelection(chosenSpec,AreAttacker);

    }

  

    /// <summary>
    /// 
    /// Starts a combat and displays the winner of the combat
    /// 
    /// </summary>
    private void DisplayWinner()
    {
        combatPanel.GetComponent<CombatComponentsClient>().continueButton.GetComponent<Button>().interactable = true;

        //If perform combat returns true, means the attacker wins
        if (GameManager.instance.PerformCombat(attackerSpecScore, selectedTarget, defenderSpecScore))
        {
            combatPanel.GetComponent<CombatComponentsClient>().winnerText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetActivePlayer().playerName;
            //Sends the IDs of the relevant players to the stealing manager
            stealPanel.GetComponent<StealingManager>().winner = GameManager.instance.GetActivePlayer();
           
        }
        else
        {
            combatPanel.GetComponent<CombatComponentsClient>().winnerText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetPlayer(selectedTarget).playerName;
            //Sends the IDs of the relevant players to the stealing manager
            stealPanel.GetComponent<StealingManager>().winner = GameManager.instance.GetPlayer(selectedTarget);
       
        }

        playerCards.GetComponent<PlayerCardManager>().UpdateAllCards();
    }

    /// <summary>
    /// 
    /// Opens the steal panel for the winner
    /// 
    /// </summary>
    public void OpenStealPanel()
    {
        stealPanel.SetActive(true);
        stealPanel.GetComponent<StealingManager>().StartStealPanel();
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
