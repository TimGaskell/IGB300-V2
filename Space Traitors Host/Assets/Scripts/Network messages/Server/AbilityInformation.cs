using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityInformation: NetMessage
{
    public AbilityInformation()
    {

        OperationCode = NetOP.AbilityInformation;

    }

    public bool CanUseAbility { set; get; }
}

