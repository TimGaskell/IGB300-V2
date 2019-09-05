using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSwitcher : MonoBehaviour
{
    private Animator animations;
    private GameObject player;

    //Lists for storing names of the animations
    public List<string> IdleAnims = new List<string>();
    public List<string> RunAnims = new List<string>();
    public List<string> IntroAnims = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        animations = gameObject.GetComponent<Animator>();

        //Add current animation names- more can be added publicly
        IdleAnims.Add("Techie-Run-End");
        IdleAnims.Add("Engineer-Idle");
        IdleAnims.Add("Butler-Idle");
        IdleAnims.Add("Punk-Idle");

        RunAnims.Add("Techie-Run-Start");
        RunAnims.Add("Engineer-Walk");
        IdleAnims.Add("Butler-Walk");
        IdleAnims.Add("Punk-Walk");

        IntroAnims.Add("Techie-Intro");
        IntroAnims.Add("Engineer-Intro");
        IdleAnims.Add("Butler-Intro");
        IdleAnims.Add("Punk-Intro");
    }

    //Note: Pass in name of the character from wherever these methods are called- 
    //locates the correct animation name within the attached Animator

    public void IdleAnimation(string playerName)
    {
        int animationIndex = 0;
        switch (playerName)
        {
            case "Techie":
                animationIndex = 0;
                break;

            case "Engineer":
                animationIndex = 1;
                break;
            case "Butler":
                animationIndex = 2;
                break;
            case "Singer":
                animationIndex = 3;
                break;
        }

        animations.Play(IdleAnims[animationIndex]);
    }

    public void RunAnimation(string playerName)
    {
        int animationIndex = 0;
        switch (playerName)
        {
            case "Techie":
                animationIndex = 0;
                break;

            case "Engineer":
                animationIndex = 1;
                break;
            case "Butler":
                animationIndex = 2;
                break;
            case "Singer":
                animationIndex = 3;
                break;
        }

        animations.Play(RunAnims[animationIndex]);
    }

    public void IntroAnimation(string playerName)
    {
        int animationIndex = 0;
        switch (playerName)
        {
            case "Techie":
                animationIndex = 0;
                break;

            case "Engineer":
                animationIndex = 1;
                break;
            case "Butler":
                animationIndex = 2;
                break;
            case "Singer":
                animationIndex = 3;
                break;
        }
        animations.Play(IntroAnims[animationIndex]);
    }

}
