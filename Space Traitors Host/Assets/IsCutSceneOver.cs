using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsCutSceneOver : MonoBehaviour
{
    public Animator sceneAnimator;

    public string nextServerScene = "Server GameLevel";
    public string nextClientScene = "Client GameLevel";

    public float delayTimer = 1f;
    
    bool transitionTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        sceneAnimator = GameObject.FindGameObjectWithTag("SceneAnimator").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (sceneAnimator.GetCurrentAnimatorStateInfo(0).IsName("Done") && !transitionTriggered)
        {
            transitionTriggered = true;
            StartCoroutine(NextScene());
        }
    }

    public IEnumerator NextScene()
    {
        yield return new WaitForSeconds(delayTimer);
        Server.Instance.SendChangeScene(nextClientScene);
        //Change to the character select
        SceneManager.LoadScene(nextServerScene);
    }
}
