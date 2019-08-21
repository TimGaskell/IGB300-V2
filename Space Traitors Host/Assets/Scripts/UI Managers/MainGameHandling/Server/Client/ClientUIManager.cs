using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class ClientUIManager : MonoBehaviour
{
    public ClientManager player;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.Find("PlayerInfoHolder").GetComponent<ClientManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Does the action roll for the player and sends the number to the server.
    /// </summary>
    public void ActionRoll() {

        int roll = 0;

        roll = GameManager.instance.RollActionPoints();

        player.ActionPoints = roll;

        Server.Instance.SendActionPoints(roll);


    }

    public void MoveToRoom() {
        
        PlayerMovement.instance.StartMoving = true;
        GameManager.instance.playerMoving = true;
        Server.Instance.SendRoomChoice(GameManager.instance.playerGoalIndex);
        

    }
}
