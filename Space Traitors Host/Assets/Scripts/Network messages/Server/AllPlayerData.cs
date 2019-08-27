using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AllPlayerData : NetMessage
{
    public AllPlayerData()
    {

        OperationCode = NetOP.AllPlayerData;

    }

    public int numPlayers { get; set; }
    public List<int> PlayerIDs { get; set; }
    public List<string> PlayerNames { get; set; }
    public List<int> CharacterTypes { get; set; }
}

