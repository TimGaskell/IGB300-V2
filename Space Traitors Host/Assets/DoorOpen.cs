using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public float openDistance = 3f;
    public float openSpeed = 0.1f;
    private Vector3 closedPosition;
    private Vector3 openedPosition;
    private bool open;

    // Start is called before the first frame update
    void Start()
    {
        open = false;
        closedPosition = gameObject.transform.position;
        
        openedPosition = gameObject.CompareTag("VerticalDoor") ? new Vector3(closedPosition.x, closedPosition.y , closedPosition.z + openDistance) : new Vector3(closedPosition.x + openDistance, closedPosition.y, closedPosition.z);
    }

    private void Update()
    {
        if (open)
        {
            if (Vector3.Distance(gameObject.transform.position, openedPosition) < 0.2f)
            {
                open = false;
            }
            
            gameObject.transform.position =
                Vector3.MoveTowards(gameObject.transform.position, openedPosition, openSpeed * Time.deltaTime);
        }
        else
        {
            gameObject.transform.position =
                Vector3.MoveTowards(gameObject.transform.position, closedPosition, openSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            open = true;
        }
        
    }

    
}
