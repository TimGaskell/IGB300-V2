using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneChange: NetMessage {
    public SceneChange() {

        OperationCode = NetOP.ChangeScenes;

    }


    
    public string SceneName { set; get; }


}
