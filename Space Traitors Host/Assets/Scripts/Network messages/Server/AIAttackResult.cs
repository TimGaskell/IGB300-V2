using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIAttackResult : NetMessage
{
    public AIAttackResult()
    {

        OperationCode = NetOP.AIAttackResult;

    }

    public bool WonAttack { get; set; }
}
