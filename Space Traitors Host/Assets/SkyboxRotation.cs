using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RenderSettings;

public class SkyboxRotation : MonoBehaviour
{
    public float rotationSpeed = 2f;

    private static readonly int Rotation = Shader.PropertyToID("_Rotation");

    // Update is called once per frame
    void Update()
    {
        skybox.SetFloat(Rotation, Time.time * rotationSpeed);
    }
}
