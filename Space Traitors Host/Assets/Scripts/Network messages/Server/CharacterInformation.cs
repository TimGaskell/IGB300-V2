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

    public int Basebrawn  { set; get; }
    public int Baseskill { set; get; }
    public int Basetech { set; get; }
    public int Basecharm { set; get; }
    public string name { set; get; }
    public string AbilityDescription { set; get; }

}

