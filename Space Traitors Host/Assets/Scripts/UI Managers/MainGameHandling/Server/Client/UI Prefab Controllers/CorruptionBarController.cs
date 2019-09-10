using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionBarController : MonoBehaviour
{
    public GameObject corruptionBar;

    public void UpdateCorruptionBar()
    {
        corruptionBar.GetComponent<Image>().fillAmount = ClientManager.instance.corruption / 100;
    }
}
