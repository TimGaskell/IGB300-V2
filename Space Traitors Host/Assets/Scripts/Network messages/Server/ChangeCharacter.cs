using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeCharacter : NetMessage {
    public ChangeCharacter() {

        OperationCode = NetOP.ChangeCharacter;

    }

    public bool AlreadySelected { get; set; }


}
