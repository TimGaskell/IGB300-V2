using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public const int STARTING_ROOM_ID = 9; //Players always start in the escape room i.e. room 9

    public const int MAX_ITEMS = 4;
    public const int MAX_EQUIPPED_ITEMS = 2;

    public const int BASE_LIFE_POINTS = 3;

    public int playerID;
    public string playerName;

    public int roomPosition;

    public int scrap;
    public int corruption;
    public Item[] items;
    public Item[] equippedItems;
    public bool hasComponent;
    public int lifePoints;
    public int maxLifePoints;

    public Character characterType;

    public int ScaledBrawn { get { return ApplyScaling(characterType.baseBrawn); } }
    public int ScaledSkill { get { return ApplyScaling(characterType.baseSkill); } }
    public int ScaledTech { get { return ApplyScaling(characterType.baseTech); } }
    public int ScaledCharm { get { return ApplyScaling(characterType.baseCharm); } }

    public Player(int PlayerID, string PlayerName, string CharacterType)
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

        characterType = new Character(CharacterType);
    }

    private int ApplyScaling (int baseScore)
    {
        return baseScore * (int) ((100 - 0.5 * corruption) / 100);
    }
}
