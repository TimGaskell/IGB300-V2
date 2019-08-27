using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatWinner : NetMessage
{
    public CombatWinner()
    {

        OperationCode = NetOP.CombatWinner;

    }

    public int LoserID { get; set; }
    public List<int>LoserInventory { get; set; } 

}

