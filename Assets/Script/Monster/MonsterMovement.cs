using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterMovement : MonoBehaviour
{
    [Header("몬스터 기본 속도")]
    public float defaultSpeed = 5.0f;

    [Header("노멀 난이도 몬스터 기본 속도")]
    public float speedPlayB1 = 5.0f;
    public float speedPlayB2 = 5.5f;
    public float speedPlayBOSS = 6.5f;

    [Header("하드 난이도 몬스터 기본 속도")]
    public float speedPlayB1_HARD = 6.0f;
    public float speedPlayB2_HARD = 6.5f;
    public float speedPlayBOSS_HARD = 7.0f;

    private float currentSpeed;

    [HideInInspector]
    public bool isDead = false;

    private Rigidbody2D rb;
    private Transform player;
    private bool turn = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1.0f;
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateMoveSpeed(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMoveSpeed(scene.name); // 새로운 씬이 로드될 때 이동 속도 업데이트
    }

    void UpdateMoveSpeed(string sceneName)
    {
        // 씬 이름에 따라 이동 속도를 설정
        if (sceneName == "PlayB1")
        {
            currentSpeed = speedPlayB1;
        }
        else if (sceneName == "PlayB2")
        {
            currentSpeed = speedPlayB2;
        }
        else if (sceneName == "PlayBOSS")
        {
            currentSpeed = speedPlayBOSS;
        }
        if (sceneName == "PlayB1_HARD")
        {
            currentSpeed = speedPlayB1_HARD;
        }
        else if (sceneName == "PlayB2_HARD")
        {
            currentSpeed = speedPlayB2_HARD;
        }
        else if (sceneName == "PlayBOSS_HARD")
        {
            currentSpeed = speedPlayBOSS_HARD;
        }
        else if (sceneName == "Tutorial")
        {
            currentSpeed = 3.0f;
        }
        else
        {
            currentSpeed = defaultSpeed;
        }
    }

    void Update()
    {
        if (isDead) return;

        Vector2 direction = (Vector2)player.position - rb.position;
        direction.Normalize();
        rb.velocity = new Vector2(direction.x * currentSpeed, rb.velocity.y);

        // 몬스터가 플레이어 쪽으로 움직이도록 방향 전환
        if (rb.velocity.x > 0 && !turn)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && turn)
        {
            Flip();
        }
    }

    void Flip()
    {
        turn = !turn;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            // 애니메이터에서 'Dead' 트리거 활성화
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Dead");
            }

            // Collider 비활성화
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // Death 사운드 재생
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            StartCoroutine(DestroyAfterAnimation());
        }
    }

    IEnumerator DestroyAfterAnimation()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            while (!stateInfo.IsName("Dead"))
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            yield return new WaitForSeconds(stateInfo.length);
        }

        Destroy(gameObject); // 몬스터 파괴
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 해제
    }
}
