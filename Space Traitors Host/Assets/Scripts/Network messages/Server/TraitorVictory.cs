using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TraitorVictory : NetMessage
{
    public TraitorVictory()
    {

        OperationCode = NetOP.TraitorVictory;

    }

    public int WinnerID { get; set; }
}

