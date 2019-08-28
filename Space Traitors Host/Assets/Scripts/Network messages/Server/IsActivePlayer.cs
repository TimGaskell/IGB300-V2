using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IsActivePlayer : NetMessage {

    public IsActivePlayer() {

        OperationCode = NetOP.IsActivePlayer;

    }


}
