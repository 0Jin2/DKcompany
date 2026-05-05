using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopupEffect : MonoBehaviour
{
    public float moveUpSpeed = 1f;         // 위로 올라가는 속도
    public float fadeOutDuration = 1f;     // 사라지는 데 걸리는 시간

    private SpriteRenderer spriteRenderer; // SpriteRenderer 컴포넌트 참조
    private Color spriteColor;             // 스프라이트 색상
    private float elapsedTime = 0f;        // 경과 시간

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        // 스프라이트가 위로 올라가도록 위치 변경
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // 투명도 점진적으로 줄이기
        elapsedTime += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
        spriteColor.a = alpha;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = spriteColor;
        }

        // 투명도가 0에 도달하면 오브젝트 삭제
        if (alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }
}