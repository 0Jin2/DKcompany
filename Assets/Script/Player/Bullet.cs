using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxTravelDistance = 10.0f; // 최대 이동 거리
    private Vector3 startPosition;

    void Start()
    {
        // 총알의 시작 위치 저장
        startPosition = transform.position;
    }

    void Update()
    {
        // 총알이 최대 이동 거리를 초과하면 파괴
        if (Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 몬스터1 또는 몬스터2 태그와 충돌했을 때만 데미지를 주고 총알 파괴
        if (collision.CompareTag("Monster1") || collision.CompareTag("Monster2"))
        {
            MonsterMovement monster = collision.GetComponent<MonsterMovement>();
            if (monster != null && !monster.isDead)
            {
                monster.Die(); // 몬스터 죽음 트리거 실행
            }

            // 충돌 후 총알 파괴
            Destroy(gameObject);
        }
    }
}