using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject flickLocation;

    public float flickTime;
    public float cardSpeed = 1700f;
    public float positionRange = 20f;

    private Vector3 cardPos, startingPos;
    private bool held = false, overCard = false;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        cardPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Find whether mouse/touch is held down or not
        GetMouseState();

        //When the card is held down by the player, have its new position be wherever the player chooses to hold
        if (held)
        {
            cardPos.y = Input.mousePosition.y;
        }

        CardMovement();

        Debug.Log(held);
    }

    //Handles the card moving towards either the position the player is holding it, or its default position when it's let go
    private void CardMovement()
    {
        if ((int)transform.position.y < (int)cardPos.y - positionRange)
            transform.position = new Vector3(transform.position.x, transform.position.y + cardSpeed * Time.deltaTime, transform.position.z);
        else if ((int)transform.position.y > (int)cardPos.y + positionRange)
            transform.position = new Vector3(transform.position.x, transform.position.y - cardSpeed * Time.deltaTime, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("h");
    }

    #region Mouse Pointers
    public void OnPointerEnter(PointerEventData eventData)
    {
        overCard = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        overCard = false;
    }

    private void GetMouseState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (overCard)
                held = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            held = false;
            cardPos.y = startingPos.y;
        }
    }
    #endregion
}


