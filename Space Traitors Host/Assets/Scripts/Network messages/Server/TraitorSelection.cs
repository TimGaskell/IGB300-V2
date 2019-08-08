using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TraitorSelection : NetMessage
{
    public TraitorSelection()
    {

        OperationCode = NetOP.TraitorSelction;

    }

    public bool isTraitor { get; set; }

}

