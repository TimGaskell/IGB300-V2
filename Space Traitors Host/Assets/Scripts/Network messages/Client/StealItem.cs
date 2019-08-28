using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StealItem : NetMessage
{
    public StealItem()
    {

        OperationCode = NetOP.StealItem;

    }

    public int ItemID { get; set; }
    public int LoserID { get; set; }
}
