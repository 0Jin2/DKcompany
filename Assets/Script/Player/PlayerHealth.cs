using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Cinemachine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("결과 UI 프리팹")]
    public GameObject gameOverUIPrefabB1;
    public GameObject gameOverUIPrefabB2;
    public GameObject gameOverUIPrefabBOSS;
    public GameObject gameOverUIPrefabB1_HARD;
    public GameObject gameOverUIPrefabB2_HARD;
    public GameObject gameOverUIPrefabBOSS_HARD;

    [Header("하트 프리팹")]
    public GameObject heartPrefab1;
    public GameObject heartPrefab2;
    public GameObject heartPrefab3;

    [Header("하트 이미지")]
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("오디오 클립")]
    [SerializeField] public AudioClip gameOverClip;
    [SerializeField] public AudioClip damageClip;
    [SerializeField] public AudioMixerGroup audioMixerGroup; 
    [SerializeField] public AudioMixerGroup damageMixerGroup;
    private AudioSource audioSource;

    [Header("체력 감소 딜레이")]
    public float invincibilityDuration = 1f;
    private bool isGameOver = false;

    private Animator animator;
    private PlayerMovement playerMovement;
    private int currentHeartIndex;
    private bool isInvincible = false;
    private GameObject gameOverUIInstance;
    private List<GameObject> hearts;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private float originalAmplitudeGain;
    private float originalFrequencyGain;

    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;

        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeComponents();
        InitializeHearts();
        ResetDieTrigger();
        HandleSceneChange(SceneManager.GetActiveScene().name);

        if (virtualCamera != null)
        {
            cameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (cameraNoise != null)
            {
                originalAmplitudeGain = cameraNoise.m_AmplitudeGain;
                originalFrequencyGain = cameraNoise.m_FrequencyGain;
            }
        }
    }

    void InitializeHearts()
    {
        hearts = new List<GameObject>();
        CreateHearts();
    }

    void CreateHearts()
    {
        GameObject canvas = GameObject.Find("Canvas");

        foreach (GameObject heart in hearts)
        {
            if (heart != null)
            {
                Destroy(heart); 
            }
        }
        hearts.Clear();

        hearts.Add(Instantiate(heartPrefab1, canvas.transform));
        hearts.Add(Instantiate(heartPrefab2, canvas.transform));
        hearts.Add(Instantiate(heartPrefab3, canvas.transform));

        currentHeartIndex = hearts.Count - 1;

        DisableHeartsInSpecificScenes();
    }


    void DisableHeartsInSpecificScenes()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "LobbyScene" || currentScene == "LoadingScene1" || currentScene == "LoadingScene2" || currentScene == "LoadingScene3")
        {
            foreach (GameObject heart in hearts)
            {
                heart.SetActive(false);
            }
        }
        else if (currentScene == "PlayB1" || currentScene == "PlayB2" || currentScene == "PlayBOSS" || currentScene == "PlayB1_HARD" || currentScene == "PlayB2_HARD" || currentScene == "PlayBOSS_HARD" || currentScene == "Tutorial")
        {
            foreach (GameObject heart in hearts)
            {
                heart.SetActive(true);
            }
        }
    }

    void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (virtualCamera != null)
        {
            cameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (cameraNoise != null)
            {
                originalAmplitudeGain = cameraNoise.m_AmplitudeGain;
                originalFrequencyGain = cameraNoise.m_FrequencyGain;
            }
        }
        else
        {
            Debug.LogWarning("PlayBOSS 씬에서 CinemachineVirtualCamera를 찾을 수 없습니다.");
        }

        HandleSceneChange(scene.name);

        ResetDieTrigger();

        Time.timeScale = 1f;

        isGameOver = false;

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        DisableHeartsInSpecificScenes();
    }

    void ResetDieTrigger()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Idle", 0, 0.0f);
        }
    }

    void HandleSceneChange(string sceneName)
    {
        if (sceneName == "PlayB1")
        {
            SetGameOverUIPrefab(gameOverUIPrefabB1);
        }
        else if (sceneName == "PlayB2")
        {
            SetGameOverUIPrefab(gameOverUIPrefabB2);
        }
        else if (sceneName == "PlayBOSS")
        {
            SetGameOverUIPrefab(gameOverUIPrefabBOSS);
        }
        else if (sceneName == "PlayB1_HARD")
        {
            SetGameOverUIPrefab(gameOverUIPrefabB1_HARD);
        }
        else if (sceneName == "PlayB2_HARD")
        {
            SetGameOverUIPrefab(gameOverUIPrefabB2_HARD);
        }
        else if (sceneName == "PlayBOSS_HARD")
        {
            SetGameOverUIPrefab(gameOverUIPrefabBOSS_HARD);
        }
        else if (sceneName == "Tutorial")
        {
            SetGameOverUIPrefab(gameOverUIPrefabBOSS);
        }
        CreateHearts();
    }

    void SetGameOverUIPrefab(GameObject prefab)
    {
        if (gameOverUIInstance != null)
        {
            Destroy(gameOverUIInstance);
        }

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null && prefab != null)
        {
            gameOverUIInstance = Instantiate(prefab, canvas.transform);
            gameOverUIInstance.SetActive(false); 
        }
    }

    void Update()
    {
        if (isGameOver)
        {
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                int dieStateHash = Animator.StringToHash("Die");
                if (stateInfo.shortNameHash == dieStateHash)
                {

                }
                else
                {

                }
            }

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(moveHorizontal, moveVertical);
            transform.Translate(movement * Time.unscaledDeltaTime);
        }
    }

    private void PlayDamageAudio()
    {
        if (damageClip != null)
        {
            audioSource.outputAudioMixerGroup = damageMixerGroup; 
            audioSource.PlayOneShot(damageClip);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Monster1") || collision.gameObject.CompareTag("Monster2")) && !isInvincible)
        {
            if (currentHeartIndex >= 0)
            {
                ChangeHeartSprite(currentHeartIndex);
                currentHeartIndex--;

                PlayDamageAudio();
                StartCoroutine(Invincibility());
            }

            if (currentHeartIndex < 0)
            {
                GameOver();
            }
        }
    }

    void ChangeHeartSprite(int heartIndex)
    {
        if (hearts[heartIndex] != null)
        {
            Image heartImage = hearts[heartIndex].GetComponent<Image>();
            if (heartImage != null)
            {
                // 하트 스프라이트를 빈 하트 이미지로 변경
                heartImage.sprite = emptyHeartSprite;
            }

            if (!isGameOver && currentHeartIndex > 0)
            {
                StartCoroutine(Invincibility());
            }
            else if (currentHeartIndex < 0) // 마지막 하트가 없어지면 GameOver 호출
            {
                GameOver();
            }
        }
    }

    IEnumerator FlashPlayer()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;

        while (isInvincible) // 무적 상태가 지속되는 동안 깜빡임 효과를 수행
        {
            if (cameraNoise != null)
            {
                cameraNoise.m_AmplitudeGain = 1.5f; // 흔들림 강도 설정
                cameraNoise.m_FrequencyGain = 0.5f; // 흔들림 빈도 설정
            }
            else
            {
                Debug.LogWarning("CinemachineBasicMultiChannelPerlin 컴포넌트를 찾을 수 없습니다.");
            }

            // 반투명 설정 (깜빡임 효과)
            spriteRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
            yield return new WaitForSeconds(0.08f);

            // 원래 색상으로 복구
            spriteRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.08f);
        }
        
        if (cameraNoise != null)
        {
            cameraNoise.m_AmplitudeGain = originalAmplitudeGain;
            cameraNoise.m_FrequencyGain = originalFrequencyGain;
        }

        // 무적 시간이 종료되면 원래 색상으로 복원
        spriteRenderer.color = originalColor;
    }

    IEnumerator Invincibility()
    {
        if (currentHeartIndex < 0) yield break; // 마지막 하트가 사라졌을 때는 아무 작업도 하지 않음

        isInvincible = true;

        StartCoroutine(FlashPlayer());
        yield return new WaitForSeconds(invincibilityDuration - 0.1f);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        yield return new WaitForSeconds(0.1f);

        // 무적 상태 해제
        isInvincible = false;

        if (currentHeartIndex < 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("FanAttack", false);
            animator.SetBool("HitterAttack", false);
            animator.SetTrigger("Die");
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        DisableBackgroundAudio();
        PlayGameOverAudio();
        audioSource.ignoreListenerPause = true;

        Time.timeScale = 0f;

        DestroyTimeAttackObject();
        StartCoroutine(ActivateGameOverUIAfterDelay(1.5f));
    }

    private void DestroyTimeAttackObject()
    {
        TimeAttack timeAttack = FindObjectOfType<TimeAttack>();
        if (timeAttack != null)
        {
            Destroy(timeAttack.gameObject); 
        }
    }

    private void PlayGameOverAudio()
    {
        if (gameOverClip != null)
        {
            audioSource.PlayOneShot(gameOverClip);
        }
    }

    private void DisableBackgroundAudio()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            BackgroundAudio bgAudio = player.GetComponent<BackgroundAudio>();
            if (bgAudio != null)
            {
                bgAudio.PauseBackgroundAudio(); 
            }
        }
    }

    private IEnumerator ActivateGameOverUIAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); 
        if (gameOverUIInstance != null)
        {
            gameOverUIInstance.SetActive(true);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}