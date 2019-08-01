using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : Navigation
{
    public static PlayerMovement instance = null;


    //Movement Variables
    public float moveSpeed = 100.0f;
    public float minDistance = 0.1f;
    public int goalIndex = 0;

    //Player Variables
    public GameObject Player;

    //Button variables
    private GameObject SelectedRoom;
    private int RoomNumber;

    // Start is called before the first frame update
    void Start()
    {
        //Find waypoint graph
        //graphNodes = GameObject.FindGameObjectWithTag("Map").GetComponent<WayPointGraph>();
        graphNodes = GameManager.instance.roomList.GetComponent<WayPointGraph>();

        //Initial node index to move to
        currentPath.Add(currentNodeIndex);

    }

    public void PlayerMoveViaNodes(int goalIndex)
    {

        currentPath = AStarSearch(currentPath[currentPathIndex], goalIndex);
        currentPathIndex = 0;

        //Move player
        if (currentPath.Count > 0)
        {

            //Looks at the Graph Node it is heading towards
            Vector3 direction = (graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position - Player.transform.position).normalized;
            Quaternion look = Quaternion.LookRotation(direction);
            Player.transform.rotation = look;

            //Start Moving towards that Graph Node
            Player.transform.position = Vector3.MoveTowards(Player.transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);

            //Increase path index
            if (Vector3.Distance(Player.transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance)
            {

                if (currentPathIndex < currentPath.Count - 1)

                    currentPathIndex++;
            }

            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;   //Store current node index
        }
        //Check if reached end of path
        if (Player.transform.position == graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position)
        {

            //If its at the end of the path look off to the side
            Vector3 lookBack = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1000);
            Player.transform.rotation = Quaternion.LookRotation(lookBack);

            Debug.Log("finished Moving");
            GameManager.instance.playerMoving = false;
        }
    }

}
