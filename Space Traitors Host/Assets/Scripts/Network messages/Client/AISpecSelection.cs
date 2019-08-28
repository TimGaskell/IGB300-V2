using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AISpecSelection : NetMessage {
    public AISpecSelection() {

        OperationCode = NetOP.AISpecSelect;

    }


    public int SelectedSpec { set; get; }
}
