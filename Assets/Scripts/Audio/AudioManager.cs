﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found an UI Audio object, destroying new one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    public void PlaySound(string audioLibrarySourceSound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(audioLibrarySourceSound, position);
    }

}
