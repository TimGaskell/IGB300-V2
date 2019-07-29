using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterInformation : NetMessage
{
    public CharacterInformation()
    {

        OperationCode = NetOP.CharacterInformation;

    }

    public int brawn  { set; get; }
    public int skill { set; get; }
    public int tech { set; get; }
    public int charm { set; get; }
   
}

