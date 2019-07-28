﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    private float defaultPos_X, defaultPos_Y, defaultPos_Z;
    public float cameraSpeed;
    private float newPos_X, newPos_Y, newPos_Z;
    private bool zoomedIn = false;
    private bool positiveX = false;
    public float ZoomInLevel_Y;
    public float ZoomInLevel_Z;

    public GameObject testObject;
    // Start is called before the first frame update
    void Start()
    {
        //Get default positions of camera to zoom out to later
        defaultPos_X = transform.position.x;
        defaultPos_Y = transform.position.y;
        defaultPos_Z = transform.position.z;

        //newPos_Y takes the default y axis of the camera into consideration
        newPos_Y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (zoomedIn)
        {
            CameraZoomIn();
        }
        else
        {
            CameraZoomOut();
        }

    }

    /////////////////////////Call these methods in other scripts to initiate zoom in/zoom out effect/////////////////////////

    //Call ZoomIn from another script to have the camera zoom in on the specified player
    public void ZoomIn(GameObject currentPlayer)
    {
        //GameObject playerObject = currentPlayer.
        zoomedIn = true;
        newPos_Z = transform.position.z + currentPlayer.transform.position.z + ZoomInLevel_Z;
        newPos_Y = transform.position.y - ZoomInLevel_Y;
        newPos_X = currentPlayer.transform.position.x;
    }

    //Call ZoomOut from another script to have the camera zoom in on the specified player
    public void ZoomOut()
    {
        zoomedIn = false;
    }


    /////////////////////////Don't call these methods in other scripts, used to make camera move in Update()/////////////////////////
    private void CameraZoomIn()
    {
        //Y Axis
        if ((transform.position.y > newPos_Y))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - cameraSpeed * Time.deltaTime, transform.position.z);
        }

        //Z Axis
        if ((transform.position.z < newPos_Z))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + cameraSpeed * Time.deltaTime);
        }

        //Determine if camera is left or right to player
        if (newPos_X > defaultPos_X)
        {
            //X Axis
            if (transform.position.x < newPos_X)
            {
                transform.position = new Vector3(transform.position.x + cameraSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
        else
        {
            //X Axis
            if (transform.position.x > newPos_X)
            {
                transform.position = new Vector3(transform.position.x - cameraSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
    }

    private void CameraZoomOut()
    {
        //Y Axis
        if ((transform.position.y < defaultPos_Y))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + cameraSpeed * Time.deltaTime, transform.position.z);
        }

        //Z Axis
        if ((transform.position.z > defaultPos_Z))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - cameraSpeed * Time.deltaTime);
        }

        //Determine if camera is left or right to player
        if (newPos_X < defaultPos_X)
        {
            //X Axis
            if (transform.position.x < defaultPos_X)
            {
                transform.position = new Vector3(transform.position.x + cameraSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
        else
        {
            //X Axis
            if (transform.position.x > defaultPos_X)
            {
                transform.position = new Vector3(transform.position.x - cameraSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
    }
}
