using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //IMPORTANT: make sure any UI Item objects are tagged "UIitem"

    private GameObject flickLocation;

    public float pointerSpeed = 1700f;
    public float positionRange = 20f;
    public Item.ItemTypes itemType;
    public string itemName;
    public bool isEqupped = false;

    private Vector3 pointerPos, origin, slotOrigin;
    private bool held = false;
    private bool overItem = false, overSlot = false;

    private GameObject inventory;
    private GameObject currentStoredItem;
    private Collider2D tempSlot;

    // Start is called before the first frame update
    void Start()
    {
        //Set tranform defaults 
        origin = transform.position;
        slotOrigin = transform.position;
        pointerPos = transform.position;

        //Get the item name
        itemName = itemType.ToString();
        string displayName = itemName.Replace('_', ' ');
        transform.GetChild(1).GetComponent<Text>().text = displayName;
    }

    // Update is called once per frame
    void Update()
    {
        
        //Find whether mouse/touch is held down or not
        GetMouseState();

        //When the card is held down by the player, have its new position be wherever the player chooses to hold
        if (held)
        {
            pointerPos.x = Input.mousePosition.x;
            pointerPos.y = Input.mousePosition.y;
        }

        CardMovement();
    }

    //Handles the card moving towards either the position the player is holding it, or its default position when it's let go
    private void CardMovement()
    {
        if (held)
        {
            //Item label appears
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);

            //X Position
            if ((int)transform.position.x < (int)pointerPos.x - positionRange)
                transform.position = new Vector3(transform.position.x + pointerSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            else if ((int)transform.position.x > (int)pointerPos.x + positionRange)
                transform.position = new Vector3(transform.position.x - pointerSpeed * Time.deltaTime, transform.position.y, transform.position.z);

            //Y Position
            if ((int)transform.position.y < (int)pointerPos.y - positionRange)
                transform.position = new Vector3(transform.position.x, transform.position.y + pointerSpeed * Time.deltaTime, transform.position.z);
            else if ((int)transform.position.y > (int)pointerPos.y + positionRange)
                transform.position = new Vector3(transform.position.x, transform.position.y - pointerSpeed * Time.deltaTime, transform.position.z);
        }
        else
        {
            //Item label disappears
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //Get the inventory- placing this in start or update seems to often result in a null reference if item starts on a collider
        if (!inventory)
        {
            inventory = GameObject.FindGameObjectWithTag("Inventory");
        }

        if (other.GetComponent<UIInventorySlot>().Equipped)
        {
            //Equip Item
            inventory.GetComponent<UIInventory>().EquipChange(itemName, true);
            isEqupped = true;
        }
        else
        {
            //Unequip Item
            inventory.GetComponent<UIInventory>().EquipChange(itemName, false);
            isEqupped = false;
        }

        //Item swapping- if an item already exists in the inv. slot's place, send it to the carried UI Item's original 
        other.GetComponent<UIInventorySlot>().StoredItem.transform.GetComponent<UIInventoryItem>().transform.position = origin;
        other.GetComponent<UIInventorySlot>().StoredItem = gameObject;
        
        //Centre the item
        transform.position = other.transform.position;

        origin = transform.position;
    }

    #region Mouse Pointers
    public void OnPointerEnter(PointerEventData eventData)
    {
        overItem = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        overItem = false;
        overSlot = false;
    }

    private void GetMouseState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (overItem)
            {
                held = true;
                origin = transform.position;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            held = false;
            pointerPos.x = origin.x;
            pointerPos.y = origin.y;
            transform.position = origin;

        }
    }
    #endregion
}


