using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip connectSound;
    public AudioClip failureSound;
    public AudioClip successSound;
    public AudioClip notificationSound;

    public static SFXManager instance;

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

    public void PlaySoundEffect(AudioClip audioClip)
    {

        if (!audioSource.isPlaying || audioSource.clip != audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
