using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSelection : NetMessage {
    public ItemSelection() {

        OperationCode = NetOP.ItemSelection;

    }


    public string SelectedItem { set; get; }
}
