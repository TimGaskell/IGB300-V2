using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StealSuccess : NetMessage
{
    public StealSuccess()
    {

        OperationCode = NetOP.StealSuccess;

    }
    public bool IsSuccessful { get; set; }
}
