using System;
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

    public enum ScanResources { Default, Scrap, Items, Components }

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
        abilityDescription = "Reduce action point cost of all paths by 1 for a player on their next turn.";

        scrapCost = 6;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        SpendScrap();

       
        GameManager.instance.GetPlayer(targetIndex).AssignActiveAbility(this);

        Debug.Log("Activated On " + GameManager.instance.GetPlayer(targetIndex).playerName);

        Debug.Log("shh its a secret");
    }

    public override void Deactivate()
    {


        GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).activeAbilitys.Remove(this);

        Debug.Log("secret path is no longer on");
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
        abilityDescription = "Give a player +1 in all their spec scores until the start of your next turn.";

        scrapCost = 8;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        SpendScrap();
        Debug.Log("Brawn Before : " + GameManager.instance.GetPlayer(targetIndex).brawnModTemp);
        GameManager.instance.GetPlayer(targetIndex).brawnModTemp += SPEC_MOD;
        GameManager.instance.GetPlayer(targetIndex).skillModTemp += SPEC_MOD;
        GameManager.instance.GetPlayer(targetIndex).techModTemp += SPEC_MOD;
        GameManager.instance.GetPlayer(targetIndex).charmModTemp += SPEC_MOD;

        GameManager.instance.GetPlayer(targetIndex).AssignActiveAbility(this);
        Debug.Log("beefed up");
        Debug.Log("Brawn after : " + GameManager.instance.GetPlayer(targetIndex).brawnModTemp);
    }

    public override void Deactivate()
    {
        Debug.Log("Brawn before : " + GameManager.instance.GetActivePlayer().brawnModTemp);
        GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).activeAbilitys.Remove(this);
        GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).brawnModTemp -= SPEC_MOD;
        GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).skillModTemp -= SPEC_MOD;
        GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).techModTemp -= SPEC_MOD;
        GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).charmModTemp -= SPEC_MOD;
        Debug.Log("beefed down");
        Debug.Log("Brawn after : " + GameManager.instance.GetActivePlayer().brawnModTemp);
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
        abilityDescription = "Reduce a player's corruption by 15%.";

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
        Debug.Log("Corruption Before : " + GameManager.instance.GetPlayer(targetIndex).Corruption);
        GameManager.instance.GetPlayer(targetIndex).Corruption -= CORRUPTION_MOD;
        Debug.Log("Corruption Before : " + GameManager.instance.GetPlayer(targetIndex).Corruption);

        if (GameManager.instance.GetPlayer(targetIndex).Corruption < 0) {

            GameManager.instance.GetPlayer(targetIndex).Corruption = 0;
        }
        Debug.Log("Thats encouraging");
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
        abilityDescription = "Turn a player invisible until the start of your next turn.";

        scrapCost = 5;
        corruptionRequirement = 0;
    }

    public override void Activate(int targetIndex)
    {
        SpendScrap();

        if (GameManager.instance.GetPlayer(targetIndex).playerObject.GetComponentsInChildren<SkinnedMeshRenderer>() != null) {

            Debug.Log("This happens");
            Component[] Mesh = GameManager.instance.GetPlayer(targetIndex).playerObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer meshRenderer in Mesh) {

                meshRenderer.enabled = false;
            }

           
        }

        if (GameManager.instance.GetPlayer(targetIndex).playerObject.GetComponentsInChildren<MeshRenderer>() != null) {

            Component[] Mesh = GameManager.instance.GetPlayer(targetIndex).playerObject.GetComponentsInChildren<MeshRenderer>();

            Debug.Log(Mesh.Length);
            foreach (MeshRenderer meshRenderer in Mesh) {

                meshRenderer.enabled = false;
            }

        }

       
        GameManager.instance.GetPlayer(targetIndex).AssignActiveAbility(this);
        Debug.Log("Goin invisable");
    }

    public override void Deactivate()
    {
        if (GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).playerObject.GetComponentInChildren<SkinnedMeshRenderer>() != null) {

            Component[] Mesh = GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).playerObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer meshRenderer in Mesh) {

                meshRenderer.enabled = true;
            }

        }
        if (GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).playerObject.GetComponentsInChildren<MeshRenderer>() != null) {

            Component[] Mesh = GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).playerObject.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer meshRenderer in Mesh) {

                meshRenderer.enabled = true;
            }
        }

        GameManager.instance.GetPlayer(GameManager.instance.GetActivePlayer().PreviousTarget).activeAbilitys.Remove(this);
        Debug.Log("wait can they see me now?");
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
        abilityDescription = "Scan the adjacent rooms to you for scrap, items or components.";

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
                    (resourceType == ScanResources.Items && choice.specItem.ItemType != Item.ItemTypes.Default) ||
                    (resourceType == ScanResources.Components && choice.component))
                {
                    roomIDs.Add(room.roomIndex);
                    //Breaks from the choice loop since if one of the choices contains the resource, the room must contain that resource
                    //and don't need to check any other choices.
                    Debug.Log("Room Id: " + room.roomIndex);
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
        abilityDescription = "Inspect a players code to see if they are a traitor or not.";

        scrapCost = 5;
        corruptionRequirement = 50;
    }

    public override void Activate(int targetIndex, out bool isTraitor)
    {
        SpendScrap();
        isTraitor = GameManager.instance.GetPlayer(targetIndex).isTraitor;
        Debug.Log("Is Traitor: " + isTraitor);
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
        abilityDescription = "Sabotage the escape shuttle so the next player to install a component takes damage.";

        scrapCost = 10;
        corruptionRequirement = 75;
    }

    public override void Activate()
    {
        SpendScrap();
        GameManager.instance.sabotageCharges += 1;
        Debug.Log("Sabotaged");
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
        abilityDescription = "Increase a player's max life points by 1 and gain a life point.";

        scrapCost = 15;
        corruptionRequirement = 100;
    }

    public override void Activate(int targetIndex)
    {
        SpendScrap();
        Debug.Log("Health Before : " + GameManager.instance.GetPlayer(targetIndex).maxLifePoints);
        GameManager.instance.GetPlayer(targetIndex).maxLifePoints += LIFE_MOD;
        GameManager.instance.GetPlayer(targetIndex).ChangeLifePoints(LIFE_MOD);
        Debug.Log("Health After : " + GameManager.instance.GetPlayer(targetIndex).maxLifePoints);
    }
}
#endregion

#endregion