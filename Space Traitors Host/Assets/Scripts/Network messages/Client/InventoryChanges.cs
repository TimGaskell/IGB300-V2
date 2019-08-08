using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryChanges : NetMessage {
    public InventoryChanges() {

        OperationCode = NetOP.InventoryChanges;

    }


    public List<string> equipedItems { set; get; }
    public List<string> UnequipedItems { set; get; }
    public List<string> discardItems { set; get; }
}
