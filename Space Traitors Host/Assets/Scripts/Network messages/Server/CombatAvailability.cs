using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAvilability: NetMessage
{
    public CombatAvilability()
    {

        OperationCode = NetOP.CombatAvailablity;

    }

    public List<GameObject>Players { get; set; } //list of gameobjects of the players nearby

}

