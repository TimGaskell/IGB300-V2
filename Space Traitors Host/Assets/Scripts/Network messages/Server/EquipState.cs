using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipState : NetMessage
{
    public EquipState()
    {

        OperationCode = NetOP.EquipState;

    }

    public int EquipError { get; set; }
}
