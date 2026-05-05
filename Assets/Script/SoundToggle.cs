using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    [Header("사운드 버튼 오브젝트")]
    public Button soundToggleButton;  

    [Header("사운드 버튼 스프라이트 이미지")]
    public Sprite soundOnSprite;      
    public Sprite soundOffSprite;  

    private bool isSoundOn = true;

    void Start()
    {
        // 씬이 로드될 때 사운드 상태에 따라 초기 스프라이트 설정
        isSoundOn = AudioListener.volume > 0;  // AudioListener의 볼륨 상태를 확인하여 초기 설정
        UpdateSoundSprite();                   // 초기 스프라이트 업데이트

        // 버튼 클릭 이벤트 연결
        soundToggleButton.onClick.AddListener(ToggleSound);

        // 주기적으로 사운드 상태에 따라 스프라이트 변경
        InvokeRepeating("UpdateSoundSprite", 0f, 0.1f);
    }

    // 사운드 상태에 따라 스프라이트 업데이트
    void UpdateSoundSprite()
    {
        if (isSoundOn)
        {
            soundToggleButton.GetComponent<Image>().sprite = soundOnSprite;
        }
        else
        {
            soundToggleButton.GetComponent<Image>().sprite = soundOffSprite;
        }
    }

    // 사운드 On/Off 기능
    public void ToggleSound()
    {
        if (isSoundOn)
        {
            AudioListener.volume = 0;  // 모든 사운드 음소거
        }
        else
        {
            AudioListener.volume = 1;  // 모든 사운드 활성화
        }

        isSoundOn = !isSoundOn;
        UpdateSoundSprite();           // 사운드 상태 전환 후 스프라이트 즉시 업데이트
    }
}