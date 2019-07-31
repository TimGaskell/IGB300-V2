﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Movement : NetMessage {
    public Movement() {

        OperationCode = NetOP.Movement;

    }

    public int ActionPoints { set; get; }
    public int SelectedRoom { set; get; }
}
