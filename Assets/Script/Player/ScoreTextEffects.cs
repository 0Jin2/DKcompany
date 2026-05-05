using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreTextEffects : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private Coroutine colorChangeCoroutine;
    private Color originalColor;

    void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        if (scoreText == null)
        {
            Debug.LogError("TextMeshProUGUI component is missing on this GameObject.");
        }
        else
        {
            originalColor = scoreText.color; // 초기 색상 저장
        }
    }

    // 점수 증가 시 호출 - 초록색으로 깜빡임
    public void FlashIncreaseEffect()
    {
        StartColorFlash(Color.green, 0.2f, 1.0f);
    }

    // 점수 감소 시 호출 - 빨간색으로 깜빡임
    public void FlashDecreaseEffect()
    {
        StartColorFlash(Color.red, 0.2f, 1.0f);
    }

    private void StartColorFlash(Color targetColor, float interval, float duration)
    {
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
        }
        colorChangeCoroutine = StartCoroutine(FlashTextColor(targetColor, interval, duration));
    }

    private IEnumerator FlashTextColor(Color targetColor, float interval, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            scoreText.color = scoreText.color == targetColor ? originalColor : targetColor;
            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }

        // 깜빡임 효과 종료 후 기존 색상으로 복원
        scoreText.color = originalColor;
    }
}
