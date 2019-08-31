using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipItem : NetMessage
{
    public EquipItem()
    {

        OperationCode = NetOP.EquipItem;

    }

    public int ItemID { get; set; }
}
