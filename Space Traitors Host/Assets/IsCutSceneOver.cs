using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VectorGraphics;
using UnityEditor.Graphing.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IsCutSceneOver : MonoBehaviour
{
    public Animator sceneAnimator;

    public string nextServerScene = "Server GameLevel";
    public string nextClientScene = "Client GameLevel";
    public GameObject videoPanel;

    public float delayTimer = 1f;
    
    bool transitionTriggered = false;

    private bool videoStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        sceneAnimator = GameObject.FindGameObjectWithTag("SceneAnimator").GetComponent<Animator>();
        videoPanel = GameObject.FindGameObjectWithTag("VideoPanel");
        videoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(sceneAnimator.GetCurrentAnimatorStateInfo(0).IsName("Done") && !videoStarted)
        {
            videoStarted = true;
            videoPanel.SetActive(true);
            videoPanel.GetComponent<VideoPlayer>().Play();

        }
        
        if ((ulong)videoPanel.GetComponent<VideoPlayer>().frame == videoPanel.GetComponent<VideoPlayer>().frameCount - 1 && !transitionTriggered || Input.GetButtonDown("Submit") && !transitionTriggered)
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
