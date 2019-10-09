using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip aiMusic;
    public AudioClip victoryMusic;

    public static MusicManager instance;

    private void Awake()
    {
        //Singleton Setup
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //Scene currentScene = SceneManager.GetActiveScene();
        //string sceneName = currentScene.name;
        

        //if (sceneName == "server")
        //{
        //    if (GameManager.instance.CurrentVictory == GameManager.VictoryTypes.NonTraitor)
        //    {
        //        ChangeMusicClip(victoryMusic);
        //    }
        //    else
        //    {
        //        ChangeMusicClip(gameMusic);
        //    }
        //}
        //else if (sceneName == "Character Select" || sceneName == "LobbyTest")
        //{
        //    ChangeMusicClip(menuMusic);
        //}
    }

    private void StopMusic()
    {
        audioSource.Stop();
    }

    public void ChangeMusicClip(AudioClip audioClip)
    {
        
        if (!audioSource.isPlaying || audioSource.clip != audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
