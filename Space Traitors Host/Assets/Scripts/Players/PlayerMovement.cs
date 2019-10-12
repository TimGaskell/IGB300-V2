using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : Navigation
{
    public static PlayerMovement instance = null;
    public bool ServerVersion = false;

    //Movement Variables
    public float moveSpeed = 100.0f;
    public float minDistance = 0.1f;
    public int goalIndex = 0;
    public bool StartMoving = false;

    //Player Variables
    public GameObject Player;
    
    //Button variables
    private GameObject SelectedRoom;
    private int RoomNumber;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
        //Find waypoint graph
        //graphNodes = GameObject.FindGameObjectWithTag("Map").GetComponent<WayPointGraph>();
        graphNodes = GameManager.instance.roomList.GetComponent<WayPointGraph>();

        //Initial node currentNode to move to
        currentPath.Add(currentNodeIndex);
        

    }

    void Update()
    {
        //Change back when doesnt give 2 million errors
        // Player.GetComponent<AnimationSwitcher>().RunAnimation(Player.GetComponent<PlayerObject>().CharacterType.ToString());
    }

    public void PlayerMoveViaNodes(int goalIndex) {

           
        currentPath = AStarSearch(currentNodeIndex, goalIndex);
        currentPathIndex = 0;



        if (StartMoving == true) {

            //Set moving animation
          //  Player.GetComponent<AnimationSwitcher>().RunAnimation(Player.GetComponent<PlayerObject>().CharacterType.ToString());

            //Move player
            if (currentPath.Count > 0) {

                //Looks at the Graph Node it is heading towards
                Vector3 direction = (graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position - Player.transform.position).normalized;
                Quaternion look = Quaternion.LookRotation(direction);
                Player.transform.rotation = look;

                //Start Moving towards that Graph Node
                Player.transform.position = Vector3.MoveTowards(Player.transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);

                //Increase path currentNode
                if (Vector3.Distance(Player.transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance) {

                    if (currentPathIndex < currentPath.Count - 1)

                        currentPathIndex++;
                }

                currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;   //Store current node currentNode
            }
            //Check if reached end of path
            if (Player.transform.position == graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) {

                StartMoving = false;
                GameManager.instance.playerMoving = false;

                Player.transform.eulerAngles = new Vector3(0,0,0);
                
                
                
                
                
                



                if (ServerVersion) {
                    GameManager.instance.GetActivePlayer().roomPosition = goalIndex;
                    Server.Instance.SendRoomChoices(GameManager.instance.GetActivePlayer().playerID, goalIndex);
                    //If its at the end of the path look off to the side
                    Player.transform.eulerAngles = new Vector3(0,180,0);
                    MoveToFinalPosition(graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>());
                }

                //Return to idle animation
                // Player.GetComponent<AnimationSwitcher>().IdleAnimation(Player.GetComponent<PlayerObject>().CharacterType.ToString());

                

                Debug.Log("finished Moving");

               

            }
        }
    }

    public void MoveToFinalPosition(LinkedNodes currentNode)
    {
        Vector3 PlayerPos = Player.transform.position;
        Vector3 targetPos = currentNode.finalPositions[GameManager.instance.GetActivePlayer().playerID].transform.position;

        Player.transform.position = new Vector3(targetPos.x,PlayerPos.y, targetPos.z);
        
         
    }
}
