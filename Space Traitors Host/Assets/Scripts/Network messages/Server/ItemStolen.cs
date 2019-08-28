using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStolen : NetMessage
{
    public ItemStolen()
    {

        OperationCode = NetOP.ItemStolen;

    }

    public string ItemName { get; set; }
}
