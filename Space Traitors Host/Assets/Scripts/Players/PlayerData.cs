using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    //Class to store the information about other players on the client
    //Only stores the regularly needed information

    public int PlayerID { get; }
    public string PlayerName { get; }
    public Character.CharacterTypes CharacterType { get; }

    public PlayerData(int playerID, string playerName, Character.CharacterTypes characterType)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        CharacterType = characterType;
    }
}
