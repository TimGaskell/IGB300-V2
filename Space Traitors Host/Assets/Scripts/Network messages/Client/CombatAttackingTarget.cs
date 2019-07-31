using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAttackingTarget : NetMessage {
    public CombatAttackingTarget() {

        OperationCode = NetOP.CombatAttackingTarget;

    }


    public GameObject SelectedPlayer { set; get; }
}
