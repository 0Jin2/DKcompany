using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TimeAttack : MonoBehaviour
{
    [Header("텍스트 오브젝트 참조")]
    [SerializeField] private TMP_Text text;

    [Header("시간")]
    [SerializeField] private float time;
    [SerializeField] private float curTime;

    [Header("GameClear 팝업 프리팹")]
    [SerializeField] private GameObject winPopup;

    [Header("카운트다운 오디오 클립")]
    [SerializeField] private AudioClip countdownClip;
    [SerializeField] private AudioMixerGroup countdownMixerGroup;

    [Header("게임 클리어 오디오 클립")]
    [SerializeField] private AudioClip gameClearClip;
    [SerializeField] private AudioMixerGroup gameClearMixerGroup;

    [Header("백그라운드 오디오 믹서 그룹")]
    [SerializeField] private AudioMixerGroup backgroundMixerGroup;

    private Animator playerAnimator; 
    private bool isWinAnimationCompleted = false;
    private bool isPaused = false;

    private Coroutine timerCoroutine;
    private AudioSource audioSource;
    public bool isWin = false;

    private int previousSecond = -1;

    int minute;
    int second;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;

        string currentSceneName = SceneManager.GetActiveScene().name;
        GameObject targetObject = null;

        // PlayBOSS 씬이면 BOSS 태그를 가진 오브젝트를 찾고, 아니면 Player 태그를 찾음
        if (currentSceneName == "PlayBOSS" || currentSceneName == "PlayBOSS_HARD")
        {
            targetObject = GameObject.FindGameObjectWithTag("BOSS");
        }
        else
        {
            targetObject = GameObject.FindGameObjectWithTag("Player");
        }

        if (targetObject != null)
        {
            playerAnimator = targetObject.GetComponent<Animator>();

            if (playerAnimator != null)
            {
                playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            else
            {
                Debug.LogError($"{targetObject.tag} 오브젝트에 Animator가 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"{targetObject.tag} 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }

        StartCoroutine(StartTimer());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        curTime = time;
        timerCoroutine = StartCoroutine(StartTimer());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드된 후에 Player 오브젝트를 찾음
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerAnimator = playerObject.GetComponent<Animator>();

            if (playerAnimator != null)
            {
                playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            else
            {
                Debug.LogError("Player 오브젝트에 Animator가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다.");
        }
    }

    public void PauseTimer()
    {
        if (!isPaused)
        {
            isPaused = true;
            StopCoroutine(timerCoroutine);
        }
    }

    public void ResumeTimer()
    {
        if (isPaused)
        {
            isPaused = false;
            timerCoroutine = StartCoroutine(StartTimer());
        }
    }

    private IEnumerator StartTimer()
    {
        BackgroundAudio backgroundAudio = GameObject.FindWithTag("Player").GetComponent<BackgroundAudio>();

        while (curTime > 0 && !isPaused)
        {
            curTime -= Time.unscaledDeltaTime;
            int minute = (int)curTime / 60;
            int second = Mathf.FloorToInt(curTime % 60);

            if (curTime > 0)
            {
                text.text = minute.ToString("00") + ":" + second.ToString("00");
            }
            else
            {
                text.text = "00:00";
            }

            // 10초 이하일 때 카운트다운 오디오 재생 시작
            if (curTime <= 10f && second != previousSecond && second > 0)
            {
                previousSecond = second; // 현재 초를 저장하여 중복 재생 방지
                StartCoroutine(PlayCountdownAudio()); // 오디오 재생
            }

            if (curTime <= 0)
            {
                curTime = 0;

                string currentScene = SceneManager.GetActiveScene().name;
                if (currentScene == "PlayBOSS" || currentScene == "PlayBOSS_HARD")
                {
                    TriggerPlayBOSSWinScenario();
                }
                else
                {
                    TriggerWinAnimation();
                }

                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator PlayCountdownAudio()
    {
        // 오디오 소스 초기 설정
        audioSource.outputAudioMixerGroup = countdownMixerGroup;
        audioSource.spatialBlend = 0f;  // 2D 설정
        audioSource.volume = 1f;        // 볼륨 설정
        audioSource.priority = 128;     // 우선순위 설정
        audioSource.clip = countdownClip;  // 카운트다운 클립 할당
        int countdown = Mathf.CeilToInt(curTime); // 현재 남은 시간을 정수로 변환하여 시작

        while (countdown > 0 && countdown <= 10)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play(); // 현재 초에서 오디오 재생
            }

            yield return new WaitForSeconds(1f); // 1초 대기

            countdown--;
        }
    }

    private void TriggerWinAnimation()
    {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (GameObject spawner in spawners)
        {
            spawner.SetActive(false);  // Spawner 오브젝트 비활성화
        }

        PlayGameClearAudio();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }

        GameObject[] monsters1 = GameObject.FindGameObjectsWithTag("Monster1");
        GameObject[] monsters2 = GameObject.FindGameObjectsWithTag("Monster2");
        foreach (GameObject monster in monsters1)
        {
            HandleMonsterDeath(monster);
        }
        foreach (GameObject monster in monsters2)
        {
            HandleMonsterDeath(monster);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Walk", false);
            playerAnimator.SetBool("FanAttack", false);
            playerAnimator.SetBool("HitterAttack", false);
            playerAnimator.SetBool("Win", true);
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
            PlayerAttack playerAttack = playerObject.GetComponent<PlayerAttack>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                playerAttack.enabled = false;
                Debug.Log("PlayerMovement disabled.");
            }
            else
            {
                Debug.LogError("PlayerMovement script not found on the Player object.");
            }
        }
        else
        {
            Debug.LogError("Player object not found.");
        }

        isWin = true;
        StartCoroutine(ResetToIdleAfterWin());
        StartCoroutine(ActivateWinPopupAfterDelay(2.0f));
    }

    private void TriggerPlayBOSSWinScenario()
    {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (GameObject spawner in spawners)
        {
            spawner.SetActive(false);  // Spawner 오브젝트 비활성화
        }
        SwitchCameraToBoss();
        PlayGameClearAudio();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Walk", false);
            playerAnimator.SetBool("FanAttack", false);
            playerAnimator.SetBool("HitterAttack", false);
            playerAnimator.SetBool("Win", true);
        }
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
            PlayerAttack playerAttack = playerObject.GetComponent<PlayerAttack>();
            if (playerMovement != null)
            {
                playerAttack.enabled = false;
                playerMovement.enabled = false;
            }
        }

        GameObject[] monsters1 = GameObject.FindGameObjectsWithTag("Monster1");
        GameObject[] monsters2 = GameObject.FindGameObjectsWithTag("Monster2");
        foreach (GameObject monster in monsters1)
        {
            HandleMonsterDeath(monster);
        }
        foreach (GameObject monster in monsters2)
        {
            HandleMonsterDeath(monster);
        }

        GameObject boss = GameObject.FindGameObjectWithTag("BOSS");
        if (boss != null)
        {
            Animator bossAnimator = boss.GetComponent<Animator>();
            if (bossAnimator != null)
            {
                bossAnimator.SetBool("Win", true);  // Win 애니메이션 활성화
                StartCoroutine(DisableOtherAnimations(bossAnimator));
            }
        }
        StartCoroutine(ResetToIdleAfterWin());
        StartCoroutine(ActivateWinPopupAfterDelay(2.0f));
    }

    IEnumerator ResetToIdleAfterWin()
    {
        yield return new WaitForSecondsRealtime(2f);
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Win", false);
            playerAnimator.Play("Idle");
        }
    }

    private void HandleMonsterDeath(GameObject monster)
    {
        Rigidbody2D monsterRb = monster.GetComponent<Rigidbody2D>();
        Collider2D monsterCollider = monster.GetComponent<Collider2D>();
        Animator monsterAnimator = monster.GetComponent<Animator>();

        if (monsterRb != null)
        {
            monsterRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;  // X, Y 축 잠금
        }

        if (monsterCollider != null)
        {
            monsterCollider.enabled = false;  // 콜라이더 비활성화
        }

        if (monsterAnimator != null)
        {
            monsterAnimator.SetTrigger("Dead");  // Dead 트리거 활성화
            StartCoroutine(DestroyAfterAnimation(monster, monsterAnimator, "Dead"));  // Dead 애니메이션 후 오브젝트 파괴
        }
    }

    private void SwitchCameraToBoss()
    {
        FindPB cameraController = FindObjectOfType<FindPB>();

        if (cameraController != null)
        {
            StartCoroutine(cameraController.FindAndAssignBoss());
        }
        else
        {
            Debug.LogError("FindPB 스크립트를 찾을 수 없습니다.");
        }
    }

    private IEnumerator DisableOtherAnimations(Animator bossAnimator)
    {
        yield return new WaitForSeconds(1f);
        while (bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("Win"))
        {
            yield return null;
        }

        // Win 애니메이션이 끝난 후에도 다른 애니메이션으로 전환되지 않도록 고정
        bossAnimator.SetBool("Win", true);
    }

    IEnumerator ActivateWinPopupAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (winPopup != null)
        {
            winPopup.SetActive(true);
        }
        DisableBackgroundAudio();
    }

    private void PlayGameClearAudio()
    {
        if (gameClearClip != null && audioSource != null)
        {
            audioSource.outputAudioMixerGroup = gameClearMixerGroup;
            audioSource.clip = gameClearClip;
            audioSource.Play();
        }
    }

    private IEnumerator DestroyAfterAnimation(GameObject monster, Animator animator, string animationName)
    {
        // Dead 애니메이션이 끝날 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return null;
        }

        // 애니메이션이 끝난 후 오브젝트 파괴
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        Destroy(monster);  // 오브젝트 파괴
    }

    private void DisableBackgroundAudio()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            BackgroundAudio bgAudio = player.GetComponent<BackgroundAudio>();
            if (bgAudio != null)
            {
                bgAudio.PauseBackgroundAudio(); // BackgroundAudio 비활성화
            }
        }
    }

    // 게임 재개
    public void ResumeGame()
    {
        if (winPopup != null)
        {
            winPopup.SetActive(false);  //팝업 숨기기
        }
        Time.timeScale = 1; 
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
