using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //Currently, the 8 bars must be the topmost children of the healthbar gameObject, in the correct order.
    private GameObject[] hpImages;
    private GameObject gameManager;

    public int HP = 8; 
    private int currentHP;
    private int MaxBars = 9;
    private float timeInterval;

    //Also acts as the speed of the bar
    public float MaxTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //Assign the HP bar segments
        hpImages = new GameObject[MaxBars];
        for (int i = 0; i < MaxBars; i++)
        {
            hpImages[i] = transform.GetChild(i).gameObject;
            hpImages[i].SetActive(false);

        }

        currentHP = HP;
    }

    // Update is called once per frame
    void Update()
    {
        //Used for when the HP value changes, creating an increase/decrease effect
        if (currentHP < HP)
        {
            timeInterval += Time.deltaTime;
        }
        else if (currentHP > HP)
        {
            timeInterval -= Time.deltaTime;
        }
        else
        {
            currentHP = HP;
        }

        TimeCheck();
    }

    private void BarIncrement(int hp)
    {
        //SetActive the number of bars required to be visibly displayed
        int i = 0;
        foreach (GameObject image in hpImages)
        {
            if (i >= hp)
            {
                image.SetActive(false);
            }
            else
            {
                image.SetActive(true);
            }

            i++;
        }
    }

    private void TimeCheck()
    {
        //Ranges: Check what point timer is and show appropriate amount of bars
        if (timeInterval <= MaxTime / 9)
        {
            currentHP = 0;

        }
        if ((timeInterval >= MaxTime / 9) && (timeInterval <= MaxTime / 4))
        {
            currentHP = 1;
        }
        else if ((timeInterval >= MaxTime / 4) && (timeInterval <= MaxTime / 2.6))
        {
            currentHP = 2;
        }
        else if ((timeInterval >= MaxTime / 2.6) && (timeInterval <= MaxTime / 2))
        {
            currentHP = 3;
        }
        else if ((timeInterval >= MaxTime / 2) && (timeInterval <= MaxTime / 1.6))
        {
            currentHP = 4;
        }
        else if ((timeInterval >= MaxTime / 1.6) && (timeInterval <= MaxTime / 1.3))
        {
            currentHP = 5;
        }
        else if ((timeInterval >= MaxTime / 1.3) && (timeInterval <= MaxTime / 1.14))
        {
            currentHP = 6;
        }
        else if ((timeInterval >= MaxTime / 1.14) && (timeInterval <= MaxTime / 1.07))
        {
            currentHP = 7;
        }
        else if ((timeInterval >= MaxTime / 1.07) && (timeInterval <= MaxTime / 1))
        {
            currentHP = 8;
        }
        else if (timeInterval >= MaxTime)
        {
            currentHP = 9;
        }
     

        BarIncrement(currentHP);
    }

}
