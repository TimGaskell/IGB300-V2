using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatBeingAttacked : NetMessage
{
    public CombatBeingAttacked()
    {

        OperationCode = NetOP.CombatBeingAttacked;

    }

    public int AttackerID { get; set; }
    public int DefenderID { get; set; }
}

