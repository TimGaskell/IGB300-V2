using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SendRoomCost : NetMessage {
    public SendRoomCost() {

        OperationCode = NetOP.SendRoomCost;

    }

    public int RoomCost { get; set; }
    public int ScrapReturn { get; set; }

}