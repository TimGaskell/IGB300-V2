using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tessst : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InteractionManager manager = GameObject.Find("GamePanels").transform.Find("InteractionPanel").gameObject.GetComponent<InteractionManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
