using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Disconnection: NetMessage {

    public Disconnection() {

        OperationCode = NetOP.SendPlayerDisconnect;

    }

    public int PlayerID { get; set; }
    public string PlayerName { get; set; }
}
