using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    private Player player;
    public int inventorySize = 3;
    private GameObject[] IVSlotsE = new GameObject[4], IVSlotsU = new GameObject[4];
    public GameObject[] UIitems = new GameObject[4];
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.GetActivePlayer();
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
