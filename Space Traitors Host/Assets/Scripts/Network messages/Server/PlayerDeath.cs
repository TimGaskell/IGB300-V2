using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDeath : NetMessage {

    public PlayerDeath() {

        OperationCode = NetOP.PlayerDeath;

    }

    public int PlayerDeathId { get; set; }
}
