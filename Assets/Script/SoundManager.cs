using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;
    [SerializeField] AudioClip columnDoneSound;
    [SerializeField] AudioClip moveDiamondSound;
    [SerializeField] AudioClip uiClickSound;
    [SerializeField] AudioClip winSound;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }
    public void PlayColumnDoneSound()
    {
        audioSource.PlayOneShot(columnDoneSound);
    }
    public void PlayMoveDiamondSound()
    {
        audioSource.PlayOneShot(moveDiamondSound);
    }
    public void PlayUIClickSound()
    {
        audioSource.PlayOneShot(uiClickSound);
    }
    public void PlayWinSound()
    {
        audioSource.PlayOneShot(winSound);
    }
}