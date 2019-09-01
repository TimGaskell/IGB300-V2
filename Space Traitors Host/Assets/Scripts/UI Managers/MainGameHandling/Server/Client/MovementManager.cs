using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementManager : MonoBehaviour
{

    public GameObject MoveToRoomPanel;
    public GameObject MoveToText;
    public GameObject CostText;
    public GameObject ScrapReturnText;

    private PlayerMovement Movement;

    public int roomCost;
    public int roomID;
    public int scrapReturn;
    public static MovementManager instance = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Movement = GameObject.Find("Players").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMoveToRoom() {

        MoveToRoomPanel.SetActive(true);
        SetCost();
        SetRoom();
        SetScrap();
        
    }

    public void MoveToRoom() {

        Movement.goalIndex = roomID;
        
        GameManager.instance.playerGoalIndex = roomID;
        GameManager.instance.playerMoving = true;

        Server.Instance.SendRoomChoice(roomID);
        Server.Instance.SendNewPhase();
        MoveToRoomPanel.SetActive(false);

    }

    public void Exit() {

        MoveToRoomPanel.SetActive(false);

    }

    public void SetCost() {

        CostText.GetComponent<Text>().text = "Cost: " + roomCost;
            

    }
    public void SetRoom() {

        MoveToText.GetComponent<Text>().text = "Move to " + GameManager.instance.roomList.GetComponent<WayPointGraph>().graphNodes[roomID];

    }
    public void SetScrap() {

        ScrapReturnText.GetComponent<Text>().text = "Scrap return: " + scrapReturn;

    }


}
