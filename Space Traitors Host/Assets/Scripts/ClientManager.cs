using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ClientManager : MonoBehaviour
{
    public const string MAIN_MENU_SCENE = "Client MainMenu";
    public const string LOBBY_SCENE = "LobbyLan Client";
    public const string NAME_ENTRY_SCENE = "Client NameEntry";
    public const string CHARACTER_SELECTION_SCENE = "Client CharacterSelect";
    public const string MAIN_GAME_SCENE = "Client GameLevel";

    public static ClientManager instance = null;

    public int playerID;
    public string playerName;

    public int scrap;
    public int corruption;
    public bool hasComponent;
    public bool isTraitor;
    public int lifePoints;
    public int maxLifePoints;
    public int ActionPoints;

    //Spec scores for player which are scaled due to corruption
    public float scaledBrawn;
    public float scaledSkill;
    public float scaledTech;
    public float scaledCharm;

    //Spec scores for player which are not scaled due to corruption,
    //but includes increases from items and abilities
    public int modBrawn;
    public int modSkill;
    public int modTech;
    public int modCharm;

    public List<Item> inventory;
    public List<Ability> abilities;
    public Character PlayerCharacter;

    Character.CharacterTypes characterType;

    List<Character> allCharacters;
    List<Ability> allAbilities;
    List<Item> allItems;

    public List<Sprite> characterPortraits;

    public int componentsInstalled;

    //Object for storing regularly needed information about other players in the game
    public List<PlayerData> playerData;
    public List<int> playerIDS;
    public List<int> CharacterTypes;
    public List<string> PlayerNames;

    #region Client Initialisation
    private void Awake()
    {
        //Singleton Setup
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        InitInfoLists();
    }

    /// <summary>
    /// 
    /// Initialise the lists containing information about characters, abilities and items.
    /// 
    /// </summary>
    private void InitInfoLists()
    {
        List<Item.ItemTypes> itemTypes = Enum.GetValues(typeof(Item.ItemTypes)).Cast<Item.ItemTypes>().ToList();
        allItems = new List<Item>();

        foreach (Item.ItemTypes itemType in itemTypes)
        {
            allItems.Add(new Item(itemType));
        }

        List<Character.CharacterTypes> characterTypes = Enum.GetValues(typeof(Character.CharacterTypes)).Cast<Character.CharacterTypes>().ToList();
        allCharacters = new List<Character>();

        foreach (Character.CharacterTypes characterType in characterTypes)
        {
            allCharacters.Add(new Character(characterType));
        }

        List<Ability.AbilityTypes> abilityTypes = Enum.GetValues(typeof(Ability.AbilityTypes)).Cast<Ability.AbilityTypes>().ToList();
        allAbilities = new List<Ability>();

        foreach (Ability.AbilityTypes abilityType in abilityTypes)
        {
            switch (abilityType)
            {
                case (Ability.AbilityTypes.Default):
                    allAbilities.Add(new Ability());
                    break;
                case (Ability.AbilityTypes.Shove):
                    allAbilities.Add(new Shove());
                    break;
                case (Ability.AbilityTypes.Secret_Paths):
                    allAbilities.Add(new SecretPaths());
                    break;
                case (Ability.AbilityTypes.Power_Boost):
                    allAbilities.Add(new PowerBoost());
                    break;
                case (Ability.AbilityTypes.Quick_Repair):
                    allAbilities.Add(new QuickRepair());
                    break;
                case (Ability.AbilityTypes.Encouraging_Song):
                    allAbilities.Add(new EncouragingSong());
                    break;
                case (Ability.AbilityTypes.Muddle_Sensors):
                    allAbilities.Add(new MuddleSensors());
                    break;
                case (Ability.AbilityTypes.Sensor_Scan):
                    allAbilities.Add(new SensorScan());
                    break;
                case (Ability.AbilityTypes.Code_Inspection):
                    allAbilities.Add(new CodeInspection());
                    break;
                case (Ability.AbilityTypes.Sabotage):
                    allAbilities.Add(new Sabotage());
                    break;
                case (Ability.AbilityTypes.Supercharge):
                    allAbilities.Add(new SuperCharge());
                    break;
                default:
                    throw new NotImplementedException("Not a valid ability type");
            }
        }
    }

    #endregion

    /// <summary>
    /// 
    /// Gets the character portrait for a character of a particular type
    /// 
    /// </summary>
    /// <param name="characterType">The required character type</param>
    /// <returns>Returns the sprite image of the relevant character type</returns>
    public Sprite GetCharacterPortrait(Character.CharacterTypes characterType)
    {
        switch (characterType)
        {
            case (Character.CharacterTypes.Brute):
                return characterPortraits[0];
            case (Character.CharacterTypes.Butler):
                return characterPortraits[1];
            case (Character.CharacterTypes.Chef):
                return characterPortraits[2];
            case (Character.CharacterTypes.Engineer):
                return characterPortraits[3];
            case (Character.CharacterTypes.Singer):
                return characterPortraits[4];
            case (Character.CharacterTypes.Techie):
                return characterPortraits[5];
            default:
                throw new NotImplementedException("Not a valid character type.");
        }
    }

   

    #region Enum Converters

    public Character GetCharacterInfo(int characterID)
    {
        return allCharacters[characterID];
    }

    public Ability GetAbilityInfo (int abilityID)
    {
        return allAbilities[abilityID];
    }

    public Item GetItemInfo(int itemID)
    {
        return allItems[itemID];
    }

    #endregion

    public PlayerData GetPlayerData(int playerID)
    {
        return playerData.Find(x => x.PlayerID == playerID);
    }

    public PlayerData GetPlayer(Character.CharacterTypes characterType) {
       return playerData.Find(x => x.CharacterType == characterType);
    }

}
