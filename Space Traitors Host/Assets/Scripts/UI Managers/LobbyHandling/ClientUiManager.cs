﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ClientUiManager : MonoBehaviour
{
    public GameObject PlayerInput;
    private GameObject PlayerObject;

    public void Start() {

       PlayerObject =  GameObject.Find("PlayerInfoHolder");

    }


    public void SubmitPlayerName(GameObject TextField) {

        string name = TextField.GetComponent<TMP_InputField>().text;

        if (name != "") {

            PlayerInput.GetComponent<CanvasGroup>().interactable = false;
            Server.Instance.SendPlayerInformation(name);
            PlayerObject.GetComponent<Player>().playerName = name;
            
        }
        else {
            Debug.Log("Enter a name");
        }
    }
}