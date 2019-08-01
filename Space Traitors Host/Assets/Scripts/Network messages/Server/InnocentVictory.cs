using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InnocentVictory : NetMessage
{
    public InnocentVictory()
    {

        OperationCode = NetOP.NonTraitorVictory;

    }


}

