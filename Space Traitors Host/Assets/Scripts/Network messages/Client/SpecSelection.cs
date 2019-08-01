﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecSelection : NetMessage {
    public SpecSelection() {

        OperationCode = NetOP.SpecSelection;

    }


    public string SelectedSpec { set; get; }
}
