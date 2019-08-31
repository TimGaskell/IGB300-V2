using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AiAttacks : NetMessage
{
    public AiAttacks()
    {

        OperationCode = NetOP.AiAttacks;

    }

    public int TargetID { get; set; }
    public bool IsTarget { get; set; }

}

