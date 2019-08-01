using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatResolution : NetMessage
{
    public CombatResolution()
    {

        OperationCode = NetOP.CombatResolution;

    }

    public bool winbattle { get; set; }
    public int damagetaken { get; set; }
    public List<string>LoserInventory { get; set; } // change from string to appropriate way of handeling

}

