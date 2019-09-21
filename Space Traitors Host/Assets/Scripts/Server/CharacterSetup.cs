using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSetup : MonoBehaviour
{
    public RenderTexture[] cameraTextures = new RenderTexture[5];
    public GameObject[] viewPanels = new GameObject[4];
    public GameObject[] characterModels = new GameObject[4];
    public TextMeshProUGUI[] PlayerNames = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] descriptionText = new TextMeshProUGUI[4];

    private void Start()
    {
       for (int i = 0; i < GameManager.instance.numPlayers; i++)
        {
            //Get the index of the reverse order of players, the assign the names of the players to each of them
            //E.G. the 4th slot is player 1 and has player 1's name, and so on
            PlayerNames[(GameManager.instance.numPlayers) - (i + 1)].text = GameManager.instance.GetOrderedPlayer(i).playerName;
        }

    }


    private void Update()
    {
        //Test
       // if (Input.GetKeyDown("space"))
       // {
        //    CharacterChosen(1, "Techie");
       // }
       
    }

    public void CharacterChosen(int playerNo, string characterName)
    {
        int characterID = 0;

        //Get the reverse player based on number of players- if player 1, they will go 4th in select, and vice versa
        //Result will be 1 less than intended player ID to work better for arrays 
        playerNo = GameManager.instance.numPlayers - playerNo;

        switch (characterName)
        {
            case ("Butler"):
                characterID = 0;
                break;

            case "Engineer":
                characterID = 1;
                break;

            case "Techie":
                characterID = 2;
                break;

            case "Singer":
                characterID = 3;
                break;
        }

        //Set the view of player's character panel to the view of the chosen character
        viewPanels[playerNo].GetComponent<RawImage>().texture = cameraTextures[characterID];

        //Play that character's intro animation
        characterModels[characterID].GetComponent<AnimationSwitcher>().IntroAnimation(characterName);

        //Update the character text to show the character's been selected
        descriptionText[playerNo].text = characterName.ToUpper() + " SELECTED";
    }
}
