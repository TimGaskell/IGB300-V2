using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        MoveToRoomPanel.SetActive(false);
        instance = this;
        Movement = GameObject.Find("Players").GetComponent<PlayerMovement>();
        GameManager.instance.playerList = GameObject.FindWithTag("PlayerList");
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

        GameManager.instance.roomSelection = false;
        Movement.goalIndex = roomID;
        Movement.StartMoving = true;
              
        GameManager.instance.playerGoalIndex = roomID;
        GameManager.instance.playerMoving = true;

        Server.Instance.SendRoomChoice(roomID);
        Server.Instance.SendNewPhase();
        MoveToRoomPanel.SetActive(false);

        for (int i = 0; i < GameManager.instance.roomList.GetComponent<WayPointGraph>().graphNodes.Length; i++) {

            GameObject room = GameManager.instance.roomList.GetComponent<WayPointGraph>().graphNodes[i];
            Debug.Log(room.name);
            room.transform.GetChild(4).gameObject.SetActive(true);

        }

    }

    public void Exit() {

        GameManager.instance.roomSelection = true;
        MoveToRoomPanel.SetActive(false);

    }

    public void SetCost() {

        CostText.GetComponent<TextMeshProUGUI>().text = "AP Cost: " + roomCost;
            

    }
    public void SetRoom() {

        string roomName = GameManager.instance.roomList.transform.GetChild(roomID).GetComponent<Room>().roomType.ToString();
        roomName = roomName.Replace("_", " ");

        MoveToText.GetComponent<TextMeshProUGUI>().text = "Move to " + roomName;

    }
    public void SetScrap() {

        ScrapReturnText.GetComponent<TextMeshProUGUI>().text = "Scrap return: " + scrapReturn;

    }


}
