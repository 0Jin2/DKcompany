using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReStart : MonoBehaviour
{
    [Header("재시작을 실행할 버튼 참조")]
    public Button ReaterButton;

    private TimeAttack timeAttack;

    void Start()
    {
        ReaterButton.onClick.AddListener(OnClick);
        timeAttack = FindObjectOfType<TimeAttack>();
    }

    void OnClick()
    {
        Time.timeScale = 1;

        if (timeAttack != null)
        {
            timeAttack.ResumeTimer();
        }
    }
}