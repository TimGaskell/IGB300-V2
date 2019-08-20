using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataSync : NetMessage
{
    public PlayerDataSync()
    {

        OperationCode = NetOP.PlayerDataSync;

    }

    public int Scrap { get; set; }
    public int Corruption { get; set; }
    public bool HasComponent { get; set; }
    public int LifePoints { get; set; }
    public int MaxLifePoints { get; set; }
    public float ScaledBrawn { get; set; }
    public float ScaledSkill { get; set; }
    public float ScaledTech { get; set; }
    public float ScaledCharm { get; set; }
    public int ModBrawn { get; set; }
    public int ModSkill { get; set; }
    public int ModTech { get; set; }
    public int ModCharm { get; set; }
    public List<int> Items { get; set; }
    public List<bool> ItemEquipped { get; set; }
}

