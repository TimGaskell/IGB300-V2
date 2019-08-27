using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private bool held = false, overItem = false;

    private GameObject inventory;
    private GameObject currentStoredItem;
    private Collider2D tempSlot;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        slotOrigin = transform.position;
        pointerPos = transform.position;
        itemName = itemType.ToString();
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
            //If not held, snap to position of inventory slot centre
            transform.position = origin;
            slotOrigin = transform.position;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {   
        //Centre the item
        origin = other.transform.position;

        tempSlot = other;
        //Get the inventory- placing this in start or update seems to often result in a null reference if item starts on a collider
        if (!inventory)
        {
            inventory = GameObject.FindGameObjectWithTag("Inventory");
        }

        if (other.GetComponent<UIInventorySlot>().Equipped)
        {
            //Equip Item
            inventory.GetComponent<UIInventory>().EquipChange(itemName, true);
        }
        else
        {
            //Unequip Item
            inventory.GetComponent<UIInventory>().EquipChange(itemName, false);
        }

        if (currentStoredItem == null)
        currentStoredItem = other.GetComponent<UIInventorySlot>().StoredItem;

        //Item swapping- if an item already exists in the inv. slot's place, send it to the carried UI Item's original 
        if ((other.GetComponent<UIInventorySlot>().StoredItem != null) && (other.GetComponent<UIInventorySlot>().StoredItem != currentStoredItem))
        {
            Debug.Log("oof");
            other.GetComponent<UIInventorySlot>().StoredItem.transform.GetComponent<UIInventoryItem>().origin = slotOrigin;
            //tempSlot.GetComponent<UIInventorySlot>().StoredItem = gameObject;
            currentStoredItem = tempSlot.GetComponent<UIInventorySlot>().StoredItem;
        }
    }

    #region Mouse Pointers
    public void OnPointerEnter(PointerEventData eventData)
    {
        overItem = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        overItem = false;
    }

    private void GetMouseState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (overItem)
            {
                held = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            held = false;
            pointerPos.x = origin.x;
            pointerPos.y = origin.y;
        }
    }
    #endregion
}


