﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public enum AbilityTypes { Default, Shove, Secret_Paths, Power_Boost, Quick_Repair, Encouraging_Song,
        Muddle_Sensors, Sensor_Scan, Code_Inspection, Sabotage, Supercharge };

    public AbilityTypes abilityType;
    public string AbilityName { get { return abilityType.ToString().Replace('_', ' '); } }
    public string abilityDescription;

    public int scrapCost;
    public int corruptionRequirement;

    public enum ScanResources { Scrap, Item, Component }

    /// <summary>
    /// 
    /// Define a default ability
    /// 
    /// </summary>
    public Ability()
    {
        abilityType = AbilityTypes.Default;
        abilityDescription = "Default";

        scrapCost = 0;
        corruptionRequirement = 0;
    }

    /// <summary>
    /// 
    /// Checks if the active player has enough scrap to use a particular ability. Returns true if they can. False otherwise
    /// 
    /// </summary>
    /// <returns>Returns true if the player has enough scrap to use the ability. False otherwise</returns>
    public bool CheckScrap()
    {
        return GameManager.instance.GetActivePlayer().scrap - scrapCost >= 0;
    }

    /// <summary>
    /// 
    /// Spends the active player's scrap in order to use the ability
    /// 
    /// </summary>
    public void SpendScrap()
    {
        GameManager.instance.GetActivePlayer().scrap -= scrapCost;
    }

    /// <summary>
    /// 
    /// Checks if the player has enough corruption to use the ability. If they do not, returns false. Otherwise
    /// returns true.
    /// 
    /// </summary>
    /// <returns>Returns false if the player does not have enough corruption to use the ability. If they do, returns true</returns>
    public bool CheckCorruption()
    {
        return GameManager.instance.GetActivePlayer().Corruption >= corruptionRequirement;
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

    public virtual void Activate(int targetIndex, out bool isTraitor)
    {
        throw new NotImplementedException("Ability not Defined");
    }

    public virtual List<int> Activate(ScanResources resourceType)
    {
        throw new NotImplementedException("Ability not Defined");
    }

    public virtual void Deactivate()
    {
        throw new NotImplementedException("Ability does not have a deactivate case.");
    }
}

#region Character Abilities

#region Shove
/// <summary>
/// 
/// Shove not in use
/// 
/// </summary>
public class Shove : Ability
{
    public Shove()
    {
        abilityType = AbilityTypes.Shove;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 7;
        corruptionRequirement = 0;
    }

    public override void Activate()
    {
        throw new NotImplementedException("Shove not to be used");

    }
}
#endregion

#region Secret Paths
/// <summary>
/// 
/// Reduces the cost of moving along a path by a modifier until start of next turn
/// 
/// </summary>
public class SecretPaths : Ability
{
    //Modifier for the cost of moving along paths
    private const int PATH_MOD = 1;

    public SecretPaths()
    {
        abilityType = AbilityTypes.Secret_Paths;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 6;
        corruptionRequirement = 0;
    }

    public override void Activate()
    {
        SpendScrap();
        Debug.Log("Secret Paths Activate- Requires action point integration");

        GameManager.instance.GetActivePlayer().AssignActiveAbility(this);
    }

    public override void Deactivate()
    {
        Debug.Log("Secret Paths Deactivated- Requires action point integration");
    }
}
#endregion

#region Power Boost
/// <summary>
/// 
/// Player gains +1 in all spec scores until start of their next turn
/// 
/// </summary>
public class PowerBoost : Ability
{
    //Modifier to the player's spec scores until start of next turn
    private const int SPEC_MOD = 1;

    public PowerBoost()
    {
        abilityType = AbilityTypes.Power_Boost;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 8;
        corruptionRequirement = 0;
    }

    public override void Activate()
    {
        SpendScrap();
        GameManager.instance.GetActivePlayer().brawnModTemp += SPEC_MOD;
        GameManager.instance.GetActivePlayer().skillModTemp += SPEC_MOD;
        GameManager.instance.GetActivePlayer().techModTemp += SPEC_MOD;
        GameManager.instance.GetActivePlayer().charmModTemp += SPEC_MOD;

        GameManager.instance.GetActivePlayer().AssignActiveAbility(this);
    }

    public override void Deactivate()
    {
        GameManager.instance.GetActivePlayer().brawnModTemp -= SPEC_MOD;
        GameManager.instance.GetActivePlayer().skillModTemp -= SPEC_MOD;
        GameManager.instance.GetActivePlayer().techModTemp -= SPEC_MOD;
        GameManager.instance.GetActivePlayer().charmModTemp -= SPEC_MOD;
    }
}
#endregion

#region Quick Repair
/// <summary>
/// 
/// Quick Repair not in use
/// 
/// </summary>
public class QuickRepair : Ability
{
    public QuickRepair()
    {
        abilityType = AbilityTypes.Quick_Repair;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 12;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        throw new NotImplementedException("Quick Repair Not Implemented");

        //if (GameManager.instance.players[targetIndex].lifePoints <= GameManager.instance.players[targetIndex].maxLifePoints) //Temporary max check, could be better handled by Player
        //{
        //    GameManager.instance.players[targetIndex].lifePoints += 1;
        //}
       
    }
}
#endregion

#region Encouraging Song
/// <summary>
/// 
/// Reduces a target player's corruption by a certain amount
/// 
/// </summary>
public class EncouragingSong : Ability
{
    private const int CORRUPTION_MOD = 15;

    public EncouragingSong()
    {
        abilityType = AbilityTypes.Encouraging_Song;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 5;
        corruptionRequirement = 0;
    }

    /// <summary>
    /// 
    /// Target a player to reduce their corruption.
    /// 
    /// </summary>
    /// <param name="targetIndex">The player ID of the target</param>
    public override void Activate(int targetIndex)
    {
        SpendScrap();
        GameManager.instance.GetPlayer(targetIndex).Corruption -= CORRUPTION_MOD;
    }
}
#endregion

#region Muddle Sensors
/// <summary>
/// 
/// Player model goes invisible on the map
/// 
/// </summary>
public class MuddleSensors : Ability
{
    public MuddleSensors()
    {
        abilityType = AbilityTypes.Muddle_Sensors;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 5;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        SpendScrap();
        GameManager.instance.GetActivePlayer().playerObject.GetComponent<MeshRenderer>().enabled = false;
        GameManager.instance.GetActivePlayer().AssignActiveAbility(this);
    }

    public override void Deactivate()
    {
        GameManager.instance.GetActivePlayer().playerObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
#endregion

#endregion

#region Corruption Abilities

#region Sensor Scan
/// <summary>
/// 
/// Scans surrounding rooms to determine if there are resources of a particular type present in those rooms
/// 
/// </summary>
public class SensorScan : Ability
{
    public SensorScan()
    {
        abilityType = AbilityTypes.Sensor_Scan;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 3;
        corruptionRequirement = 25;
    }

    /// <summary>
    /// 
    /// Returns a list of room indexes adjacent to the player which contain a resource of a particular type
    /// 
    /// </summary>
    /// <param name="resourceType">The type of resource which the player wants to check for</param>
    /// <returns>The room indexes of the adjacent rooms which contains the given resource</returns>
    public override List<int> Activate(ScanResources resourceType)
    {
        SpendScrap();

        List<int> roomIDs = new List<int>();
        //Gets the rooms which are adjacent to the active player
        List<Room> adjacentRooms = GameManager.instance.GetAdjacentRooms(GameManager.instance.GetActivePlayer().roomPosition);

        //Checks each room
        foreach (Room room in adjacentRooms)
        {
            //Checks each choice
            foreach (Choice choice in room.roomChoices)
            {
                //Checks if the resources are present in the current choice, adding the room's index to the list if it is present
                if((resourceType == ScanResources.Scrap && choice.scrapChange >= 0) ||
                    (resourceType == ScanResources.Item && choice.specItem.ItemType != Item.ItemTypes.Default) ||
                    (resourceType == ScanResources.Component && choice.component))
                {
                    roomIDs.Add(room.roomIndex);
                    //Breaks from the choice loop since if one of the choices contains the resource, the room must contain that resource
                    //and don't need to check any other choices.
                    break;
                }
            }
        }

        return roomIDs;
    }
}
#endregion

#region Code Inspection

/// <summary>
/// 
/// Player reveals if another player is a traitor 
/// 
/// </summary>
public class CodeInspection : Ability
{
    public CodeInspection()
    {
        abilityType = AbilityTypes.Code_Inspection;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 5;
        corruptionRequirement = 50;
    }

    public override void Activate(int targetIndex, out bool isTraitor)
    {
        SpendScrap();
        isTraitor = GameManager.instance.GetPlayer(targetIndex).isTraitor;
    }
}
#endregion

#region Sabotage
/// <summary>
/// 
/// Sabotages the escape shuttle so the next player who tries to install a component fails and takes 1 damage
/// 
/// </summary>
public class Sabotage : Ability
{
    public Sabotage()
    {
        abilityType = AbilityTypes.Sabotage;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 10;
        corruptionRequirement = 75;
    }

    public override void Activate()
    {
        SpendScrap();
        GameManager.instance.sabotageCharges += 1;
    }
}
#endregion

#region Super Charge
/// <summary>
/// 
/// Players increases their max life points by an amount and gains the same amount of life points
/// 
/// </summary>
public class SuperCharge : Ability
{
    private const int LIFE_MOD = 1;

    public SuperCharge()
    {
        abilityType = AbilityTypes.Supercharge;
        abilityDescription = "DESCRIPTION TO ADD";

        scrapCost = 15;
        corruptionRequirement = 100;
    }

    public override void Activate(int targetIndex)
    {
        SpendScrap();
        GameManager.instance.GetPlayer(targetIndex).maxLifePoints += LIFE_MOD;
        GameManager.instance.GetPlayer(targetIndex).lifePoints += LIFE_MOD;
    }
}
#endregion

#endregion