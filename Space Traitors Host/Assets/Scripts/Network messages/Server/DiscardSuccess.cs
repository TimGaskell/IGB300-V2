using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiscardSuccess : NetMessage
{
    public DiscardSuccess()
    {

        OperationCode = NetOP.DiscardSuccess;

    }
}
