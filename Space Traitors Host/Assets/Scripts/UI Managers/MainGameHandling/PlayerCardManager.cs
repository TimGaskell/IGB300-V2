using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCardManager : MonoBehaviour
{
    public GameObject activePlayerPanel;

    public List<GameObject> playerPanels;

    public void UpdateActivePlayer()
    {
        activePlayerPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameManager.instance.GetActivePlayer().playerName;
    }
}
