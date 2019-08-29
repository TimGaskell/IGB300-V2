using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentStealSuccess : NetMessage
{
    public ComponentStealSuccess()
    {

        OperationCode = NetOP.ComponentStealSuccess;

    }

    public bool IsSuccessful { get; set; }
}
