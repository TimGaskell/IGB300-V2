﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);
    private JoinRoomDelegate joinRoomCallback;

    [SerializeField]
    private Text RoomNameText;

    private MatchInfoSnapshot match;

    public void setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallback) {

        match = _match;
        joinRoomCallback = _joinRoomCallback;

        RoomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";


    }

    public void JoinRoom() {

        joinRoomCallback.Invoke(match);


    }



}
