using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeStop : MonoBehaviour
{
    [Header("일시정지를 실행할 버튼 참조")]
    public Button StopButton;
    private TimeAttack timeAttack;

    void Start()
    {
        StopButton.onClick.AddListener(OnClick);
        timeAttack = FindObjectOfType<TimeAttack>();
    }

    void OnClick()
    {
        Time.timeScale = 0; 
        if (timeAttack != null)
        {
            timeAttack.PauseTimer(); 
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (timeAttack != null)
        {
            timeAttack.ResumeTimer();
        }
    }
}