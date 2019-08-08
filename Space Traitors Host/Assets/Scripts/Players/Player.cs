using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    public const int STARTING_ROOM_ID = 9; //Players always start in the escape room i.e. room 9

    public const int MAX_ITEMS = 4;
    public const int MAX_EQUIPPED_ITEMS = 2;

    public const int BASE_LIFE_POINTS = 3;

    private const int MAX_CORRUPTION = 100;

    //playerID should mirror the connection ID of the player to know which client information needs to be passed to
    public int playerID;
    //The name the player inputs when they start the game
    public string playerName;

    public int roomPosition;

    //Connection
    public bool isConnected;

    //Player Resources
    public int scrap;
    private int corruption;
    public int Corruption { get { return corruption; } set { corruption = Mathf.Clamp(value, 0, MAX_CORRUPTION); } }
    public List<Item> items;
    public bool hasComponent;
    public int lifePoints;
    public int maxLifePoints;
    public int ActionPoints;

    public bool IsDead { get { return lifePoints == 0; } }

    public Character Character { get; set; }

    #region Static Changes on Spec Scores
    //Changes to spec scores which originate from the player's items and abilities
    private int BrawnChange
    {
        get
        {
            return items.Where(x => x.isEquipped).Sum(x => x.BrawnChange) + brawnModTemp;
        }
    }
    private int SkillChange
    {
        get
        {
            return items.Where(x => x.isEquipped).Sum(x => x.SkillChange) + skillModTemp;
        }
    }
    private int TechChange
    {
        get
        {
            return items.Where(x => x.isEquipped).Sum(x => x.TechChange) + techModTemp;
        }
    }
    private int CharmChange
    {
        get
        {
            return items.Where(x => x.isEquipped).Sum(x => x.CharmChange) + charmModTemp;
        }
    }
    #endregion

    public int BaseBrawn { get { return Character.baseBrawn; } }
    public int BaseSkill { get { return Character.baseSkill; } }
    public int BaseTech { get { return Character.baseTech; } }
    public int BaseCharm { get { return Character.baseCharm; } }

    public int brawnModTemp;
    public int skillModTemp;
    public int techModTemp;
    public int charmModTemp;

    //Output the spec scores scaled by their corruption. Should be readonly so only get is defined
    public float ScaledBrawn { get { return ApplyScaling(Character.baseBrawn, BrawnChange); } }
    public float ScaledSkill { get { return ApplyScaling(Character.baseSkill, SkillChange); } }
    public float ScaledTech { get { return ApplyScaling(Character.baseTech, TechChange); } }
    public float ScaledCharm { get { return ApplyScaling(Character.baseCharm, CharmChange); } }

    public int ModBrawn { get { return Character.baseBrawn + BrawnChange; } }
    public int ModSkill { get { return Character.baseSkill + SkillChange; } }
    public int ModTech { get { return Character.baseTech + TechChange; } }
    public int ModCharm { get { return Character.baseCharm + CharmChange; } }


    //Character Specific Variables
    public bool ChefBuffed = false; //Used for Chef's 'Preparation' ability
    public bool IsInvisible = false; //Used for Techie's 'Muddle Sensors' ability

    //Says if the player has been selected as traitor or not
    public bool isTraitor;
    //Says if the player has been revealled as a traitor or not. Should always be false if player is not a traitor
    public bool isRevealed;

    //Reference to the players model in the game world
    public GameObject playerObject;

    public Player(int PlayerID, string PlayerName)
    {
        playerID = PlayerID;
        playerName = PlayerName;

        roomPosition = STARTING_ROOM_ID;

        scrap = 0;
        corruption = 0;

        items = new List<Item>();

        hasComponent = false;
        lifePoints = BASE_LIFE_POINTS;
        maxLifePoints = BASE_LIFE_POINTS;

        Character = new Character();

        brawnModTemp = 0;
        skillModTemp = 0;
        techModTemp = 0;
        charmModTemp = 0;

        isTraitor = false;
        isRevealed = false;

        playerObject = null;
    }

    //Techie only, have player turn invisible on main screen
    public void MuddleSensors(bool visible)
    {
       // this.gameObject.GetComponent<MeshRenderer>().enabled = visible;
    }

    /// <summary>
    /// 
    /// Constructor for a player if the character type has already been defined.
    /// 
    /// </summary>
    public Player(int PlayerID, string PlayerName, Character.CharacterTypes characterType) : this(PlayerID, PlayerName)
    {
        Character = new Character(characterType);
    }

    /// <summary>
    /// 
    /// Apply the scaling to a player's spec score based upon their corruption
    /// 
    /// </summary>
    /// <param name="baseScore">The relevant spec score base</param>
    /// <param name="itemModifier">The modifier to the relevant spec score based upon their items</param>
    /// <returns>The scaled spec score</returns>
    private float ApplyScaling(int baseScore, int itemModifier)
    {
        return baseScore * ((100.0f - 0.5f * corruption) / 100.0f) + itemModifier;
    }

    #region Item Handling

    /// <summary>
    /// 
    /// Determines if the player can be given a new item into their inventory and assigns it if so
    /// 
    /// </summary>
    /// <param name="item">The item to be assigned</param>
    /// <returns>If the player's inventory is full, returns false</returns>
    public bool GiveItem(Item item)
    {
        //Cannot give the player the item if there are more than the maximum number of items in their inventory
        if (items.Count < MAX_ITEMS)
        {
            //If the player has valid slots for the item to be equipped, equips the item.
            //May need to be changed if forcing inventory management when picking up an item
            if(items.Where(x => x.isEquipped).Count() < MAX_EQUIPPED_ITEMS)
            {
                item.isEquipped = true;
            }
            else
            {
                item.isEquipped = false;
            }
            items.Add(item);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// Remove the item from a player inventory
    /// 
    /// </summary>
    /// <param name="itemIndex">The index of the item in their inventory</param>
    public void RemoveItem(int itemIndex)
    {
        items.RemoveAt(itemIndex);
    }

    /// <summary>
    /// 
    /// Equips an item for a player if it can be done. Reasons for failure are having too many items equipped or 
    /// already having the same type of item equipped
    /// 
    /// </summary>
    /// <param name="itemIndex">The index of the item within the items list</param>
    /// <returns>If the equip action fails for any reason will return false</returns>
    public bool EquipItem(int itemIndex)
    {
        int numEquipped = 0;
        Item testingItem = items[itemIndex];

        foreach (Item item in items)
        {
            //Only need to verify conditions if the item is already equipped
            if (item.isEquipped)
            {
                //If the item is already equipped, returns false
                if (item == testingItem)
                {
                    Debug.Log("Item already Equipped."); //Can replace this with some other form of output to give feedback to player if needed
                    return false;
                }


                numEquipped++;

                //If the number of items equipped exceeds the maximum, returns false
                if (numEquipped >= MAX_EQUIPPED_ITEMS)
                {
                    Debug.Log("Too many Items Equipped."); //Can replace this with some other form of output to give feedback to player if needed
                    return false;
                }
            }
        }

        //Equips the items then returns true
        items[itemIndex].isEquipped = true;
        return true;
    }

    /// <summary>
    /// 
    /// Unequip and item in the player's inventory
    /// 
    /// </summary>
    /// <param name="itemIndex">The index of the item in their inventory</param>
    public void UnequipItem(int itemIndex)
    {
        items[itemIndex].isEquipped = false;
    }

    /// <summary>
    /// 
    /// Discards an item from a players inventory, removing it and returning it to the choice it came from
    /// 
    /// </summary>
    /// <param name="itemIndex">The index of the item in the players inventory</param>
    /// <param name="rooms">The parent object of all the room objects</param>
    public void DiscardItem(int itemIndex)
    {
        items[itemIndex].ReturnItem();
        RemoveItem(itemIndex);
    }

    #endregion
}
