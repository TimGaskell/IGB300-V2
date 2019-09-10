using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpecGraphManager : MonoBehaviour
{
    public GameManager.SpecScores specScore;

    public GameObject specCounter;
    public GameObject moddedBar;
    public GameObject scaledBar;

    public int maxSpecScore;

    public void UpdateSpecGraph()
    {
        float scaledSpec = ClientManager.instance.GetScaledSpecScore(specScore);
        float moddedSpec = ClientManager.instance.GetModdedSpecScore(specScore);

        specCounter.GetComponent<TextMeshProUGUI>().text = scaledSpec.ToString();
        //The max spec score will be when the image is at its fullest. Scale by maxSpecScore to get the spec scores scaling
        //relative to this maximum
        scaledBar.GetComponent<Image>().fillAmount = scaledSpec / (maxSpecScore * 100);
        moddedBar.GetComponent<Image>().fillAmount = moddedSpec / (maxSpecScore * 100);
    }
}
