using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice
{
    public int choiceID;
    //Name which appears at top of choice for player
    public string choiceName;

    //Weighting of choice appearing in randomisation. Default value is 1. Less than 1 means less likely. More than 1 means more likely
    public float weighting;

    //Whether the choice can appear more than once. If true cannot appear more than once. Can otherwise.
    //If mandatory is not 0 and unique is true, then choice cannot appear more than mandatory number of times
    public bool unique;

    //Number of times the choice must occur in the game
    public int mandatory;

    //Whether the choice is disabled after it has been selected. Disabled choices can be reselected if the item is discarded.
    public bool oneOff;

    //Whether choice is a spec challenge, what type of challenge it is and its associated target score.
    //Spec Challenge is "Null" if not a spec challenge and target score will be 0 (this will throw math errors if passed into spec challenge formula)
    public string specChallenge;
    public int targetScore;

    //Outcomes of the choice. If choice is a spec challenge, these will be the outcomes of a successful spec challenge
    public int scrapChange;
    public int corruptionChange;
    public int powerChange;
    public Item specItem;
    public int lifeChange;
    public bool component;

    //Only required if choice is a spec challenge. 0 otherwise.
    //Outcomes of a failed spec challenge
    public int corruptionFail;
    public int lifeFail;

    //Determination of which rooms choice can appear in
    public bool inBar;
    public bool inDining;
    public bool inEngineering;
    public bool inKitchen;
    public bool inSleeping;
    public bool inSpa;

    public Choice()
    {
        choiceID = 0;
        choiceName = "Default";

        weighting = 1;
        unique = false;
        mandatory = 0;
        oneOff = false;

        specChallenge = "Null";
        targetScore = 0;

        scrapChange = 0;
        corruptionChange = 0;
        powerChange = 0;
        specItem = new Item();
        lifeChange = 0;
        component = false;

        corruptionFail = 0;
        lifeFail = 0;

        inBar = true;
        inDining = true;
        inEngineering = true;
        inKitchen = true;
        inSleeping = true;
        inSpa = true;
    }
    
    /// <summary>
    /// 
    /// Check if the choice is avaialbe for the player which is attempting to select it. If this returns false, player should not
    /// be able to select this choice
    /// 
    /// </summary>
    /// <param name="playerID">The player which is attempting to select the choice</param>
    /// <returns>If the player can select the choice returns true. False otherwise.</returns>
    private bool IsAvailable(int playerID)
    {
        Player checkedPlayer = GameManager.instance.players[playerID];

        //Below statements check if the conditional is relevant to the choice, based on the changes which the choice causes. If the condition is not relevant will 
        //return true due to the and
        //Then checks the condition for each relevant change. Since the condition is met, then will overall return false, since the condition being met means the
        //player cannot select the choice
        bool hasComponent =  !(component && checkedPlayer.hasComponent);
        bool hasDamage = !(lifeChange == 0 && checkedPlayer.lifePoints == checkedPlayer.maxLifePoints);
        bool hasCorruption = !(corruptionChange == 0 && checkedPlayer.lifePoints == 0);
        bool powerNotAtMax = !(powerChange == 0 && GameManager.instance.aiPower == GameManager.instance.MAX_POWER);

        //If the choice reduces a players scrap, then they need to have enough scrap to pay for the choice, so will return false if this is not the case
        bool hasScrap = checkedPlayer.scrap + scrapChange < 0;

        //If any of the above conditions are false, then needs to return false, since this means the choice is not availabe
        return hasComponent && hasScrap && hasDamage && hasCorruption && powerNotAtMax;
    }
}
