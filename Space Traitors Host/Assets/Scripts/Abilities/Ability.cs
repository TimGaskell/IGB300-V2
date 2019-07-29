using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public string abilityName;
    public string abilityDescription;

    public int scrapCost;
    public int corruptionRequirement;

    /// <summary>
    /// 
    /// Define a default ability
    /// 
    /// </summary>
    public Ability()
    {
        abilityName = "Default";
        abilityDescription = "Default";

        scrapCost = 0;
        corruptionRequirement = 0;
    }

    public bool CheckCorruption(int playerCorruption, int corruptionRequirement)
    {
        return playerCorruption >= corruptionRequirement;
    }

    //If the corruption value is higher or lower than specified threshold, player can use the ability
    public virtual bool CheckUse(int targetIndex)
    {
        if (CheckCorruption(GameManager.instance.players[targetIndex].Corruption,  corruptionRequirement))
        {

            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// 
    /// Activate the ability. Default abilities should never be activated, so throws an exception
    /// 
    /// </summary>
    public virtual void Activate()
    {
        throw new NotImplementedException("Ability not Defined");
    }

    public virtual void Activate(int targetIndex)
    {
        throw new NotImplementedException("Ability not Defined");
    }

    /// <summary>
    /// 
    /// Deactivate the ability, reverting any changes that were made
    /// 
    /// Used for the Chef's 'Preparation' ability- buffs reset at the end of each turn
    /// 
    /// </summary>
    public virtual void Deactivate()
    {
        //throw new NotImplementedException("Ability not Defined");

        foreach (Player player in GameManager.instance.players)
        {
            if (player.ChefBuffed)
            {
                //Reset the buffs (-1 to all specs)
                player.brawnModTemp--;
                player.skillModTemp--;
                player.techModTemp--;
                player.charmModTemp--;

                player.ChefBuffed = false;
            }
        }
    }


/// <summary>
/// 
/// Deactivate the ability, reverting any changes that were made
/// 
/// Used for disabling the Techie's 'Muddle Sensors' ability (invisibility)
/// 
/// Made seperate to Deactivate() as unsure how long invisibility lasts
/// 
/// </summary>

    public virtual void MuddleDeactivate(int targetIndex)
    {
        GameManager.instance.players[targetIndex].MuddleSensors(true);
    }
}

#region Character Abilities
public class Shove : Ability
{
    public Shove()
    {
        abilityName = "Shove";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 7;
        corruptionRequirement = 0;
    }

    public override void Activate()
    {
        Debug.Log("Shove Activate");

    }
}

public class SecretPaths : Ability
{
    public SecretPaths()
    {
        abilityName = "Secret Paths";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 6;
        corruptionRequirement = 0;
    }

    public override void Activate()
    {
        Debug.Log("Secret Paths Activate");
    }
}

public class Preparation : Ability
{
    public Preparation()
    {
        abilityName = "Preparation";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 8;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        Debug.Log("Preperation Activate");
        //+1 to all spec modifiers
        GameManager.instance.players[targetIndex].brawnModTemp++;
        GameManager.instance.players[targetIndex].skillModTemp++;
        GameManager.instance.players[targetIndex].techModTemp++;
        GameManager.instance.players[targetIndex].charmModTemp++;

        GameManager.instance.players[targetIndex].ChefBuffed = true;
    }
}

public class QuickRepair : Ability
{
    public QuickRepair()
    {
        abilityName = "Quick Repair";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 12;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        Debug.Log("Quick Repair Activate");

        if (GameManager.instance.players[targetIndex].lifePoints <= GameManager.instance.players[targetIndex].maxLifePoints) //Temporary max check, could be better handled by Player
        {
            GameManager.instance.players[targetIndex].lifePoints += 1;
        }
       
    }
}

public class EncouragingSong : Ability
{
    public EncouragingSong()
    {
        abilityName = "Encouraging Song";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 5;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        Debug.Log("Encouraging Song Activate");
       
        if (GameManager.instance.players[targetIndex].Corruption >= 15)
            GameManager.instance.players[targetIndex].Corruption -= 15;
        else
            GameManager.instance.players[targetIndex].Corruption = 0;  //Corruption should never be a negative
    }
}

public class MuddleSensors : Ability
{
    public MuddleSensors()
    {
        abilityName = "Muddle Sensors";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 5;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        Debug.Log("Muddle Sensors Activate");
        GameManager.instance.players[targetIndex].MuddleSensors(false); //Void in player script that disable/enables 
    }
}
#endregion

#region Corruption Abilities
public class SensorScan : Ability
{
    public SensorScan()
    {
        abilityName = "Sensor Scan";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 3;
        corruptionRequirement = 25;
    }

    public override void Activate()
    {
        Debug.Log("Sensor Scan Activate");
    }
}

public class CodeInspection : Ability
{
    public CodeInspection()
    {
        abilityName = "Code Inspection";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 5;
        corruptionRequirement = 50;
    }

    public override void Activate()
    {
        Debug.Log("Code Inspection Activate");
    }
}

public class Sabotage : Ability
{
    public Sabotage()
    {
        abilityName = "Sabotage";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 10;
        corruptionRequirement = 75;
    }

    public override void Activate()
    {
        Debug.Log("Sabotage Activate");
    }
}

public class PowerUp : Ability
{
    public PowerUp()
    {
        abilityName = "Power Up";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 15;
        corruptionRequirement = 100;
    }

    public override void Activate(int targetIndex)
    {
        Debug.Log("Power Up Activate");

        GameManager.instance.players[targetIndex].maxLifePoints += 1;
        GameManager.instance.players[targetIndex].lifePoints += 1;
    }
}
#endregion