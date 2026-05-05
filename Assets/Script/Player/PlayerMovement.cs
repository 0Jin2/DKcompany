using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("키 설정")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;   // 기본값 방향키 왼쪽
    [SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow; // 기본값 방향키 오른쪽

    [Header("기본 이동 속도")]
    public float defaultMoveSpeed = 7.0f;

    [Header("노멀 플레이어 이동 속도")]
    public float moveSpeedPlayB1 = 7.0f;         // PlayB1 씬에서의 이동 속도
    public float moveSpeedPlayB2 = 7.0f;         // PlayB2 씬에서의 이동 속도
    public float moveSpeedPlayBOSS = 7.5f;       // PlayBOSS 씬에서의 이동 속도

    [Header("노멀 플레이어 이동 속도")]
    public float moveSpeedPlayB1_HARD = 8.0f;    // PlayB1 씬에서의 이동 속도
    public float moveSpeedPlayB2_HARD = 8.0f;    // PlayB2 씬에서의 이동 속도
    public float moveSpeedPlayBOSS_HARD = 9.0f;  // PlayBOSS 씬에서의 이동 속도

    private float moveSpeed;  // 현재 씬에서 사용할 이동 속도

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool turn = true;
    private Animator anim;
    private TimeAttack timeAttack;
    private PlayerAttack playerAttack;
    private bool isDead = false; 
    private bool canMove = true; 

    void Start()
    {
        timeAttack = FindObjectOfType<TimeAttack>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateMoveSpeed(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        anim.SetBool("Win", false);
        UpdateMoveSpeed(scene.name);

        if (scene.name == "PlayBOSS")
        {
            canMove = false;
        }
        if (!this.enabled)
        {
            this.enabled = true;
        }
    }

    public void EnableMovement()
    {
        if (SceneManager.GetActiveScene().name == "PlayBOSS")
        {
            canMove = true;
        }
    }

    void UpdateMoveSpeed(string sceneName)
    {
        if (sceneName == "PlayB1")
        {
            moveSpeed = moveSpeedPlayB1;
        }
        else if (sceneName == "PlayB2")
        {
            moveSpeed = moveSpeedPlayB2; 
        }
        else if (sceneName == "PlayBOSS")
        {
            moveSpeed = moveSpeedPlayBOSS;
        }
        else if (sceneName == "PlayB1_HARD")
        {
            moveSpeed = moveSpeedPlayB1_HARD; 
        }
        else if (sceneName == "PlayB2_HARD")
        {
            moveSpeed = moveSpeedPlayB2_HARD;
        }
        else if (sceneName == "PlayBOSS_HARD")
        {
            moveSpeed = moveSpeedPlayBOSS_HARD;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
        }
    }

    void Update()
    {
        if (timeAttack != null && timeAttack.isWin)
        {
            anim.SetBool("Walk", false); 
            anim.SetBool("FanAttack", false); 
            anim.SetBool("HitterAttack", false);
            return;  // Win 상태에서는 입력 처리 중단
        }

        if (Input.GetKey(moveLeftKey))
        {
            movement.x = -1; // 왼쪽으로 이동
        }
        else if (Input.GetKey(moveRightKey))
        {
            movement.x = 1; // 오른쪽으로 이동
        }
        else
        {
            movement.x = 0; // 멈춤
        }

        if (movement.x != 0)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        // 공격 중이 아닐 때만 캐릭터 뒤집기
        if (playerAttack != null && !playerAttack.isAttacking)
        {
            if (movement.x > 0 && !turn)
            {
                Flip();
            }
            else if (movement.x < 0 && turn)
            {
                Flip();
            }
        }
    }


    void FixedUpdate()
    {
        if (!isDead && canMove)
        {
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }
    }

    void Flip()
    {
        turn = !turn;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
