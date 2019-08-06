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

    public int scaledbrawn { set; get; }
    public int scaledskill { set; get; }
    public int scaledtech { set; get; }
    public int scaledcharm { set; get; }
    public int scrap { set; get; }
    public float corruption { set; get; }
    public int lifepoints { set; get; }
    public List<string> EquippedItems { set; get; } //Be sure to change from string to suit how this is handled
    public List<string> UnEquippedItems { set; get; }
    public bool isTraitor { set; get; }



}

