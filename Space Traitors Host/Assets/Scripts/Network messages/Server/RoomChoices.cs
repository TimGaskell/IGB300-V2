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

    public string[] ChoiceNames { get; set; }
    public string[] SuccessTexts { get; set; }
    public string[] FailTexts { get; set; }
    public int[] IsAvailables { get; set; }
    public int[] SpecScores { get; set; }
    public float[] SuccessChances { get; set; }
    public List<int> AttackablePlayers { get; set; }

}

