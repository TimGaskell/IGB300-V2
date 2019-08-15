using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject flickLocation;

    public float cardSpeed = 1700f;
    public float positionRange = 20f;
    public float maxFlickTime = 0.5f;

    //Spec Variables
    public enum Specs { Default, Brawn, Skill, Tech, Charm };
    public int specNumber = 0;

    private Vector3 cardPos, startingPos;
    public float flickTime;
    private bool held = false, overCard = false;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        cardPos = transform.position;
        flickTime = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Find whether mouse/touch is held down or not
        GetMouseState();

        //When the card is held down by the player, have its new position be wherever the player chooses to hold
        if (held)
        {
            cardPos.x = Input.mousePosition.x;
            cardPos.y = Input.mousePosition.y;
        }

        CardMovement();

        if (flickTime > 0)
            flickTime -= Time.deltaTime;

        Debug.Log(held);
    }

    //Handles the card moving towards either the position the player is holding it, or its default position when it's let go
    private void CardMovement()
    {
        //X Position
        if ((int)transform.position.x < (int)cardPos.x - positionRange)
            transform.position = new Vector3(transform.position.x + cardSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        else if ((int)transform.position.x > (int)cardPos.x + positionRange)
            transform.position = new Vector3(transform.position.x - cardSpeed * Time.deltaTime, transform.position.y, transform.position.z);

        //Y Position
        if ((int)transform.position.y < (int)cardPos.y - positionRange)
            transform.position = new Vector3(transform.position.x, transform.position.y + cardSpeed * Time.deltaTime, transform.position.z);
        else if ((int)transform.position.y > (int)cardPos.y + positionRange)
            transform.position = new Vector3(transform.position.x, transform.position.y - cardSpeed * Time.deltaTime, transform.position.z);
    }

    //When the card hits the point it's supposed to 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (flickTime > 0)
        {
            SpecHit(collision);
        }
    
    }

    public void SpecHit(Collider2D specObject)
    {
        //Code for when spec is hit
        switch (specObject.GetComponent<SpecIcon>().SpecType)
        {
            case "" :
                specNumber = (int)Specs.Default;
                break;

            case "Charm":
                specNumber = (int)Specs.Charm;
                break;

            case "Brawn":
                specNumber = (int)Specs.Brawn;
                break;

            case "Tech":
                specNumber = (int)Specs.Tech;
                break;

            case "Skill":
                specNumber = (int)Specs.Skill;
                break;
        }
        Debug.Log("Spec Number is: " + specNumber);
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
            {
                held = true;
                flickTime = maxFlickTime;
            } 
        }

        if (Input.GetMouseButtonUp(0))
        {
            held = false;
            cardPos.x = startingPos.x;
            cardPos.y = startingPos.y;
            flickTime = 0;
        }
    }
    #endregion
}


