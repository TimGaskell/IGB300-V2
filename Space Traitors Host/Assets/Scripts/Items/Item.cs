using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string itemName;

    public int brawnChange;
    public int skillChange;
    public int techChange;
    public int charmChange;

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

        brawnChange = 0;
        skillChange = 0;
        techChange = 0;
        charmChange = 0;

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
        brawnChange = 0;
        skillChange = 0;
        techChange = 0;
        charmChange = 0;

        switch (itemName)
        {
            case "Boxing Gloves":
                brawnChange = 1;
                break;
            case "Speed Boots":
                skillChange = 1;
                break;
            case "Robo-Brain":
                techChange = 1;
                break;
            case "Dazzling Outfit":
                charmChange = 1;
                break;
            case "Fancy Glasses":
                techChange = 2;
                charmChange = 1;
                break;
            case "Extra Arms":
                brawnChange = 1;
                skillChange = 2;
                break;
            case "Hyper Fuel":
                brawnChange = 2;
                techChange = 1;
                break;
            case "Spy Suit":
                skillChange = 1;
                charmChange = 2;
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
    public void ReturnItem(GameObject rooms)
    {
        //Retrieve the choice and store it temporarily
        Choice tempChoice = rooms.transform.GetChild(roomOrigin[0]).GetComponent<Room>().roomChoices[roomOrigin[1]];

        //Assign the choice and reenable the choice
        tempChoice.specItem = this;
        if (tempChoice.oneOff)
        {
            tempChoice.disabled = false;
        }

        //Reassigns the choice
        rooms.transform.GetChild(roomOrigin[0]).GetComponent<Room>().roomChoices[roomOrigin[1]] = tempChoice;
    }
}
