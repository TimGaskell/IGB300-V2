using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Assigned to the room object which contains all the room models
public class Room : MonoBehaviour
{
    public enum roomTypes { Default, Bar, Dining_Hall, Engineering, Kitchen, Sleeping_Pods, Spa, Escape };
    public roomTypes roomType;

    public string RoomName { get { return roomType.ToString().Replace('_', ' '); } }

    public Choice[] roomChoices;

    /// <summary>
    /// 
    /// Initialise the room with a default choice which is to be overwritten when choices are randomised
    /// 
    /// </summary>
    /// <param name="choicesPerRoom">The number of choices which are to be initialised in the room</param>
    public void InitialiseRoom(int choicesPerRoom)
    {
        roomChoices = new Choice[choicesPerRoom];
        for (int choicePos = 0; choicePos < choicesPerRoom; choicePos++)
        {
            roomChoices[choicePos] = new Choice();
        }
    }
}
