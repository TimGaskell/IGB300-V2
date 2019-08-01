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

    public int AiPowerIncrease{ get; set; }

}

