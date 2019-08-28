using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDetails : NetMessage {

    public PlayerDetails() {

        OperationCode = NetOP.PlayerDetails;

    }

    public string PlayerName { set; get; }
}
