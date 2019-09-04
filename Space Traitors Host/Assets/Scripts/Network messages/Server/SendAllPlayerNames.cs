using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SendAllPlayerNames : NetMessage
{
    public SendAllPlayerNames()
    {

        OperationCode = NetOP.SendAllPlayerNames;

    }

    
    public List<string> PlayerNames { get; set; }

}
