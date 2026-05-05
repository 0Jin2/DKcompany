using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // 오디오 소스를 받을 public 변수
    public AudioSource audioSource;

    // 초기화 메서드
    void Start()
    {
        // AudioSource가 있을 경우 볼륨을 0.3으로 설정합니다.
        if (audioSource != null)
        {
            audioSource.volume = 0.3f;
        }
        else
        {
            Debug.LogWarning("AudioSource is not assigned.");
        }
    }
}