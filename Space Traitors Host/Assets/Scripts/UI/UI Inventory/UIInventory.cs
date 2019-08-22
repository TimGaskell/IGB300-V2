using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    private Player player;
    public int inventorySize = 0;
    public GameObject inventorySlot;
    private GameObject[] IVSlotsE = new GameObject[4], IVSlotsU = new GameObject[4];
    public GameObject[] UIitems = new GameObject[4];
    // Start is called before the first frame update
    void Start()
    {
        player = new Player(0, "TestPlayer");
        player.GiveItem(new Item(Item.ItemTypes.Boxing_Gloves));
        inventorySize = player.items.Count;

        foreach (Item item in player.items)
        {
           if (item.isEquipped)
            {
                GameObject IVSlot = Instantiate(inventorySlot, transform.GetChild(0).transform.position, Quaternion.identity);
                IVSlot.transform.parent = transform.GetChild(0);
                IVSlot.GetComponent<UIInventorySlot>().Equipped = true;
            }
            else
            {
                GameObject IVSlot = Instantiate(inventorySlot, transform.GetChild(1).transform.position, Quaternion.identity);
                IVSlot.transform.parent = transform.GetChild(1);
                IVSlot.GetComponent<UIInventorySlot>().Equipped = false;
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
}
