using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SendPlayerItems : NetMessage {

    public SendPlayerItems() {

        OperationCode = NetOP.SendPlayerItems;

    }

    public List<int> Items { get; set; }
    public List<bool> ItemEquipped { get; set; }
}