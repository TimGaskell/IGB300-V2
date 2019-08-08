﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollActionPoints : MonoBehaviour
{
    public Image[] barImages;
    public Text numberText;

    public float timeRange = 0.2f;
    public float randomTime;
    public float MaxTime = 0.5f;

    private float timeInterval;

    private int actionPoints;
    private int rollValue, rollAdd;
    private int MaxBars = 8;
    private int MinRandom = 2;

    private bool increasing = true, rollStop = false,  timerStop = false;


    // Start is called before the first frame update
    void Start()
    {
        foreach (Image image in barImages)
        {
            image.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!timerStop)
        {
            if (increasing)
                timeInterval += Time.deltaTime;
            else
                timeInterval -= Time.deltaTime;
        }
        
        TimeCheck();

        //Stops the bars from moving as soon as the value the player rolled has been found
        if (rollStop)
        {
            if (actionPoints == rollValue)
            {
                timerStop = true;
            }
        }
    }

    //CALL THIS when the player is rolling
    public void GetRoll()
    {
        //Randomly decides an add on amount to the roll between 0 and 4 (inclusive)
        rollAdd = Random.Range(MinRandom, MaxBars);
        //the value of the roll is the current amount of action points plus the decided add on amount
        rollValue = actionPoints + rollAdd;

        //If the roll goes higher that the maximum amount, it will reverse by the number of its remainder. E.G. roll = 9, 9-8=1, roll-1=7
        if (increasing &&(rollValue > MaxBars))
        {
            rollAdd -= (MaxBars - actionPoints);
            rollValue = MaxBars - rollAdd;
        }

        if (numberText != null)
        {
            numberText.text = rollValue.ToString();
        }

        rollStop = true;

        Debug.Log("RA: "+ rollAdd + " AP: " + actionPoints + " RV: " + rollValue);

        //Assign Action Points to Game Manager
        GameManager.instance.actionPoints = rollValue;
    }

    private void BarIncrement(int AP)
    {
        int i = 0;
        foreach (Image image in barImages)
        {
            if (i >= AP)
            {
                image.enabled = false;
            }
            else
            {
                image.enabled = true;
            }

            i++;
        }
    }

    private void TimeCheck()
    {
        //Ranges: Check what point timer is and show appropriate amount of bars
        if (timeInterval <= MaxTime / 8)
        {
            actionPoints = 0;
           
        }
        if ((timeInterval >= MaxTime / 8) && (timeInterval <= MaxTime / 4))
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
        else if ((timeInterval >= MaxTime / 1.6) && (timeInterval <= MaxTime / 1.3))
        {
            actionPoints = 5;
        }
        else if ((timeInterval >= MaxTime / 1.3) && (timeInterval <= MaxTime / 1.14))
        {
            actionPoints = 6;
        }
        else if ((timeInterval >= MaxTime / 1.14) && (timeInterval <= MaxTime / 1))
        {
            actionPoints = 7;
        }
        else if (timeInterval >= MaxTime)
        {
            actionPoints = 8;
        }

        BarIncrement(actionPoints);

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
    
    
}
