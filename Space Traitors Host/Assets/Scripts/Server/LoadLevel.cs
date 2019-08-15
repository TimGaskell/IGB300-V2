using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadLevel : MonoBehaviour
{
    private NetworkManager networkManager;


    void Start() {

        networkManager = NetworkManager.singleton;

    }

    public void ChangeScene(string SceneName) {

        NetworkManager.singleton.ServerChangeScene(SceneName);

        


    }
}
