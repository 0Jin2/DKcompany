using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialTimeAttack : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TutorialManager tutorialManager;
    private float timer = 5f;
    private bool isTimerActive = true;

    void Start()
    {
        UpdateTutorialManagerReference();
        SceneManager.sceneLoaded += OnSceneLoaded;
        ResetTimer(0f);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateTutorialManagerReference();
    }

    void UpdateTutorialManagerReference()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }
        else
        {
            tutorialManager = null; 
        }
    }

    void Update()
    {
        if (isTimerActive)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();

            if (timer <= 0)
            {
                isTimerActive = false;
                timerText.gameObject.SetActive(false); // 타이머 텍스트 비활성화

                if (tutorialManager != null && tutorialManager.GetCurrentStage() != 5)
                {
                    tutorialManager.NextStage();
                }
            }
        }
    }

    public void ResetTimer(float time)
    {
        if (time > 0)
        {
            timer = time;
            isTimerActive = true;
            timerText.gameObject.SetActive(true); // 타이머 텍스트 활성화
        }
        else
        {
            StopTimer(); // 타이머가 0이거나 필요 없는 경우 비활성화
        }
    }

    public void StopTimer()
    {
        isTimerActive = false;
        timerText.gameObject.SetActive(false); // 타이머 텍스트 비활성화
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}