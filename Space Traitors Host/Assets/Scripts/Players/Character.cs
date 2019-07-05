using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character
{
    public string characterName;

    public int baseBrawn;
    public int baseSkill;
    public int baseTech;
    public int baseCharm;

    Ability characterAbility;

    public Character ()
    {
        characterName = "Default";

        baseBrawn = 0;
        baseSkill = 0;
        baseTech = 0;
        baseCharm = 0;

        characterAbility = new Ability();
    }

    public Character (string characterType)
    {
        characterName = characterType;

        AssignSpecScores();
    }

    private void AssignSpecScores()
    {
        switch (characterName)
        {
            case "Brute":
                baseBrawn = 6;
                baseSkill = 3;
                baseTech = 2;
                baseCharm = 4;
                break;
            case "Butler":
                baseBrawn = 4;
                baseSkill = 5;
                baseTech = 3;
                baseCharm = 3;
                break;
            case "Chef":
                baseBrawn = 3;
                baseSkill = 6;
                baseTech = 4;
                baseCharm = 2;
                break;
            case "Engineer":
                baseBrawn = 4;
                baseSkill = 3;
                baseTech = 5;
                baseCharm = 3;
                break;
            case "Singer":
                baseBrawn = 2;
                baseSkill = 5;
                baseTech = 2;
                baseCharm = 6;
                break;
            case "Techie":
                baseBrawn = 2;
                baseSkill = 2;
                baseTech = 6;
                baseCharm = 5;
                break;
            default:
                baseBrawn = 0;
                baseSkill = 0;
                baseTech = 0;
                baseCharm = 0;
                break;
        }
    }
}
