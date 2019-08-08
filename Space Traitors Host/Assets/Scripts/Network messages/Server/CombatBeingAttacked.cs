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



}

