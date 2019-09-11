using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurgeInformation : NetMessage
{
    public SurgeInformation()
    {

        OperationCode = NetOP.SurgeInformation;

    }

    public float NewAiPower{ get; set; }
    public float PowerIncrease { get; set; }
    public float PlayerIncrease { get; set; }
    public float ChoiceIncrease { get; set; }
    public float baseIncrease { get; set; }

}

