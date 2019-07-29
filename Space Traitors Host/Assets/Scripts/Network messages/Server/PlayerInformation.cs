using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInformation: NetMessage
{
    public PlayerInformation()
    {

        OperationCode = NetOP.PlayerInformation;

    }

    public int basebrawn { set; get; }
    public int baseskill { set; get; }
    public int basetech { set; get; }
    public int basecharm { set; get; }
    public int scaledbrawn { set; get; }
    public int scaledskill { set; get; }
    public int scaledtech { set; get; }
    public int scaledcharm { set; get; }
    public int scrap { set; get; }
    public int corruption { set; get; }
    public int lifepoints { set; get; }
    public List<string> EquippedItems { set; get; } //Be sure to change from string to suit how this is handled
    public List<string> UnEquippedItems { set; get; }
    public bool isTraitor { set; get; }



}

