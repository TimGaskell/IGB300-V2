﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RollActionPoints : MonoBehaviour
{
    public static RollActionPoints instance = null;
    
    //Currently, the 8 bars must be the topmost children of the battery gameObject, in the correct order.
    private GameObject[] barImages;
    public GameObject numberText;
    private GameObject gameManager;

    public float timeRange = 0.5f;
    public float randomTime;
    //Also acts as the speed of the bar
    public float MaxTime = 0.5f;

    private float timeInterval = 0;

    //How slow the bar speed will become
    public float decelerationRate = 1.7f;
    public float acceleration = 1;
    public float minAcceleration = 0.3f;
    public float MaxEndTime = 2.0f;

    private int actionPoints = 0, currentActionPoints = 0;
    private int rollValue = 0, rollAdd = 0;
    private int MaxBars = 4;
    private int MinRandom = 2;

    private float endTime;

    private bool sent = false;
    private bool increasing = true;
    private bool rollStop = false,  timerStop = false;
    private bool endTimer = false;
    private bool endTimerFinished = false;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        barImages = new GameObject[MaxBars];
        for (int i = 0; i < MaxBars; i++)
        {
            barImages[i] = transform.GetChild(i).gameObject; 
            barImages[i].SetActive(false);
           
        }

        endTime = MaxEndTime;
    }

    // Update is called once per frame
    void Update()
    {
        //Show the display number
        if (numberText != null)
        {
            numberText.GetComponent<TextMeshProUGUI>().text = actionPoints.ToString();
        }

        if (!timerStop)
        {
            //Increment/decrement bars
            if (increasing)
                timeInterval += Time.deltaTime * acceleration;
            else
                timeInterval -= Time.deltaTime * acceleration;
        }
        
        //Quick acceleration check, to prevent bar from becoming too sluggish
        if (acceleration < minAcceleration)
        {
            acceleration = minAcceleration;
        }

        //Method for displaying bars
        TimeCheck();

        if (endTimer)
        {
            endTime -= Time.time;
            if (endTime <= 0)
            {
                endTimerFinished = true;
            }
        }

        //Stops the bars from moving as soon as the value the player rolled has been found
        if (rollStop)
        {
            if (actionPoints == rollValue) {
                timerStop = true;

                if (endTimerFinished)
                {
                    if (!sent)
                    {
                        Debug.Log("Sent");
                        Server.Instance.SendActionPoints(actionPoints);
                        Server.Instance.SendNewPhase(); 
                        sent = true;
                    }
                    endTimerFinished = false;
                    endTimer = false;
                }
                else
                {
                    endTimer = true;
                    endTime = MaxEndTime;
                }

            }
        }
    }


    #region Usable Methods 
    //CALL THIS when the player is rolling
    public void GetRoll()
    {
        sent = false;
        //Randomly decides an add on amount to the roll between 0 and 4 (inclusive)
        rollAdd = Random.Range(MinRandom, MaxBars);
        //the value of the roll is the current amount of action points plus the decided add on amount

        if (increasing)
            rollValue = actionPoints + rollAdd;
        else
        {
            rollValue = actionPoints - rollAdd;
            if (rollValue < 0)
            rollValue = -rollValue;
        }

        //If the roll goes higher that the maximum amount, it will reverse by the number of its remainder. E.G. roll = 9, 9-8=1, roll-1=7
        if (rollValue > MaxBars)
        {
            rollAdd -= (MaxBars - actionPoints);
            rollValue = MaxBars - rollAdd;
        }

        rollStop = true;
        //Makes the deceleration feel 'smoother'
        acceleration /= 2;

        Debug.Log("RA: "+ rollAdd + " AP: " + actionPoints + " RV: " + rollValue);

        //Assign Action Points to Game Manager
        GameManager.instance.actionPoints = rollValue;
        
    }

    //CALL THIS to reset the roll for next turn
    public void ResetRoll()
    {
        rollStop = false;
        timerStop = false;
        increasing = true;
        acceleration = 1;
        rollAdd = 0;
        timeInterval = 0;
        decelerationRate = 1.7f;
        currentActionPoints = 0;

    }
    #endregion

    #region Back-End Methods
    private void BarIncrement(int AP)
    {
        //SetActive the number of bars required to be visibly displayed
        int i = 0;
        foreach (GameObject image in barImages)
        {
            if (i >= AP)
            {
                image.SetActive(false);
            }
            else
            {
                image.SetActive(true);
            }

            i++;
        }

        if (rollStop)
        {
            acceleration /= decelerationRate;
        }
    }

    private void TimeCheck()
    {
        if (timeInterval <= MaxTime / 4)
        {
            actionPoints = 1;
        }
        else if ((timeInterval >= MaxTime / 4) && (timeInterval <= MaxTime / 2.6))
        {
            actionPoints = 2;
        }
        else if ((timeInterval >= MaxTime / 2.6) && (timeInterval <= MaxTime / 2))
        {
            actionPoints = 3;
        }
        else if ((timeInterval >= MaxTime / 2) && (timeInterval <= MaxTime / 1.6))
        {
            actionPoints = 4;
        }
       

        //Bars only change when an alteration in the current number of Action Points is detected
        if (actionPoints != currentActionPoints)
        {
            currentActionPoints = actionPoints;
            BarIncrement(actionPoints);
        }

        //Check if bars have reached max/min amount, reverse direction
        if (timeInterval >= MaxTime * 1.14)
        {
            increasing = false;
        }
        else if (timeInterval <= 0)
        {
            increasing = true;
        }
    }

    #endregion
}
