using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomChoices : NetMessage
{
    public RoomChoices()
    {

        OperationCode = NetOP.RoomChoices;

    }


    //Need to find out what needs to be sent over to the player to display the right choices
    public int brawn { set; get; }
    

}

