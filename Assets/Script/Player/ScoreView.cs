using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    [Header("WIN, OVER 팝업 점수 표시 프리팹")]
    public TextMeshProUGUI scoreText;
    private ScoreManager scoreManager; // ScoreManager 스크립트

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");


        if (player != null)
        {
            scoreManager = player.GetComponent<ScoreManager>();
        }
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager를 Player 태그의 오브젝트에서 찾을 수 없습니다.");
        }

        StartCoroutine(UpdateScoreRoutine());
    }

    IEnumerator UpdateScoreRoutine()
    {
        while (true)
        {
            UpdateScoreText();
            yield return new WaitForSeconds(0.5f);//0.5초마다 업데이트
        }
    }

    private void UpdateScoreText()
    {
        if (scoreManager != null && scoreText != null)
        {
            scoreText.text = scoreManager.GetCurrentScore().ToString();
        }
    }
}