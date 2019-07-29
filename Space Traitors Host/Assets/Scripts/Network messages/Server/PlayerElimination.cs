using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerElimination: NetMessage
{
    public PlayerElimination()
    {

        OperationCode = NetOP.PlayerElimination;

    }



}


