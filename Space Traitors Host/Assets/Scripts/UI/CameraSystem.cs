using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraSystem : MonoBehaviour
{
    public Vector3 defaultOffset;
    public Vector3 zoomOffset;
    public float smoothSpeed = 0.125f;
    private bool zoomedIn = false;


    [FormerlySerializedAs("testObject")] public GameObject target;
    public GameObject DefaultTarget;

    public static CameraSystem instance;

    private void Awake()
    {
        //Singleton Setup
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        

        //Used for testing the zoom
        if (target != null)
        {
            ZoomIn(target);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //switch (GameManager.instance.currentPhase)
        //{
        //    //When player's turn starts, zoom to that player
        //    case (GameManager.TurnPhases.Default):
        //        ZoomIn(GameManager.instance.GetActivePlayer().playerObject);
        //        break;
        //    //When the 'Movement phase' begins, zoom out again
        //    case (GameManager.TurnPhases.Movement):
        //        ZoomOut();
        //        break;
        //}

        //When character starts actually moving during movement phase, zoom in again
        //if (GameManager.instance.GetActivePlayer().playerObject.GetComponent<PlayerNavigation>().isMoving)
        //if(GameManager.instance.playerMoving && !zoomedIn)
        //{
        //    ZoomIn(GameManager.instance.GetActivePlayer().playerObject);
        //}

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

    //CALL THIS METHOD from another script to have the camera zoom in on the specified player
    //Note: Depending on camera angle, the camera's view may not zoom in on the player properly. To fix this, tweak the ZoomInLevel_Y and ZoomInLevel_Z public variables
    public void ZoomIn(GameObject playerObject)
    { 
        zoomedIn = true;
        target = playerObject;
    }

    //Call ZoomOut from another script to have the camera return to its original position
    public void ZoomOut()
    {
        target = DefaultTarget;
        zoomedIn = false;
    }


    /////////////////////////Don't call these methods in other scripts, used to make camera move in Update()/////////////////////////
    private void CameraZoomIn()
    {
        //Y Axis
        Vector3 desiredPosition = target.transform.position + zoomOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target.transform);
    }

    private void CameraZoomOut()
    {
        Vector3 desiredPosition = target.transform.position + defaultOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target.transform);
    }

}

