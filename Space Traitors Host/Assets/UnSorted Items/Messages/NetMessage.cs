using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetOP
{
    public const int None = 0;

    #region ServerToClient
    public const int CharacterInformation = 1;
    public const int AbilityInformation = 2;
    public const int AvilableRooms = 3;
    public const int RoomChoices = 4;
    public const int SpecChallenge = 5;
    public const int PlayerInformation = 6;
    public const int TraitorSelction = 7;
    public const int SurgeInformation = 8;
    public const int AiAttacks = 9;
    public const int CombatResolution = 10;
    public const int CombatAvailablity = 11;
    public const int CombatBeingAttacked = 12;
    public const int PlayerElimination = 13;
    public const int NonTraitorVictory = 14;


    #endregion

    #region ClientToServer
    public const int PlayerDetails = 15;
    public const int CharacterSelection = 16 ;
    public const int AbilityUsage = 17;
    public const int Movement = 18 ;
    public const int SelectedChoice = 19 ;
    public const int InventoryChanges = 20;
    public const int SpecSelection = 21;
    public const int CombatAttackingTarget = 22 ;
    public const int ItemSelection = 23;
    public const int InstallComponent = 24 ;
    #endregion
}

[System.Serializable]
public abstract class NetMessage
{
    public byte OperationCode { set; get; }
    
    public NetMessage()
    {
        OperationCode = NetOP.None;
    }
}

