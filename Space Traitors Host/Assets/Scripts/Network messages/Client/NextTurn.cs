using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NextTurn : NetMessage {
    public NextTurn() {

        OperationCode = NetOP.NextTurn;

    }
}
