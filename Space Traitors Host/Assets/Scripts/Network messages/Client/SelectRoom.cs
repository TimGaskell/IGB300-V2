using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectRoom : NetMessage {
    public SelectRoom() {

        OperationCode = NetOP.SelectRoom;

    }

    public int roomID { get; set; }
   
}
