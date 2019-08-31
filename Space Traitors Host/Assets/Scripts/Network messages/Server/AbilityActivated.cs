using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityActivated : NetMessage
{
    public AbilityActivated()
    {

        OperationCode = NetOP.AbilityActivated;

    }

    public int AbilityType { get; set; }
    public bool IsTraitor { get; set; }
}
