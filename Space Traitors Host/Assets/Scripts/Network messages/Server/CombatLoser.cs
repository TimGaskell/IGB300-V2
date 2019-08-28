using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatLoser : NetMessage
{
    public CombatLoser()
    {

        OperationCode = NetOP.CombatLoser;

    }

    public int WinnerID { get; set; }
}

