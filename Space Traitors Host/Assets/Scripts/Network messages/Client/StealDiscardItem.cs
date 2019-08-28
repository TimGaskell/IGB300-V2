using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StealDiscardItem : NetMessage
{
    public StealDiscardItem()
    {

        OperationCode = NetOP.StealDiscardItem;

    }

    public int ItemID { get; set; }
    public int LoserID { get; set; }
}
