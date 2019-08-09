using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIPowerBar : MonoBehaviour
{
    private Image powerBar;
    // Start is called before the first frame update
    void Start()
    {
        powerBar = gameObject.GetComponent<Image>();
        powerBar.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Always gets the amount of AI Power in the Game Manager
        //Maximum AI Power is 100, fill amount is between 0 and 1. 
        powerBar.fillAmount = GameManager.instance.AIPower / 100;
    }
}
