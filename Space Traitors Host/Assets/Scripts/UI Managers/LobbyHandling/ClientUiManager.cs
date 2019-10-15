using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientUiManager : MonoBehaviour
{
    public GameObject PlayerInput;
    public GameObject errorText;
    private const int maxChar = 10;

    public void Start() {

        SetErrorText("", Color.black);

    }


    public void SubmitPlayerName(GameObject TextField)
    {

        string name = TextField.GetComponent<TMP_InputField>().text;

        if (name != "")
        {
            if (name.Length > maxChar)
            {
                SetErrorText("Name is too long", Color.red);
                TextField.GetComponent<TMP_InputField>().text = "";
            }
            else
            {
                PlayerInput.GetComponent<CanvasGroup>().interactable = false;
                SetErrorText("Please wait", Color.white);
                Server.Instance.SendPlayerInformation(name);
                ClientManager.instance.playerName = name;
            }
        }
        
        else
        {
            SetErrorText("Please enter a name", Color.red);
        }
    }

    private void SetErrorText(string errorString, Color fontColor)
    {
        errorText.GetComponent<TextMeshProUGUI>().text = errorString;
        errorText.GetComponent<TextMeshProUGUI>().color = fontColor;
    }
}
