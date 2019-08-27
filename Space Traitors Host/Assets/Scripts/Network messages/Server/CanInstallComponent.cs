using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CanInstallComponent : NetMessage
{
    public CanInstallComponent()
    {

        OperationCode = NetOP.CanInstallComponent;

    }

    public bool CanInstall { get; set; }
}
