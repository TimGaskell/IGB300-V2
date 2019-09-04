using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SendAllPlayerIDS : NetMessage
{
    public SendAllPlayerIDS()
    {

        OperationCode = NetOP.SendAllPlayerIDS;

    }


    public List<int> PlayerIDS { get; set; }

}
