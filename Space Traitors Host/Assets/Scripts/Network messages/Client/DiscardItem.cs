using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiscardItem : NetMessage
{
    public DiscardItem()
    {

        OperationCode = NetOP.DiscardItem;

    }

    public int ItemID { get; set; }
}
