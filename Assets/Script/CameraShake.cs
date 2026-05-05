using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("CinemachineVirtualCamera 참조")]
    public CinemachineVirtualCamera virtualCamera;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    void Start()
    {
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise == null)
            {
                //Debug.LogError("CinemachineBasicMultiChannelPerlin 컴포넌트가 비주얼 카메라에 없습니다.");
            }
        }
        else
        {
            //Debug.LogError("버츄얼 카메라가 없습니다.");
        }

        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
        }
    }

    public void StartShake(float intensity, float duration)
    {
        if (noise == null)
        {
            return;
        }

        noise.m_AmplitudeGain = intensity;
        shakeTimer = duration;
        shakeTimerTotal = duration;
        startingIntensity = intensity;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            noise.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
        else if (noise.m_AmplitudeGain != 0f)
        {
            noise.m_AmplitudeGain = 0f;
        }
    }
}