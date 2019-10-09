using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionPointRollManager : MonoBehaviour
{
    public GameObject cellContainer;
    public GameObject initialCell;
    private List<GameObject> cellList;

    private int maxRoll;

    private int rolledPoints;
    private bool startRolling;
    private bool phaseWait;
    //If addCell is true, roller is adding new cells in the animation. If false is removing.
    //This should flip when the number of cells active reaches the extremes of the action point roller,
    //either 1 or maxRoll.
    private bool addCell;
    //The number of current cells active
    private int currentCells;

    public GameObject rollButton;
    public GameObject rolledText;

    //Speeds are the amount of time the game waits before adding or removing a new
    //cell image. Accelerations are changes in those speeds every frame
    public float initialSpeed;
    private float currentSpeed;
    public float minSpeed;
    public float decelFactor;

    public float waitTime = 3.0f;

    private float timer = 0.0f;

    public GameObject movementDisplayText;

    private void Start()
    {
        cellList = new List<GameObject>();

        //The maximum number that can be rolled in the number of dice multiplied
        //the number of sides those dice have
        maxRoll = GameManager.AP_DICE_SIDES * GameManager.AP_NUM_DICE;

        //loop to instantiate the other battery cells. The initial cell already exists in the game world
        //So just needs to be added to the list. Horizontal layout group handles positioning of the cells
        for (int i = 0; i < maxRoll; i++)
        {
            GameObject newCell;

            if (i != 0)
            {
                newCell = Instantiate(initialCell, cellContainer.transform);
                newCell.SetActive(false);
            }
            else
            {
                newCell = initialCell;
            }

            cellList.Add(newCell);
        }

        ResetRoller();
    }

    public void ResetRoller()
    {
        rollButton.GetComponent<Button>().interactable = true;
        rolledPoints = 1;
        startRolling = false;
        addCell = true;
        currentCells = 1;
        currentSpeed = initialSpeed;
        SetActiveCells();
    }

    private void Update()
    {
        if (startRolling)
        {
            timer += Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed + decelFactor, minSpeed);

            if (timer > currentSpeed)
            {
                timer = 0;

                if (addCell)
                {
                    currentCells += 1;
                }
                else
                {
                    currentCells -= 1;
                }

                SetActiveCells();
                TestAddCell();

                if (currentSpeed == minSpeed && currentCells == rolledPoints)
                {
                    startRolling = false;
                    phaseWait = true;
                    timer = 0;
                }
            }
        }

        rolledText.GetComponent<TextMeshProUGUI>().text = currentCells.ToString();

        if (phaseWait)
        {
            timer += Time.deltaTime;

            if(timer > waitTime)
            {
                Server.Instance.SendActionPoints(rolledPoints);
                Server.Instance.SendNewPhase();
                movementDisplayText.GetComponent<TextMeshProUGUI>().text = string.Format("{0} AP", rolledPoints);
                phaseWait = false;
            }
        }
    }

    public void RollActionPoints()
    {
        ResetRoller();
        rollButton.GetComponent<Button>().interactable = false;
        rolledPoints = GameManager.RollActionPoints();
        startRolling = true;
    }

    private void TestAddCell()
    {
        if (currentCells == 1)
        {
            addCell = true;
        }
        else if (currentCells == maxRoll)
        {
            addCell = false;
        }
    }

    private void SetActiveCells()
    {
        for (int i = 0; i < maxRoll; i++)
        {
            cellList[i].SetActive(i < currentCells);
        }
    }
}
