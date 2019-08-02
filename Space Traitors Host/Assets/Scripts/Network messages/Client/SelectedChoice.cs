using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectedChoice : NetMessage {
    public SelectedChoice() {

        OperationCode = NetOP.SelectedChoice;

    }


    public int ChoiceId { set; get; }
    
}
