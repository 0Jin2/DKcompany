using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio; // AudioMixer 관련 네임스페이스

public class BackgroundAudio : MonoBehaviour
{
    [Header("배경 오디오 클립 리스트")]
    public BackgroundMusic backgroundMusic;  // 배경 음악을 묶어서 관리

    [System.Serializable]
    public class BackgroundMusic
    {
        [Header("로비 배경 오디오 클립")]
        public AudioClip lobbySceneAudio;
        [Header("로딩1 배경 오디오 클립")]
        public AudioClip loadingScene1Audio;
        [Header("로딩2 배경 오디오 클립")]
        public AudioClip loadingScene2Audio;
        [Header("1스테이지 배경 오디오 클립")]
        public AudioClip playB1Audio;
        [Header("2스테이지 배경 오디오 클립")]
        public AudioClip playB2Audio;
        [Header("보스 스테이지 배경 오디오 클립")]
        public AudioClip playBOSSAudio;
    }

    [Header("클릭 오디오 클립 리스트")]
    public AudioClip clickSound;  // 마우스 클릭 시 재생될 오디오 클립

    [Header("오디오 믹서 그룹 참조")]
    public AudioMixerGroup backgroundMusicOutputGroup;
    public AudioMixerGroup clickSoundOutputGroup;

    private AudioSource backgroundAudioSource;
    private AudioSource clickAudioSource;
    private static BackgroundAudio instance;
    private float currentAudioTime = 0f;

    void Awake()
    {
        // Singleton 패턴으로 Audio 유지
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 오디오 소스 설정
            backgroundAudioSource = gameObject.AddComponent<AudioSource>();  // 배경음악용
            clickAudioSource = gameObject.AddComponent<AudioSource>();       // 클릭 사운드용
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (backgroundAudioSource == null)
        {
            backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        }
        if (backgroundAudioSource != null && backgroundMusicOutputGroup != null)
        {
            backgroundAudioSource.outputAudioMixerGroup = backgroundMusicOutputGroup;
        }

        if (clickAudioSource == null)
        {
            clickAudioSource = gameObject.AddComponent<AudioSource>();
        }
        if (clickAudioSource != null && clickSoundOutputGroup != null)
        {
            clickAudioSource.outputAudioMixerGroup = clickSoundOutputGroup;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        backgroundAudioSource.loop = true;
        PlayBackgroundAudio("LobbyScene");
    }

    void Update()
    {
        // 마우스 클릭 시 클릭 사운드 재생
        if (Input.GetMouseButtonDown(0))  // 왼쪽 마우스 버튼 클릭
        {
            PlayClickSound();
        }
    }

    // 씬이 로드될 때 호출되는 메서드
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!backgroundAudioSource.isPlaying)
        {
            ResumeBackgroundAudio();  // 비활성화된 경우 다시 활성화
        }

        PlayBackgroundAudio(scene.name);  // 씬에 맞는 배경 음악 재생
    }

    // 각 씬에 맞는 배경 음악 재생
    void PlayBackgroundAudio(string sceneName)
    {
        AudioClip clipToPlay = null;

        // 씬 이름에 따라 배경 오디오 클립을 변경
        if (sceneName == "LobbyScene")
        {
            clipToPlay = backgroundMusic.lobbySceneAudio;
        }
        else if (sceneName == "LoadingScene1")
        {
            clipToPlay = backgroundMusic.loadingScene1Audio;
        }
        else if (sceneName == "LoadingScene2")
        {
            clipToPlay = backgroundMusic.loadingScene2Audio;
        }
        else if (sceneName == "LoadingScene3")
        {
            clipToPlay = backgroundMusic.loadingScene2Audio;
        }
        else if (sceneName == "PlayB1")
        {
            clipToPlay = backgroundMusic.playB1Audio;
        }
        else if (sceneName == "PlayB2")
        {
            clipToPlay = backgroundMusic.playB2Audio;
        }
        else if (sceneName == "PlayBOSS")
        {
            clipToPlay = backgroundMusic.playBOSSAudio;
        }
        else if (sceneName == "PlayB1_HARD")
        {
            clipToPlay = backgroundMusic.playB1Audio;
        }
        else if (sceneName == "PlayB2_HARD")
        {
            clipToPlay = backgroundMusic.playB2Audio;
        }
        else if (sceneName == "PlayBOSS_HARD")
        {
            clipToPlay = backgroundMusic.playBOSSAudio;
        }
        else if (sceneName == "Tutorial")
        {
            clipToPlay = backgroundMusic.playBOSSAudio;
        }

        // 배경 음악이 새로 설정된 경우만 재생
        if (clipToPlay != null)
        {
            // 현재 재생 중인 클립이 다를 경우에만 오디오 변경
            if (backgroundAudioSource.clip != clipToPlay)
            {
                backgroundAudioSource.clip = clipToPlay;
                backgroundAudioSource.time = 0;  // 새로운 클립은 처음부터 재생
                backgroundAudioSource.Play();
            }
            else
            {
                // 같은 클립일 경우, 이어서 재생
                if (!backgroundAudioSource.isPlaying)
                {
                    backgroundAudioSource.Play();
                }
            }
        }
    }

    // 마우스 클릭 시 클릭 사운드 재생 (클릭 사운드는 중복 재생 가능)
    void PlayClickSound()
    {
        if (clickSound != null)
        {
            clickAudioSource.PlayOneShot(clickSound);  // 클릭 사운드 재생
        }
    }

    // 배경 음악 멈추기 (게임 일시정지나 게임 오버 시)
    public void PauseBackgroundAudio()
    {
        if (backgroundAudioSource.isPlaying)
        {
            currentAudioTime = backgroundAudioSource.time;  // 현재 재생 위치 저장
            backgroundAudioSource.Pause(); // 배경 음악 일시정지
        }
    }

    // 배경 음악 다시 재생 (게임 재시작 시)
    public void ResumeBackgroundAudio()
    {
        if (backgroundAudioSource.clip != null)
        {
            backgroundAudioSource.time = currentAudioTime;  // 저장된 재생 위치에서 다시 재생
            backgroundAudioSource.Play();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}