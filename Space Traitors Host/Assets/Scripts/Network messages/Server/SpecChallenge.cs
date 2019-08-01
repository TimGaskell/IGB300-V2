using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecChallenge : NetMessage
{
    public SpecChallenge()
    {

        OperationCode = NetOP.SpecChallenge;

    }

    public bool result { get; set; }

}

