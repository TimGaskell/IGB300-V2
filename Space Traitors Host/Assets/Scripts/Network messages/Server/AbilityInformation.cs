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

    public int[] AbilityTypes { get; set; }
    public bool[] CheckCorruption { get; set; }
    public bool [] CheckScrap { get; set; }
}

