using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public int inventorySize = 0;
    private int invIndexE = 0, invIndexU = 0;
    public GameObject inventorySlot;
    public GameObject UIitem;

    private Player player;
    private GameObject[] IVSlotsE, IVSlotsU;
    
   // public GameObject[] UIitems = new GameObject[4];
    // Start is called before the first frame update
    void Start()
    {
        player = new Player(0, "TestPlayer");
        player.GiveItem(new Item(Item.ItemTypes.Boxing_Gloves));
        player.GiveItem(new Item(Item.ItemTypes.Dazzling_Outfit));
        player.GiveItem(new Item(Item.ItemTypes.Extra_Arms));
        player.GiveItem(new Item(Item.ItemTypes.Hyper_Fuel));

        inventorySize = player.items.Count;

        IVSlotsE = new GameObject[inventorySize];
        IVSlotsU = new GameObject[inventorySize];


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
    }

    private void CreateSlot(bool equip, Item item)
    {
        int row = 0;
        if (!equip)
            row = 1;
        //Instantiate inventory slot
        GameObject IVSlot = Instantiate(inventorySlot, transform.GetChild(row).transform.position, Quaternion.identity);
        IVSlot.transform.parent = transform.GetChild(row);
        IVSlot.GetComponent<UIInventorySlot>().Equipped = equip;
        Vector3 UIitemPos;
        if (equip)
        {
            IVSlotsE[invIndexE] = IVSlot;
            UIitemPos = IVSlotsE[invIndexE].transform.position;
            invIndexE++;
        }
        else
        {
            IVSlotsU[invIndexU] = IVSlot;
            UIitemPos = IVSlotsU[invIndexU].transform.position;
            invIndexU++;
            Debug.Log(invIndexU);
        }

        //Instantiate inventory item
        GameObject IVItem = Instantiate(UIitem, UIitemPos, Quaternion.identity);
        IVItem.transform.parent = transform.GetChild(row);
        NameItem(IVItem, item);

    }


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
