using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BossIntro : MonoBehaviour
{
    [Header("보스 애니메이터")]
    public Animator bossAnimator;
    [Header("보스 오브젝트")]
    public Transform boss;
    [Header("보스 이동 타겟")]
    public Transform walkTarget;
    [Header("보스 이동 속도")]
    public float walkSpeed = 2f;

    [Header("카메라")]
    public CameraShake cameraShake;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 5.0f;

    [Header("오디오 클립")]
    public AudioClip roarClip; // Roar 사운드 클립
    public AudioClip footClip; // Footstep 사운드 클립

    [Header("오디오 믹서 그룹")]
    public AudioMixerGroup roarMixerGroup; // Roar 사운드용 믹서 그룹
    public AudioMixerGroup footMixerGroup; // Footstep 사운드용 믹서 그룹

    private AudioSource audioSource;
    private bool isWalking = true;
    private bool hasRoared = false;
    private FindPB findPB;
    private bool hasPlayedRoar = false;
    private bool hasPlayedFootstep = false;

    void Start()
    {
        findPB = FindObjectOfType<FindPB>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
    }

    void Update()
    {
        if (isWalking)
        {
            boss.position = Vector3.MoveTowards(boss.position, walkTarget.position, walkSpeed * Time.deltaTime);

            if (Vector3.Distance(boss.position, walkTarget.position) < 0.1f)
            {
                isWalking = false;
                bossAnimator.SetTrigger("Roar");
            }
        }

        if (!isWalking && !hasRoared && bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("Roar") && bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            hasRoared = true;
            bossAnimator.SetTrigger("Idle");
            findPB.SwitchToPlayer();
        }
    }

    public void StartShake()
    {
        if (cameraShake != null)
        {
            cameraShake.StartShake(shakeMagnitude, shakeDuration);
        }
        else
        {
            Debug.LogError("CameraShake reference is missing");
        }
    }

    public void PlayRoarSound()
    {
        if (!hasPlayedRoar && roarClip != null)
        {
            audioSource.outputAudioMixerGroup = roarMixerGroup; // 믹서 그룹 설정
            audioSource.PlayOneShot(roarClip); // Roar 클립 재생
            hasPlayedRoar = true; // 재생 여부 업데이트
        }
    }

    public void PlayFootstepSound()
    {
        if (!hasPlayedFootstep && footClip != null)
        {
            audioSource.outputAudioMixerGroup = footMixerGroup; // 믹서 그룹 설정
            audioSource.PlayOneShot(footClip); // Foot 클립 재생
            hasPlayedFootstep = true; // 재생 여부 업데이트
        }
    }
}