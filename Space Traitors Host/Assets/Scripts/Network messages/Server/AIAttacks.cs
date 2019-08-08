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

    public string spec { get; set; }
    public int specAmount { get; set; }
    public int damage { get; set; }


}

