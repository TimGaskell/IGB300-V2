using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EndAttack : NetMessage {
    public EndAttack() {

        OperationCode = NetOP.EndAttack;

    }

}

