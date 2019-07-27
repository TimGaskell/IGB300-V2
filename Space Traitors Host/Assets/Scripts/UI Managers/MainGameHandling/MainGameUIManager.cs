using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainGameUIManager : MonoBehaviour
{
    public GameObject serverActivePanel;
    public GameObject noServerPanel;

    public GameObject playerCards;

    public GameObject abilityPanel;
    public GameObject actionPointPanel;
    public GameObject movementPanel;
    public GameObject interactionPanel;
    public GameObject basicSurgePanel;
    public GameObject attackSurgePanel;

    private void Start()
    {
        if (GameManager.instance.serverActive)
        {
            serverActivePanel.SetActive(true);
            noServerPanel.SetActive(false);
        }
        else
        {
            serverActivePanel.SetActive(false);
            noServerPanel.SetActive(true);

            abilityPanel.SetActive(false);
            actionPointPanel.SetActive(false);
            movementPanel.SetActive(false);
            interactionPanel.SetActive(false);
            basicSurgePanel.SetActive(false);
            attackSurgePanel.SetActive(false);

            DisplayCurrentPhase();
        }
    }

    private void Update()
    {
        if (GameManager.instance.serverActive)
        {
            throw new NotImplementedException("Server Not Implemented");
        }
        else
        {
            //Detects if the movement phase is over when the player model has stopped moving.
            //Need to detect if room selection is true otherwise will entirely skip over the movement phase.
            if (!GameManager.instance.playerMoving && !GameManager.instance.roomSelection &&
                GameManager.instance.currentPhase == GameManager.TurnPhases.Movement)
            {
                IncrementPhase();
            }
        }
    }

    #region No Server Handling

    /// <summary>
    /// 
    /// Enables or disables the panels pertaining to the current phase of the game
    /// 
    /// </summary>
    private void DisplayCurrentPhase()
    {
        switch (GameManager.instance.currentPhase)
        {
            case (GameManager.TurnPhases.Abilities):
                basicSurgePanel.SetActive(false);
                attackSurgePanel.SetActive(false);
                abilityPanel.SetActive(true);
                playerCards.GetComponent<PlayerCardManager>().UpdateActivePlayer();
                break;
            case (GameManager.TurnPhases.ActionPoints):
                abilityPanel.SetActive(false);
                actionPointPanel.SetActive(true);
                break;
            case (GameManager.TurnPhases.Movement):
                actionPointPanel.SetActive(false);
                movementPanel.SetActive(true);
                break;
            case (GameManager.TurnPhases.Interaction):
                movementPanel.SetActive(false);
                interactionPanel.SetActive(true);
                break;
            case (GameManager.TurnPhases.BasicSurge):
                interactionPanel.SetActive(false);
                basicSurgePanel.SetActive(true);
                break;
            case (GameManager.TurnPhases.AttackSurge):
                interactionPanel.SetActive(false);
                attackSurgePanel.SetActive(true);
                break;
            default:
                throw new NotImplementedException("Not a valid phase");
        }
    }

    /// <summary>
    /// 
    /// Increments the current phase of the game
    /// 
    /// </summary>
    private void IncrementPhase()
    {
        GameManager.instance.IncrementPhase();
        DisplayCurrentPhase();
    }

    #endregion
}
