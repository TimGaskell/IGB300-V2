using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public int inventorySize = 0;
    public float posAdjustment = -100f, padding = 100f;
    private float posAdjustmentE, posAdjustmentU;
    public GameObject inventorySlot;
    public GameObject UIitem;

    private Player player;
    private List<GameObject> UIitems;

    // public GameObject[] UIitems = new GameObject[4];
    // Start is called before the first frame update
    void Start()
    {
        //TEST: used for assigning the player items
        player = new Player(0, "TestPlayer");
        player.GiveItem(new Item(Item.ItemTypes.Boxing_Gloves));
        player.GiveItem(new Item(Item.ItemTypes.Dazzling_Outfit));
        player.GiveItem(new Item(Item.ItemTypes.Extra_Arms));
        player.GiveItem(new Item(Item.ItemTypes.Hyper_Fuel));

        inventorySize = player.items.Count;

        UIitems = new List<GameObject>(inventorySize);

        //Adjustment variables for when inventory slots are instantiated
        posAdjustmentE = posAdjustment;
        posAdjustmentU = posAdjustment;

        foreach (Item item in player.items)
        {
           if (item.isEquipped)
            {
                CreateSlot(true, item);
            }
            else
            {
                CreateSlot(false, item);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EquipChange(string UIitemName, bool equipping)
    {
   
        foreach(Item item in player.items)
        {
            if (item.ItemName == UIitemName)
            {
                if (equipping)
                    item.isEquipped = true;
                else
                    item.isEquipped = false;
            }
        }
        foreach (GameObject UII in UIitems)
        {
            UII.transform.SetAsLastSibling();
        }
    }

    private void CreateSlot(bool equip, Item item)
    {
        int row = 0;
        if (!equip)
            row = 1;
        //Instantiate inventory slot- both rows have to be adjusted seperately, to remain consistent with one another
        GameObject IVSlot;
        if (equip)
        {
            IVSlot = Instantiate(inventorySlot, new Vector3(transform.GetChild(row).transform.position.x - posAdjustmentE, transform.GetChild(row).transform.position.y, transform.GetChild(row).transform.position.z), Quaternion.identity);
            posAdjustmentE += padding;
        }
        else
        {
            IVSlot = Instantiate(inventorySlot, new Vector3(transform.GetChild(row).transform.position.x - posAdjustmentU, transform.GetChild(row).transform.position.y, transform.GetChild(row).transform.position.z), Quaternion.identity);
            posAdjustmentU += padding;
        }

        //Assign slot to the correct parent within the canvas for positioning, then to the main gameobject for laying purposes (basically, makes everything look nice)
        IVSlot.transform.parent = transform.GetChild(row);
        IVSlot.transform.parent = gameObject.transform;
        //Set the slot to equipped or unequipped based on what the item will be- important for later checks
        IVSlot.GetComponent<UIInventorySlot>().Equipped = equip;
        
        //Set the position for the UI Item to appear (over its inventory slot)
        Vector3 UIitemPos = IVSlot.transform.position;
        
        //Instantiate inventory item, assign it to canvas
        GameObject IVItem = Instantiate(UIitem, UIitemPos, Quaternion.identity);
        IVItem.transform.parent = transform.GetChild(row);
        IVItem.transform.parent = gameObject.transform;

        //The item within the current inventory slot is the current inventory item- important for item swapping
        IVSlot.GetComponent<UIInventorySlot>().StoredItem = IVItem;

        //Add to a list for layering purposes later
        UIitems.Add(IVItem);

        NameItem(IVItem, item);
    }

    //Gives the item entered its parameters so player knows what it is
    private void NameItem(GameObject UIitem, Item item)
    {
        switch (item.ItemType)
        {
            case Item.ItemTypes.Boxing_Gloves:
                UIitem.GetComponent<UIInventoryItem>().itemName = "Boxing Gloves";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Boxing_Gloves;
                break;
            case Item.ItemTypes.Dazzling_Outfit:
                UIitem.GetComponent<UIInventoryItem>().itemName = "Dazzling Outfit";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Dazzling_Outfit;
                break;
            case Item.ItemTypes.Extra_Arms:
                UIitem.GetComponent<UIInventoryItem>().itemName = "Extra Arms";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Extra_Arms;
                break;
            case Item.ItemTypes.Fancy_Glasses:
                UIitem.GetComponent<UIInventoryItem>().itemName = "Fancy Glasses";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Fancy_Glasses;
                break;
            case Item.ItemTypes.Hyper_Fuel:
                UIitem.GetComponent<UIInventoryItem>().itemName = "Hyper Fuel";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Hyper_Fuel;
                break;
            case Item.ItemTypes.Robo_Brain:
                UIitem.GetComponent<UIInventoryItem>().itemName = "Robo Brain";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Robo_Brain;
                break;
            case Item.ItemTypes.Speed_Boots:
                UIitem.GetComponent<UIInventoryItem>().itemName = "Speed Boots";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Speed_Boots;
                break;
            case Item.ItemTypes.Spy_Suit:
                UIitem.GetComponent<UIInventoryItem>().itemName = "Spy Suit";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Spy_Suit;
                break;
            case Item.ItemTypes.Default :
                UIitem.GetComponent<UIInventoryItem>().itemName = "None";
                UIitem.GetComponent<UIInventoryItem>().itemType = Item.ItemTypes.Default;
                break;
        }
    }
}
