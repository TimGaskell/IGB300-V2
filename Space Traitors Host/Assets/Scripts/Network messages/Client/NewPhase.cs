using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewPhase : NetMessage {
    public NewPhase() {

        OperationCode = NetOP.NewPhase;

    }
}
