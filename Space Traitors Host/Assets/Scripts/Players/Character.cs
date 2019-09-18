using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character
{
    public enum CharacterTypes { Default, Brute, Butler, Chef, Engineer, Singer, Techie };

    public CharacterTypes CharacterType { get; private set; }

    public string CharacterName { get { return CharacterType.ToString(); } }

    public int baseBrawn;
    public int baseSkill;
    public int baseTech;
    public int baseCharm;
    public int abilityIcon; //used for icon retrieval

    public Ability characterAbility;

    /// <summary>
    /// 
    /// Define a default character
    /// 
    /// </summary>
    public Character ()
    {
        CharacterType = CharacterTypes.Default;

        baseBrawn = 0;
        baseSkill = 0;
        baseTech = 0;
        baseCharm = 0;

        characterAbility = new Ability();
    }

    /// <summary>
    /// 
    /// Define a character of a particular type
    /// 
    /// </summary>
    /// <param name="characterType">The name of the character to be created</param>
    public Character (CharacterTypes characterType)
    {
        CharacterType = characterType;

        AssignCharacterInfo();
    }

    /// <summary>
    /// 
    /// Define the character stats based on their character name. If the character does not exist, then has default stats.
    /// 
    /// </summary>
    private void AssignCharacterInfo()
    {
        switch (CharacterType)
        {
            case CharacterTypes.Brute:
                baseBrawn = 0;
                baseSkill = 0;
                baseTech = 0;
                baseCharm = 0;
                characterAbility = new Shove();
                break;
            case CharacterTypes.Butler:
                baseBrawn = 4;
                baseSkill = 6;
                baseTech = 2;
                baseCharm = 3;
                abilityIcon = 0;
                characterAbility = new SecretPaths();
                break;
            case CharacterTypes.Chef:
                baseBrawn = 0;
                baseSkill = 0;
                baseTech = 0;
                baseCharm = 0;
                characterAbility = new PowerBoost();
                break;
            case CharacterTypes.Engineer:
                baseBrawn = 6;
                baseSkill = 3;
                baseTech = 4;
                baseCharm = 2;
                abilityIcon = 1;
                characterAbility = new PowerBoost();
                break;
            case CharacterTypes.Singer:
                baseBrawn = 2;
                baseSkill = 4;
                baseTech = 3;
                baseCharm = 6;
                abilityIcon = 2;
                characterAbility = new EncouragingSong();
                break;
            case CharacterTypes.Techie:
                baseBrawn = 3;
                baseSkill = 2;
                baseTech = 6;
                baseCharm = 4;
                abilityIcon = 3;
                characterAbility = new MuddleSensors();
                break;
            default:
                CharacterType = CharacterTypes.Default;
                baseBrawn = 0;
                baseSkill = 0;
                baseTech = 0;
                baseCharm = 0;
                characterAbility = new Ability();
                break;
        }
    }
}