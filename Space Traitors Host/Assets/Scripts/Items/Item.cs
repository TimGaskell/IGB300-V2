﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string itemName;

    public int BrawnChange;
    public int SkillChange;
    public int TechChange;
    public int CharmChange;

    //The position in the map where the item originally came from. Used for discarding items
    //First number indicates the room ID. Second number indicates the choice ID.
    public int[] roomOrigin;

    //Tells if the player has the item equipped or not
    public bool isEquipped;

    /// <summary>
    /// 
    /// Define a default item
    /// 
    /// </summary>
    public Item()
    {
        itemName = "Null";

        BrawnChange = 0;
        SkillChange = 0;
        TechChange = 0;
        CharmChange = 0;

        //All items start out in a room that does not exist until they are assigned
        roomOrigin = new int[] { -1, -1 };

        isEquipped = false;
    }

    /// <summary>
    /// 
    /// Define an item of a particular type
    /// 
    /// </summary>
    /// <param name="newItemName">The name of the particular item</param>
    public Item(string newItemName)
    {
        itemName = newItemName;

        DetermineItemScores();

        //All items start out in a room that does not exist until they are assigned
        roomOrigin = new int[] { -1, -1 };

        isEquipped = false;
    }

    /// <summary>
    /// 
    /// Determines the changes in spec scores for the item based on its name. If the item name does not exist, item will have no change in spec scores assigned
    /// 
    /// </summary>
    private void DetermineItemScores()
    {
        //Resets spec scores to default of no change
        BrawnChange = 0;
        SkillChange = 0;
        TechChange = 0;
        CharmChange = 0;

        switch (itemName)
        {
            case "Boxing Gloves":
                BrawnChange = 1;
                break;
            case "Speed Boots":
                SkillChange = 1;
                break;
            case "Robo-Brain":
                TechChange = 1;
                break;
            case "Dazzling Outfit":
                CharmChange = 1;
                break;
            case "Fancy Glasses":
                TechChange = 2;
                CharmChange = 1;
                break;
            case "Extra Arms":
                BrawnChange = 1;
                SkillChange = 2;
                break;
            case "Hyper Fuel":
                BrawnChange = 2;
                TechChange = 1;
                break;
            case "Spy Suit":
                SkillChange = 1;
                CharmChange = 2;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 
    /// Returns an item to its original position in the level map and reenables the choice it came from
    /// 
    /// </summary>
    /// <param name="rooms">The parent object of all the room objects</param>
    public void ReturnItem()
    {
        //Retrieve the choice and store it temporarily
        Choice tempChoice = GameManager.instance.roomList.GetComponent<ChoiceRandomiser>().GetChoice(roomOrigin[0], roomOrigin[1]);

        //Assign the choice and reenable the choice
        tempChoice.specItem = this;
        if (tempChoice.oneOff)
        {
            tempChoice.disabled = false;
        }

        //Reassigns the choice
        GameManager.instance.GetComponent<ChoiceRandomiser>().SetChoice(roomOrigin[0], roomOrigin[1], tempChoice);
    }
}
