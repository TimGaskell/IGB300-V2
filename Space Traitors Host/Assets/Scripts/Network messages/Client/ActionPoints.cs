using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionPoints : NetMessage
{
    public ActionPoints() {

        OperationCode = NetOP.ActionPoints;

    }

    public int actionPoints { get; set; }
    
}
