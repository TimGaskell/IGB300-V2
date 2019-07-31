using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityUsage : NetMessage {
    public AbilityUsage() {

        OperationCode = NetOP.AbilityUsage;

    }


    public string Ability { set; get; }
    public GameObject target { set; get; }
}
