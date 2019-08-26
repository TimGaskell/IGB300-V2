using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NumComponentsInstalled : NetMessage
{
    public NumComponentsInstalled()
    {

        OperationCode = NetOP.NumComponentsInstalled;

    }

    public int InstalledComponents { get; set; }
    public bool AllComponentsInstalled { get; set; }
}
