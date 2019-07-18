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

    //public virtual bool CheckUse()
    //{

    //}

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
    /// </summary>
    public virtual void Deactivate()
    {
        throw new NotImplementedException("Ability not Defined");
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

public class Preperation : Ability
{
    public Preperation()
    {
        abilityName = "Preperation";
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 8;
        corruptionRequirement = 0;
    }

    public override void Activate()
    {
        Debug.Log("Preperation Activate");
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
        GameManager.instance.players[targetIndex].lifePoints += 1; //Need to add in check for max 
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

    public override void Activate()
    {
        Debug.Log("Encouraging Song Activate");
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

    public override void Activate()
    {
        Debug.Log("Muddle Sensors Activate");
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

    public override void Activate()
    {
        Debug.Log("Power Up Activate");
    }
}
#endregion