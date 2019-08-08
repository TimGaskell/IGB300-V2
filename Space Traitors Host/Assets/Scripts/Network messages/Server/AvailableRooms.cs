using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AvailableRooms : NetMessage
{
    public AvailableRooms()
    {

        OperationCode = NetOP.AvailableRooms;

    }

    public List<int> AvailableRoomsIDs { get; set; }
}

