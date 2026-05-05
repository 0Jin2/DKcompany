using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingBarManagerPlay : MonoBehaviour
{
    public Transform startPoint;     // 시작점 UI 오브젝트 (RectTransform)
    public Transform targetPoint;    // 목표 지점 UI 오브젝트 (RectTransform)
    public TextMeshProUGUI percentageText; // 퍼센트 텍스트 (TextMeshPro)
    public RectTransform loadingBar;     // 로딩바 UI 오브젝트 (RectTransform)
    public float speed = 1f;             // 오브젝트가 목표 지점까지 가는 속도

    private float totalDistance;         // 시작점과 목표 지점 사이의 총 거리
    private bool isLoadingComplete = false;

    void Start()
    {
        // 시작점과 목표 지점 사이의 총 거리를 계산 (UI 요소는 localPosition 사용)
        totalDistance = Vector3.Distance(startPoint.localPosition, targetPoint.localPosition);

        // 퍼센트 텍스트 초기값 설정
        percentageText.text = "0%";

        // 로딩바를 시작점에 위치
        loadingBar.localPosition = startPoint.localPosition;
    }

    void Update()
    {
        if (!isLoadingComplete)
        {
            // 현재 로딩바와 목표 지점 사이의 거리 계산
            float currentDistance = Vector3.Distance(loadingBar.localPosition, targetPoint.localPosition);

            // 0에서 100%까지 거리 비율 계산 (1 - 남은 거리 / 총 거리)
            float progress = Mathf.Clamp01(1 - (currentDistance / totalDistance));

            // 퍼센트 텍스트 업데이트 (TextMeshPro)
            percentageText.text = Mathf.RoundToInt(progress * 100) + "%";

            // 로딩바를 목표 지점으로 이동
            loadingBar.localPosition = Vector3.MoveTowards(loadingBar.localPosition, targetPoint.localPosition, speed * Time.deltaTime);

            // 100%에 도달했을 때
            if (progress >= 1f)
            {
                isLoadingComplete = true;
                StartCoroutine(LoadNextSceneAfterDelay());
            }
        }
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1초 대기 후
        SceneManager.LoadScene("PlayB1");    // PlayB1 씬으로 전환
    }
}