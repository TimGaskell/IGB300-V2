using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentInstalled : NetMessage
{
    public ComponentInstalled()
    {
        OperationCode = NetOP.ComponentInstalled;
    }

    public bool SuccessfulInstall { get; set; }
    public bool AllComponentsInstalled { get; set; }
}
