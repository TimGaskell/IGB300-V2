﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DisconnectChoice : NetMessage {

    public DisconnectChoice() {

        OperationCode = NetOP.DisconnectChoice;

    }

    public bool InGame { get; set; }

}