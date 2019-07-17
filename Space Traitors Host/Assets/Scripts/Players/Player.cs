using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public const int STARTING_ROOM_ID = 9; //Players always start in the escape room i.e. room 9

    public const int MAX_ITEMS = 4;
    public const int MAX_EQUIPPED_ITEMS = 2;

    public const int BASE_LIFE_POINTS = 3;

    //playerID should mirror the connection ID of the player to know which client information needs to be passed to
    public int playerID;
    //The name the player inputs when they start the game
    public string playerName;

    public int roomPosition;

    //Player Resources
    public int scrap;
    public int corruption;
    public Item[] items;
    public Item[] equippedItems;
    public bool hasComponent;
    public int lifePoints;
    public int maxLifePoints;

    public bool IsDead { get { return lifePoints == 0; } }

    public string CharacterType { get { return characterType.characterName; } set { characterType = new Character(value); } }
    public Character characterType;

    #region Item Changes on Spec Scores
    //Changes to spec scores which originate from the player's items
    private int BrawnChange
    {
        get
        {
            int brawnScore = 0;
            for (int itemID = 0; itemID < MAX_EQUIPPED_ITEMS; itemID++)
            {
                brawnScore += items[itemID].brawnChange;
            }

            return brawnScore;
        }
    }
    private int SkillChange
    {
        get
        {
            int skillScore = 0;
            for (int itemID = 0; itemID < MAX_EQUIPPED_ITEMS; itemID++)
            {
                skillScore += items[itemID].skillChange;
            }

            return skillScore;
        }
    }
    private int TechChange
    {
        get
        {
            int techScore = 0;
            for (int itemID = 0; itemID < MAX_EQUIPPED_ITEMS; itemID++)
            {
                techScore += items[itemID].techChange;
            }

            return techScore;
        }
    }
    private int CharmChange
    {
        get
        {
            int charmScore = 0;
            for (int itemID = 0; itemID < MAX_EQUIPPED_ITEMS; itemID++)
            {
                charmScore += items[itemID].charmChange;
            }

            return charmScore;
        }
    }
    #endregion

    //Output the spec scores scaled by their corruption. Should be readonly so only get is defined
    public int ScaledBrawn { get { return ApplyScaling(characterType.baseBrawn, BrawnChange); } }
    public int ScaledSkill { get { return ApplyScaling(characterType.baseSkill, SkillChange); } }
    public int ScaledTech { get { return ApplyScaling(characterType.baseTech, TechChange); } }
    public int ScaledCharm { get { return ApplyScaling(characterType.baseCharm, CharmChange); } }

    public Player(int PlayerID, string PlayerName)
    {
        playerID = PlayerID;
        playerName = PlayerName;

        roomPosition = STARTING_ROOM_ID;

        scrap = 0;
        corruption = 0;

        items = new Item[MAX_ITEMS];
        for (int itemIndex = 0; itemIndex < MAX_ITEMS; itemIndex++)
        {
            items[itemIndex] = new Item();
        }

        equippedItems = new Item[MAX_EQUIPPED_ITEMS];
        for (int itemIndex = 0; itemIndex < MAX_EQUIPPED_ITEMS; itemIndex++)
        {
            equippedItems[itemIndex] = new Item();
        }

        hasComponent = false;
        lifePoints = BASE_LIFE_POINTS;
        maxLifePoints = BASE_LIFE_POINTS;

        characterType = new Character();
    }

    public Player(int PlayerID, string PlayerName, string CharacterType) : this(PlayerID, PlayerName)
    {
        characterType = new Character(CharacterType);
    }

    /// <summary>
    /// 
    /// Apply the scaling to a player's spec score based upon their corruption
    /// 
    /// </summary>
    /// <param name="baseScore">The relevant spec score base</param>
    /// <param name="itemModifier">The modifier to the relevant spec score based upon their items</param>
    /// <returns>The scaled spec score</returns>
    private int ApplyScaling (int baseScore, int itemModifier)
    {
        return baseScore * (int) ((100 - 0.5 * corruption) / 100) + itemModifier;
    }

    /// <summary>
    /// 
    /// Determines if the player can be given a new item into their inventory and assigns it if so
    /// 
    /// </summary>
    /// <param name="item">The item to be assigned</param>
    /// <returns>If the player's inventory is full, returns false</returns>
    public bool GiveItem(Item item)
    {
        for (int itemID = 0; itemID < MAX_ITEMS; itemID++)
        {
            if (items[itemID].itemName == "Default")
            {
                items[itemID] = item;
                return true;
            }
        }

        return false;
    }
}
