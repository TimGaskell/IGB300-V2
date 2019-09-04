using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetOP
{
    public const int None = 0;

    #region ServerToClient
    public const int CharacterInformation = 1;
    public const int AbilityInformation = 2;
    public const int AvailableRooms = 3;
    public const int RoomChoices = 4;
    public const int SpecChallenge = 5;
    public const int PlayerInformation = 6;
    public const int TraitorSelction = 7;
    public const int SurgeInformation = 8;
    public const int AiAttacks = 9;
    public const int CombatWinner = 10;
    public const int CombatAvailablity = 11;
    public const int CombatBeingAttacked = 12;
    public const int PlayerElimination = 13;
    public const int NonTraitorVictory = 14;
    public const int ChangeScenes = 26;
    public const int IsActivePlayer = 27;
    public const int ChangeCharacter = 28;
    public const int TraitorVictory = 30;
    public const int PlayerDataSync = 31;
    public const int AbilityActivated = 32;
    public const int ComponentInstalled = 33;
    public const int NumComponentsInstalled = 34;
    public const int CanInstallComponent = 35;
    public const int CombatLoser = 36;
    public const int UnequipSuccess = 42;
    public const int EquipState = 43;
    public const int DiscardSuccess = 44;
    public const int StealSuccess = 45;
    public const int StealDiscardSuccess = 46;
    public const int ItemStolen = 47;
    public const int AIAttackResult = 49;
    public const int ComponentStealSuccess = 50;
    public const int SendRoomCost = 51;
    public const int ComponentStolen = 52;
    public const int SendAllPlayerNames = 54;
    public const int SendAllPlayerIDS = 55;
    public const int SendAllPlayerCharacterTypes = 56;


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
    public const int ActionPoints = 25;
    public const int NewPhase = 29;
    public const int EquipItem = 38;
    public const int DiscardItem = 39;
    public const int StealItem = 40;
    public const int StealDiscardItem = 41;
    public const int AISpecSelect = 48;
    public const int SelectRoom = 53;
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

