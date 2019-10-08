using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.Serialization;

public class ShineController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> Shines = new List<GameObject>();
    [FormerlySerializedAs("WaitTime")] public float maxWaitTime = 1f;
    public float minWaitTime = 1f;
    private static readonly int Shine1 = Animator.StringToHash("Shine");


    void Start()
    {
        StartCoroutine(Shine());
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    IEnumerator Shine()
    {
        while (true)
        {
            foreach (var shine in Shines)
            {
                if (shine != null) shine.GetComponent<Animator>().SetTrigger(Shine1);
                yield return new WaitForSeconds(Random.Range(min: minWaitTime, max: maxWaitTime));
            }
        }
        
    }

    
}


