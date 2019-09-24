using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public bool disabled;

    //Whether choice is a spec challenge, what type of challenge it is and its associated target score.
    //Spec Challenge is "Null" if not a spec challenge and target score will be 0 (this will throw math errors if passed into spec challenge formula)
    public GameManager.SpecScores specChallenge;
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
        disabled = false;

        specChallenge = GameManager.SpecScores.Default;
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

    public Choice(Choice choice)
    {
        choiceID = choice.choiceID;
        choiceName = choice.choiceName;

        weighting = choice.weighting;
        unique = choice.unique;
        mandatory = choice.mandatory;
        oneOff = choice.oneOff;
        disabled = choice.disabled;

        specChallenge = choice.specChallenge;
        targetScore = choice.targetScore;

        scrapChange = choice.scrapChange;
        corruptionChange = choice.corruptionChange;
        powerChange = choice.powerChange;
        specItem = new Item(choice.specItem);
        lifeChange = choice.lifeChange;
        component = choice.component;

        corruptionFail = choice.corruptionFail;
        lifeFail = choice.lifeFail;

        inBar = choice.inBar;
        inDining = choice.inDining;
        inEngineering = choice.inEngineering;
        inKitchen = choice.inKitchen;
        inSleeping = choice.inSleeping;
        inSpa = choice.inSpa;
    }

    #region Check Availability

    /// <summary>
    /// 
    /// enum for outputting the reason a choice is not available to a player
    /// 
    /// </summary>
    public enum IsAvailableTypes { hasComponent, hasNoDamage, hasNoCorruption, powerAtMax, hasScrap, disabled, available, maxItems };

    /// <summary>
    /// 
    /// Check if the choice is avaialbe for the player which is attempting to select it, and if not returns the reason it cannot be selected
    /// 
    /// </summary>
    /// <param name="playerID">The player which is attempting to select the choice</param>
    /// <returns>The reason the choice cannot be selected by the player. If returns enabled, then choice can be selected</returns>
    public IsAvailableTypes IsAvailable()
    {
        Player checkedPlayer = GameManager.instance.GetActivePlayer();

        //Determines if the choice has been disabled due to its one-off status
        if (disabled)
        {
            return IsAvailableTypes.disabled;
        }

        //If the players scrap goes below 0 due to the choice, choice is unavailable
        if (checkedPlayer.scrap + scrapChange < 0)
        {
            return IsAvailableTypes.hasScrap;
        }

        //If choice gives a component, and the player already has a component, choice is unavailable
        if (component && checkedPlayer.hasComponent)
        {
            return IsAvailableTypes.hasComponent;
        }
        
        //If the choice has a life change and the player has no damage, choice is unavailable
        if (lifeChange != 0 && checkedPlayer.lifePoints == checkedPlayer.maxLifePoints)
        {
            return IsAvailableTypes.hasNoDamage;
        }

        //If the choice has a corruption change and the players corruption is already at 0, choice is unavailable
        if (corruptionChange < 0 && checkedPlayer.Corruption == 0)
        {
            return IsAvailableTypes.hasNoCorruption;
        }

        //If the choice has a power change and the aiPower is already at maximum, choice is unavailable
        if (powerChange != 0 && GameManager.instance.AIPower == GameManager.instance.MAX_POWER)
        {
            return IsAvailableTypes.powerAtMax;
        }

        if (specItem.ItemType != Item.ItemTypes.Default && checkedPlayer.items.Count == Player.MAX_ITEMS)
        {
            return IsAvailableTypes.maxItems;
        }

        //If the choice has been determined that it is not unavailable for a reason, returns that it is available
        return IsAvailableTypes.available;
    }

    public string ConvertErrorText(IsAvailableTypes errorType)
    {
        switch (errorType)
        {
            case (IsAvailableTypes.disabled):
                return "Item has already been taken";
            case (IsAvailableTypes.hasScrap):
                return "You do not have enough scrap";
            case (IsAvailableTypes.hasComponent):
                return "You already have a component";
            case (IsAvailableTypes.hasNoDamage):
                return "You already have max life points";
            case (IsAvailableTypes.hasNoCorruption):
                return "You have no corruption";
            case (IsAvailableTypes.powerAtMax):
                return "AI Power already at max";
            case (IsAvailableTypes.maxItems):
                return "You cannot carry any more items";
            default:
                return "Not a valid Error Text";
        }
    }
    #endregion

    #region Choice Selection

    /// <summary>
    /// 
    /// Updates the active players resources based on the choice they select. If they succeed on the selection, returns true.
    /// Otherwise returns false
    /// 
    /// </summary>
    /// <param name="playerIndex">The player who is selecting the choice</param>
    public bool SelectChoice()
    {
        //Test whether the choice is a spec challenge and what type of spec challenge it is
        //If the choice is not a spec challenge will simply apply the resource changes
        switch (specChallenge)
        {
            case GameManager.SpecScores.Default:
                SuccessfulSelection();
                //Disable the choice if it can only be selected once
                disabled = oneOff;
                return true;
            case GameManager.SpecScores.Brawn:
            case GameManager.SpecScores.Skill:
            case GameManager.SpecScores.Tech:
            case GameManager.SpecScores.Charm:
                return ApplySpecChallenge(GameManager.instance.GetActivePlayer().GetScaledSpecScore(specChallenge));
            default:
                throw new NotImplementedException("Not a valid spec score");
        }
    }

    /// <summary>
    /// 
    /// Test whether a player is successful or not in a spec challenge when they select the choice, updating their resources accordingly.
    /// If they are successful, returns true. Otherwise false
    /// 
    /// </summary>
    /// <param name="specScore">The player's relevant spec score</param>
    /// <returns>True if the player succeeds on the spec challenge. False otherwise</returns>
    private bool ApplySpecChallenge(float specScore)
    {
        //If the player suceeds on the spec challenge, then will apply the resource changes for a success. IF they failed
        //then will apply the resource changes for a failure.
        if (GameManager.PerformSpecChallenge(specScore, targetScore))
        {
            SuccessfulSelection();

            //Disable the choice if it can only be selected once. Only functions if the player is successful in a spec challenge
            disabled = oneOff;

            return true;
        }
        else
        {
            FailedSelection();

            return false;
        }

    }


    /// <summary>
    /// 
    /// Update the active player's resources if they are successful in a spec challenge. Also used if the choice is not a spec challenge
    /// 
    /// </summary>
    private void SuccessfulSelection()
    {
        Debug.Log("scrap Change " + scrapChange);
        Debug.Log("corruption Change " + corruptionChange);
        Debug.Log("Power Change " + powerChange);

        GameManager.instance.GetActivePlayer().scrap += scrapChange;
        GameManager.instance.GetActivePlayer().Corruption += corruptionChange;
        GameManager.instance.aiPowerChange += powerChange;
        //Checks if the choice has an item to give before assignment
        if (specItem.ItemType != Item.ItemTypes.Default)
        {
            GameManager.instance.GetActivePlayer().GiveItem(specItem);
        }
        if (!GameManager.instance.GetActivePlayer().hasComponent)
        {
            GameManager.instance.GetActivePlayer().hasComponent = component;
        }
        GameManager.instance.GetActivePlayer().ChangeLifePoints(lifeChange);
    }

    /// <summary>
    /// 
    /// Update the active player's resource if they failed a spec challenge
    /// 
    /// </summary>
    private void FailedSelection()
    {
        GameManager.instance.GetActivePlayer().Corruption += corruptionFail;
        GameManager.instance.GetActivePlayer().ChangeLifePoints(lifeFail);
    }

    #endregion

    #region Display Text Handling

    /// <summary>
    /// 
    /// Returns the display text for the choice in the success case
    /// 
    /// </summary>
    /// <returns>The display text</returns>
    public string SuccessText()
    {
        string scrapText = IntResourceChange(scrapChange, " <sprite name=\"Scrap\">");
        string corruptionText = IntResourceChange(corruptionChange, "% <sprite name=\"Corruption\">");
        string aiPowerText = IntResourceChange(powerChange, "% AI Power");
        string itemText = ItemString();
        string lifeText = IntResourceChange(lifeChange, " <sprite name=\"Health\">");
        string componentText = component ? "+1 <sprite name=\"Component\">\n" : "";

        Debug.Log(scrapText + corruptionText + aiPowerText + itemText + lifeText + componentText);

        return scrapText + corruptionText + aiPowerText + itemText + lifeText + componentText;
    }

    /// <summary>
    /// 
    /// Returns the display text for the choice in the failure case
    /// 
    /// </summary>
    /// <returns>The display text</returns>
    public string FailText()
    {
        string corruptionText = IntResourceChange(corruptionFail, "% <sprite name=\"Corruption\">");
        string lifeText = IntResourceChange(lifeFail, " <sprite name=\"Health\">");

        return corruptionText + lifeText;
    }

    /// <summary>
    /// 
    /// Returns the string of a integer value for a resource change.
    /// If the value is positive, includes a + value out the front of the string.
    /// 
    /// </summary>
    /// <param name="resourceVal">The resource change which is to be displayed</param>
    /// <param name="valueName">The name of the resource to be displayed after the change</param>
    /// <returns>The string which is to be displayed</returns>
    private string IntResourceChange(int resourceVal, string valueName)
    {
        if(resourceVal > 0)
        {
            return string.Format("+{0}{1}\n\n", resourceVal.ToString(), valueName);
        }
        else if(resourceVal < 0)
        {
            return string.Format("{0}{1}\n\n", resourceVal.ToString(), valueName);
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 
    /// Returns the display string for an item which includes its modifiers in spec scores after the item name
    /// 
    /// </summary>
    /// <returns>The string which is to be displayed for the item</returns>
    private string ItemString()
    {
        //If there is no item, returns no string
        if (specItem.ItemType == Item.ItemTypes.Default)
        {
            return "";
        }
        else
        {
            // If there is no modifier for a particular spec score, does not include it in the string
            string brawnMod = specItem.BrawnChange != 0 ? string.Format("+{0} <sprite name=\"Brawn\">", specItem.BrawnChange) : "";
            string skillMod = specItem.SkillChange != 0 ? string.Format("+{0} <sprite name=\"Skill\">", specItem.SkillChange) : "";
            string techMod = specItem.TechChange != 0 ? string.Format("+{0} <sprite name=\"Tech\">", specItem.TechChange) : "";
            string charmMod = specItem.CharmChange != 0 ? string.Format("+{0} <sprite name=\"Charm\">", specItem.CharmChange) : "";

            return string.Format("+1 {0} {1}\n( {2}{3}{4}{5})", specItem.ItemName, Item.itemSpriteString(specItem.ItemType), brawnMod, skillMod, techMod, charmMod);
        }
    }

    #endregion
}
