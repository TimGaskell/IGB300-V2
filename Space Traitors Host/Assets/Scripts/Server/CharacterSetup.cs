﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSetup : MonoBehaviour
{
    private const string SELECTING_TEXT = "SELECTING";

    public RenderTexture[] cameraTextures = new RenderTexture[5];
    public GameObject[] viewPanels = new GameObject[4];
    public GameObject[] characterModels = new GameObject[4];
    public TextMeshProUGUI[] PlayerNames = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] descriptionText = new TextMeshProUGUI[4];
    public GameObject[] portraitGroups;

    private int playerPos;

    private void Start()
    {
        playerPos = 0;
        descriptionText[playerPos].text = SELECTING_TEXT;

        for (int i = 1; i < GameManager.instance.MAX_PLAYERS + 1; i++)
        {
            if(i <= GameManager.instance.numPlayers)
            {
                portraitGroups[i-1].SetActive(true);
                //Get the index of the reverse order of players, the assign the names of the players to each of them
                //E.G. the 4th slot is player 1 and has player 1's name, and so on
                PlayerNames[(GameManager.instance.numPlayers) - (i)].text = GameManager.instance.GetOrderedPlayer(i).playerName;
            }
            else
            {
                portraitGroups[i-1].SetActive(false);
            }
        }

    }

    public void CharacterChosen(int playerNo, Character.CharacterTypes characterType)
    {
        int characterID = 0;

        //Get the reverse player based on number of players- if player 1, they will go 4th in select, and vice versa
        //Result will be 1 less than intended player ID to work better for arrays 
        switch (characterType)
        {
            case Character.CharacterTypes.Butler:
                characterID = 0;
                break;

            case Character.CharacterTypes.Engineer:
                characterID = 1;
                break;

            case Character.CharacterTypes.Techie:
                characterID = 2;
                break;

            case Character.CharacterTypes.Singer:
                characterID = 3;
                break;
        }

        //Set the view of player's character panel to the view of the chosen character
        viewPanels[playerPos].GetComponent<RawImage>().texture = cameraTextures[characterID];

        //Update the character text to show the character's been selected
        descriptionText[playerPos].text = characterType.ToString().ToUpper();

        playerPos++;

        descriptionText[playerPos].text = SELECTING_TEXT;

        //Play that character's intro animation
        characterModels[characterID].GetComponent<AnimationSwitcher>().IntroAnimation(characterType);

        
    }
}