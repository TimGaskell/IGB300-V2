using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Disconnection: NetMessage {

    public Disconnection() {

        OperationCode = NetOP.SendPlayerDisconnect;

    }

    public string PlayerName { get; set; }
}
