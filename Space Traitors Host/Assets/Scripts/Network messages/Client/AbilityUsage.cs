using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityUsage : NetMessage {
    public AbilityUsage() {

        OperationCode = NetOP.AbilityUsage;

    }


    public int AbilityType { set; get; }
    public int TargetID { set; get; }
    public int ScanResource { set; get; }
}
