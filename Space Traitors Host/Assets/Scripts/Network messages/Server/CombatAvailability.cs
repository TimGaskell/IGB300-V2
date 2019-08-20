using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAvailability: NetMessage
{
    public CombatAvailability()
    {

        OperationCode = NetOP.CombatAvailablity;

    }

    public List<int>Players { get; set; } 

}

