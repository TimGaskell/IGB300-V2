using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBackgrounds : MonoBehaviour
{
    public string colourName = "white"; // {white, blue, orange, green, purple};
    private Color colour;
    public bool ChangeColour = true;
    // Start is called before the first frame update
    void Start()
    {
        if (ChangeColour)
        {
            switch (colourName)
            {
                case "blue":
                    colour = new Color(0f, 0.7f, 1f);
                    break;
                case "orange":
                    colour = new Color(0.7f, 0.4f, 0.1f);
                    break;
                case "green":
                    colour = new Color(0.6f, 0.9f, 0.7f);
                    break;
                case "purple":
                    colour = new Color(0.4f, 0.2f, 1f);
                    break;
                case "white":
                    colour = Color.white;
                    break;
            }
            gameObject.GetComponent<Renderer>().material.color = colour;
        }
    }
}
