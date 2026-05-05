using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySoundOnDisable : MonoBehaviour
{
    public GameObject backgroundMusicObject;  // 배경 음악 오디오 소스가 있는 오브젝트
    public AudioSource activationAudioSource; // 활성화될 때 재생할 오디오 소스
    public AudioClip activationAudioClip;     // 활성화될 때 재생할 오디오 클립
    public AudioMixerGroup audioMixerGroup;   // 오디오 출력 경로 설정을 위한 AudioMixerGroup

    void OnEnable()
    {
        if (backgroundMusicObject != null)
        {
            backgroundMusicObject.SetActive(false);
        }

        if (activationAudioSource != null && audioMixerGroup != null)
        {
            activationAudioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        if (activationAudioSource != null && activationAudioClip != null)
        {
            activationAudioSource.clip = activationAudioClip;
            activationAudioSource.Play();
        }
    }

    void OnDisable()
    {
        if (backgroundMusicObject != null)
        {
            backgroundMusicObject.SetActive(true);
        }
    }
}