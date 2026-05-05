using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PlayerAttack : MonoBehaviour
{
    [Header("키 설정")]
    [SerializeField] private KeyCode fanAttackKey = KeyCode.Q;    // 기본값 Q
    [SerializeField] private KeyCode hitterAttackKey = KeyCode.W; // 기본값 W

    [Header("공격 범위 설정 및 몬스터 레이어 지정")]
    public float attackRange = 2.0f; // 공격 범위
    public LayerMask enemyLayers;    // 적 레이어

    [Header("공격 이펙트 오브젝트")]
    public GameObject fanAttackEffectPrefab;    // 팬 어택 이펙트 프리팹
    public GameObject hitterAttackEffectPrefab; // 히터 어택 이펙트 프리팹
    public Vector2 fanAttackEffectOffset = new Vector2(1.4f, 0.45f);
    public Vector2 hitterAttackEffectOffset = new Vector2(1.4f, 0.4f);

    [Header("공격 애니메이션 설정")]
    [SerializeField] private float attackAnimationDuration = 0.1f;

    [Header("씬별 공격 쿨타임 설정")]
    public float attackCooldownB1 = 0.01f;
    public float attackCooldownB2 = 0.01f;
    public float attackCooldownBOSS = 0.01f;

    [Header("씬별 공격 쿨타임 설정")]
    public float attackCooldownB1_HARD = 0.01f;
    public float attackCooldownB2_HARD = 0.01f;
    public float attackCooldownBOSS_HARD = 0.01f;

    [Header("점수 팝업 스프라이트 프리팹")]
    public GameObject correctAttackPopupPrefab;  // 올바른 공격 프리팹
    public GameObject wrongAttackPopupPrefab;    // 잘못된 공격 프리팹

    [Header("오디오 클립, 믹서 그룹 참조")]
    public AudioClip attackSound; // 공격 사운드
    public AudioMixerGroup attackSoundOutputGroup;
    private AudioSource audioSource; // 사운드 플레이어

    [Header("점수 시스템")]
    public int scoreIncreaseAmount = 50;
    public int scoreDecreaseAmount = 50;

    [Header("공격 여부 테스트")]
    public bool isAttacking = false; // 플레이어가 공격 중인지 여부

    private bool canAttack = true; // 공격 가능 여부
    private bool isHitterAttack = false;

    private float currentAttackCooldown;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform playerTransform;
    private ScoreTextEffects scoreTextEffects;
    private TutorialManager tutorialManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerTransform = transform;
        UpdateTutorialManagerReference(); // 초기화
        SceneManager.sceneLoaded += OnSceneLoaded;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource 컴포넌트가 할당되지 않았습니다.");
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 시 호출되는 이벤트 등록
        UpdateAttackCooldown(SceneManager.GetActiveScene().name); // 현재 씬에 맞게 쿨타임 업데이트
        FindScoreTextEffects();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        if (sceneName == "LoadingScene1" || sceneName == "LoadingScene2" ||
            sceneName == "LoadingScene3" || sceneName == "LobbyScene")
        {
            this.enabled = false;
        }
        else
        {
            this.enabled = true;
        }

        UpdateAttackCooldown(scene.name);
        FindScoreTextEffects();
        UpdateTutorialManagerReference();
    }

    void UpdateTutorialManagerReference()
    {
        // 현재 씬 이름이 Tutorial이면 TutorialManager를 참조
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }
        else
        {
            tutorialManager = null; // 다른 씬에서는 참조 제거
        }
    }

    void FindScoreTextEffects()
    {
        GameObject scoreTextObject = GameObject.FindWithTag("ScoreText");
        if (scoreTextObject != null)
        {
            scoreTextEffects = scoreTextObject.GetComponent<ScoreTextEffects>();
        }
    }

    public void SetTutorialManager(TutorialManager manager)
    {
        tutorialManager = manager;
    }

    void UpdateAttackCooldown(string sceneName)
    {
        // 씬 이름에 따라 공격 쿨타임을 설정
        if (sceneName == "PlayB1")
        {
            currentAttackCooldown = attackCooldownB1;
        }
        else if (sceneName == "PlayB2")
        {
            currentAttackCooldown = attackCooldownB2;
        }
        else if (sceneName == "PlayBOSS")
        {
            currentAttackCooldown = attackCooldownBOSS;
        }
        if (sceneName == "PlayB1_HARD")
        {
            currentAttackCooldown = attackCooldownB1_HARD;
        }
        else if (sceneName == "PlayB2_HARD")
        {
            currentAttackCooldown = attackCooldownB2_HARD;
        }
        else if (sceneName == "PlayBOSS_HARD")
        {
            currentAttackCooldown = attackCooldownBOSS_HARD;
        }
        else
        {
            currentAttackCooldown = attackCooldownB1;
        }
    }

    void Update()
    {
        if (!isAttacking && canAttack)
        {
            if (Input.GetKeyDown(fanAttackKey))
            {
                isHitterAttack = false;
                PerformAttack();
            }
            else if (Input.GetKeyDown(hitterAttackKey))
            {
                isHitterAttack = true;
                PerformAttack();
            }
        }

        // 공격 중일 때는 X축 이동과 스케일을 고정
        if (isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // X축 이동 속도를 0으로 설정
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // X축 위치 및 회전 고정
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 공격이 끝나면 X축 움직임 해제
        }
    }

    void PerformAttack()
    {
        isAttacking = true;// 공격 시작
        canAttack = false; // 공격 쿨다운 시작

        // X축 이동과 스케일 고정
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // 사운드 재생 (공격 시작 시 항상 재생)
        PlayAttackSound();

        // 가장 가까운 적을 찾고 공격 처리
        float playerDirection = playerTransform.localScale.x > 0 ? 1f : -1f;
        Collider2D closestEnemy = FindClosestEnemy(playerDirection);

        if (closestEnemy != null)
        {
            MonsterMovement monster = closestEnemy.GetComponent<MonsterMovement>();
            if (monster != null && !monster.isDead)
            {
                bool isCorrectAttack = (isHitterAttack && closestEnemy.CompareTag("Monster1")) || (!isHitterAttack && closestEnemy.CompareTag("Monster2"));
                bool isWrongAttack = !isCorrectAttack;

                if (isCorrectAttack)
                {
                    ScoreManager.instance.IncreaseScore(scoreIncreaseAmount);
                    scoreTextEffects?.FlashIncreaseEffect();
                    ShowScorePopup(correctAttackPopupPrefab);
                    if (tutorialManager != null && tutorialManager.GetCurrentStage() == 4)
                    {
                        tutorialManager.OnCorrectAttack();
                    }
                }
                else if (isWrongAttack)
                {
                    ScoreManager.instance.IncreaseScore(-scoreDecreaseAmount);
                    scoreTextEffects?.FlashDecreaseEffect();
                    ShowScorePopup(wrongAttackPopupPrefab);
                    tutorialManager?.RestartStage();
                }

                if (tutorialManager != null)
                {
                    tutorialManager.NextStage();
                }
                monster.Die();
            }
        }

        // 점수 팝업 스프라이트 표시 메서드
        void ShowScorePopup(GameObject popupPrefab)
        {
            if (popupPrefab != null)
            {
                // 플레이어의 Y 좌표 위에 팝업 생성
                Vector3 popupPosition = transform.position + new Vector3(0, 1.2f, 0);
                Instantiate(popupPrefab, popupPosition, Quaternion.identity);
            }
        }

        // 공격 애니메이션을 시작 (bool 값 설정)
        if (isHitterAttack)
        {
            animator.SetBool("HitterAttack", true);
        }
        else
        {
            animator.SetBool("FanAttack", true);
        }
        SpawnAttackEffect(playerDirection); //공격 이펙트 생성 메서드
        StartCoroutine(ResetAttackAnimation()); 
    }

    void SpawnAttackEffect(float direction)
    {
        GameObject effectPrefab = isHitterAttack ? hitterAttackEffectPrefab : fanAttackEffectPrefab;
        Vector2 effectOffset = isHitterAttack ? hitterAttackEffectOffset : fanAttackEffectOffset;

        if (effectPrefab != null)
        {
            // 이펙트 생성 위치와 방향 설정
            Vector3 effectPosition = transform.position + new Vector3(effectOffset.x * direction, effectOffset.y, 0);
            GameObject effect = Instantiate(effectPrefab, effectPosition, Quaternion.identity);

            // 이펙트의 방향 설정
            Vector3 effectScale = effect.transform.localScale;
            effectScale.x *= direction; // 방향에 따라 이펙트 뒤집기
            effect.transform.localScale = effectScale;
        }
    }

    // 특정 방향에서 가장 가까운 적을 탐지하는 함수
    Collider2D FindClosestEnemy(float direction)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);
        Collider2D closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 enemyPosition = enemy.transform.position;
            float distance = Vector2.Distance(transform.position, enemyPosition);

            // 적이 플레이어의 X 좌표 앞에 있는지 확인 (direction에 따라 필터링)
            if ((enemyPosition.x - transform.position.x) * direction > 0 && distance < closestDistance)
            {
                MonsterMovement monster = enemy.GetComponent<MonsterMovement>();
                if (monster != null && !monster.isDead) // 죽은 몬스터는 무시
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }


    IEnumerator ResetAttackAnimation()
    {
        // 인스펙터에서 설정한 애니메이션 지속 시간 동안 대기
        yield return new WaitForSeconds(attackAnimationDuration);

        // 애니메이션 종료
        animator.SetBool("HitterAttack", false);
        animator.SetBool("FanAttack", false);

        // 공격이 끝나면 X축 이동 및 스케일 제한 해제
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전만 고정하고 X축 이동 해제
        isAttacking = false; // 공격 상태 해제

        // 공격 쿨다운 시간 동안 대기 (씬별 쿨다운 적용)
        yield return new WaitForSeconds(currentAttackCooldown);

        // 쿨다운이 끝나면 다시 공격 가능
        canAttack = true;
    }

    void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound); // 변경 후 코드
        }
    }

    // 시각적 디버깅을 위한 Gizmos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 해제
    }
}