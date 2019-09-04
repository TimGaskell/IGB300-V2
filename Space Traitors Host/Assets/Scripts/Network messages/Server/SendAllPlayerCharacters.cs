using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SendAllPlayerCharacters : NetMessage
{
    public SendAllPlayerCharacters()
    {

        OperationCode = NetOP.SendAllPlayerCharacterTypes;

    }
    public List<int> CharacterTypes { get; set; }

}

