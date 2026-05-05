using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI tutorialText;    
    public TutorialTimeAttack timeAttack;   

    [Header("Monster Prefabs")]
    public GameObject[] monsterPrefabs;     

    [Header("Stage Timers")]
    public float[] stageTimers;

    [Header("Monster Spawn Points")]
    public Transform[] monsterSpawnPoints;

    private GameObject player;             
    private int currentStage = 0;           
    private GameObject activeMonster = null;  
    private bool isTextFullyVisible = false; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.SetTutorialManager(this);
        }

        StartCoroutine(UpdateTutorialTextWithFade());
    }

    public void NextStage()
    {
        if (currentStage >= 6)
        {
            return;
        }

        if (isTextFullyVisible == false)
        {
            return;
        }

        currentStage++;

        StartCoroutine(UpdateTutorialTextWithFade());
        if (currentStage == 3)
        {
            StartCoroutine(ReenablePlayerMovement());
        }
    }

    // 현재 단계를 다시 시작하는 메서드
    public void RestartStage()
    {
        StartCoroutine(UpdateTutorialTextWithFade());
    }

    IEnumerator UpdateTutorialTextWithFade()
    {
        isTextFullyVisible = false;

        for (float alpha = 1f; alpha >= 0; alpha -= Time.deltaTime)

        {
            tutorialText.color = new Color(tutorialText.color.r, tutorialText.color.g, tutorialText.color.b, alpha);
            yield return null;
        }

        switch (currentStage)
        {
            case 0:
                tutorialText.text = "방향키를 눌러 좌우로 이동";
                break;
            case 1:
                tutorialText.text = "Q 키로 선풍기 공격";
                break;
            case 2:
                tutorialText.text = "W 키로 히터 공격";
                break;
            case 3:
                tutorialText.text = "몬스터와 플레이어가 부딪히면 체력이 닳습니다.";
                break;
            case 4:
                tutorialText.text = "Q선풍기 공격으로 몬스터를 얼려주세요!";

                // SceneMove의 MovePlayerToTP 메서드 호출
                PlayerSceneMove playerSceneMove = FindObjectOfType<PlayerSceneMove>();
                if (playerSceneMove != null)
                {
                    playerSceneMove.MovePlayerToTP();
                }
                SpawnMonster();
                break;
            case 5:
                tutorialText.text = "W히터 공격으로 몬스터를 녹여주세요!";
                SpawnMonster();
                break;
            case 6:
                tutorialText.text = "ESC키를 눌러 로비로 이동";
                timeAttack.StopTimer();
                break;
        }

        // 텍스트 변경 후 페이드 인
        for (float alpha = 0; alpha <= 1f; alpha += Time.deltaTime)
        {
            tutorialText.color = new Color(tutorialText.color.r, tutorialText.color.g, tutorialText.color.b, alpha);
            yield return null;
        }

        // 텍스트가 완전히 불투명해짐
        isTextFullyVisible = true;

        // 현재 단계 타이머 시작
        if (currentStage < stageTimers.Length)
        {
            float stageTimer = stageTimers[currentStage];
            if (stageTimer > 0)
            {
                timeAttack.ResetTimer(stageTimer);  // 설정된 타이머 값으로 타이머 시작
            }
            else
            {
                timeAttack.StopTimer();  // 타이머 값이 0이면 비활성화
            }
        }
    }

    void SpawnMonster()
    {
        if (currentStage == 4 || currentStage == 5)
        {
            int monsterIndex = currentStage - 4;

            // 지정된 위치 배열에서 해당하는 좌표 가져오기
            if (monsterSpawnPoints.Length > monsterIndex && monsterSpawnPoints[monsterIndex] != null)
            {
                Vector3 spawnPosition = monsterSpawnPoints[monsterIndex].position;
                activeMonster = Instantiate(monsterPrefabs[monsterIndex], spawnPosition, Quaternion.identity);
            }
        }
    }

    IEnumerator ReenablePlayerMovement()
    {
        yield return new WaitForSeconds(5f);
        player.GetComponent<Rigidbody2D>().simulated = true;
    }

    public void OnCorrectAttack()
    {
        if (currentStage == 4)
        {
            currentStage = 5;
            StartCoroutine(UpdateTutorialTextWithFade());
            return;
        }

        if (currentStage == 5)
        {
            return;
        }
    }

    public int GetCurrentStage()
    {
        return currentStage;
    }
}