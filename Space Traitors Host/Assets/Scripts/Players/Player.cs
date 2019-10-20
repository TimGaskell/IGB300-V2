using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class Player 
{
    public const int STARTING_ROOM_ID = 9; //Players always start in the escape room i.e. room 9

    public const int MAX_ITEMS = 4;
    public const int MAX_EQUIPPED_ITEMS = 2;

    public const int BASE_LIFE_POINTS = 3;

    private const int MAX_CORRUPTION = 100;

    public const int NUM_ABILITIES = 5;

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
    private float ScaledBrawn { get { return ApplyScaling(Character.baseBrawn, BrawnChange); } }
    private float ScaledSkill { get { return ApplyScaling(Character.baseSkill, SkillChange); } }
    private float ScaledTech { get { return ApplyScaling(Character.baseTech, TechChange); } }
    private float ScaledCharm { get { return ApplyScaling(Character.baseCharm, CharmChange); } }

    private int ModBrawn { get { return Character.baseBrawn + BrawnChange; } }
    private int ModSkill { get { return Character.baseSkill + SkillChange; } }
    private int ModTech { get { return Character.baseTech + TechChange; } }
    private int ModCharm { get { return Character.baseCharm + CharmChange; } }

    //Says if the player has been selected as traitor or not
    public bool isTraitor;
    //Says if the player has been revealled as a traitor or not. Should always be false if player is not a traitor
    public bool isRevealed;

    private List<Ability> abilities;
    //If an abilities effects persist beyond the turn they are used in, will store the ability hear to deactivate at the start of their next turn
    public List<Ability> activeAbilitys;
    public int PreviousTarget = 0;
    public Ability PreviousAbility;
    public bool activeThisTurn;
    public int ScrapReturn;

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
        lifePoints = 1;
        maxLifePoints = BASE_LIFE_POINTS;

        Character = new Character();

        brawnModTemp = 0;
        skillModTemp = 0;
        techModTemp = 0;
        charmModTemp = 0;

        isTraitor = false;
        isRevealed = false;

        abilities = new List<Ability>();
        activeAbilitys = new List<Ability>();

        playerObject = null;
    }
    void Start() {
        
    }
 

    /// <summary>
    /// 
    /// Constructor for a player if the character type has already been defined.
    /// 
    /// </summary>
    public Player(int PlayerID, string PlayerName, Character.CharacterTypes characterType) : this(PlayerID, PlayerName)
    {
        Character = new Character(characterType);
        //If the character is predefined by the game manager, will not generate the ability list, so needs to be done here
        GenerateAbilityList();
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

    /// <summary>
    /// 
    /// Modifies the players life points by a certain amount
    /// 
    /// </summary>
    /// <param name="lifePointChange">The amount to change their life points by</param>
    public void ChangeLifePoints(int lifePointChange)
    {
        lifePoints += lifePointChange;
        
       
    }

    public void Disconnect() {

        if (IsDead) {

            if (SceneManager.GetActiveScene().name == "Server GameLevel") {

                string PlayerName;

                PlayerName = GameManager.instance.GetPlayer(playerID).playerName;

                Server.Instance.SendPlayerDeath(playerID);

                ReturnItems();

                GameManager.instance.numPlayers -= 1;

                GameManager.instance.playerOrder.Remove(playerID);

                playerObject.SetActive(false);

                GameManager.instance.players.Remove(this);

                GameManager.instance.CheckTraitorVictory();
            }
            else if (SceneManager.GetActiveScene().name == "ServerLobby") {

                GameManager.instance.players.Remove(this);

            }


        }
        else {
            if (SceneManager.GetActiveScene().name == "Server GameLevel") {

                string PlayerName;

                PlayerName = GameManager.instance.GetPlayer(playerID).playerName;

                Server.Instance.SendPlayerDisconnection(PlayerName);

                ReturnItems();

                GameManager.instance.numPlayers -= 1;

                GameManager.instance.playerOrder.Remove(playerID);

                playerObject.SetActive(false);

                GameManager.instance.players.Remove(this);

                GameManager.instance.CheckTraitorVictory();
            }
            else if (SceneManager.GetActiveScene().name == "ServerLobby") {

                GameManager.instance.players.Remove(this);

            }
        }

           
        
    }

  

    /// <summary>
    /// 
    /// Gets a particular scaled spec score from the player
    /// 
    /// </summary>
    /// <param name="specScore"></param>
    /// <returns></returns>
    public float GetScaledSpecScore(GameManager.SpecScores specScore)
    {
        switch (specScore)
        {
            case (GameManager.SpecScores.Brawn):
                return ScaledBrawn;
            case (GameManager.SpecScores.Skill):
                return ScaledSkill;
            case (GameManager.SpecScores.Tech):
                return ScaledTech;
            case (GameManager.SpecScores.Charm):
                return ScaledCharm;
            default:
                throw new NotImplementedException("Not a valid spec score");
        }
    }

    /// <summary>
    /// 
    /// Gets a particular modded spec score from the player
    /// 
    /// </summary>
    /// <param name="specScore"></param>
    /// <returns></returns>
    public int GetModdedSpecScore(GameManager.SpecScores specScore)
    {
        switch (specScore)
        {
            case (GameManager.SpecScores.Brawn):
                return ModBrawn;
            case (GameManager.SpecScores.Skill):
                return ModSkill;
            case (GameManager.SpecScores.Tech):
                return ModTech;
            case (GameManager.SpecScores.Charm):
                return ModCharm;
            default:
                throw new NotImplementedException("Not a valid spec score");
        }
    }

    #region Abilitiy Handling
    /// <summary>
    /// 
    /// Generates a list of the players abilities to access
    /// 
    /// </summary>
    public void GenerateAbilityList()
    {
        abilities = new List<Ability>
        {
            Character.characterAbility,
            new SensorScan(),
            new CodeInspection(),
            new Sabotage(),
            new SuperCharge()
        };
    }

    /// <summary>
    /// 
    /// Gets an ability of a particular type
    /// 
    /// </summary>
    /// <param name="abilityType">The type of the ability</param>
    /// <returns>The ability</returns>
    public Ability GetAbility(Ability.AbilityTypes abilityType)
    {
        return abilities.Find(x => x.abilityType == abilityType);
    }

    /// <summary>
    /// 
    /// Get an ability based on its ID in the list
    /// 
    /// </summary>
    /// <param name="abilityID">The ID of the ability in the player ability list</param>
    /// <returns>The ability</returns>
    public Ability GetAbility(int abilityID)
    {
        return abilities[abilityID];
    }

    /// <summary>
    /// 
    /// Assigns the active ability to be deactivated later.
    /// 
    /// </summary>
    /// <param name="ability"></param>
    public void AssignActiveAbility(Ability ability)
    {

        activeAbilitys.Add(ability);
    }

    /// <summary>
    /// 
    /// Checks if the active ability is of the type specified. If it is returns true. False otherwise
    /// 
    /// </summary>
    /// <param name="abilityType">The type of ability to compare with</param>
    /// <returns>True if the active ability is of this type. False otherwise</returns>
    public bool CheckActiveAbility(Ability.AbilityTypes abilityType)
    {

        return activeAbilitys.Contains(GetAbility(abilityType));

       
    }

    /// <summary>
    /// 
    /// Deactivate any active abilities a player may have
    /// 
    /// </summary>
    public void DisableActiveAbility(Ability ability)
    {

        Debug.Log("THIS ABILITY IS " + ability.ToString());     
        ability.Deactivate();

          

        
    }
    #endregion

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
            items.Add(item);
            //Automatically equips the last given item if the player is able to
            EquipItem(items.Count - 1);
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

    public enum EquipErrors { Default, AlreadyEquipped, TooManyEquipped };

    /// <summary>
    /// 
    /// Attempts to equip an itme 
    /// 
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <returns></returns>
    public EquipErrors EquipItem(int itemIndex)
    {
        int numEquipped = 0;
        Item testingItem = items[itemIndex];
        //If item is being stolen have to force the item to be unequipped when transferred between
        //inventories.
        testingItem.isEquipped = false;

        foreach (Item item in items)
        {
            //Only need to verify conditions if the item is already equipped
            if (item.isEquipped)
            {
                //If the item is already equipped, returns error
                if (item.ItemType == testingItem.ItemType)
                {
                    return EquipErrors.AlreadyEquipped;
                }

                numEquipped++;

                //If the number of items equipped exceeds the maximum, returns error
                if (numEquipped >= MAX_EQUIPPED_ITEMS)
                {
                    return EquipErrors.TooManyEquipped;
                }
            }
        }

        //Equips the items then returns true
        items[itemIndex].isEquipped = true;
        return EquipErrors.Default;
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

    public void ReturnItems() {

        foreach (Item item in items) {

            item.ReturnItem();

        }
    }

    #endregion
}
