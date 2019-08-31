using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentStolen : NetMessage
{
    public ComponentStolen()
    {

        OperationCode = NetOP.ComponentStolen;

    }
}
