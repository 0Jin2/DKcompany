using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager1 : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource != null)
        {
            audioSource.volume = 0.3f;
        }
    }
}