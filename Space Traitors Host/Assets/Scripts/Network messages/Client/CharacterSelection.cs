using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSelection : NetMessage {

    public CharacterSelection() {

        OperationCode = NetOP.CharacterSelection;

    }
    public int SelectedCharacter{ set; get; }
    
}